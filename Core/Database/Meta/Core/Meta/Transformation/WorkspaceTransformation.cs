// <copyright file="MetaPopulation.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;

    public class WorkspaceTransformation
    {
        public MetaPopulation Transform(MetaPopulation from)
        {
            var to = new MetaPopulation();

            foreach (var fromUnit in from.Units)
            {
                _ = new Unit(to, fromUnit.Id)
                {
                    UnitTag = fromUnit.UnitTag,
                    SingularName = fromUnit.SingularName
                };
            }

            static bool IsWorkspace(Composite v) => v.AssociationTypes.Any(w => w.Workspace) || v.RoleTypes.Any(w => w.Workspace) || v.MethodTypes.Any(w => w.Workspace);

            foreach (var fromInterface in from.Interfaces.Where((Func<Composite, bool>)IsWorkspace))
            {
                _ = new Interface(to, fromInterface.Id)
                {
                    SingularName = fromInterface.SingularName,
                    PluralName = fromInterface.PluralName,
                };
            }

            foreach (var fromClass in from.Classes.Where((Func<Composite, bool>)IsWorkspace))
            {
                _ = new Class(to, fromClass.Id)
                {
                    SingularName = fromClass.SingularName,
                    PluralName = fromClass.PluralName
                };
            }

            foreach (var fromInheritance in from.Inheritances.Where(v => to.Find(v.Subtype.Id) != null && to.Find(v.Supertype.Id) != null))
            {
                _ = new Inheritance(to)
                {
                    Subtype = (Composite)to.Find(fromInheritance.Subtype.Id),
                    Supertype = (Interface)to.Find(fromInheritance.Supertype.Id),
                };
            }

            foreach (var fromRelationType in from.RelationTypes.Where(v => v.Workspace))
            {
                var fromAssociationType = fromRelationType.AssociationType;
                var fromRoleType = fromRelationType.RoleType;

                var toRelationType = new RelationType(to, fromRelationType.Id, fromAssociationType.Id, fromRoleType.Id)
                {
                    AssignedMultiplicity = fromRelationType.AssignedMultiplicity,
                    IsDerived = fromRelationType.IsDerived,
                };

                var toAssociationType = toRelationType.AssociationType;
                toAssociationType.ObjectType = (Composite)to.Find(fromAssociationType.ObjectType.Id);

                var toRoleType = toRelationType.RoleType;
                toRoleType.ObjectType = (ObjectType)to.Find(fromRoleType.ObjectType.Id);
                toRoleType.SingularName = fromRoleType.SingularName;
                toRoleType.PluralName = fromRoleType.PluralName;
                toRoleType.IsRequired = fromRoleType.IsRequired;
                toRoleType.MediaType = fromRoleType.MediaType;

                if (toAssociationType.ObjectType is Interface toInterface)
                {
                    foreach (var toClass in toInterface.Classes)
                    {
                        foreach (var toConcreteRoleType in toClass.ConcreteRoleTypes)
                        {
                            var fromClass = (Class)from.Find(toClass.Id);
                            var fromConcreteRoleType = fromClass.ConcreteRoleTypes
                                .Single(v => v.RelationType.Id == toConcreteRoleType.RelationType.Id);

                            toConcreteRoleType.IsRequiredOverride = fromConcreteRoleType.IsRequiredOverride;
                        }
                    }
                }
            }

            foreach (var fromMethodType in from.MethodTypes.Where(v => v.Workspace))
            {
                var toMethodType = new MethodType(to, fromMethodType.Id)
                {
                    ObjectType = (Composite)to.Find(fromMethodType.ObjectType.Id),
                    Name = fromMethodType.Name,
                    Workspace = fromMethodType.Workspace
                };

                if (toMethodType.ObjectType is Interface toInterface)
                {
                    foreach (var toClass in toInterface.Classes)
                    {
                        foreach (var toConcreteMethodType in toClass.ConcreteMethodTypes)
                        {
                            var fromClass = (Class)from.Find(toClass.Id);
                            var fromConcreteMethodType = fromClass.ConcreteMethodTypes
                                .Single(v => v.MethodType.Id == toConcreteMethodType.MethodType.Id);
                        }
                    }
                }
            }

            return to;
        }
    }
}
