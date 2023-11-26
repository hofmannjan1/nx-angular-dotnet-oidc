import { patchState, signalStore, withComputed, withMethods, withState } from "@ngrx/signals";
import { computed, inject } from "@angular/core";
import { CartPosition, Product } from "./core/models";
import { CartService, ProductService } from "./core/services";
import { createRecord } from "./core/utils";

type AppState = {
  products: Record<number, Product>;
  productsLoading: boolean;
  cartPositions: Record<number, CartPosition>;
  cartPositionsLoading: boolean;
};

export const AppStore = signalStore(
  { providedIn: "root" },
  // Set (initial) state.
  withState<AppState>({
    products: {},
    productsLoading: false,
    cartPositions: {},
    cartPositionsLoading: false,
  }),
  // Add computed signal selectors.
  withComputed((state) => ({
    cartPositionCount: computed(() => Object.values(state.cartPositions()).length),
    cartPositionsWithProducts: computed(() =>
      Object.values(state.cartPositions())
        .map((x) => ({
          ...x,
          product: Object.values(state.products()).find((y) => y.id == x.productId),
        }))
        // Use the type guard to ensure that TypeScript does not infer the product to be of type
        // `Product | undefined` but instead to be definitely of type `Product`. Intersection is
        // the easiest way to apply a type guard to one property without listing all properties.
        .filter((x): x is CartPosition & { product: Product } => x.product !== undefined)
    ),
  })),
  // Add methods to update the state and execute side-effects.
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
    };
  })
);
