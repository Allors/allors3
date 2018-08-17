// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Load2.cs" company="Allors bvba">
//   Copyright 2002-2017 Allors bvba.
// Dual Licensed under
//   a) the Lesser General Public Licence v3 (LGPL)
//   b) the Allors License
// The LGPL License is included in the file lgpl.txt.
// The Allors License is an addendum to your contract.
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Adapters.Object.SqlClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Xml;

    using Adapters;

    using Allors.Meta;

    public class Load2
    {
        public const long InitialVersion = 0;

        private static readonly byte[] EmptyByteArray = new byte[0];

        private readonly Database database;
        private readonly SqlConnection connection;
        private readonly ObjectNotLoadedEventHandler objectNotLoaded;
        private readonly RelationNotLoadedEventHandler relationNotLoaded;

        private readonly Dictionary<long, IClass> classByObjectId;

        public Load2(Database database, SqlConnection connection, ObjectNotLoadedEventHandler objectNotLoaded, RelationNotLoadedEventHandler relationNotLoaded)
        {
            this.database = database;
            this.connection = connection;
            this.objectNotLoaded = objectNotLoaded;
            this.relationNotLoaded = relationNotLoaded;

            this.classByObjectId = new Dictionary<long, IClass>();
        }

        public void Execute(XmlReader reader)
        {
            reader.MoveToContent();

            while (reader.Read())
            {
                if (reader.NodeType.Equals(XmlNodeType.Element))
                {
                    if (reader.Name.Equals(Serialization.Population))
                    {
                        var version = reader.GetAttribute(Serialization.Version);
                        if (string.IsNullOrEmpty(version))
                        {
                            throw new ArgumentException("Save population has no version.");
                        }

                        Serialization.CheckVersion(int.Parse(version));

                        if (!reader.IsEmptyElement)
                        {
                            this.LoadPopulation(reader);
                        }

                        break;
                    }
                }
            }
        }

        private void LoadPopulation(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // eat everything but elements
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Objects))
                        {
                            if (!reader.IsEmptyElement)
                            {
                                this.LoadObjects(reader.ReadSubtree());
                            }
                        }
                        else if (reader.Name.Equals(Serialization.Relations))
                        {
                            if (!reader.IsEmptyElement)
                            {
                                this.LoadRelations(reader);
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" + Serialization.Population + ">");
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (!reader.Name.Equals(Serialization.Population))
                        {
                            throw new Exception("Expected closing element </" + Serialization.Population + ">");
                        }

                        return;
                }
            }
        }

        private void LoadObjects(XmlReader reader)
        {
            reader.MoveToContent();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Database))
                        {
                            if (!reader.IsEmptyElement)
                            {
                                this.LoadObjectsDatabase(reader.ReadSubtree());
                            }
                        }
                        else if (reader.Name.Equals(Serialization.Workspace))
                        {
                            throw new Exception("Can not load workspace objects in a database.");
                        }
                        else
                        {
                            throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" + Serialization.Objects + ">");
                        }

                        break;
                }
            }
        }

        private void LoadObjectsDatabase(XmlReader reader)
        {
            reader.MoveToContent();

            var xmlObjects = new Objects(this.database, this.OnObjectNotLoaded, this.classByObjectId, reader);
            using (var objectsReader = new ObjectsReader(xmlObjects))
            {
                using (var sqlBulkCopy = new SqlBulkCopy(this.connection, SqlBulkCopyOptions.KeepIdentity, null))
                {
                    sqlBulkCopy.BulkCopyTimeout = 0;
                    sqlBulkCopy.BatchSize = 5000;
                    sqlBulkCopy.DestinationTableName = this.database.Mapping.TableNameForObjects;
                    sqlBulkCopy.WriteToServer(objectsReader);
                }
            }

            // TODO: move this to a stored procedure
            // insert from _o table into class tables
            using (var transaction = this.connection.BeginTransaction())
            {
                foreach (var @class in this.database.MetaPopulation.Classes)
                {
                    var tableName = this.database.Mapping.TableNameForObjectByClass[@class];

                    using (var command = this.connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandTimeout = 0;
                        command.CommandType = CommandType.Text;
                        command.CommandText = $@"
insert into {tableName} (o, c)
select o, c from allors._o
where c = '{@class.Id}'";

                        command.ExecuteNonQuery();
                    }
                }

                transaction.Commit();
            }
        }

        private void LoadRelations(XmlReader reader)
        {
            reader.MoveToContent();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Database))
                        {
                            if (!reader.IsEmptyElement)
                            {
                                this.LoadRelationsDatabase(reader);
                            }
                        }
                        else if (reader.Name.Equals(Serialization.Workspace))
                        {
                            throw new Exception("Can not load workspace relations in a database.");
                        }
                        else
                        {
                            throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" + Serialization.Relations + ">");
                        }

                        break;
                }
            }
        }

        private void LoadRelationsDatabase(XmlReader reader)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // eat everything but elements
                    case XmlNodeType.Element:
                        if (!reader.IsEmptyElement)
                        {
                            if (reader.Name.Equals(Serialization.RelationTypeUnit)
                                || reader.Name.Equals(Serialization.RelationTypeComposite))
                            {
                                var relationTypeIdString = reader.GetAttribute(Serialization.Id);
                                if (string.IsNullOrEmpty(relationTypeIdString))
                                {
                                    throw new Exception("Relation type has no id");
                                }

                                var relationTypeId = new Guid(relationTypeIdString);
                                var relationType = (IRelationType)this.database.MetaPopulation.Find(relationTypeId);

                                if (reader.Name.Equals(Serialization.RelationTypeUnit))
                                {
                                    if (relationType == null || relationType.RoleType.ObjectType is IComposite)
                                    {
                                        this.CantLoadUnitRole(reader, relationTypeId);
                                    }
                                    else
                                    {
                                        this.LoadUnitRelations(reader.ReadSubtree(), relationType);
                                    }
                                }
                                else if (reader.Name.Equals(Serialization.RelationTypeComposite))
                                {
                                    if (relationType == null || relationType.RoleType.ObjectType is IUnit)
                                    {
                                        this.CantLoadCompositeRole(reader, relationTypeId);
                                    }
                                    else
                                    {
                                        this.LoadCompositeRelations(reader, relationType);
                                    }
                                }
                            }
                            else
                            {
                                throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" + Serialization.Database + ">");
                            }
                        }

                        break;
                }
            }
        }

        private void LoadUnitRelations(XmlReader reader, IRelationType relationType)
        {
            reader.MoveToContent();

            var allowedClasses = new HashSet<IClass>(relationType.AssociationType.ObjectType.Classes);
            var unitRelationsByClass = new Dictionary<IClass, List<UnitRelation>>();

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // eat everything but elements
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Relation))
                        {
                            var associationIdString = reader.GetAttribute(Serialization.Association);
                            var associationId = long.Parse(associationIdString);

                            this.classByObjectId.TryGetValue(associationId, out var @class);

                            if (@class == null || !allowedClasses.Contains(@class))
                            {
                                this.CantLoadUnitRole(reader, relationType.Id);
                            }
                            else
                            {
                                if (!unitRelationsByClass.TryGetValue(@class, out var unitRelations))
                                {
                                    unitRelations = new List<UnitRelation>();
                                    unitRelationsByClass[@class] = unitRelations;
                                }

                                var value = string.Empty;
                                if (!reader.IsEmptyElement)
                                {
                                    value = reader.ReadElementContentAsString();
                                }

                                try
                                {
                                    object unit = null;
                                    if (reader.IsEmptyElement)
                                    {
                                        var unitType = (IUnit)relationType.RoleType.ObjectType;
                                        switch (unitType.UnitTag)
                                        {
                                            case UnitTags.String:
                                                unit = string.Empty;
                                                break;
                                            case UnitTags.Binary:
                                                unit = EmptyByteArray;
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        var unitType = (IUnit)relationType.RoleType.ObjectType;
                                        var unitTypeTag = unitType.UnitTag;
                                        unit = Serialization.ReadString(value, unitTypeTag);
                                    }

                                    unitRelations.Add(new UnitRelation(associationId, unit));
                                }
                                catch
                                {
                                    this.OnRelationNotLoaded(relationType.Id, associationId, value);
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" + Serialization.RelationTypeUnit + ">");
                        }

                        break;

                    case XmlNodeType.EndElement:
                        if (!reader.Name.Equals(Serialization.RelationTypeUnit))
                        {
                            throw new Exception("Expected closing element </" + Serialization.RelationTypeUnit + ">");
                        }

                        var con = this.database.ConnectionFactory.Create(this.database);
                        try
                        {
                            foreach (var kvp in unitRelationsByClass)
                            {
                                var @class = kvp.Key;
                                var unitRelations = kvp.Value;

                                var tableTypeName = this.database.Mapping.GetTableTypeName(relationType.RoleType);

                                var sql = this.database.Mapping.ProcedureNameForSetUnitRoleByRelationTypeByClass[@class][relationType];
                                var command = con.CreateCommand();
                                command.CommandText = sql;
                                command.CommandType = CommandType.StoredProcedure;

                                var sqlParameter = command.CreateParameter();
                                sqlParameter.SqlDbType = SqlDbType.Structured;
                                sqlParameter.TypeName = tableTypeName;
                                sqlParameter.ParameterName = Mapping.ParamNameForTableType;
                                sqlParameter.Value = this.database.CreateRelationTable(relationType.RoleType, unitRelations);

                                command.Parameters.Add(sqlParameter);

                                command.ExecuteNonQuery();
                            }

                            con.Commit();
                        }
                        catch
                        {
                            con.Rollback();
                        }

                        return;
                }
            }
        }

        private void LoadCompositeRelations(XmlReader reader, IRelationType relationType)
        {
            reader.MoveToContent();

            var allowedAssociationClasses = new HashSet<IClass>(relationType.AssociationType.ObjectType.Classes);
            var allowedRoleClasses = new HashSet<IClass>(((IComposite)relationType.RoleType.ObjectType).Classes);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    // eat everything but elements
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Relation))
                        {
                            var associationIdString = reader.GetAttribute(Serialization.Association);
                            var associationId = long.Parse(associationIdString);
                            
                            this.classByObjectId.TryGetValue(associationId, out var associationClass);

                            if (associationClass == null || !allowedAssociationClasses.Contains(associationClass))
                            {
                                this.CantLoadCompositeRole(reader, relationType.Id);
                            }
                            else
                            {
                                var value = string.Empty;
                                if (!reader.IsEmptyElement)
                                {
                                    value = reader.ReadElementContentAsString();

                                    var roleIdsString = value;
                                    var roleIdStringArray = roleIdsString.Split(Serialization.ObjectsSplitterCharArray);

                                    if (relationType.RoleType.IsOne && roleIdStringArray.Length != 1)
                                    {
                                        foreach (var roleId in roleIdStringArray)
                                        {
                                            this.OnRelationNotLoaded(relationType.Id, associationId, roleId);
                                        }
                                    }
                                    else
                                    {
                                        if (relationType.RoleType.IsOne)
                                        {
                                            var roleId = long.Parse(roleIdStringArray[0]);

                                            this.classByObjectId.TryGetValue(roleId, out var roleClass);

                                            if (roleClass == null || !allowedRoleClasses.Contains(roleClass))
                                            {
                                                this.OnRelationNotLoaded(relationType.Id, associationId, roleIdStringArray[0]);
                                            }
                                            else
                                            {
                                                if (relationType.RoleType.AssociationType.IsMany)
                                                {
                                                    // association.SetCompositeRoleMany2One(relationType.RoleType, role.GetObject());
                                                }
                                                else
                                                {
                                                    // association.SetCompositeRoleOne2One(relationType.RoleType, role.GetObject());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var roleStrategies = new HashSet<Strategy>();
                                            foreach (var roleIdString in roleIdStringArray)
                                            {
                                                var roleId = long.Parse(roleIdString);

                                                this.classByObjectId.TryGetValue(roleId, out var roleClass);

                                                if (roleClass == null || !allowedRoleClasses.Contains(roleClass))
                                                {
                                                    this.OnRelationNotLoaded(relationType.Id, associationId, roleId.ToString());
                                                }
                                                else
                                                {
                                                    // roleStrategies.Add(role);
                                                }
                                            }

                                            if (relationType.RoleType.AssociationType.IsMany)
                                            {
                                                // association.SetCompositeRolesMany2Many(relationType.RoleType, roleStrategies);
                                            }
                                            else
                                            {
                                                // association.SetCompositesRoleOne2Many(relationType.RoleType, roleStrategies);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Unknown child element <" + reader.Name + "> in parent element <" +
                                                Serialization.RelationTypeComposite + ">");
                        }

                        break;
                    case XmlNodeType.EndElement:
                        if (!reader.Name.Equals(Serialization.RelationTypeComposite))
                        {
                            throw new Exception("Expected closing element </" + Serialization.RelationTypeComposite +
                                                ">");
                        }

                        return;
                }
            }
        }

        private void CantLoadUnitRole(XmlReader reader, Guid relationTypeId)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Relation))
                        {
                            var a = reader.GetAttribute(Serialization.Association);
                            var value = string.Empty;

                            if (!reader.IsEmptyElement)
                            {
                                value = reader.ReadElementContentAsString();
                            }

                            this.OnRelationNotLoaded(relationTypeId, long.Parse(a), value);
                        }

                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        private void CantLoadCompositeRole(XmlReader reader, Guid relationTypeId)
        {
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.Equals(Serialization.Relation))
                        {
                            var associationIdString = reader.GetAttribute(Serialization.Association);
                            var associationId = long.Parse(associationIdString);
                            if (string.IsNullOrEmpty(associationIdString))
                            {
                                throw new Exception("Association id is missing");
                            }

                            if (reader.IsEmptyElement)
                            {
                                this.OnRelationNotLoaded(relationTypeId, associationId, null);
                            }
                            else
                            {
                                var value = reader.ReadElementContentAsString();
                                var rs = value.Split(Serialization.ObjectsSplitterCharArray);
                                foreach (var r in rs)
                                {
                                    this.OnRelationNotLoaded(relationTypeId, associationId, r);
                                }
                            }
                        }

                        break;
                    case XmlNodeType.EndElement:
                        return;
                }
            }
        }

        #region Load Errors
        private void OnObjectNotLoaded(Guid objectTypeId, long allorsObjectId)
        {
            if (this.objectNotLoaded != null)
            {
                this.objectNotLoaded(this, new ObjectNotLoadedEventArgs(objectTypeId, allorsObjectId));
            }
            else
            {
                throw new Exception("Object not loaded: " + objectTypeId + ":" + allorsObjectId);
            }
        }

        private void OnRelationNotLoaded(Guid relationTypeId, long associationObjectId, string roleContents)
        {
            var args = new RelationNotLoadedEventArgs(relationTypeId, associationObjectId, roleContents);
            if (this.relationNotLoaded != null)
            {
                this.relationNotLoaded(this, args);
            }
            else
            {
                throw new Exception("Role not loaded: " + args);
            }
        }
        #endregion
    }
}