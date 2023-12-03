/**
 * ABOUT THIS FILE
 *
 * This file encapsulates all computed signals into a custom signal store feature.
 * The @ngrx/signals computed signals correspond to @ngrx/store selectors.
 *
 * See https://next.ngrx.io/guide/signals/signal-store
 * See https://next.ngrx.io/guide/signals/signal-store/custom-store-features
 */
import { signalStoreFeature, type, withComputed } from "@ngrx/signals";
import { computed } from "@angular/core";
import { CartPosition, Product } from "../core/models";
import { AppState } from "./app.store";

export function withAppComputed() {
  return signalStoreFeature(
    { state: type<AppState>() },
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
    }))
  );
}
