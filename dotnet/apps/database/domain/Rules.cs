// <copyright file="ObjectsBase.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Domain
{
    using Derivations.Rules;
    using Meta;

    public static partial class Rules
    {
        public static Rule[] Create(MetaPopulation m) =>
            new Rule[]
            {
                // Core
                new UserNormalizedUserNameRule(m),
                new UserNormalizedUserEmailRule(m),
                new UserInUserPasswordRule(m),
                new AccessControlEffectiveUsersRule(m),
                new AccessControlEffectivePermissionsRule(m),

                // Base
                new MediaRule(m),
                new TransitionalDeniedPermissionRule(m),
                new NotificationListRule(m),

                // Apps
                new BasePriceOrderQuantityBreakRule(m),
                new BasePriceOrderValueRule(m),
                new BasePriceProductFeatureRule(m),
                new BasePriceProductRule(m),
                new DiscountComponentRule(m),
                new SurchargeComponentRule(m),
                new AccountingPeriodRule(m),
                new BankRule(m),
                new BankAccountIbanRule(m),
                new BankAccountRule(m),
                new CashRule(m),
                new GeneralLedgerAccountCostUnitAllowedRule(m),
                new GeneralLedgerAccountCostUnitRequiredRule(m),
                new GeneralLedgerAccountCostCenterAllowedRule(m),
                new GeneralLedgerAccountCostCenterRequiredRule(m),
                new GeneralLedgerAccountRule(m),
                new JournalContraAccountRule(m),
                new JournalTypeRule(m),
                new OwnBankAccountRule(m),
                new OwnCreditCardInternalOrganisationPaymentMethodsRule(m),
                new OwnCreditCardRule(m),
                new BudgetDeniedPermissionRule(m),
                new CaseDeniedPermissionRule(m),
                new CustomerReturnDeniedPermissionRule(m),
                new CustomerShipmentDeniedPermissionRule(m),
                new DropShipmentDeniedPermissionRule(m),
                new CommunicationEventDeniedPermissionRule(m),
                new EngineeringChangeDeniedPermissionRule(m),
                new NonSerialisedInventoryItemDeniedPermissionRule(m),
                new PaymentApplicationValidationRule(m),
                new PartSpecificationDeniedPermissionRule(m),
                new PickListDeniedPermissionRule(m),
                new ProductQuoteDeniedPermissionRule(m),
                new ProposalDeniedPermissionRule(m),
                new CountryRule(m),
                new CountryVatRegimesRule(m),
                new PostalAddressRule(m),
                new StoreRule(m),
                new PurchaseInvoiceDeniedPermissionRule(m),
                new PurchaseInvoiceItemRule(m),
                new PurchaseInvoiceItemStateRule(m),
                new PurchaseInvoiceItemDeniedPermissionRule(m),
                new PurchaseOrderDeniedPermissionRule(m),
                new PurchaseOrderItemDeniedPermissionRule(m),
                new PurchaseReturnDeniedPermissionRule(m),
                new PurchaseShipmentDeniedPermissionRule(m),
                new QuoteItemDeniedPermissionRule(m),
                new RequestForInformationDeniedPermissionRule(m),
                new RequestForProposalDeniedPermissionRule(m),
                new RequestForQuoteDeniedPermissionRule(m),
                new RequestItemDeniedPermissionRule(m),
                new RequirementDeniedPermissionRule(m),
                new SalesOrderItemDeniedPermissionRule(m),
                new SalesOrderDeniedPermissionRule(m),
                new SalesOrderTransferRule(m),
                new SalesOrderCanInvoiceRule(m),
                new SalesOrderCanShipRule(m),
                new SalesOrderPrintRule(m),
                new SalesOrderRule(m),
                new SalesOrderCustomerRule(m),
                new SalesOrderOrderNumberRule(m),
                new SalesOrderValidationsRule(m),
                new SalesOrderSyncSalesOrderItemsRule(m),
                new SalesOrderShipRule(m),
                new SalesOrderProvisionalPaymentMethodRule(m),
                new SalesOrderProvisionalLocaleRule(m),
                new SalesOrderProvisionalVatRegimeRule(m),
                new SalesOrderProvisionalVatClauseRule(m),
                new SalesOrderProvisionalIrpfRegimeRule(m),
                new SalesOrderProvisionalCurrencyRule(m),
                new SalesOrderProvisionalTakenByContactMechanismRule(m),
                new SalesOrderProvisionalBillToContactMechanismRule(m),
                new SalesOrderProvisionalBillToEndCustomerContactMechanismRule(m),
                new SalesOrderProvisionalShipToEndCustomerAddressRule(m),
                new SalesOrderProvisionalShipFromAddressRule(m),
                new SalesOrderProvisionalShipToAddressRule(m),
                new SalesOrderProvisionalShipmentMethodRule(m),
                new SalesOrderPriceRule(m),
                new SalesOrderStateRule(m),
                new SalesOrderItemRule(m),
                new SalesOrderItemInventoryItemRule(m),
                new SalesOrderItemValidationRule(m),
                new SalesOrderItemSalesOrderItemsByProductRule(m),
                new SalesOrderItemInvoiceItemTypeRule(m),
                new SalesOrderItemProvisionalShipFromAddressRule(m),
                new SalesOrderItemProvisionalShipToAddressRule(m),
                new SalesOrderItemProvisionalShipToPartyRule(m),
                new SalesOrderItemProvisionalDeliveryDateRule(m),
                new SalesOrderItemProvisionalVatRegimeRule(m),
                new SalesOrderItemProvisionalIrpfRegimeRule(m),
                new SalesOrderItemByProductRule(m),
                new SalesOrderItemPriceRule(m),
                new SalesOrderItemQuantitiesRule(m),
                new SalesOrderItemShipmentRule(m),
                new SalesOrderItemStateRule(m),
                new SalesOrderItemInventoryAssignmentRule(m),
                new WorkTaskRule(m),
                new WorkTaskActualHoursRule(m),
                new WorkTaskStateRule(m),
                new WorkTaskWorkEffortAssignmentRule(m),
                new WorkTaskTakenByRule(m),
                new WorkTaskExecutedByRule(m),
                new WorkTaskCanInvoiceRule(m),
                new SerialisedInventoryItemDeniedPermissionRule(m),
                new SerialisedItemPurchaseOrderRule(m),
                new SerialisedItemPurchaseInvoiceRule(m),
                new SerialisedItemPurchasePriceRule(m),
                new ShipmentItemDeniedPermissionRule(m),
                new StatementOfWorkDeniedPermissionRule(m),
                new TransferDeniedPermissionRule(m),
                new WorkTaskDeniedPermissionRule(m),
                new ShipmentRule(m),
                new SupplierOfferingRule(m),
                new SupplierOfferingExistSupplierRule(m),
                new SupplierOfferingExistCurrencyRule(m),
                new SerialisedItemCharacteristicRule(m),
                new SerialisedItemCharacteristicTypeRule(m),
                new SerialisedItemOwnerRule(m),
                new SerialisedInventoryItemRule(m),
                new SerialisedInventoryItemQuantitiesRule(m),
                new PriceComponentDerivePricedByRule(m),
                new PriceComponentRule(m),
                new PartCategoryLocalisedDescriptionsRule(m),
                new PartCategoryImageRule(m),
                new PartCategoryNameRule(m),
                new PartCategoryRule(m),
                new NonUnifiedGoodVariantsRule(m),
                new NonUnifiedGoodProductIdentificationsRule(m),
                new StatementOfWorkRule(m),
                new SurchargeComponentRule(m),
                new SerialisedItemSuppliedByRule(m),
                new SerialisedItemNameRule(m),
                new SerialisedItemPartWhereSerialisedItemRule(m),
                new SerialisedItemOwnedByPartyNameRule(m),
                new SerialisedItemRentedByPartyNameRule(m),
                new SerialisedItemOwnershipByOwnershipNameRule(m),
                new SerialisedItemSerialisedItemAvailabilityNameRule(m),
                new SerialisedItemQuoteItemWhereSerialisedItemRule(m),
                new SerialisedItemSalesOrderItemWhereSerialisedItemRule(m),
                new SerialisedItemSearchStringRule(m),
                new SerialisedItemWorkEffortFixedAssetAssignemtsWhereFixedAssetRule(m),
                new SerialisedItemSuppliedByPartyNameRule(m),
                new SerialisedItemRule(m),
                new SerialisedItemDisplayProductCategoriesRule(m),
                new SerialisedItemDeniedPermissionRule(m),
                new QuoteItemRule(m),
                new QuoteItemCreatedIrpfRegimeRule(m),
                new QuoteItemCreatedVatRegimeRule(m),
                new QuoteItemDetailsRule(m),
                new QuoteItemPriceRule(m),
                new PurchaseOrderApprovalLevel1Rule(m),
                new ProposalRule(m),
                new ProductQuoteApprovalRule(m),
                new ProductQuoteApprovalParticipantsRule(m),
                new ProductQuoteRule(m),
                new ProductQuotePrintRule(m),
                new ProductQuoteAwaitingApprovalRule(m),
                new ProductQuoteItemByProductRule(m),
                new EngagementBillToPartyRule(m),
                new EngagementPlacingPartyRule(m),
                new QuoteRule(m),
                new QuoteQuoteItemsRule(m),
                new QuoteCreatedIrpfRegimeRule(m),
                new QuoteCreatedLocalRule(m),
                new QuoteCreatedCurrencyRule(m),
                new QuoteCreatedVatRegimeRule(m),
                new QuotePriceRule(m),
                new PurchaseOrderApprovalLevel2Rule(m),
                new PurchaseReturnExistShipToAddressRule(m),
                new PurchaseReturnExistShipmentNumberRule(m),
                new PurchaseReturnRule(m),
                new PurchaseShipmentShipFromAddressRule(m),
                new PurchaseShipmentShipToPartyRule(m),
                new PurchaseShipmentStateRule(m),
                new ShipmentPackageRule(m),
                new PickListItemRule(m),
                new PickListItemQuantityPickedRule(m),
                new PickListRule(m),
                new PickListStateRule(m),
                new PackagingContentRule(m),
                new DropShipmentExistShipmentNumberRule(m),
                new DropShipmentShipToAddressRule(m),
                new DropShipmentRule(m),
                new TransferDeriveShipFromAddressRule(m),
                new TransferDeriveShipToAddressRule(m),
                new ShipmentReceiptRule(m),
                new CustomerReturnExistShipToAddressRule(m),
                new CustomerReturnExistShipmentNumberRule(m),
                new CustomerReturnRule(m),
                new DeliverableBasedServiceRule(m),
                new TimeAndMaterialsServiceRule(m),
                new GoodProductIdentificationsRule(m),
                new GoodLocalisedDescriptionRule(m),
                new GoodLocalisedNamesRule(m),
                new UnifiedGoodSearchStringRule(m),
                new UnifiedGoodRule(m),
                new NonSerialisedInventoryItemRule(m),
                new NonSerialisedInventoryItemQuantitiesRule(m),
                new NonUnifiedPartProductIdentificationsRule(m),
                new NonUnifiedPartRule(m),
                new PartSuppliedByRule(m),
                new PartQuantitiesRule(m),
                new NonUnifiedPartDeniedPermissionRule(m),
                new UnifiedGoodDeniedPermissionRule(m),
                new NonUnifiedGoodDeniedPermissionRule(m),
                new PartRule(m),
                new PartInventoryItemRule(m),
                new PartDisplayNameRule(m),
                new PartSyncInventoryItemsRule(m),
                new InventoryItemTransactionRule(m),
                new InventoryItemRule(m),
                new InventoryItemPartDisplayNameRule(m),
                new InventoryItemSearchStringRule(m),
                new CatalogueImageRule(m),
                new CatalogueLocalisedDescriptionsRule(m),
                new CatalogueLocalisedNamesRule(m),
                new SingletonLocalesRule(m),
                new PayrollPreferenceRule(m),
                new PayHistoryRule(m),
                new PhoneCommunicationRule(m),
                new ProfessionalServicesRelationshipRule(m),
                new OrganisationContactRelationshipDateRule(m),
                new OrganisationContactRelationshipPartyRule(m),
                new InternalOrganisationCustomerReturnSequenceRule(m),
                new InternalOrganisationPurchaseShipmentSequenceRule(m),
                new InternalOrganisationWorkEffortSequenceRule(m),
                new InternalOrganisationQuoteSequenceRule(m),
                new InternalOrganisationRequestSequenceRule(m),
                new InternalOrganisationInvoiceSequenceRule(m),
                new InternalOrganisationExistDefaultCollectionMethodRule(m),
                new InternalOrganisationRule(m),
                new FaceToFaceCommunicationRule(m),
                new EmploymentRule(m),
                new CommunicationTaskRule(m),
                new CommunicationEventRule(m),
                new AutomatedAgentRule(m),
                new AgreementProductApplicabilityRule(m),
                new AgreementTermRule(m),
                new EmailCommunicationRule(m),
                new OrganisationRule(m),
                new OrganisationPartyNameRule(m),
                new OrganisationContactUserGroupRule(m),
                new OrganisationSyncContactRelationshipsRule(m),
                new OrganisationDeniedPermissionRule(m),
                new PersonRule(m),
                new PersonSalutationRule(m),
                new PersonPartyNameRule(m),
                new PersonTimeSheetWorkerRule(m),
                new PersonDeniedPermissionRule(m),
                new PartyRule(m),
                new WebSiteCommunicationsRule(m),
                new CustomerRelationshipRule(m),
                new FaxCommunicationRule(m),
                new LetterCorrespondenceRule(m),
                new OrganisationRollupRule(m),
                new PartyContactMechanismRule(m),
                new SubcontractorRelationshipRule(m),
                new RequestItemRule(m),
                new RequestItemValidationRule(m),
                new RequestForQuoteRule(m),
                new RequestForQuoteRequestItemsRule(m),
                new RequestForProposalRule(m),
                new RequestForProposalDeriveRequestItemsRule(m),
                new RequestForInformationRequestItemsRule(m),
                new RequestForInformationRule(m),
                new RequestForInformationRequestItemsRule(m),
                new RequestRule(m),
                new RequestCurrencyRule(m),
                new RequestAnonymousRule(m),
                new PurchaseInvoiceRule(m),
                new PurchaseInvoiceBilledRule(m),
                new PurchaseInvoiceAmountPaidRule(m),
                new PurchaseInvoiceCreatedInvoiceItemRule(m),
                new PurchaseInvoiceCreatedCurrencyRule(m),
                new PurchaseInvoiceCreatedRule(m),
                new PurchaseInvoiceStateRule(m),
                new PurchaseInvoicePriceRule(m),
                new PurchaseInvoicePrintRule(m),
                new PurchaseInvoiceSerialisedItemRule(m),
                new PurchaseInvoiceAwaitingApprovalRule(m),
                new PurchaseOrderItemBillingsWhereOrderItemRule(m),
                new PurchaseOrderItemRule(m),
                new PaymentRule(m),
                new PurchaseInvoiceApprovalRule(m),
                new SalesInvoiceItemRule(m),
                new SalesInvoiceItemSalesInvoiceRule(m),
                new SalesInvoiceItemPaymentApplicationAmountAppliedRule(m),
                new SalesInvoiceItemInvoiceItemTypeRule(m),
                new SalesInvoiceItemAssignedIrpfRegimeRule(m),
                new SalesInvoiceItemAssignedVatRegimeRule(m),
                new SalesInvoiceItemSubTotalItemRule(m),
                new RepeatingPurchaseInvoiceRule(m),
                new RepeatingSalesInvoiceRule(m),
                new SalesInvoicePrintRule(m),
                new SalesInvoiceRule(m),
                new SalesInvoiceDueDateRule(m),
                new SalesInvoiceStoreRule(m),
                new SalesInvoiceIsRepeatingInvoiceRule(m),
                new SalesInvoiceInvoiceNumberRule(m),
                new SalesInvoiceBilledFromValidationRule(m),
                new SalesInvoicePreviousBillToCustomerRule(m),
                new SalesInvoiceCustomerRule(m),
                new SalesInvoiceBillingWorkEffortBillingRule(m),
                new SalesInvoiceBillingShipmentItemBillingRule(m),
                new SalesInvoiceBillingOrderItemBillingRule(m),
                new SalesInvoiceBillingtimeEntryBillingRule(m),
                new SalesInvoiceReadyForPostingDerivedShipToAddressRule(m),
                new SalesInvoiceReadyForPostingDerivedBillToEndCustomerContactMechanismRule(m),
                new SalesInvoiceReadyForPostingDerivedLocaleRule(m),
                new SalesInvoiceReadyForPostingDerivedBillToContactMechanismRule(m),
                new SalesInvoiceReadyForPostingDerivedCurrencyRule(m),
                new SalesInvoiceReadyForPostingRule(m),
                new SalesInvoiceReadyForPostingDerivedShipToEndCustomerAddressRule(m),
                new SalesInvoiceReadyForPostingDerivedBilledFromContactMechanismRule(m),
                new SalesInvoicePriceRule(m),
                new SalesInvoiceStateRule(m),
                new SalesInvoiceDeniedPermissionRule(m),
                new SalesInvoiceItemDeniedPermissionRule(m),
                new PartyFinancialRelationshipAmountDueRule(m),
                new PartyFinancialRelationshipOpenOrderAmountRule(m),
                new PaymentApplicationRule(m),
                new SupplierRelationshipRule(m),
                new PurchaseOrderItemIsReceivableRule(m),
                new PurchaseOrderItemCreatedIrpfRateRule(m),
                new PurchaseOrderItemCreatedDeliveryDateRule(m),
                new PurchaseOrderItemCreatedVatRegimeRule(m),
                new PurchaseOrderItemStateRule(m),
                new PurchaseOrderItemByProductRule(m),
                new PurchaseOrderStateRule(m),
                new PurchaseOrderPriceRule(m),
                new PurchaseOrderRule(m),
                new PurchaseOrderCreatedLocaleRule(m),
                new PurchaseOrderCreatedCurrencyRule(m),
                new PurchaseOrderCreatedVatRegimeRule(m),
                new PurchaseOrderCreatedIrpfRegimeRule(m),
                new PurchaseOrderCreatedShipToAddressRule(m),
                new PurchaseOrderCreatedBillToContactMechanismRule(m),
                new PurchaseOrderCreatedTakenViaContactMechanismRule(m),
                new PurchaseOrderAwaitingApprovalLevel1Rule(m),
                new PurchaseOrderAwaitingApprovalLevel2Rule(m),
                new PurchaseOrderApprovalLevel1Rule(m),
                new PurchaseOrderApprovalLevel2Rule(m),
                new PurchaseOrderStateRule(m),
                new PurchaseOrderPriceRule(m),
                new PurchaseOrderPrintRule(m),
                new ShipmentItemRule(m),
                new ShipmentItemStateRule(m),
                new CustomerShipmentExistShipmentNumberRule(m),
                new CustomerShipmentExistShipToAddressRule(m),
                new CustomerShipmentRule(m),
                new CustomerShipmentInvoiceRule(m),
                new CustomerShipmentShipmentValueRule(m),
                new CustomerShipmentShipRule(m),
                new CustomerShipmentStateRule(m),
                new OrderShipmentRule(m),
                new InvoiceItemTotalIncVatRule(m),
                new TimeEntryRule(m),
                new TimeEntryWorkerRule(m),
                new WorkEffortAssignmentRateWorkEffortRule(m),
                new WorkEffortAssignmentRateValidationRule(m),
                new WorkTaskPrintRule(m),
                new WorkEffortInventoryAssignmentSyncInventoryTransactionsRule(m),
                new WorkEffortInventoryAssignmentCostOfGoodsSoldRule(m),
                new WorkEffortInventoryAssignmentDerivedBillableQuantityRule(m),
                new WorkEffortInventoryAssignmentUnitSellingPriceRule(m),
                new WorkEffortPartyAssignmentRule(m),
                new WorkEffortPurchaseOrderItemAssignmentPurchaseOrderItemRule(m),
                new WorkEffortPurchaseOrderItemAssignmentRule(m),
                new WorkEffortPurchaseOrderItemAssignmentUnitSellingRule(m),
                new WorkEffortTotalLabourRevenueRule(m),
                new WorkEffortTotalMaterialRevenueRule(m),
                new WorkEffortTotalSubContractedRevenueRule(m),
                new WorkEffortTotalOtherRevenueRule(m),
                new WorkEffortGrandTotalRule(m),
                new WorkEffortTotalRevenueRule(m),
                new WorkEffortTypeRule(m),
                new PurchaseInvoiceApprovalParticipantsRule(m),
                new PurchaseOrderApprovalLevel1ParticipantsRule(m),
                new PurchaseOrderApprovalLevel2ParticipantsRule(m),
                new CommunicationTaskParticipantsRule(m),
                new ProductCategoryImageRule(m),
                new ProductCategoryLocalisedDescriptionRule(m),
                new ProductCategoryLocalisedNameRule(m),
                new ProductCategoryRule(m),
                new ExchangeRateRule(m),
            };
    }
}
