import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { CartPosition } from "../models";

@Injectable({ providedIn: "root" })
export class CartService {
  private httpClient = inject(HttpClient);

  async getCartPositions(): Promise<CartPosition[]> {
    return await firstValueFrom(
      this.httpClient.get<CartPosition[]>("https://localhost:7101/cart/positions")
    );
  }

  async addProductToCart(productId: number, quantity?: number): Promise<void> {
    await firstValueFrom(
      this.httpClient.post("https://localhost:7101/cart/positions", { productId, quantity })
    );
  }
}
