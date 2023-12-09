/*
 * ABOUT THIS FILE
 *
 * This file includes the service that communicates with the shop API's ProductsController via HTTP.
 */
import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { Product } from "../models";
import { environment } from "../../../environments/environment";

@Injectable({ providedIn: "root" })
export class ProductsService {
  private httpClient = inject(HttpClient);

  getProducts = (): Promise<Product[]> =>
    firstValueFrom(this.httpClient.get<Product[]>(`${environment.shopApiUrl}/products`));
}
