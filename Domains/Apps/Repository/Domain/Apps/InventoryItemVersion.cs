namespace Allors.Repository
{
    using Attributes;

    #region Allors
    [Id("F3D6AC19-E987-4C01-B582-A4567B7818A9")]
    #endregion
    public partial interface InventoryItemVersion : Version, Deletable
    {
        #region Allors
        [Id("2C73BA77-C8EA-455D-A541-7425D01FABB4")]
        [AssociationId("D5BC1071-2E4D-4481-9144-7304529888E0")]
        [RoleId("CE7D79D7-534B-4AEF-ADB9-0EB3BFD544F3")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Workspace]
        InventoryItemVariance[] InventoryItemVariances { get; set; }

        #region Allors
        [Id("E18AD324-B38C-4603-A1A0-30D7150F5FE6")]
        [AssociationId("0E687A2F-9275-4071-8F8B-CED011A0F58D")]
        [RoleId("68F707E3-A869-4199-BE14-B6DCC3586881")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Part Part { get; set; }

        #region Allors
        [Id("86010471-E2F6-4F60-AD2D-0665644CE1F6")]
        [AssociationId("D82E39DF-FBDB-42BA-9A73-F5B0363A6746")]
        [RoleId("200E8E63-43F4-4AFA-9005-C92B343069B2")]
        #endregion
        [Derived]
        [Required]
        [Size(256)]
        [Workspace]
        string Name { get; set; }

        #region Allors
        [Id("8885CCDE-B630-450C-9B01-3D18BCAA3795")]
        [AssociationId("F1E9C583-1039-4BB9-B39E-CF295DED8A18")]
        [RoleId("C9034BBC-DCBD-4D3F-AF7E-E412B51C2E8D")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Lot Lot { get; set; }

        #region Allors
        [Id("39C9C0B0-A606-4FEB-BDDE-15255304741C")]
        [AssociationId("63F47C35-8F8F-4A00-8E69-D1C945DEB33D")]
        [RoleId("858101A6-9D80-4012-B4C3-CFA054D3A7EF")]
        #endregion
        [Derived]
        [Required]
        [Size(256)]
        [Workspace]
        string Sku { get; set; }

        #region Allors
        [Id("CF0C6FDA-8E01-47B8-9787-FB0528285877")]
        [AssociationId("80F164C3-9C75-4A9B-A3B2-07101EAF445A")]
        [RoleId("762B5A5C-AC75-4BF5-8EE9-3465148FA52D")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Derived]
        [Workspace]
        UnitOfMeasure UnitOfMeasure { get; set; }

        #region Allors
        [Id("E4746E18-462E-4F81-849B-FB06A4E79CA9")]
        [AssociationId("14628D3A-D50D-4DD8-BBA5-6475A745544B")]
        [RoleId("253180C5-B8FF-4B5F-A41F-CD824F29FD44")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Derived]
        ProductCategory[] DerivedProductCategories { get; set; }

        #region Allors
        [Id("0A32B021-6CE9-41E9-817D-366490A82271")]
        [AssociationId("FB05D2DD-DEE9-4E42-A229-629DABA98353")]
        [RoleId("41964ACB-6688-46EC-ADB9-3AA5F1E93819")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        Good Good { get; set; }

        #region Allors
        [Id("876D12FC-D379-4FAD-B6A6-2E7BE0EB22ED")]
        [AssociationId("15E77069-EA7D-41F1-9AC3-98EE12A3C13B")]
        [RoleId("0338AAA2-9F29-4797-8D7F-BB8BB0323F8C")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Required]
        [Workspace]
        Facility Facility { get; set; }
    }
}