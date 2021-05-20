// <copyright file="IDatabase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System.Threading.Tasks;
    using Data;
    using Meta;
    using System.Collections.Generic;

    public interface ISession
    {
        IWorkspace Workspace { get; }

        ISessionLifecycle Lifecycle { get; }

        T Create<T>() where T : class, IObject;

        T Create<T>(IClass @class) where T : class, IObject;

        T Get<T>(IObject @object) where T : IObject;

        T Get<T>(T @object) where T : IObject;

        T Get<T>(long? id) where T : IObject;

        T Get<T>(long id) where T : IObject;

        T Get<T>(string idAsString) where T : IObject;

        IEnumerable<T> Get<T>(IEnumerable<IObject> objects) where T : IObject;

        IEnumerable<T> Get<T>(IEnumerable<T> objects) where T : IObject;

        IEnumerable<T> Get<T>(IEnumerable<long> identities) where T : IObject;

        IEnumerable<T> Get<T>(IEnumerable<string> responseVersionErrors) where T : IObject;

        IEnumerable<T> GetAll<T>() where T : IObject;

        IEnumerable<T> GetAll<T>(IComposite objectType) where T : IObject;

        Task<IInvokeResult> Invoke(Method method, InvokeOptions options = null);

        Task<IInvokeResult> Invoke(Method[] methods, InvokeOptions options = null);

        Task<IPullResult> Pull(params Pull[] pulls);

        Task<IPullResult> Pull(Procedure procedure, params Pull[] pulls);

        Task<IPushResult> Push();

        IChangeSet Checkpoint();
    }
}
