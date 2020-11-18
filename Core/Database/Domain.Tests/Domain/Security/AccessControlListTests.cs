// <copyright file="AccessControlListTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain.Tests
{
    using Allors;
    using Allors.Database;
    using Allors.Database.Domain;
    using Allors.Database.Meta;
    using Xunit;

    public class AccessControlListTests : DomainTest, IClassFixture<Fixture>
    {
        public AccessControlListTests(Fixture fixture) : base(fixture) { }

        public override Config Config => new Config { SetupSecurity = true };

        [Fact]
        public void GivenAnAuthenticationPopulatonWhenCreatingAnAccessListForGuestThenPermissionIsDenied()
        {
            this.Session.Derive();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var guest = new Users(this.Session).FindBy(this.M.User.UserName, "guest@example.com");
                var acls = new DatabaseAccessControlLists(guest);
                foreach (Object aco in (IObject[])session.Extent(this.M.Organisation.ObjectType))
                {
                    // When
                    var accessList = acls[aco];

                    // Then
                    Assert.False(accessList.CanExecute(this.M.Organisation.JustDoIt));
                }

                session.Rollback();
            }
        }

        [Fact]
        public void GivenAUserAndAnAccessControlledObjectWhenGettingTheAccessListThenUserHasAccessToThePermissionsInTheRole()
        {
            var permission = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var role = new RoleBuilder(this.Session).WithName("Role").WithPermission(permission).Build();
            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            new AccessControlBuilder(this.Session).WithSubject(person).WithRole(role).Build();

            this.Session.Derive();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                this.Session.Derive();

                Assert.False(this.Session.Derive(false).HasErrors);

                var acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.True(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        [Fact]
        public void GivenAUserGroupAndAnAccessControlledObjectWhenGettingTheAccessListThenUserHasAccessToThePermissionsInTheRole()
        {
            var permission = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var role = new RoleBuilder(this.Session).WithName("Role").WithPermission(permission).Build();
            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            new UserGroupBuilder(this.Session).WithName("Group").WithMember(person).Build();

            new AccessControlBuilder(this.Session).WithSubject(person).WithRole(role).Build();

            this.Session.Derive();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                Assert.False(this.Session.Derive(false).HasErrors);

                var acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.True(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        [Fact]
        public void GivenAnotherUserAndAnAccessControlledObjectWhenGettingTheAccessListThenUserHasAccessToThePermissionsInTheRole()
        {
            var readOrganisationName = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var databaseRole = new RoleBuilder(this.Session).WithName("Role").WithPermission(readOrganisationName).Build();

            Assert.False(this.Session.Derive(false).HasErrors);

            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            var anotherPerson = new PersonBuilder(this.Session).WithFirstName("Jane").WithLastName("Doe").Build();

            this.Session.Derive();
            this.Session.Commit();

            new AccessControlBuilder(this.Session).WithSubject(anotherPerson).WithRole(databaseRole).Build();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var role = (Role)session.Instantiate(new Roles(this.Session).FindBy(this.M.Role.Name, "Role"));
                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                Assert.False(this.Session.Derive(false).HasErrors);

                var acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.False(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        [Fact]
        public void GivenAnotherUserGroupAndAnAccessControlledObjectWhenGettingTheAccessListThenUserHasAccessToThePermissionsInTheRole()
        {
            var readOrganisationName = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var databaseRole = new RoleBuilder(this.Session).WithName("Role").WithPermission(readOrganisationName).Build();

            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            new UserGroupBuilder(this.Session).WithName("Group").WithMember(person).Build();
            var anotherUserGroup = new UserGroupBuilder(this.Session).WithName("AnotherGroup").Build();

            this.Session.Derive();
            this.Session.Commit();

            new AccessControlBuilder(this.Session).WithSubjectGroup(anotherUserGroup).WithRole(databaseRole).Build();

            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var role = (Role)session.Instantiate(new Roles(this.Session).FindBy(this.M.Role.Name, "Role"));
                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                Assert.False(this.Session.Derive(false).HasErrors);

                var acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.False(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        [Fact]
        public void GivenAnAccessListWhenRemovingUserFromACLThenUserHasNoAccessToThePermissionsInTheRole()
        {
            var permission = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var role = new RoleBuilder(this.Session).WithName("Role").WithPermission(permission).Build();
            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            var person2 = new PersonBuilder(this.Session).WithFirstName("Jane").WithLastName("Doe").Build();
            new AccessControlBuilder(this.Session).WithSubject(person).WithRole(role).Build();

            this.Session.Derive();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                this.Session.Derive();

                var acl = new DatabaseAccessControlLists(person)[organisation];

                accessControl.RemoveSubject(person);
                accessControl.AddSubject(person2);

                this.Session.Derive();

                acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.False(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        [Fact]
        public void DeniedPermissions()
        {
            var readOrganisationName = this.FindPermission(this.M.Organisation.Name, Operations.Read);
            var databaseRole = new RoleBuilder(this.Session).WithName("Role").WithPermission(readOrganisationName).Build();
            var person = new PersonBuilder(this.Session).WithFirstName("John").WithLastName("Doe").Build();
            new AccessControlBuilder(this.Session).WithRole(databaseRole).WithSubject(person).Build();

            this.Session.Derive();
            this.Session.Commit();

            var sessions = new[] { this.Session };
            foreach (var session in sessions)
            {
                session.Commit();

                var organisation = new OrganisationBuilder(session).WithName("Organisation").Build();

                var token = new SecurityTokenBuilder(session).Build();
                organisation.AddSecurityToken(token);

                var role = (Role)session.Instantiate(new Roles(this.Session).FindBy(this.M.Role.Name, "Role"));
                var accessControl = (AccessControl)session.Instantiate(role.AccessControlsWhereRole.First);
                token.AddAccessControl(accessControl);

                Assert.False(this.Session.Derive(false).HasErrors);

                var acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.True(acl.CanRead(this.M.Organisation.Name));

                organisation.AddDeniedPermission(readOrganisationName);

                acl = new DatabaseAccessControlLists(person)[organisation];

                Assert.False(acl.CanRead(this.M.Organisation.Name));

                session.Rollback();
            }
        }

        private Permission FindPermission(RoleType roleType, Operations operation)
        {
            var objectType = (Class)roleType.AssociationType.ObjectType;
            return new Permissions(this.Session).Get(objectType, roleType, operation);
        }
    }
}
