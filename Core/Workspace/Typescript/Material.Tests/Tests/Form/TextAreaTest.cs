// <copyright file="TextAreaTest.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tests
{
    using src.allors.material.custom.tests.form;
    using System.Linq;

    using Allors.Domain;

    using Components;

    using Xunit;

    [Collection("Test collection")]
    public class TextAreaTest : Test
    {
        private readonly FormComponent page;

        public TextAreaTest(TestFixture fixture)
            : base(fixture)
        {
            this.Login();
            this.page = this.Sidenav.NavigateToForm();
        }

        [Fact]
        public void Initial()
        {
            var before = new Datas(this.Session).Extent().ToArray();

            this.page.TextArea.Value = "Hello";

            this.page.SAVE.Click();

            this.Driver.WaitForAngular();
            this.Session.Rollback();

            var after = new Datas(this.Session).Extent().ToArray();

            Assert.Equal(after.Length, before.Length + 1);

            var data = after.Except(before).First();

            Assert.Equal("Hello", data.TextArea);
        }
    }
}
