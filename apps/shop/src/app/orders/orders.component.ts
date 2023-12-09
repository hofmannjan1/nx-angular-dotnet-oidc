import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { CurrencyPipe, DatePipe, KeyValuePipe } from "@angular/common";
import { AppStore } from "../+state";

@Component({
  selector: "shop-orders",
  standalone: true,
  imports: [KeyValuePipe, DatePipe, CurrencyPipe],
  templateUrl: "orders.component.html",
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrdersComponent {
  private appStore = inject(AppStore);

  ordersByDate = this.appStore.ordersByDate;
  ordersLoading = this.appStore.ordersLoading;
}
