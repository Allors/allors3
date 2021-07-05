// <copyright file="DefaultDatabaseScope.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the DomainTest type.</summary>

namespace Allors.Database.Configuration
{
    using System;
    using Bogus;
    using Data;
    using Domain;
    using Domain.Derivations.Rules.Default;
    using Meta;
    using Microsoft.AspNetCore.Http;
    using Services;

    public abstract class DomainDatabaseServices : IDomainDatabaseServices
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private IDatabase database;

        private IMetaCache metaCache;

        private IClassById classById;

        private IVersionedIdByStrategy versionedIdByStrategy;

        private IPrefetchPolicyCache prefetchPolicyCache;

        private IPreparedSelects preparedSelects;

        private IPreparedExtents preparedExtents;

        private ITreeCache treeCache;

        private IPermissionsCache permissionsCache;

        private IAccessControlCache accessControlCache;

        private ITime time;

        private ICaches caches;

        private ISingletonId singletonId;

        private IPasswordHasher passwordHasher;

        private IMailer mailer;

        private IBarcodeGenerator barcodeGenerator;

        private ITemplateObjectCache templateObjectCache;

        private IDerivationFactory derivationFactory;

        private Faker faker;

        protected DomainDatabaseServices(Engine engine, IHttpContextAccessor httpContextAccessor = null)
        {
            this.Engine = engine;
            this.httpContextAccessor = httpContextAccessor;
        }

        public virtual void OnInit(IDatabase database) => this.database = database;

        public MetaPopulation M => (MetaPopulation)this.database.MetaPopulation;

        public ITransactionServices CreateTransactionServices() => new DefaultDomainTransactionServices(this.httpContextAccessor);

        public T Get<T>() =>
            typeof(T).Name switch
            {
                nameof(IMetaCache) => (T)(this.metaCache ??= new MetaCache(this.database)),
                nameof(IClassById) => (T)(this.classById ??= new ClassById()),
                nameof(IVersionedIdByStrategy) => (T)(this.versionedIdByStrategy ??= new VersionedIdByStrategy()),

                nameof(IPrefetchPolicyCache) => (T)(this.prefetchPolicyCache ??= new PrefetchPolicyCache(this)),
                nameof(IPreparedSelects) => (T)(this.preparedSelects ??= new PreparedSelects(this.database)),
                nameof(IPreparedExtents) => (T)(this.preparedExtents ??= new PreparedExtents(this.database)),
                nameof(ITreeCache) => (T)(this.treeCache ??= new TreeCache()),

                nameof(IPermissionsCache) => (T)(this.permissionsCache ??= new PermissionsCache(this, this.database)),
                nameof(IAccessControlCache) => (T)(this.accessControlCache ??= new AccessControlCache()),

                nameof(ITime) => (T)(this.time ??= new Time()),
                nameof(ICaches) => (T)(this.caches ??= new Caches()),
                nameof(ISingletonId) => (T)(this.singletonId ??= new SingletonId()),
                nameof(IPasswordHasher) => (T)(this.passwordHasher ??= this.CreatePasswordHasher()),

                nameof(IMailer) => (T)(this.mailer ??= new MailKitMailer()),
                nameof(IBarcodeGenerator) => (T)(this.barcodeGenerator ??= new ZXingBarcodeGenerator()),
                nameof(ITemplateObjectCache) => (T)(this.templateObjectCache ??= new TemplateObjectCache()),

                nameof(IDerivationFactory) => (T)(this.derivationFactory ??= this.CreateDerivationFactory()),

                nameof(Faker) => (T)(object)(this.faker ??= new Faker()),

                _ => throw new NotSupportedException($"Service {typeof(T)} not supported")
            };

        protected abstract IPasswordHasher CreatePasswordHasher();

        protected abstract IDerivationFactory CreateDerivationFactory();

        protected Engine Engine { get; }

        public void Dispose() { }
    }
}
