import { Route } from "@angular/router";
import { ProductsComponent } from "./products/products.component";
import { CartComponent } from "./cart/cart.component";
import { OrdersComponent } from "./orders/orders.component";
import { autoLoginPartialRoutesGuard } from "angular-auth-oidc-client";

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
  },
  {
    path: "orders",
    component: OrdersComponent,
    canActivate: [autoLoginPartialRoutesGuard],
  },
];
