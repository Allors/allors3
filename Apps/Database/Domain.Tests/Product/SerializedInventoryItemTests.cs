//------------------------------------------------------------------------------------------------- 
// <copyright file="SerializedInventoryItemTests.cs" company="Allors bvba">
// Copyright 2002-2009 Allors bvba.
// 
// Dual Licensed under
//   a) the General Public Licence v3 (GPL)
//   b) the Allors License
// 
// The GPL License is included in the file gpl.txt.
// The Allors License is an addendum to your contract.
// 
// Allors Platform is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// For more information visit http://www.allors.com/legal
// </copyright>
// <summary>Defines the MediaTests type.</summary>
//-------------------------------------------------------------------------------------------------

namespace Allors.Domain
{
    using Meta;
    using NUnit.Framework;

    [TestFixture]
    public class SerializedInventoryItemTests : DomainTest
    {
        [Test]
        public void GivenInventoryItem_WhenDeriving_ThenRequiredRelationsMustExist()
        {
            var part = new FinishedGoodBuilder(this.DatabaseSession).WithName("part").WithManufacturerId("10101").WithSku("sku").Build();
            this.DatabaseSession.Commit();

            var builder = new SerializedInventoryItemBuilder(this.DatabaseSession);
            builder.Build();

            Assert.IsTrue(this.DatabaseSession.Derive().HasErrors);

            this.DatabaseSession.Rollback();

            builder.WithPart(part);
            builder.Build();

            Assert.IsTrue(this.DatabaseSession.Derive().HasErrors);

            this.DatabaseSession.Rollback();

            builder.WithSerialNumber("1");
            builder.Build();

            Assert.IsFalse(this.DatabaseSession.Derive().HasErrors);

            builder.WithGood(new GoodBuilder(this.DatabaseSession)
                .WithSku("10101")
                .WithLocalisedName(new LocalisedTextBuilder(this.DatabaseSession).WithText("good").WithLocale(Singleton.Instance(this.DatabaseSession).DefaultLocale).Build())
                .Build());

            builder.Build();

            Assert.IsTrue(this.DatabaseSession.Derive().HasErrors);
        }

        [Test]
        public void GivenInventoryItem_WhenBuild_ThenPostBuildRelationsMustExist()
        {
            var item = new SerializedInventoryItemBuilder(this.DatabaseSession)
                .WithPart(new FinishedGoodBuilder(this.DatabaseSession).WithName("part").WithManufacturerId("10101").Build())
                .Build();

            Assert.AreEqual(new SerializedInventoryItemObjectStates(this.DatabaseSession).Good, item.CurrentObjectState);
            Assert.AreEqual(new Warehouses(this.DatabaseSession).FindBy(M.Warehouse.Name, "facility"), item.Facility);
        }
    }
}
