import { Component, Self, OnInit, HostBinding } from '@angular/core';
import { PanelService, NavigationService, RefreshService, ErrorService, Action, MetaService } from '../../../../../../angular';
import { PartyRelationship } from '../../../../../../domain';
import { Meta } from '../../../../../../meta';
import { DeleteService, TableRow, Table, CreateData, EditService, EditData } from '../../../../..';
import * as moment from 'moment';

interface Row extends TableRow {
  object: PartyRelationship;
  type: string;
  parties: string;
  from: string;
  through: string;
}

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'partyrelationship-overview-panel',
  templateUrl: './partyrelationship-overview-panel.component.html',
  providers: [PanelService]
})
export class PartyRelationshipOverviewPanelComponent implements OnInit {

  @HostBinding('class.expanded-panel') get expandedPanelClass() {
    return this.panel.isExpanded;
  }

  m: Meta;

  objects: PartyRelationship[];
  table: Table<Row>;

  delete: Action;
  edit: Action;

  get createData(): CreateData {
    return {
      associationId: this.panel.manager.id,
      associationObjectType: this.panel.manager.objectType,
    };
  }

  collection = 'Current';
  currentPartyRelationships: PartyRelationship[];
  inactivePartyRelationships: PartyRelationship[];
  allPartyRelationships: PartyRelationship[] = [];

  constructor(
    @Self() public panel: PanelService,
    public metaService: MetaService,
    public refreshService: RefreshService,
    public navigationService: NavigationService,
    public errorService: ErrorService,
    public deleteService: DeleteService,
    public editService: EditService
  ) {

    this.m = this.metaService.m;
  }

  ngOnInit() {

    this.panel.name = 'partyrelationship';
    this.panel.title = 'Party Relationships';
    this.panel.icon = 'contacts';
    this.panel.expandable = true;

    this.delete = this.deleteService.delete(this.panel.manager.context);
    this.edit = this.editService.edit();

    this.table = new Table({
      selection: true,
      columns: [
        { name: 'type' },
        { name: 'parties' },
        { name: 'from' },
        { name: 'through' },
      ],
      actions: [
        this.edit,
        this.delete,
      ],
      defaultAction: this.edit
    });

    const pullName = `${this.panel.name}_${this.m.PartyRelationship.name}`;
    const active = `${this.panel.name}_${this.m.PartyRelationship.name}_active`;
    const inactive = `${this.panel.name}_${this.m.PartyRelationship.name}_inactive`;

    this.panel.onPull = (pulls) => {

      const { pull, x } = this.metaService;
      const id = this.panel.manager.id;

      pulls.push(
        pull.Party({
          name: pullName,
          object: id,
          fetch: {
            PartyRelationshipsWhereParty: {
              include: {
                Parties: x
              }
            }
          }
        }),
        pull.Party({
          name: active,
          object: id,
          fetch: {
            CurrentPartyRelationships: {
              include: {
                Parties: x
              }
            }
          }
        }),
        pull.Party({
          name: inactive,
          object: id,
          fetch: {
            InactivePartyRelationships: {
              include: {
                Parties: x
              }
            }
          }
        })
      );
    };

    this.panel.onPulled = (loaded) => {
      this.objects = loaded.collections[pullName] as PartyRelationship[];

      this.currentPartyRelationships = loaded.collections[active] as PartyRelationship[];
      this.inactivePartyRelationships = loaded.collections[inactive] as PartyRelationship[];

      this.allPartyRelationships = [];

      if (this.currentPartyRelationships !== undefined) {
        this.allPartyRelationships = this.allPartyRelationships.concat(this.currentPartyRelationships);
      }

      if (this.inactivePartyRelationships !== undefined) {
        this.allPartyRelationships = this.allPartyRelationships.concat(this.inactivePartyRelationships);
      }

      if (this.objects) {
        this.table.total = loaded.values[`${pullName}_total`] || this.objects.length;
        this.refreshTable();
      }
    };
  }

  public refreshTable() {
    this.table.data = this.partyRelationships.map((v: PartyRelationship) => {
      return {
        object: v,
        type: v.objectType.name,
        parties: v.Parties.map(w => w.displayName).join(', '),
        from: moment(v.FromDate).format('L'),
        through: v.ThroughDate !== null ? moment(v.ThroughDate).format('L') : '',
      } as Row;
    });
  }

  get partyRelationships(): any {

    switch (this.collection) {
      case 'Current':
        return this.currentPartyRelationships;
      case 'Inactive':
        return this.inactivePartyRelationships;
      case 'All':
      default:
        return this.allPartyRelationships;
    }
  }
}