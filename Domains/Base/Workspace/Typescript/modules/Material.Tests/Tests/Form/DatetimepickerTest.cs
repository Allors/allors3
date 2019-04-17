namespace Tests
{
    using System;
    using System.Linq;

    using Allors;
    using Allors.Domain;

    using Angular;
    using Pages.Relations;

    using Xunit;

    [Collection("Test collection")]
    public class DatetimepickerTest : Test
    {
        private FormPage page;

        public DatetimepickerTest(TestFixture fixture)
            : base(fixture)
        {
            var dashboard = this.Login();
            this.page = dashboard.Sidenav.NavigateToForm();
        }


        [Fact]
        public void Populated()
        {
            var data = new DataBuilder(this.Session).Build();

            {
                // Wintertime
                var expected = new DateTime(2018, 1, 1, 12, 0, 0, DateTimeKind.Utc);
                data.DateTime = expected;
                this.Session.Commit();

                var homePage = this.page.Sidenav.NavigateToHome();
                this.page = homePage.Sidenav.NavigateToForm();

                var actual = this.page.Datetime.Value;
                Assert.Equal(expected, actual);
            }

            {
                // Summertime
                var expected = new DateTime(2018, 6, 1, 12, 0, 0, DateTimeKind.Utc);
                data.DateTime = expected;
                this.Session.Commit();

                var homePage = this.page.Sidenav.NavigateToHome();
                this.page = homePage.Sidenav.NavigateToForm();

                var actual = this.page.Datetime.Value;
                Assert.Equal(expected, actual);
            }
        }

        [Fact]
        public void Initial()
        {
            var before = new Datas(this.Session).Extent().ToArray();

            var date = new DateTime(2018, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            this.page.Datetime.Value = date;

            this.page.Save.Click();

            this.Driver.WaitForAngular();
            this.Session.Rollback();

            var after = new Datas(this.Session).Extent().ToArray();

            Assert.Equal(after.Length, before.Length + 1);

            var data = after.Except(before).First();

            Assert.True(data.ExistDateTime);
            Assert.Equal(date, data.DateTime);
        }

        [Fact]
        public void Change()
        {
            var before = new Datas(this.Session).Extent().ToArray();

            var date = new DateTime(2019, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            this.page.Datetime.Value = date;

            this.page.Save.Click();

            date = new DateTime(2019, 1, 1, 18, 0, 0, DateTimeKind.Utc);
            this.page.Datetime.Value = date;

            this.page.Save.Click();

            this.Driver.WaitForAngular();
            this.Session.Rollback();

            var after = new Datas(this.Session).Extent().ToArray();

            Assert.Equal(after.Length, before.Length + 1);

            var data = after.Except(before).First();

            Assert.Equal(date, data.DateTime);
        }

    }
}
