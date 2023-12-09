/*
 * ABOUT THIS FILE
 *
 * This file includes the service that communicates with the shop API's OrdersController via HTTP.
 */
import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { Order } from "../models";
import { environment } from "../../../environments/environment";

@Injectable({ providedIn: "root" })
export class OrdersService {
  private httpClient = inject(HttpClient);

  getOrders = (): Promise<Order[]> =>
    firstValueFrom(this.httpClient.get<Order[]>(`${environment.shopApiUrl}/orders`));
}
