// <copyright file="Composite.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the ObjectType type.</summary>

namespace Allors.Meta
{
    using System.Collections.Generic;
    using System.Linq;

    public abstract partial class Composite : ObjectType, IComposite
    {
        private bool assignedIsSynced;
        private bool isSynced;

        private LazySet<Interface> derivedDirectSupertypes;
        private LazySet<Interface> derivedSupertypes;

        private LazySet<AssociationType> derivedAssociationTypes;
        private LazySet<RoleType> derivedRoleTypes;
        private LazySet<MethodType> derivedMethodTypes;

        private string xmlDoc;

        protected Composite(MetaPopulation metaPopulation)
            : base(metaPopulation)
        {
        }

        public override Origin Origin => this.AssignedOrigin;

        public Origin AssignedOrigin { get; set; }

        public string XmlDoc
        {
            get => this.xmlDoc;

            set => this.xmlDoc = !string.IsNullOrWhiteSpace(value) ? value : null;
        }

        public string XmlDocComment
        {
            get
            {
                var lines = this.xmlDoc?.Split('\n').Select(v => "   /// " + v).ToArray();
                if (lines != null && lines.Any())
                {
                    return string.Join("\n", lines);
                }

                return null;
            }
        }

        public bool AssignedIsSynced
        {
            get => this.assignedIsSynced;

            set
            {
                this.MetaPopulation.AssertUnlocked();
                this.assignedIsSynced = value;
                this.MetaPopulation.Stale();
            }
        }

        public bool IsSynced
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.isSynced;
            }
        }

        public bool ExistExclusiveClass
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.ExclusiveSubclass != null;
            }
        }

        public abstract bool ExistClass { get; }

        public bool ExistDirectSupertypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedDirectSupertypes.Count > 0;
            }
        }

        public bool ExistSupertypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedSupertypes.Count > 0;
            }
        }

        public bool ExistAssociationTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedAssociationTypes.Count > 0;
            }
        }

        public bool ExistRoleTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedAssociationTypes.Count > 0;
            }
        }

        public bool ExistMethodTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedMethodTypes.Count > 0;
            }
        }

        IClass IComposite.ExclusiveClass => this.ExclusiveSubclass;

        /// <summary>
        /// Gets the exclusive concrete subclass.
        /// </summary>
        /// <value>The exclusive concrete subclass.</value>
        public abstract Class ExclusiveSubclass { get; }

        IEnumerable<IClass> IComposite.Classes => this.Classes;

        /// <summary>
        /// Gets the root classes.
        /// </summary>
        /// <value>The root classes.</value>
        public abstract IEnumerable<Class> Classes { get; }

        /// <summary>
        /// Gets the direct super types.
        /// </summary>
        /// <value>The super types.</value>
        public IEnumerable<Interface> DirectSupertypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedDirectSupertypes;
            }
        }

        /// <summary>
        /// Gets the super types.
        /// </summary>
        /// <value>The super types.</value>
        public IEnumerable<Interface> Supertypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedSupertypes;
            }
        }

        IEnumerable<IAssociationType> IComposite.AssociationTypes => this.AssociationTypes;

        /// <summary>
        /// Gets the associations.
        /// </summary>
        /// <value>The associations.</value>
        public IEnumerable<AssociationType> AssociationTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedAssociationTypes;
            }
        }

        public IEnumerable<AssociationType> ExclusiveAssociationTypes => this.AssociationTypes.Where(associationType => this.Equals(associationType.RoleType.ObjectType)).ToArray();

        IEnumerable<IRoleType> IComposite.RoleTypes => this.RoleTypes;

        /// <summary>
        /// Gets the roles.
        /// </summary>
        /// <value>The roles.</value>
        public IEnumerable<RoleType> RoleTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedRoleTypes;
            }
        }

        public IEnumerable<RoleType> UnitRoleTypes => this.RoleTypes.Where(roleType => roleType.ObjectType.IsUnit).ToArray();

        public IEnumerable<RoleType> CompositeRoleTypes => this.RoleTypes.Where(roleType => roleType.ObjectType.IsComposite).ToArray();

        public IEnumerable<RoleType> ExclusiveRoleTypes => this.RoleTypes.Where(roleType => this.Equals(roleType.AssociationType.ObjectType)).ToArray();

        public IEnumerable<RoleType> SortedExclusiveRoleTypes => this.ExclusiveRoleTypes.OrderBy(v => v.Name);

        /// <summary>
        /// Gets the method types.
        /// </summary>
        /// <value>The method types.</value>
        public IEnumerable<MethodType> MethodTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.derivedMethodTypes;
            }
        }

        public IEnumerable<MethodType> ExclusiveMethodTypes => this.MethodTypes.Where(methodType => this.Equals(methodType.ObjectType)).ToArray();

        public IEnumerable<MethodType> InheritedMethodTypes => this.MethodTypes.Except(this.ExclusiveMethodTypes);

        public IEnumerable<RoleType> InheritedRoleTypes => this.RoleTypes.Except(this.ExclusiveRoleTypes);

        public IEnumerable<AssociationType> InheritedAssociationTypes => this.AssociationTypes.Except(this.ExclusiveAssociationTypes);

        #region Workspace

        public IEnumerable<Composite> RelatedComposites
        {
            get
            {
                this.MetaPopulation.Derive();
                return this
                    .Supertypes
                    .Union(this.RoleTypes.Where(m => m.ObjectType.IsComposite).Select(v => (Composite)v.ObjectType))
                    .Union(this.AssociationTypes.Select(v => v.ObjectType)).Distinct()
                    .Except(new[] { this }).ToArray();
            }
        }

        public IEnumerable<RoleType> ExclusiveCompositeRoleTypes
        {
            get
            {
                this.MetaPopulation.Derive();
                return this.ExclusiveRoleTypes.Where(roleType => roleType.ObjectType.IsComposite);
            }
        }

        public abstract IEnumerable<Composite> Subtypes { get; }

        public IEnumerable<RoleType> ExclusiveRoleTypesWithDatabaseOrigin => this.ExclusiveRoleTypes.Where(roleType => roleType.RelationType.HasDatabaseOrigin);

        public IEnumerable<RoleType> ExclusiveRoleTypesWithWorkspaceOrigin => this.ExclusiveRoleTypes.Where(roleType => roleType.RelationType.HasWorkspaceOrigin);

        public IEnumerable<RoleType> ExclusiveRoleTypesWithSessionOrigin => this.ExclusiveRoleTypes.Where(roleType => roleType.RelationType.HasSessionOrigin);

        public IEnumerable<AssociationType> ExclusiveAssociationTypesWithDatabaseOrigin => this.ExclusiveAssociationTypes.Where(roleType => roleType.RelationType.HasDatabaseOrigin);

        public IEnumerable<AssociationType> ExclusiveAssociationTypesWithWorkspaceOrigin => this.ExclusiveAssociationTypes.Where(roleType => roleType.RelationType.HasWorkspaceOrigin);

        public IEnumerable<AssociationType> ExclusiveAssociationTypesWithSessionOrigin => this.ExclusiveAssociationTypes.Where(roleType => roleType.RelationType.HasSessionOrigin);

        #endregion Workspace

        public bool ExistSupertype(IInterface @interface)
        {
            this.MetaPopulation.Derive();
            return this.derivedSupertypes.Contains(@interface);
        }

        public bool ExistAssociationType(IAssociationType associationType)
        {
            this.MetaPopulation.Derive();
            return this.derivedAssociationTypes.Contains(associationType);
        }

        public bool ExistRoleType(IRoleType roleType)
        {
            this.MetaPopulation.Derive();
            return this.derivedRoleTypes.Contains(roleType);
        }

        /// <summary>
        /// Contains this concrete class.
        /// </summary>
        /// <param name="objectType">
        /// The concrete class.
        /// </param>
        /// <returns>
        /// True if this contains the concrete class.
        /// </returns>
        public abstract bool IsAssignableFrom(IComposite objectType);

        /// <summary>
        /// Derive direct super type derivations.
        /// </summary>
        /// <param name="directSupertypes">The direct super types.</param>
        internal void DeriveDirectSupertypes(HashSet<Interface> directSupertypes)
        {
            directSupertypes.Clear();
            foreach (var inheritance in this.MetaPopulation.Inheritances.Where(inheritance => this.Equals(inheritance.Subtype)))
            {
                directSupertypes.Add(inheritance.Supertype);
            }

            this.derivedDirectSupertypes = new LazySet<Interface>(directSupertypes);
        }

        /// <summary>
        /// Derive super types.
        /// </summary>
        /// <param name="superTypes">The super types.</param>
        internal void DeriveSupertypes(HashSet<Interface> superTypes)
        {
            superTypes.Clear();

            this.DeriveSupertypesRecursively(this, superTypes);

            this.derivedSupertypes = new LazySet<Interface>(superTypes);
        }

        /// <summary>
        /// Derive method types.
        /// </summary>
        /// <param name="methodTypes">
        /// The method types.
        /// </param>
        internal void DeriveMethodTypes(HashSet<MethodType> methodTypes)
        {
            methodTypes.Clear();
            foreach (var methodType in this.MetaPopulation.MethodTypes.Where(m => this.Equals(m.ObjectType)))
            {
                methodTypes.Add(methodType);
            }

            foreach (var superType in this.Supertypes)
            {
                var type = superType;
                foreach (var methodType in this.MetaPopulation.MethodTypes.Where(m => type.Equals(m.ObjectType)))
                {
                    methodTypes.Add(methodType);
                }
            }

            this.derivedMethodTypes = new LazySet<MethodType>(methodTypes);
        }

        /// <summary>
        /// Derive role types.
        /// </summary>
        /// <param name="roleTypes">The role types.</param>
        /// <param name="roleTypesByAssociationObjectType">RoleTypes grouped by the ObjectType of the Association.</param>
        internal void DeriveRoleTypes(HashSet<RoleType> roleTypes, Dictionary<ObjectType, HashSet<RoleType>> roleTypesByAssociationObjectType)
        {
            roleTypes.Clear();

            if (roleTypesByAssociationObjectType.TryGetValue(this, out var classRoleTypes))
            {
                roleTypes.UnionWith(classRoleTypes);
            }

            foreach (var superType in this.Supertypes)
            {
                if (roleTypesByAssociationObjectType.TryGetValue(superType, out var superTypeRoleTypes))
                {
                    roleTypes.UnionWith(superTypeRoleTypes);
                }
            }

            this.derivedRoleTypes = new LazySet<RoleType>(roleTypes);
        }

        /// <summary>
        /// Derive association types.
        /// </summary>
        /// <param name="associations">The associations.</param>
        /// <param name="relationTypesByRoleObjectType">AssociationTypes grouped by the ObjectType of the Role.</param>
        internal void DeriveAssociationTypes(HashSet<AssociationType> associations, Dictionary<ObjectType, HashSet<AssociationType>> relationTypesByRoleObjectType)
        {
            associations.Clear();

            if (relationTypesByRoleObjectType.TryGetValue(this, out var classAssociationTypes))
            {
                associations.UnionWith(classAssociationTypes);
            }

            foreach (var superType in this.Supertypes)
            {
                if (relationTypesByRoleObjectType.TryGetValue(superType, out var interfaceAssociationTypes))
                {
                    associations.UnionWith(interfaceAssociationTypes);
                }
            }

            this.derivedAssociationTypes = new LazySet<AssociationType>(associations);
        }

        internal void DeriveIsSynced() => this.isSynced = this.assignedIsSynced || this.derivedSupertypes.Any(v => v.assignedIsSynced);

        /// <summary>
        /// Derive super types recursively.
        /// </summary>
        /// <param name="type">The type .</param>
        /// <param name="superTypes">The super types.</param>
        private void DeriveSupertypesRecursively(ObjectType type, HashSet<Interface> superTypes)
        {
            foreach (var directSupertype in this.derivedDirectSupertypes)
            {
                if (!Equals(directSupertype, type))
                {
                    superTypes.Add(directSupertype);
                    directSupertype.DeriveSupertypesRecursively(type, superTypes);
                }
            }
        }
    }
}
