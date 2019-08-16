﻿namespace Allors.Services
{
    using System;
    using System.Collections.Concurrent;

    using Allors.Data;
    using Allors.Domain;
    using Allors.Meta;

    public class ExtentService : IExtentService
    {
        private readonly IDatabaseService databaseService;

        private readonly ConcurrentDictionary<Guid, IExtent> extentById;

        public ExtentService(IDatabaseService databaseService)
        {
            this.databaseService = databaseService;
            this.extentById = new ConcurrentDictionary<Guid, IExtent>();
        }

        public IExtent Get(Guid id)
        {
            if (!this.extentById.TryGetValue(id, out var extent))
            {
                using (var session = this.databaseService.Database.CreateSession())
                {
                    var filter = new Filter(M.PreparedExtent.Class)
                    {
                        Predicate = new Equals(M.PreparedExtent.UniqueId.RoleType) { Value = id }
                    };

                    var preparedExtent = (PreparedExtent)filter.Build(session).First;
                    if (preparedExtent != null)
                    {
                        extent = preparedExtent.Extent;
                        this.extentById[id] = extent;
                    }
                }
            }

            return extent;
        }
    }
}
