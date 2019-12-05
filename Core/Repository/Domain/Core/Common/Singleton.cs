// <copyright file="Singleton.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using Allors.Repository.Attributes;

    #region Allors
    [Id("313b97a5-328c-4600-9dd2-b5bc146fb13b")]
    #endregion
    public partial class Singleton : Object
    {
        #region inherited properties
        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        #endregion

        #region Allors
        [Id("9c1634ab-be99-4504-8690-ed4b39fec5bc")]
        [AssociationId("45a4205d-7c02-40d4-8d97-6d7d59e05def")]
        [RoleId("1e051b37-cf30-43ed-a623-dd2928d6d0a3")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        [Indexed]
        public Locale DefaultLocale { get; set; }

        #region Allors
        [Id("9e5a3413-ed33-474f-adf2-149ad5a80719")]
        [AssociationId("33d5d8b9-3472-48d8-ab1a-83d00d9cb691")]
        [RoleId("e75a8956-4d02-49ba-b0cf-747b7a9f350d")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Workspace]
        public Locale[] AdditionalLocales { get; set; }

        #region Allors
        [Id("615AC72B-B3DF-4057-9B7C-C8528341F5FE")]
        [AssociationId("5848B9B7-5DAC-41C0-9655-79EA24A814F6")]
        [RoleId("56DDB161-B92F-4D73-8CDE-61D4BB275DA1")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Derived]
        [Workspace]
        public Locale[] Locales { get; set; }

        #region Allors
        [Id("f16652b0-b712-43d7-8d4e-34a22487514d")]
        [AssociationId("c92466b5-55ba-496a-8880-2821f32f8f8e")]
        [RoleId("3a12d798-40c3-40e0-ba9f-9d01b1e39e89")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Workspace]
        [Indexed]
        public AutomatedAgent Guest { get; set; }

        #region Allors
        [Id("F5DB3956-2715-487B-B872-CEF97F70566B")]
        [AssociationId("2702B7E9-EDB0-4DF5-968F-1F68AEF6DCA0")]
        [RoleId("D60B0381-24B3-4EF0-97D7-CF0BFB952830")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Workspace]
        [Indexed]
        public AutomatedAgent Scheduler { get; set; }

        #region Allors
        [Id("1AEFD075-5D5C-4920-ABB6-3F1BA9F9DB34")]
        [AssociationId("B7BD78A5-01F0-46F8-9196-AE112CF3429E")]
        [RoleId("227BE09B-6608-40EE-9086-FE9FB590383F")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Indexed]
        public AccessControl CreatorsAccessControl { get; set; }

        #region Allors
        [Id("FF5E235A-9CA3-42C1-B501-C13915946779")]
        [AssociationId("A8CFEEBB-A997-45DD-BA88-C7AABCA1453B")]
        [RoleId("59DE2B56-8F38-4638-B17D-5CA83235A1DE")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Indexed]
        public AccessControl AdministratorsAccessControl { get; set; }

        #region Allors
        [Id("B302534B-0A79-4B77-B3FA-1D3AB981B802")]
        [AssociationId("0CB2364D-56F9-4472-A713-ECC1A6AB901E")]
        [RoleId("46E40245-541D-44A4-B5BB-55C340366068")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Indexed]
        public AccessControl GuestCreatorsAccessControl { get; set; }

        #region Allors
        [Id("785662B2-605D-45BB-A3B0-A972BB946EC2")]
        [AssociationId("194F2346-4762-4FC5-AEDC-EA0071BF9C62")]
        [RoleId("0FB862DA-F53E-48CD-B95E-895BC7978BED")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Indexed]
        public AccessControl GuestAccessControl { get; set; }

        #region Allors
        [Id("B2166062-84DA-449D-B34F-983A0C81BC31")]
        [AssociationId("22096B27-ED3C-4640-BB60-EB7338A779FB")]
        [RoleId("1E931D15-5137-4C6D-91ED-9CC5C3C95BEF")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        public Media LogoImage { get; set; }

        #region inherited methods

        public void OnBuild() { }

        public void OnPostBuild() { }

        public void OnInit()
        {
        }

        public void OnPreDerive() { }

        public void OnDerive() { }

        public void OnPostDerive() { }

        #endregion
    }
}
