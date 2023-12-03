/**
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
import { Product, CartPosition } from "../core/models";
import { ProductService, CartService } from "../core/services";
import { createRecord } from "../core/utils";
import { AppState } from ".";

export function withAppMethods() {
  return signalStoreFeature(
    { state: type<AppState>() },
    withMethods((state) => {
      const productService = inject(ProductService);
      const cartService = inject(CartService);

      return {
        // Products
        async loadProducts(): Promise<void> {
          patchState(state, { productsLoading: true });
          const products = await productService.getProducts();
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
      };
    })
  );
}
