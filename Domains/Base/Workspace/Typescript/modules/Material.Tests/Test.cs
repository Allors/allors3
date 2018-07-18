namespace Intranet.Tests
{
    using System;
    using System.Globalization;

    using Allors;
    using Allors.Adapters.Object.SqlClient;
    using Allors.Domain;
    using Allors.Meta;
    using Allors.Services;

    using Intranet.Pages;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using OpenQA.Selenium;

    using ObjectFactory = Allors.ObjectFactory;

    public abstract class Test : IDisposable
    {
        public const string ClientUrl = "http://localhost:4200";
        public const string ServerUrl = "http://localhost:5000";

        public static readonly string DatabaseInithUrl = $"{ServerUrl}/Test/Init";
        public static readonly string DatabaseTimeShiftUrl = $"{ServerUrl}/Test/TimeShift";

        protected Test(TestFixture fixture)
        {
            // Init Browser
            this.Driver = fixture.Driver;

            // Init Server
            this.Driver.Navigate().GoToUrl(Test.DatabaseInithUrl);

            // Init Allors
            CultureInfo.CurrentUICulture = CultureInfo.CurrentCulture;

            var appConfiguration = new ConfigurationBuilder()
                .AddJsonFile(@"appSettings.json")
                .Build();
            var objectFactory = new ObjectFactory(MetaPopulation.Instance, typeof(User));

            var services = new ServiceCollection();
            services.AddAllorsTesting();
            var serviceProvider = services.BuildServiceProvider();

            var configuration = new Configuration
                                    {
                                        ConnectionString = appConfiguration["allors"],
                                        ObjectFactory = objectFactory,
                                    };

            var database = new Database(serviceProvider, configuration);

            database.Init();
            this.Session = database.CreateSession();
            new Setup(this.Session, null).Apply();
            this.Session.Commit();

            new Population(this.Session, null).Execute();

            this.Session.Commit();
        }

        public ISession Session { get; set; }

        public IWebDriver Driver { get; set; }

        public virtual void Dispose()
        {
        }

        public DashboardPage Login(string userName = "administrator")
        {
            this.Driver.Navigate().GoToUrl(Test.ClientUrl + "/login");

            var page = new LoginPage(this.Driver);
            return page.Login();
        }
    }
}