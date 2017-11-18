import { AfterViewInit, ChangeDetectorRef, Component, OnDestroy } from "@angular/core";
import { FormBuilder, FormGroup } from "@angular/forms";
import { MatSnackBar } from "@angular/material";
import { Title } from "@angular/platform-browser";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, Subscription } from "rxjs/Rx";

import { TdDialogService, TdMediaService } from "@covalent/core";

import { AllorsService, ErrorService, Loaded, Scope } from "@allors";
import { And, Like, Page, Predicate, PullRequest, Query, TreeNode } from "@allors";
import { ProductCharacteristic } from "@allors";
import { MetaDomain } from "@allors";

interface SearchData {
  name: string;
}

@Component({
  templateUrl: "./productcharacteristics-overview.component.html",
})
export class ProductCharacteristicsOverviewComponent implements AfterViewInit, OnDestroy {

  public title: string = "Product Characteristics";
  public total: number;
  public searchForm: FormGroup;
  public data: ProductCharacteristic[];
  public filtered: ProductCharacteristic[];

  private refresh$: BehaviorSubject<Date>;
  private page$: BehaviorSubject<number>;

  private subscription: Subscription;
  private scope: Scope;

  constructor(
    private allors: AllorsService,
    private errorService: ErrorService,
    private formBuilder: FormBuilder,
    private titleService: Title,
    private snackBar: MatSnackBar,
    private router: Router,
    private dialogService: TdDialogService,
    public media: TdMediaService, private changeDetectorRef: ChangeDetectorRef) {

    this.scope = new Scope(allors.database, allors.workspace);
    this.refresh$ = new BehaviorSubject<Date>(undefined);

    this.searchForm = this.formBuilder.group({
      name: [""],
    });

    this.page$ = new BehaviorSubject<number>(50);

    const search$: Observable<SearchData> = this.searchForm.valueChanges
      .debounceTime(400)
      .distinctUntilChanged()
      .startWith({});

    const combined$: Observable<any> = Observable
    .combineLatest(search$, this.page$, this.refresh$)
    .scan(([previousData, previousTake, previousDate]: [SearchData, number, Date], [data, take, date]: [SearchData, number, Date]): [SearchData, number, Date] => {
      return [
        data,
        data !== previousData ? 50 : take,
        date,
      ];
    }, [] as [SearchData, number, Date]);

    this.subscription = combined$
      .switchMap(([data, take]: [SearchData, number]) => {
        const m: MetaDomain = this.allors.meta;

        const predicate: And = new And();
        const predicates: Predicate[] = predicate.predicates;

        if (data.name) {
          const like: string = data.name.replace("*", "%") + "%";
          predicates.push(new Like({ roleType: m.ProductCharacteristic.Name, value: like }));
        }

        const query: Query[] = [new Query(
          {
            name: "productCharacteristics",
            objectType: m.ProductCharacteristic,
            predicate,
            page: new Page({ skip: 0, take: take }),
            include: [
              new TreeNode({ roleType: m.ProductCharacteristic.LocalisedNames }),
            ],
          })];

        return this.scope.load("Pull", new PullRequest({ query }));

      })
      .subscribe((loaded: Loaded) => {
        this.data = loaded.collections.productCharacteristics as ProductCharacteristic[];
        this.total = loaded.values.productCharacteristics_total;
      },
      (error: any) => {
        this.errorService.message(error);
        this.goBack();
      });
  }

  public more(): void {
    this.page$.next(this.data.length + 50);
  }

  public goBack(): void {
    window.history.back();
  }

  public ngAfterViewInit(): void {
    this.titleService.setTitle("Product Characteristics");
    this.media.broadcast();
    this.changeDetectorRef.detectChanges();
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public delete(productCharacteristic: ProductCharacteristic): void {
    this.dialogService
      .openConfirm({ message: "Are you sure you want to delete this characteristic?" })
      .afterClosed().subscribe((confirm: boolean) => {
        if (confirm) {
          // TODO: Logical, physical or workflow delete
        }
      });
  }

  public onView(productCharacteristic: ProductCharacteristic): void {
    this.router.navigate(["/productCharacteristic/" + productCharacteristic.id]);
  }
}
