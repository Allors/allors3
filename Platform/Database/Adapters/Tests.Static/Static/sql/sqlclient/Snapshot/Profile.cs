// <copyright file="Profile.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Adapters.SqlClient.Snapshot
{
    using System;
    using System.Collections.Generic;

    using Allors.Adapters;
    using Allors.Meta;
    using Caching;
    using Debug;
    using Microsoft.Extensions.DependencyInjection;

    public class Profile : SqlClient.Profile
    {
        private readonly Prefetchers prefetchers = new Prefetchers();

        private readonly DebugConnectionFactory connectionFactory;
        private readonly DefaultCacheFactory cacheFactory;

        public Profile()
        {
            var services = new ServiceCollection();
            this.ServiceProvider = services.BuildServiceProvider();
        }

        public Profile(DebugConnectionFactory connectionFactory, DefaultCacheFactory cacheFactory)
        : this()
        {
            this.connectionFactory = connectionFactory;
            this.cacheFactory = cacheFactory;
        }

        public ServiceProvider ServiceProvider { get; set; }

        public override Action[] Markers
        {
            get
            {
                var markers = new List<Action>
                {
                    () => { },
                    () => this.Session.Commit(),
                };

                if (Settings.ExtraMarkers)
                {
                    markers.Add(
                        () =>
                        {
                            foreach (var @class in this.Session.Database.MetaPopulation.Classes)
                            {
                                var prefetchPolicy = this.prefetchers[@class];
                                this.Session.Prefetch(prefetchPolicy, this.Session.Extent(@class).ToArray());
                            }
                        });
                }

                return markers.ToArray();
            }
        }

        protected override string ConnectionString
        {
            get
            {
                if (Settings.IsWindows)
                {
                    return @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true";
                }

                return "server=localhost;database=object;User Id=SA;Password=Allors2018";
            }
        }

        public IDatabase CreateDatabase(IMetaPopulation metaPopulation, bool init)
        {
            var configuration = new SqlClient.Configuration
            {
                ObjectFactory = this.CreateObjectFactory(metaPopulation),
                ConnectionString = this.ConnectionString,
                ConnectionFactory = this.connectionFactory,
                CacheFactory = this.cacheFactory,
            };
            var database = new Database(this.ServiceProvider, configuration);

            if (init)
            {
                database.Init();
            }

            return database;
        }

        public override IDatabase CreatePopulation() => new Adapters.Memory.Database(this.ServiceProvider, new Adapters.Memory.Configuration { ObjectFactory = this.ObjectFactory });

        public override IDatabase CreateDatabase()
        {
            var configuration = new SqlClient.Configuration
            {
                ObjectFactory = this.ObjectFactory,
                ConnectionString = this.ConnectionString,
                ConnectionFactory = this.connectionFactory,
                CacheFactory = this.cacheFactory,
            };

            var database = new Database(this.ServiceProvider, configuration);

            return database;
        }

        protected override bool Match(ColumnTypes columnType, string dataType)
        {
            dataType = dataType.Trim().ToLowerInvariant();

            switch (columnType)
            {
                case ColumnTypes.ObjectId:
                    return dataType.Equals("int");
                case ColumnTypes.TypeId:
                    return dataType.Equals("uniqueidentifier");
                case ColumnTypes.CacheId:
                    return dataType.Equals("int");
                case ColumnTypes.Binary:
                    return dataType.Equals("varbinary");
                case ColumnTypes.Boolean:
                    return dataType.Equals("bit");
                case ColumnTypes.Decimal:
                    return dataType.Equals("decimal");
                case ColumnTypes.Float:
                    return dataType.Equals("float");
                case ColumnTypes.Integer:
                    return dataType.Equals("int");
                case ColumnTypes.String:
                    return dataType.Equals("nvarchar");
                case ColumnTypes.Unique:
                    return dataType.Equals("uniqueidentifier");
                default:
                    throw new Exception("Unsupported columntype " + columnType);
            }
        }
    }
}