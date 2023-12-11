import { CartPosition, Product } from "../core/models";

export interface CartViewModel {
  positionsWithProducts: (CartPosition & { product: Product })[];
  positionsLoading: boolean;
  positionCount: number;
  productCount: number;
  positionIds: number[];
}
