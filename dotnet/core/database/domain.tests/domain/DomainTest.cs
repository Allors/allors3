// <copyright file="DomainTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the DomainTest type.</summary>

namespace Allors.Database.Domain.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using Database;
    using Adapters.Memory;
    using Domain;
    using Allors.Database.Security;
    using Configuration;
    using Meta;
    using Moq;
    using User = Domain.User;

    public class DomainTest : IDisposable
    {
        public DomainTest(Fixture fixture, bool populate = true)
        {
            var database = new Database(
                new TestDomainDatabaseServices(fixture.Engine),
                new Configuration
                {
                    ObjectFactory = new ObjectFactory(fixture.MetaPopulation, typeof(User)),
                });

            this.M = database.Services().M;

            this.Setup(database, populate);
        }

        public MetaPopulation M { get; }

        public virtual Config Config { get; } = new Config { SetupSecurity = false };

        public ITransaction Transaction { get; private set; }

        public ITime Time => this.Transaction.Database.Services().Get<ITime>();

        public IDerivationFactory DerivationFactory => this.Transaction.Database.Services().Get<IDerivationFactory>();

        public TimeSpan? TimeShift
        {
            get => this.Time.Shift;

            set => this.Time.Shift = value;
        }

        public Mock<IAccessControlLists> AclsMock
        {
            get
            {
                var aclMock = new Mock<IAccessControlList>();
                aclMock.Setup(acl => acl.CanRead(It.IsAny<IRoleType>())).Returns(true);
                var aclsMock = new Mock<IAccessControlLists>();
                aclsMock.Setup(acls => acls[It.IsAny<IObject>()]).Returns(aclMock.Object);
                return aclsMock;
            }
        }

        public void Dispose()
        {
            this.Transaction.Rollback();
            this.Transaction = null;
        }

        protected void Setup(IDatabase database, bool populate)
        {
            database.Init();

            this.Transaction = database.CreateTransaction();

            if (populate)
            {
                new Setup(this.Transaction, this.Config).Apply();
                this.Transaction.Commit();

                new TestPopulation(this.Transaction, "full").Apply();
                this.Transaction.Commit();
            }
        }

        protected Stream GetResource(string name)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(name);
        }

        protected byte[] GetResourceBytes(string name)
        {
            var assembly = this.GetType().GetTypeInfo().Assembly;
            var resource = assembly.GetManifestResourceStream(name);
            using var ms = new MemoryStream();
            resource?.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
