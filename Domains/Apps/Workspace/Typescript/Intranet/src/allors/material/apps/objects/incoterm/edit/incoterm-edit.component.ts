import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, Self } from '@angular/core';
import { MatSnackBar } from '@angular/material';
import { ActivatedRoute, Router, UrlSegment } from '@angular/router';

import { BehaviorSubject, Observable, Subscription, combineLatest } from 'rxjs';

import { ErrorService, Field, SearchFactory, Loaded, Saved, SessionService, MetaService } from '../../../../../angular';
import { IncoTermType, SalesInvoice, SalesTerm } from '../../../../../domain';
import { Fetch, PullRequest, Sort, TreeNode, Equals } from '../../../../../framework';
import { MetaDomain } from '../../../../../meta';
import { AllorsMaterialDialogService } from '../../../../base/services/dialog';
import { switchMap } from 'rxjs/operators';

@Component({
  templateUrl: './incoterm-edit.component.html',
  providers: [SessionService]
})
export class IncoTermEditComponent implements OnInit, OnDestroy {

  public m: MetaDomain;

  public title = 'Edit Sales Invoice Incoterm';
  public subTitle: string;
  public invoice: SalesInvoice;
  public salesTerm: SalesTerm;
  public incoTermTypes: IncoTermType[];

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() private allors: SessionService,
    public metaService: MetaService,
    private errorService: ErrorService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogService: AllorsMaterialDialogService) {

    this.m = this.metaService.m;
    this.refresh$ = new BehaviorSubject<Date>(undefined);
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = combineLatest(this.route.url, this.refresh$)
      .pipe(
        switchMap(([urlSegments, date]) => {

          const id: string = this.route.snapshot.paramMap.get('id');
          const termId: string = this.route.snapshot.paramMap.get('termId');

          const pulls = [
            pull.SalesInvoice({ object: id }),
            pull.SalesTerm(
              {
                object: termId,
                include: {
                  TermType: x,
                }
              }),
            pull.IncoTermType({
              predicate: new Equals({ propertyType: m.IncoTermType.IsActive, value: true }),
              sort: [
                new Sort(m.IncoTermType.Name),
              ],
            })
          ];

          return this.allors
            .load('Pull', new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.invoice = loaded.objects.salesInvoice as SalesInvoice;
        this.salesTerm = loaded.objects.salesTerm as SalesTerm;
        this.incoTermTypes = loaded.collections.incoTermTypes as IncoTermType[];

        if (!this.salesTerm) {
          this.title = 'Add Invoice Incoterm';
          this.salesTerm = this.allors.session.create('IncoTerm') as SalesTerm;
          this.invoice.AddSalesTerm(this.salesTerm);
        }
      }, this.errorService.handler);
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public save(): void {

    this.allors
      .save()
      .subscribe((saved: Saved) => {
        this.router.navigate(['/accountsreceivable/invoice/' + this.invoice.id]);
      },
        (error: Error) => {
          this.errorService.handle(error);
        });
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public goBack(): void {
    window.history.back();
  }
}
