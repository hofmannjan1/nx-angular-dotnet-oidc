/*
 * ABOUT THIS FILE
 *
 * This files includes the state and state management using @ngrx/signals store. This is a light-
 * weight alternative to the @ngrx/store Redux store. It uses custom store features to add custom
 * computed signals and custom methods to update the state or execute side-effects.
 *
 * It is not required to place the store and related files into a `+state` folder but is is merely
 * a convention that keeps the store-related files at the top of the file tree. Inital impulse from
 * https://github.com/angular-architects.
 */
import { signalStore, withState } from "@ngrx/signals";
import { CartPosition, Order, Product } from "../core/models";
import { withAppComputed } from "./app.computed";
import { withAppMethods } from "./app.methods";

export type AppState = {
  products: Record<number, Product>;
  productsLoading: boolean;
  cartPositions: Record<number, CartPosition>;
  cartPositionsLoading: boolean;
  orders: Record<number, Order>;
  ordersLoading: boolean;
};

export const initialAppState = {
  products: {},
  productsLoading: false,
  cartPositions: {},
  cartPositionsLoading: false,
  orders: {},
  ordersLoading: false,
};

export const AppStore = signalStore(
  { providedIn: "root" },
  // Set (initial) state.
  withState<AppState>(initialAppState),
  // Add computed signals via the custom store feature.
  withAppComputed(),
  // Add methods via the custom store feature.
  withAppMethods()
);
