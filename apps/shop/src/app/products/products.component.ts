import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { AsyncPipe, CommonModule, CurrencyPipe } from "@angular/common";
import { AppStore } from "../app.store";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { map } from "rxjs";

@Component({
  selector: "shop-products",
  standalone: true,
  imports: [CommonModule, AsyncPipe, CurrencyPipe],
  templateUrl: "products.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsComponent {
  private appStore = inject(AppStore);
  private oidcSecurityService = inject(OidcSecurityService);

  isUserAuthenticated$ = this.oidcSecurityService.isAuthenticated$.pipe(
    map((x) => x.isAuthenticated)
  );

  products = this.appStore.products;
  productsLoading = this.appStore.productsLoading;

  addProductToCart(productId: number) {
    this.appStore.addProductToCart(productId);
  }
}
