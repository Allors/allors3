// <copyright file="QuoteExtensions.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    public static partial class QuoteExtensions
    {
        public static bool IsDeletable(this Quote @this)
        {
            var productQuote = @this as ProductQuote;

            if(@this is ProductQuote)
            {
                return ((@this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Created)
                  || @this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Cancelled)
                  || @this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Rejected))
                  && !@this.ExistRequest
                  && !productQuote.ExistSalesOrderWhereQuote);
            }
            else
            {
                return ((@this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Created)
                  || @this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Cancelled)
                  || @this.QuoteState.Equals(new QuoteStates(@this.Strategy.Session).Rejected))
                  && !@this.ExistRequest);
            }
        } 

        public static void AppsOnBuild(this Quote @this, ObjectOnBuild method)
        {
            if (!@this.ExistQuoteState)
            {
                @this.QuoteState = new QuoteStates(@this.Strategy.Session).Created;
            }
        }

        public static void AppsOnDerive(this Quote @this, ObjectOnDerive method)
        {
            //var session = @this.Strategy.Session;
            //var derivation = method.Derivation;

            //if (!@this.ExistIssuer)
            //{
            //    var internalOrganisations = new Organisations(session).InternalOrganisations();

            //    if (internalOrganisations.Count() == 1)
            //    {
            //        @this.Issuer = internalOrganisations.First();
            //    }
            //}

            //if (!@this.ExistQuoteNumber && @this.ExistIssuer)
            //{
            //    @this.QuoteNumber = @this.Issuer.NextQuoteNumber(session.Now().Year);
            //    (@this).SortableQuoteNumber = @this.Session().GetSingleton().SortableNumber(@this.Issuer.QuoteNumberPrefix, @this.QuoteNumber, @this.IssueDate.Year.ToString());
            //}

            //@this.Currency ??= @this.Receiver?.PreferredCurrency ?? @this.Issuer?.PreferredCurrency;

            //foreach (QuoteItem quoteItem in @this.QuoteItems)
            //{
            //    var quoteItemDerivedRoles = quoteItem;

            //    quoteItemDerivedRoles.VatRegime = quoteItem.AssignedVatRegime ?? @this.VatRegime;
            //    quoteItemDerivedRoles.VatRate = quoteItem.VatRegime?.VatRate;

            //    quoteItemDerivedRoles.IrpfRegime = quoteItem.AssignedIrpfRegime ?? @this.IrpfRegime;
            //    quoteItemDerivedRoles.IrpfRate = quoteItem.IrpfRegime?.IrpfRate;
            //}

            //@this.AddSecurityToken(new SecurityTokens(session).DefaultSecurityToken);

            //@this.Sync(derivation);
        }

        public static void AppsDelete(this Quote @this, DeletableDelete method)
        {
            var productQuote = @this as ProductQuote;
            var propasal = @this as Proposal;
            var statementOfWork = @this as StatementOfWork;

            if ((productQuote != null && productQuote.IsDeletable())
                || (propasal != null && propasal.IsDeletable)
                || (statementOfWork != null && statementOfWork.IsDeletable))
            {
                foreach (OrderAdjustment orderAdjustment in @this.OrderAdjustments)
                {
                    orderAdjustment.Delete();
                }

                foreach (QuoteItem item in @this.QuoteItems)
                {
                    item.Delete();
                }
            }
        }

        public static void AppsApprove(this Quote @this, QuoteApprove method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).InProcess;
            SetItemState(@this);
        }

        public static void AppsSend(this Quote @this, QuoteSend method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).AwaitingAcceptance;
            SetItemState(@this);
        }

        public static void AppsAccept(this Quote @this, QuoteAccept method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).Accepted;
            SetItemState(@this);
        }

        public static void AppsRevise(this Quote @this, QuoteRevise method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).Created;
            SetItemState(@this);
        }

        public static void AppsReopen(this Quote @this, QuoteReopen method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).Created;
            SetItemState(@this);
        }

        public static void AppsReject(this Quote @this, QuoteReject method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).Rejected;
            SetItemState(@this);
        }

        public static void AppsCancel(this Quote @this, QuoteCancel method)
        {
            @this.QuoteState = new QuoteStates(@this.Strategy.Session).Cancelled;
            SetItemState(@this);
        }

        public static void SetItemState(this Quote @this)
        {
            var quoteItemStates = new QuoteItemStates(@this.Strategy.Session);

            foreach (QuoteItem quoteItem in @this.QuoteItems)
            {
                if (@this.QuoteState.IsCreated)
                {
                    quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).Draft;
                }

                if (@this.QuoteState.IsCancelled)
                {
                    if (!Equals(quoteItem.QuoteItemState, quoteItemStates.Rejected))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).Cancelled;
                    }
                }

                if (@this.QuoteState.IsRejected)
                {
                    if (!Equals(quoteItem.QuoteItemState, quoteItemStates.Cancelled))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).Rejected;
                    }
                }

                if (@this.QuoteState.IsAwaitingApproval)
                {
                    if (Equals(quoteItem.QuoteItemState, quoteItemStates.Draft))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).AwaitingApproval;
                    }
                }

                if (@this.QuoteState.IsInProcess)
                {
                    if (!Equals(quoteItem.QuoteItemState, quoteItemStates.Cancelled)
                        && !Equals(quoteItem.QuoteItemState, quoteItemStates.Rejected))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).InProcess;
                    }
                }

                if (@this.QuoteState.IsAwaitingAcceptance)
                {
                    if (Equals(quoteItem.QuoteItemState, quoteItemStates.InProcess))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).AwaitingAcceptance;
                    }
                }

                if (@this.QuoteState.IsAccepted)
                {
                    if (Equals(quoteItem.QuoteItemState, quoteItemStates.AwaitingAcceptance))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).Accepted;
                    }
                }

                if (@this.QuoteState.IsOrdered)
                {
                    if (Equals(quoteItem.QuoteItemState, quoteItemStates.Accepted))
                    {
                        quoteItem.QuoteItemState = new QuoteItemStates(@this.Strategy.Session).Ordered;
                    }
                }
            }
        }

        private static void Sync(this Quote @this, IDerivation derivation)
        {
            //var QuoteDerivedRoles = @this;
        }
    }
}
