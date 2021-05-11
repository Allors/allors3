// <copyright file="Validation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Adapters.Npgsql
{
    using System.Collections.Generic;
    using System.Text;

    public class Validation
    {
        public HashSet<string> MissingTableNames { get; }
        public HashSet<SchemaTable> InvalidTables { get; }

        public HashSet<string> MissingTableTypeNames { get; }
        public HashSet<SchemaTableType> InvalidTableTypes { get; }

        public HashSet<string> MissingProcedureNames { get; }
        public HashSet<SchemaProcedure> InvalidProcedures { get; }

        private readonly Mapping mapping;

        public Validation(Database database)
        {
            this.Database = database;
            this.mapping = database.Mapping;
            this.Schema = new Schema(database);

            this.MissingTableNames = new HashSet<string>();
            this.InvalidTables = new HashSet<SchemaTable>();

            this.MissingTableTypeNames = new HashSet<string>();
            this.InvalidTableTypes = new HashSet<SchemaTableType>();

            this.MissingProcedureNames = new HashSet<string>();
            this.InvalidProcedures = new HashSet<SchemaProcedure>();

            this.Validate();

            this.IsValid =
                this.MissingTableNames.Count == 0 &&
                this.InvalidTables.Count == 0 &&
                this.MissingTableTypeNames.Count == 0 &&
                this.InvalidTableTypes.Count == 0 &&
                this.MissingProcedureNames.Count == 0 &&
                this.InvalidProcedures.Count == 0;
        }

        public bool IsValid { get; }

        public Database Database { get; }

        public Schema Schema { get; }

        public string Message
        {
            get
            {
                var message = new StringBuilder();

                if (this.MissingTableNames.Count > 0)
                {
                    _ = message.Append("Missing Tables:\n");
                    foreach (var missingTable in this.MissingTableNames)
                    {
                        _ = message.Append("- " + missingTable + ":\n");
                    }
                }

                if (this.InvalidTables.Count > 0)
                {
                    _ = message.Append("Invalid Tables:\n");
                    foreach (var invalidTable in this.InvalidTables)
                    {
                        _ = message.Append("- " + invalidTable.Name + ":\n");
                    }
                }

                if (this.MissingTableTypeNames.Count > 0)
                {
                    _ = message.Append("Missing Table Types:\n");
                    foreach (var missingTableType in this.MissingTableTypeNames)
                    {
                        _ = message.Append("- " + missingTableType + ":\n");
                    }
                }

                if (this.InvalidTableTypes.Count > 0)
                {
                    _ = message.Append("Invalid Table Types:\n");
                    foreach (var invalidTableType in this.InvalidTableTypes)
                    {
                        _ = message.Append("- " + invalidTableType.Name + ":\n");
                    }
                }

                if (this.MissingProcedureNames.Count > 0)
                {
                    _ = message.Append("Missing Procedures:\n");
                    foreach (var missingProcedure in this.MissingProcedureNames)
                    {
                        _ = message.Append("- " + missingProcedure + ":\n");
                    }
                }

                if (this.InvalidProcedures.Count > 0)
                {
                    _ = message.Append("Invalid Procedures:\n");
                    foreach (var invalidProcedure in this.InvalidProcedures)
                    {
                        _ = message.Append("- " + invalidProcedure.Name + ":\n");
                    }
                }

                return message.ToString();
            }
        }

        private void Validate()
        {
            // Objects Table
            var objectsTable = this.Schema.GetTable(this.Database.Mapping.TableNameForObjects);
            if (objectsTable == null)
            {
                _ = this.MissingTableNames.Add(this.mapping.TableNameForObjects);
            }
            else
            {
                if (objectsTable.ColumnByLowercaseColumnName.Count != 3)
                {
                    _ = this.InvalidTables.Add(objectsTable);
                }

                this.ValidateColumn(objectsTable, Mapping.ColumnNameForObject, Mapping.SqlTypeForObject);
                this.ValidateColumn(objectsTable, Mapping.ColumnNameForClass, Mapping.SqlTypeForClass);
                this.ValidateColumn(objectsTable, Mapping.ColumnNameForVersion, Mapping.SqlTypeForVersion);
            }

            // Object Tables
            foreach (var @class in this.Database.MetaPopulation.DatabaseClasses)
            {
                var tableName = this.mapping.TableNameForObjectByClass[@class];
                var table = this.Schema.GetTable(tableName);

                if (table == null)
                {
                    _ = this.MissingTableNames.Add(tableName);
                }
                else
                {
                    this.ValidateColumn(table, Mapping.ColumnNameForObject, Mapping.SqlTypeForObject);
                    this.ValidateColumn(table, Mapping.ColumnNameForClass, Mapping.SqlTypeForClass);

                    foreach (var associationType in @class.DatabaseAssociationTypes)
                    {
                        var relationType = associationType.RelationType;
                        var roleType = relationType.RoleType;
                        if (!(associationType.IsMany && roleType.IsMany) && relationType.ExistExclusiveDatabaseClasses
                            && roleType.IsMany)
                        {
                            this.ValidateColumn(
                                table,
                                this.Database.Mapping.ColumnNameByRelationType[relationType],
                                Mapping.SqlTypeForObject);
                        }
                    }

                    foreach (var roleType in @class.DatabaseRoleTypes)
                    {
                        var relationType = roleType.RelationType;
                        var associationType = relationType.AssociationType;
                        if (roleType.ObjectType.IsUnit)
                        {
                            this.ValidateColumn(
                                table,
                                this.Database.Mapping.ColumnNameByRelationType[relationType],
                                this.Database.Mapping.GetSqlType(relationType.RoleType));
                        }
                        else
                        {
                            if (!(associationType.IsMany && roleType.IsMany) && relationType.ExistExclusiveDatabaseClasses
                                && !roleType.IsMany)
                            {
                                this.ValidateColumn(
                                    table,
                                    this.Database.Mapping.ColumnNameByRelationType[relationType],
                                    Mapping.SqlTypeForObject);
                            }
                        }
                    }
                }
            }

            // Relation Tables
            foreach (var relationType in this.Database.MetaPopulation.DatabaseRelationTypes)
            {
                var associationType = relationType.AssociationType;
                var roleType = relationType.RoleType;

                if (!roleType.ObjectType.IsUnit &&
                    ((associationType.IsMany && roleType.IsMany) || !relationType.ExistExclusiveDatabaseClasses))
                {
                    var tableName = this.mapping.TableNameForRelationByRelationType[relationType];
                    var table = this.Schema.GetTable(tableName);

                    if (table == null)
                    {
                        _ = this.MissingTableNames.Add(tableName);
                    }
                    else
                    {
                        if (table.ColumnByLowercaseColumnName.Count != 2)
                        {
                            _ = this.InvalidTables.Add(table);
                        }

                        this.ValidateColumn(table, Mapping.ColumnNameForAssociation, Mapping.SqlTypeForObject);

                        var roleSqlType = relationType.RoleType.ObjectType.IsComposite ? Mapping.SqlTypeForObject : this.mapping.GetSqlType(relationType.RoleType);
                        this.ValidateColumn(table, Mapping.ColumnNameForRole, roleSqlType);
                    }
                }
            }

            // Procedures Tables
            foreach (var dictionaryEntry in this.mapping.ProcedureDefinitionByName)
            {
                var procedureName = dictionaryEntry.Key;
                var procedureDefinition = dictionaryEntry.Value;

                var procedure = this.Schema.GetProcedure(procedureName);

                if (procedure == null)
                {
                    _ = this.MissingProcedureNames.Add(procedureName);
                }
                else
                {
                    if (!procedure.IsDefinitionCompatible(procedureDefinition))
                    {
                        _ = this.InvalidProcedures.Add(procedure);
                    }
                }
            }

            // TODO: Primary Keys and Indeces
        }

        private void ValidateColumn(SchemaTable table, string columnName, string sqlType)
        {
            var normalizedColumnName = this.mapping.NormalizeName(columnName);
            var objectColumn = table.GetColumn(normalizedColumnName);

            if (objectColumn == null || !objectColumn.SqlType.Equals(sqlType))
            {
                _ = this.InvalidTables.Add(table);
            }
        }

        private void ValidateColumn(SchemaTableType tableType, string columnName, string sqlType)
        {
            var objectColumn = tableType.GetColumn(columnName);

            if (objectColumn == null || !objectColumn.SqlType.Equals(sqlType))
            {
                _ = this.InvalidTableTypes.Add(tableType);
            }
        }
    }
}
