// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyncDepthC1.cs" company="Allors bvba">
//   Copyright 2002-2016 Allors bvba.
// 
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// 
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Applications is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Domain
{
    public partial class SyncDepthC1
    {
        public void CustomOnPreDerive(ObjectOnPreDerive method)
        {
            var derivation = method.Derivation;

            if (!derivation.IsCreated(this) && derivation.IsModified(this, RelationKind.Regular))
            {
                derivation.Mark(this.SyncRootWhereSyncDepth1);
                derivation.AddDependency(this, this.SyncRootWhereSyncDepth1);
            }
        }

        public void Sync()
        {
            if (!this.ExistSyncDepth2)
            {
                this.SyncDepth2 = new SyncDepth2Builder(this.strategy.Session).Build();
            }
        }
    }
}
