import { patchState, signalStore, withMethods, withState } from "@ngrx/signals";
import { inject } from "@angular/core";
import { Product } from "./core/models";
import { ProductService } from "./core/services";
import { createRecord } from "./core/utils";

type AppState = {
  products: Record<number, Product>;
  productsLoading: boolean;
};

export const AppStore = signalStore(
  { providedIn: "root" },
  // Set (initial) state.
  withState<AppState>({
    products: {},
    productsLoading: false,
  }),
  // Add methods to update the state and execute side-effects.
  withMethods((state) => {
    const productService = inject(ProductService);

    return {
      async loadProducts() {
        patchState(state, { productsLoading: true });
        const products = await productService.getProducts();
        patchState(state, {
          products: createRecord<number, Product>(products, (x) => x.id),
          productsLoading: false,
        });
      },
    };
  })
);
