import { Component, Self } from '@angular/core';
import { PanelService, NavigationService, MetaService, Invoked, RefreshService,  Action } from '../../../../../../angular';
import { PurchaseShipment, ShipmentItem, PurchaseOrder } from '../../../../../../domain';
import { Meta } from '../../../../../../meta';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Sort, Equals } from '../../../../../../../allors/framework';
import { PrintService, SaveService } from '../../../../../../material';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'purchaseshipment-overview-summary',
  templateUrl: './purchaseshipment-overview-summary.component.html',
  providers: [PanelService]
})
export class PurchaseShipmentOverviewSummaryComponent {

  m: Meta;

  shipment: PurchaseShipment;
  purchaseOrders: PurchaseOrder[] = [];
  shipmentItems: ShipmentItem[] = [];

  constructor(
    @Self() public panel: PanelService,
    public metaService: MetaService,
    public navigation: NavigationService,
    public printService: PrintService,
    public refreshService: RefreshService,
    private saveService: SaveService,
    public snackBar: MatSnackBar) {

    this.m = this.metaService.m;

    panel.name = 'summary';

    const shipmentPullName = `${panel.name}_${this.m.Shipment.name}`;

    panel.onPull = (pulls) => {
      const { m, pull, x } = this.metaService;

      pulls.push(

        pull.Shipment({
          name: shipmentPullName,
          object: this.panel.manager.id,
          include: {
            ShipmentItems: {
              Good: x,
              Part: x
            },
            ShipFromParty: x,
            ShipFromContactPerson: x,
            ShipToParty: x,
            ShipToContactPerson: x,
            ShipmentState: x,
            CreatedBy: x,
            LastModifiedBy: x,
            ShipToAddress: {
              Country: x,
            },
          }
        }),
        pull.Shipment({
          object: this.panel.manager.id,
          fetch: {
            ShipmentItems: {
              OrderShipmentsWhereShipmentItem: {
                OrderItem: {
                  OrderWhereValidOrderItem: x
                }
              }
            }
          }
        }),
      );
    };

    panel.onPulled = (loaded) => {
      this.shipment = loaded.objects[shipmentPullName] as PurchaseShipment;
      this.shipmentItems = loaded.collections[shipmentPullName] as ShipmentItem[];
      this.purchaseOrders = loaded.collections.Orders as PurchaseOrder[];
    };
  }

  public receive(): void {

    this.panel.manager.context.invoke(this.shipment.Receive)
      .subscribe((invoked: Invoked) => {
        this.panel.toggle();
        this.snackBar.open('Successfully received.', 'close', { duration: 5000 });
        this.refreshService.refresh();
      },
      this.saveService.errorHandler);
  }
}
