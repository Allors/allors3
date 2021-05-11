// <copyright file="Permission.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using System.Linq;
    using System.Text;

    using Meta;
    using Database.Security;

    public partial class ReadPermission : IReadPermission
    {
        IClass IPermission.Class => this.Class;
        public Class Class
        {
            get => (Class)this.Strategy.Transaction.Database.MetaPopulation.FindById(this.ClassPointer);

            set
            {
                if (value == null)
                {
                    this.RemoveClassPointer();
                }
                else
                {
                    this.ClassPointer = value.Id;
                }
            }
        }

        public bool ExistClass => this.Class != null;

        public bool ExistOperandType => this.ExistRelationTypePointer;

        public bool ExistOperation => true;

        public IOperandType OperandType => this.RelationType.RoleType;

        IRelationType IReadPermission.RelationType => this.RelationType;
        public RelationType RelationType
        {
            get => (RelationType)this.Strategy.Transaction.Database.MetaPopulation.FindById(this.RelationTypePointer);

            set
            {
                if (value == null)
                {
                    this.RemoveRelationTypePointer();
                }
                else
                {
                    this.RelationTypePointer = value.Id;
                }
            }
        }

        public Operations Operation => Operations.Read;

        public bool InWorkspace(string workspaceName) => this.Class.WorkspaceNames.Contains(workspaceName) && this.RelationType.WorkspaceNames.Contains(workspaceName);

        public void CoreOnPreDerive(ObjectOnPreDerive method)
        {
            var (iteration, changeSet, derivedObjects) = method;

            if (changeSet.IsCreated(this) || changeSet.HasChangedRoles(this))
            {
                foreach (Role role in this.RolesWherePermission)
                {
                    iteration.AddDependency(role, this);
                    iteration.Mark(role);
                }

                this.DatabaseContext().PermissionsCache.Clear();
            }
        }

        public override string ToString()
        {
            var toString = new StringBuilder();
            if (this.ExistOperation)
            {
                var operation = this.Operation;
                _ = toString.Append(operation);
            }
            else
            {
                _ = toString.Append("[missing operation]");
            }

            _ = toString.Append(" for ");

            _ = toString.Append(this.ExistOperandType ? this.OperandType.GetType().Name + ":" + this.OperandType : "[missing operand]");

            return toString.ToString();
        }
    }
}
