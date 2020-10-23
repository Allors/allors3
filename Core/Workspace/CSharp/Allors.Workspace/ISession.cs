// <copyright file="IDatabase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System.Threading.Tasks;
    using Protocol.Database.Invoke;
    using Allors.Workspace.Data;
    using Allors.Workspace.Meta;

    public interface ISession
    {
        IWorkspace Workspace { get; }

        ISessionStateLifecycle StateLifecycle { get; }

        T Create<T>() where T : class, IObject;

        IObject Create(IClass @class);

        T Instantiate<T>(T @object) where T : IObject;

        IObject Instantiate(long id);

        void Reset();

        void Refresh(bool merge = false);

        Task<ICallResult> Call(Method method, CallOptions options = null);

        Task<ICallResult> Call(Method[] methods, CallOptions options = null);

        Task<ICallResult> Call(string service, object args);

        Task<ILoadResult> Load(params Pull[] pulls);

        Task<ILoadResult> Load(object args, string pullService = null);

        Task<ISaveResult> Save();
    }
}