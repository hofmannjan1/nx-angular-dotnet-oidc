/*
 * ABOUT THIS FILE
 *
 * This file encapsulates all methods into a custom signal store feature.
 * The @ngrx/signals methods correspond to @ngrx/store reducers and effects.
 *
 * See https://next.ngrx.io/guide/signals/signal-store
 * See https://next.ngrx.io/guide/signals/signal-store/custom-store-features
 */
import { patchState, signalStoreFeature, type, withMethods } from "@ngrx/signals";
import { inject } from "@angular/core";
import { Product, CartPosition, Order } from "../core/models";
import { ProductsService, CartService } from "../core/services";
import { createRecord } from "../core/utils";
import { AppState } from ".";
import { OrdersService } from "../core/services/orders.service";

export function withAppMethods() {
  return signalStoreFeature(
    { state: type<AppState>() },
    withMethods((state) => {
      const productsService = inject(ProductsService);
      const cartService = inject(CartService);
      const ordersService = inject(OrdersService);

      return {
        // Products
        async loadProducts(): Promise<void> {
          patchState(state, { productsLoading: true });
          const products = await productsService.getProducts();
          patchState(state, {
            products: createRecord<number, Product>(products, (x) => x.id),
            productsLoading: false,
          });
        },
        // Cart
        async loadCartPositions(): Promise<void> {
          patchState(state, { cartPositionsLoading: true });
          const cartPositions = await cartService.getCartPositions();
          patchState(state, {
            cartPositions: createRecord<number, CartPosition>(cartPositions, (x) => x.id),
            cartPositionsLoading: false,
          });
        },
        async addProductToCart(productId: number, quantity?: number): Promise<void> {
          await cartService.addProductToCart(productId, quantity);
          this.loadCartPositions();
        },
        async deleteCartPositions(ids: number[]): Promise<void> {
          await cartService.deleteCartPosition(ids);
          this.loadCartPositions();
        },
        async orderCart(): Promise<void> {
          await cartService.orderCart();
          this.loadCartPositions();
          this.loadOrders();
        },
        // Orders
        async loadOrders(): Promise<void> {
          patchState(state, { ordersLoading: true });
          const orders = await ordersService.getOrders();
          patchState(state, {
            orders: createRecord<number, Order>(orders, (x) => x.id),
            ordersLoading: false,
          });
        },
      };
    })
  );
}
