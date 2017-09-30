//------------------------------------------------------------------------------------------------- 
// <copyright file="EmploymentTests.cs" company="Allors bvba">
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
    using System;
    using Meta;
    using Xunit;


    public class EmploymentTests : DomainTest
    {
        private Person employee;
        private InternalOrganisation internalOrganisation;
        private Employment employment;

        public EmploymentTests()
        {
            this.employee = new PersonBuilder(this.DatabaseSession).WithLastName("slave").WithPersonRole(new PersonRoles(this.DatabaseSession).Employee).Build();

            this.employment = new EmploymentBuilder(this.DatabaseSession)
                .WithEmployee(this.employee)
                .WithFromDate(DateTime.UtcNow)
                .Build();

            this.DatabaseSession.Derive();
            this.DatabaseSession.Commit();
        }

        [Fact]
        public void GivenActiveEmployment_WhenDeriving_ThenInternalOrganisationEmployeesContainsEmployee()
        {
            var employee = new PersonBuilder(this.DatabaseSession).WithLastName("customer").WithPersonRole(new PersonRoles(this.DatabaseSession).Customer).Build();
            var employer = Singleton.Instance(this.DatabaseSession).InternalOrganisation;

            new EmploymentBuilder(this.DatabaseSession)
                .WithEmployee(employee)
                .Build();

            this.DatabaseSession.Derive();

            Assert.Contains(employee, employer.ActiveEmployees);
        }

        [Fact]
        public void GivenEmploymentToCome_WhenDeriving_ThenInternalOrganisationEmployeesDosNotContainEmployee()
        {
            var employee = new PersonBuilder(this.DatabaseSession).WithLastName("customer").WithPersonRole(new PersonRoles(this.DatabaseSession).Customer).Build();
            var employer = Singleton.Instance(this.DatabaseSession).InternalOrganisation;

            new EmploymentBuilder(this.DatabaseSession)
                .WithEmployee(employee)
                .WithFromDate(DateTime.UtcNow.AddDays(1))
                .Build();

            this.DatabaseSession.Derive();

            Assert.False(employer.ActiveEmployees.Contains(employee));
        }

        [Fact]
        public void GivenEmploymentThatHasEnded_WhenDeriving_ThenInternalOrganisationEmployeesDosNotContainEmployee()
        {
            var employee = new PersonBuilder(this.DatabaseSession).WithLastName("customer").WithPersonRole(new PersonRoles(this.DatabaseSession).Customer).Build();
            var employer = Singleton.Instance(this.DatabaseSession).InternalOrganisation;

            new EmploymentBuilder(this.DatabaseSession)
                .WithEmployee(employee)
                .WithFromDate(DateTime.UtcNow.AddDays(-10))
                .WithThroughDate(DateTime.UtcNow.AddDays(-1))
                .Build();

            this.DatabaseSession.Derive();

            Assert.False(employer.ActiveEmployees.Contains(employee));
        }

        private void InstantiateObjects(ISession session)
        {
            this.employee = (Person)session.Instantiate(this.employee);
            this.internalOrganisation = (InternalOrganisation)session.Instantiate(this.internalOrganisation);
            this.employment = (Employment)session.Instantiate(this.employment);
        }
    }
}
