import { HttpClient } from "@angular/common/http";
import { Injectable, inject } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { Product } from "../models";

@Injectable({ providedIn: "root" })
export class ProductService {
  private httpClient = inject(HttpClient);

  getProducts(): Promise<Product[]> {
    const products = this.httpClient.get<Product[]>("https://localhost:7101/products");
    return firstValueFrom(products);
  }
}
