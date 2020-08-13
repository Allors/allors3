import { Component, OnDestroy, OnInit, Self, Optional, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription, combineLatest, BehaviorSubject } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { isBefore, isAfter } from 'date-fns';

import { ContextService, TestScope, MetaService, RefreshService, SingletonId, Saved, NavigationService, SearchFactory } from '@allors/angular/core';
import { PullRequest } from '@allors/protocol/system';
import { ObjectData, SaveService, AllorsMaterialDialogService } from '@allors/angular/material/core';
import {
  Organisation,
  Facility,
  ProductType,
  ProductIdentificationType,
  Settings,
  Part,
  SupplierRelationship,
  InventoryItemKind,
  SupplierOffering,
  Brand,
  Model,
  PartNumber,
  UnitOfMeasure,
  PartCategory,
  NonUnifiedPart,
  CustomOrganisationClassification,
  IndustryClassification,
  CustomerRelationship,
  InternalOrganisation,
  OrganisationRole,
  LegalForm,
  VatRegime,
  IrpfRegime,
  Currency,
  Person,
  OrganisationContactRelationship,
  Enumeration,
  OrganisationContactKind,
  PersonRole,
  Employment,
  PostalAddress,
  Country,
  Party,
  PartyContactMechanism,
  PurchaseInvoice,
  PurchaseInvoiceType,
  ContactMechanism,
  SerialisedItem,
  Ownership,
  SerialisedItemState,
} from '@allors/domain/generated';
import { Equals, Sort, And, Not, Exists } from '@allors/data/system';
import { FetcherService, InternalOrganisationId, FiltersService } from '@allors/angular/base';
import { IObject, ISessionObject } from '@allors/domain/system';
import { Meta } from '@allors/meta/generated';


@Component({
  templateUrl: './serialiseditem-create.component.html',
  providers: [ContextService]
})
export class SerialisedItemCreateComponent extends TestScope implements OnInit, OnDestroy {

  readonly m: Meta;
  serialisedItem: SerialisedItem;

  public title = 'Add Serialised Asset';

  locales: Locale[];
  ownerships: Ownership[];
  organisations: Organisation[];
  organisationFilter: SearchFactory;
  serialisedItemStates: SerialisedItemState[];
  owner: Party;
  part: Part;
  itemPart: Part;
  selectedPart: Part;

  private subscription: Subscription;
  serialisedItemAvailabilities: Enumeration[];

  constructor(
    @Self() public allors: ContextService,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: ObjectData,
    public filtersService: FiltersService,
    public dialogRef: MatDialogRef<SerialisedItemCreateComponent>,
    public metaService: MetaService,
    private refreshService: RefreshService,
    private saveService: SaveService,
    private fetcher: FetcherService,
    private internalOrganisationId: InternalOrganisationId,
  ) {
    super();

    this.m = this.metaService.m;
  }

  public ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = combineLatest(this.refreshService.refresh$, this.internalOrganisationId.observable$)
      .pipe(
        switchMap(() => {

          const pulls = [
            this.fetcher.internalOrganisation,
            this.fetcher.locales,
            pull.Party({ object: this.data.associationId }),
            pull.Ownership({ sort: new Sort(m.Ownership.Name) }),
            pull.Part({
              name: 'forPart',
              object: this.data.associationId,
              include: {
                SerialisedItems: x
              }
            }),
            pull.SerialisedItemState({
              predicate: new Equals({ propertyType: m.SerialisedItemState.IsActive, value: true }),
              sort: new Sort(m.SerialisedInventoryItemState.Name),
            }),
            pull.SerialisedItemAvailability({
              predicate: new Equals({ propertyType: m.SerialisedItemAvailability.IsActive, value: true }),
              sort: new Sort(m.SerialisedItemAvailability.Name),
            }),
          ];

          return this.allors.context
            .load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.allors.context.reset();

        const internalOrganisation = loaded.objects.InternalOrganisation as Organisation;
        const externalOwner = loaded.objects.Party as Organisation;
        this.owner = externalOwner || internalOrganisation;

        this.part = loaded.objects.forPart as Part;

        this.serialisedItemStates = loaded.collections.SerialisedItemStates as SerialisedItemState[];
        this.serialisedItemAvailabilities = loaded.collections.SerialisedItemAvailabilities as Enumeration[];
        this.ownerships = loaded.collections.Ownerships as Ownership[];
        this.locales = loaded.collections.AdditionalLocales as Locale[];

        this.serialisedItem = this.allors.context.create('SerialisedItem') as SerialisedItem;
        this.serialisedItem.AvailableForSale = false;
        this.serialisedItem.OwnedBy = this.owner;

        if (this.part) {
          this.partSelected(this.part);
        }
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public partSelected(obj: ISessionObject): void {
    if (obj) {
      const part = obj as Part;
      this.selectedPart = part;
      this.serialisedItem.Name = part.Name;

      const { pull, x } = this.metaService;

      const pulls = [
        pull.Part(
          {
            object: part,
            include: {
              SerialisedItems: x
            }
          }
        ),
      ];

      this.allors.context
        .load(new PullRequest({ pulls }))
        .subscribe((loaded) => {
          this.selectedPart = loaded.objects.Part as Part;
          this.serialisedItem.Name = this.selectedPart.Name;
        });

    } else {
      this.selectedPart = undefined;
    }
  }

  public save(): void {

    this.selectedPart.AddSerialisedItem(this.serialisedItem);

    this.allors.context
      .save()
      .subscribe(() => {
        const data: IObject = {
          id: this.serialisedItem.id,
          objectType: this.serialisedItem.objectType,
        };

        this.dialogRef.close(data);
        this.refreshService.refresh();
      },
        this.saveService.errorHandler
      );
  }
}