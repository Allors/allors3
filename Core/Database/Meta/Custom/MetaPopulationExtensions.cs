// <copyright file="MetaPopulationExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Meta
{
    using System;
    using System.Linq;

    public static partial class MetaPopulationExtensions
    {
        public static Class WorkspaceClass(this MetaPopulation @this, Guid id, Origins origin, string singularName, string pluralName) =>
            new Class(@this, id)
            {
                Origin = origin,
                SingularName = singularName,
                PluralName = pluralName
            };

        public static RelationType WorkspaceRelationType(
            this MetaPopulation @this,
            Origins origin,
            Guid relationId,
            Guid associationId,
            Guid roleId,
            Composite associationObjectType,
            ObjectType roleObjectType,
            string singularName,
            string pluralName,
            Multiplicity multiplicity,
            bool isDerived,
            bool isRequired,
            string mediaType)
        {
            var relationType = new RelationType(@this, relationId, associationId, roleId)
            {
                Origin = origin,
                AssignedMultiplicity = multiplicity,
                IsDerived = isDerived,
            };

            var associationType = relationType.AssociationType;
            associationType.ObjectType = associationObjectType;

            var roleType = relationType.RoleType;
            roleType.ObjectType = roleObjectType;
            roleType.SingularName = singularName;
            roleType.PluralName = pluralName;
            roleType.IsRequired = isRequired;
            roleType.MediaType = mediaType;

            return relationType;
        }

        public static void SetupWorkspace(this MetaPopulation @this)
        {
            var workspacePerson = @this.WorkspaceClass(new Guid("D8FCFF7D-953F-4AC3-BCDC-6A6F9EDE3CC5"), Origins.Workspace, "WorkspacePerson", "WorkspacePeople");

            var workspacePersonFirstName = @this.WorkspaceRelationType(
                Origins.Workspace,
                new Guid("7703E5C7-EF1C-4449-AC79-AFB51BD84829"),
                new Guid("97EE1C41-409C-4027-87EE-36C329673BB9"),
                new Guid("FE134B92-AB96-4B5E-840F-D20D63605A25"),
                workspacePerson,
                (ObjectType)@this.Find(MetaString.Instance.Unit.Id),
                "FirstName",
                "FirstNames",
                Multiplicity.OneToOne,
                false,
                false,
                null
                );

            var workspacePersonLastName = @this.WorkspaceRelationType(
                Origins.Workspace,
                new Guid("4A9B163B-D7F5-46F2-B12D-F2DBEB93B195"),
                new Guid("A2570E69-43D6-48B5-8252-B0AB4716EC52"),
                new Guid("48628353-2E85-427F-8D0A-CB23E2EAEF5D"),
                workspacePerson,
                (ObjectType)@this.Find(MetaString.Instance.Unit.Id),
                "LastName",
                "LastName",
                Multiplicity.OneToOne,
                false,
                false,
                null
            );

            var workspacePersonDisplayName = @this.WorkspaceRelationType(
                Origins.Session,
                new Guid("EA183BD0-C9F2-4C4D-9CEB-766EBD0E1719"),
                new Guid("52051BD8-6527-4169-96BF-EA400062AE6D"),
                new Guid("E22F4D17-D025-4D3B-A0CA-E2AF8CC2B475"),
                workspacePerson,
                (ObjectType)@this.Find(MetaString.Instance.Unit.Id),
                "DisplayName",
                "DisplayNames",
                Multiplicity.OneToOne,
                true,
                false,
                null
            );

            var personDisplayName = @this.WorkspaceRelationType(
               Origins.Session,
               new Guid("E4609648-C933-4134-8875-9503A5EAE566"),
               new Guid("1BEAF36C-CB16-442A-8F90-38504834BF6F"),
               new Guid("190464CA-A401-487F-9A41-EC3A748382FE"),
               (Composite)@this.Find(M.Person.Class.Id),
               (ObjectType)@this.Find(MetaString.Instance.Unit.Id),
               "DisplayName",
               "DisplayNames",
               Multiplicity.OneToOne,
               true,
               false,
               null
               );

            var organisationDisplayName = @this.WorkspaceRelationType(
                Origins.Session,
                new Guid("9EA468AF-3701-423C-9C93-1FC2A531F1BC"),
                new Guid("11565108-3785-43B6-A0BD-5048BA695E0C"),
                new Guid("C5E95FB4-7DC8-4075-856C-F0B5D68D57C8"),
                (Composite)@this.Find(M.Organisation.Class.Id),
                (ObjectType)@this.Find(MetaString.Instance.Unit.Id),
                "DisplayName",
                "DisplayNames",
                Multiplicity.OneToOne,
                true,
                false,
                null
            );
        }
    }
}
