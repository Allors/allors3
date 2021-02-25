using System;
using System.Collections.Generic;
using MysticMind.PostgresEmbed;
using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

partial class Build
{
    Target PostgresDocker => _ => _
        .Executes(() =>
        {
            DockerImagePull(v => v.SetName($"postgres"));
            DockerRun(v => v
                .SetDetach(true)
                .SetImage($"postgres")
                .SetName($"pg")
                .SetPublish("5432:5432")
                .AddEnv("POSTGRES_USER=allors")
                .AddEnv("POSTGRES_PASSWORD=Password1234"));
        });

    partial class Postgres : IDisposable
    {
        private readonly PgServer pgServer;

        public Postgres()
        {
            var pgServerParams = new Dictionary<string, string> {{"timezone", "UTC"}, {"synchronous_commit", "off"},};

            this.pgServer = new PgServer(
                "10.7.1",
                "allors",
                port: 5432,
                pgServerParams: pgServerParams,
                addLocalUserAccessPermission: true,
                locale: "English_Belgium.1252");

            this.pgServer.Start();
        }

        public void Dispose()
        {
            this.pgServer.Stop();
            this.pgServer.Dispose();
        }

        public void Init(string database)
        {
            using var conn = new Npgsql.NpgsqlConnection($"Server=localhost;User Id=allors;Database=postgres");

            using var cmd =
                new Npgsql.NpgsqlCommand(
                    @$"DROP DATABASE IF EXISTS {database};
                            CREATE DATABASE {database};",
                    conn);

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
