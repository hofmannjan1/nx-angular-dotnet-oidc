/*
 * ABOUT THIS FILE
 *
 * This file includes the service that communicates with the shop API's CartController via HTTP.
 */
import { HttpClient, HttpParams } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { CartPosition } from "../models";
import { environment } from "../../../environments/environment";

@Injectable({ providedIn: "root" })
export class CartService {
  private httpClient = inject(HttpClient);

  getCartPositions = (): Promise<CartPosition[]> =>
    firstValueFrom(this.httpClient.get<CartPosition[]>(`${environment.shopApiUrl}/cart/positions`));

  addProductToCart = (productId: number, quantity?: number): Promise<any> =>
    firstValueFrom(
      this.httpClient.post(`${environment.shopApiUrl}/cart/positions`, { productId, quantity })
    );

  deleteCartPosition = (ids: number[]): Promise<any> => {
    let params = new HttpParams().appendAll({ ids: ids });

    return firstValueFrom(
      this.httpClient.delete(`${environment.shopApiUrl}/cart/positions?${params}`)
    );
  };

  orderCart = (): Promise<any> =>
    firstValueFrom(this.httpClient.post(`${environment.shopApiUrl}/cart/positions/order`, {}));
}
