namespace Allors.Repository
{
    using System;

    using Attributes;

    #region Allors
    [Id("BC0A7408-9C82-40FF-A86B-0146F88FD54D")]
    #endregion
    public partial interface ISalesInvoiceItem : IInvoiceItem 
    {
        #region Allors
        [Id("0854aece-6ca1-4b8d-99a9-6d424de8dfd4")]
        [AssociationId("cebb5430-809a-4d46-bc7b-563ee72f0848")]
        [RoleId("f1f68b89-b95f-43c9-82d5-cb9eec635869")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        ProductFeature ProductFeature { get; set; }

        #region Allors
        [Id("0a93f639-a456-4318-a8fa-8d3c2a107379")]
        [AssociationId("f9476899-7bd7-472a-ae64-0a7f4610cb87")]
        [RoleId("56ce0901-621f-407f-81be-9921ad6d19be")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Required]
        [Workspace]
        SalesInvoiceItemObjectState CurrentObjectState { get; set; }

        #region Allors
        [Id("103d42a5-fdee-4689-af19-2ea4c8060de3")]
        [AssociationId("ee01bcc4-b926-444d-8982-8c56158327f1")]
        [RoleId("a1643b4c-c95e-427c-a6b8-44860bc79d6e")]
        #endregion
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal RequiredProfitMargin { get; set; }

        #region Allors
        [Id("1a18b2f1-a31e-4ec3-8981-5f65af2ff907")]
        [AssociationId("398e3c8d-1b7f-40c5-a4f1-4a086d369199")]
        [RoleId("514101cb-6833-4935-81e7-79c64b417a26")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal InitialMarkupPercentage { get; set; }

        #region Allors
        [Id("2f6e0b52-d37c-4caf-91d0-862666195247")]
        [AssociationId("898628e9-2191-4a2f-b05d-517b5ac90e5c")]
        [RoleId("6a71a9f7-f572-4cd2-b8ca-86e3c85a5d71")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal MaintainedMarkupPercentage { get; set; }

        #region Allors
        [Id("4daa5c18-85c6-49c0-8f23-8e419e44471c")]
        [AssociationId("061348dc-59a2-41d1-92bb-ccf16a1f31aa")]
        [RoleId("a037ec30-f0f4-4dda-8eb5-80a042b26399")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        Product Product { get; set; }

        #region Allors
        [Id("4f9e110d-fca8-4956-9d2f-178843eb9b9f")]
        [AssociationId("95aa4883-8bd0-4cd7-a060-4efabaef6530")]
        [RoleId("02e0ee54-d063-4b00-87be-c2d3747ef3a6")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal UnitPurchasePrice { get; set; }

        #region Allors
        [Id("5bdae88b-856d-4746-8645-9bded2a4a3bd")]
        [AssociationId("2b93a791-124c-45ac-8f3c-bf33f2dcfc13")]
        [RoleId("b303f168-96d8-478a-b42c-6b7594b8db42")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Derived]
        [Indexed]
        [Workspace]
        SalesOrderItem SalesOrderItem { get; set; }

        #region Allors
        [Id("6dd4e8ee-48ed-400d-a129-99a3a651586a")]
        [AssociationId("f99e5e01-943c-4de9-862c-c472d2d873f2")]
        [RoleId("6cb182c2-b481-4e26-869e-609990ea68b3")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Required]
        [Workspace]
        SalesInvoiceItemType SalesInvoiceItemType { get; set; }

        #region Allors
        [Id("90866201-03a1-44b2-9318-5048639b58c8")]
        [AssociationId("0618fddc-dee4-4cd4-9d4d-b7356be9dc65")]
        [RoleId("d61277d3-b916-4783-9de0-48f9eb6808c4")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        Person SalesRep { get; set; }

        #region Allors
        [Id("a04f506f-7ac9-4ab9-8f3f-1aba1ae76a67")]
        [AssociationId("7774b9a7-e842-4b3d-b608-5d039b0811fb")]
        [RoleId("b7b589f5-59f2-4004-862f-0fb6c790137d")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal InitialProfitMargin { get; set; }

        #region Allors
        [Id("ba9acc7e-635d-4387-98eb-67ea26e9e2db")]
        [AssociationId("0198d048-f14e-419d-ac2f-1f7f8e2d0bbc")]
        [RoleId("7d6f6274-24bb-47e4-892b-ce95cd197d77")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal MaintainedProfitMargin { get; set; }

        #region Allors
        [Id("bd485f1f-6937-4270-8695-6f9a50e671c3")]
        [AssociationId("4314e405-2692-4cda-9617-804b43d7090f")]
        [RoleId("b8ab5103-31c0-41cb-b6a0-e8f3e18a7945")]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Indexed]
        TimeEntry[] TimeEntries { get; set; }

        #region Allors
        [Id("bfd8c2d5-57f9-4650-97ae-2f2b1819b3a9")]
        [AssociationId("6dbc805e-2360-49ef-bdd5-644a454cae40")]
        [RoleId("b6e9179b-b7d8-4ad8-9aee-0ca3adef40af")]
        #endregion
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal RequiredMarkupPercentage { get; set; }
    }
}