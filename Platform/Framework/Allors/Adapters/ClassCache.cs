// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClassCache.cs" company="Allors bvba">
//   Copyright 2002-2016 Allors bvba.
// 
// Dual Licensed under
//   a) the Lesser General Public Licence v3 (LGPL)
//   b) the Allors License
// 
// The LGPL License is included in the file lgpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Allors.Adapters
{
    using System.Collections.Generic;
    using Allors.Meta;

    public class ClassCache : IClassCache
    {
        private Dictionary<long, IClass> classByObject;

        public ClassCache()
        {
            this.classByObject = new Dictionary<long, IClass>();
        }

        public bool TryGet(long objectId, out IClass @class)
        {
            return this.classByObject.TryGetValue(objectId, out @class);
        }

        public void Set(long objectId, IClass @class)
        {
            this.classByObject[objectId] = @class;
        }

        public void Invalidate()
        {
            this.classByObject = new Dictionary<long, IClass>();
        }

        public void Invalidate(long[] objectsToInvalidate)
        {
            foreach (var objectToInvalidate in objectsToInvalidate)
            {
                this.classByObject.Remove(objectToInvalidate);
            }
        }
    }
}