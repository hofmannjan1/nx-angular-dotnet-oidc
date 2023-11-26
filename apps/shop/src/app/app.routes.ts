import { Route } from "@angular/router";
import { autoLoginPartialRoutesGuard } from "angular-auth-oidc-client";
import { ProductsComponent } from "./products/products.component";
import { CartComponent } from "./cart/cart.component";
import { OrdersComponent } from "./orders/orders.component";

export const appRoutes: Route[] = [
  {
    path: "",
    pathMatch: "full",
    redirectTo: "products",
  },
  {
    path: "products",
    component: ProductsComponent,
  },
  {
    path: "cart",
    component: CartComponent,
    canActivate: [autoLoginPartialRoutesGuard],
  },
  {
    path: "orders",
    component: OrdersComponent,
    canActivate: [autoLoginPartialRoutesGuard],
  },
];
