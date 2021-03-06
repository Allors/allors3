// <auto-generated />
// Do not edit this file, changes will be overwritten.
namespace Allors.Database.Domain
{
    public static partial class ObjectsExtensions
    {
        public static IDomainTransactionServices TransactionServices(this IObjects @this) => @this.Transaction.Services();

        public static IDomainDatabaseServices DatabaseServices(this IObjects @this) => @this.Transaction.Database.Services();
    }

    public static partial class ObjectExtensions
    {
        public static IDomainTransactionServices TransactionServices(this IObject @this) => @this.Strategy.Transaction.Services();

        public static IDomainDatabaseServices DatabaseServices(this IObject @this) => @this.Strategy.Transaction.Database.Services();
    }

    public static partial class TransactionExtensions
    {
        public static IDomainTransactionServices Services(this ITransaction @this) => ((IDomainTransactionServices)@this.Services);
    }

    public static partial class DatabaseExtensions
    {
        public static IDomainDatabaseServices Services(this IDatabase @this) => ((IDomainDatabaseServices)@this.Services);
    }
}
