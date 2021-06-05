// <copyright file="Connection.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Adapters.Sql.SqlClient
{
    using Microsoft.Data.SqlClient;

    public class XConnection : IConnection
    {
        internal Database Database { get; }

        internal XConnection(Database database) => this.Database = database;

        protected SqlConnection SqlConnection { get; private set; }

        protected SqlTransaction SqlTransaction { get; private set; }

        public ICommand CreateCommand()
        {
            if (this.SqlConnection == null)
            {
                this.SqlConnection = new SqlConnection(this.Database.ConnectionString);
                this.SqlConnection.Open();
                this.SqlTransaction = this.SqlConnection.BeginTransaction(this.Database.IsolationLevel ?? Database.DefaultIsolationLevel);
            }

            var sqlCommand = this.SqlConnection.CreateCommand();
            sqlCommand.Transaction = this.SqlTransaction;
            if (this.Database.CommandTimeout.HasValue)
            {
                sqlCommand.CommandTimeout = this.Database.CommandTimeout.Value;
            }

            return this.CreateCommand(this.Database.Mapping, sqlCommand);
        }

        public void Commit()
        {
            try
            {
                if (this.SqlTransaction != null)
                {
                    this.SqlTransaction.Commit();
                }
            }
            finally
            {
                this.SqlTransaction = null;

                if (this.SqlConnection != null)
                {
                    this.SqlConnection?.Close();
                }

                this.SqlConnection = null;
            }
        }

        public void Rollback()
        {
            try
            {
                if (this.SqlTransaction != null)
                {
                    this.SqlTransaction?.Rollback();
                }
            }
            finally
            {
                this.SqlTransaction = null;

                if (this.SqlConnection != null)
                {
                    this.SqlConnection?.Close();
                }

                this.SqlConnection = null;
            }
        }

        protected ICommand CreateCommand(Mapping mapping, SqlCommand sqlCommand) => new XCommand(mapping, sqlCommand);
    }
}