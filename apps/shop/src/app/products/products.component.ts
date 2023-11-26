import { ChangeDetectionStrategy, Component, OnInit, inject } from "@angular/core";
import { AsyncPipe, CommonModule, CurrencyPipe } from "@angular/common";
import { AppStore } from "../app.store";

@Component({
  selector: "shop-products",
  standalone: true,
  imports: [CommonModule, AsyncPipe, CurrencyPipe],
  templateUrl: "products.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsComponent implements OnInit {
  private appStore = inject(AppStore);

  products = this.appStore.products;
  productsLoading = this.appStore.productsLoading;

  ngOnInit(): void {
    // Load products into signal store.
    this.appStore.loadProducts();
  }
}
