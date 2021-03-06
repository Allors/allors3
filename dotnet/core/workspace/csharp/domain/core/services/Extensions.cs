// <auto-generated />
// Do not edit this file, changes will be overwritten.
namespace Allors.Workspace.Domain
{
    public static partial class ObjectExtensions
    {
        public static ISessionContext SessionContext(this IObject @this) => @this.Strategy.Session.Context();

        public static IWorkspaceContext WorkspaceContext(this IObject @this) => @this.Strategy.Session.Workspace.Context();
    }

    public static partial class SessionExtensions
    {
        public static ISessionContext Context(this ISession @this) => ((ISessionContext)@this.Services);
    }

    public static partial class WorkspaceExtensions
    {
        public static IWorkspaceContext Context(this IWorkspace @this) => ((IWorkspaceContext)@this.Services);
    }
}
