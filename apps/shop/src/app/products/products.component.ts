import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { AsyncPipe, CommonModule, CurrencyPipe } from "@angular/common";
import { ProductService } from "../core/services";

@Component({
  selector: "shop-products",
  standalone: true,
  imports: [CommonModule, AsyncPipe, CurrencyPipe],
  template: `
    <h1 class="mb-3">Products</h1>
    <div class="row row-cols-1 row-cols-sm-2 row-cols-lg-3 g-4">
      @for (product of products$ | async; track product.id) {
      <div class="col">
        <div class="card">
          <div class="row g-0">
            <div class="col-4">
              <img
                src="https://placehold.co/200x300?text={{ product.id }}"
                class="img-fluid rounded-start" />
            </div>
            <div class="col-8">
              <div class="card-body">
                <h5 class="card-title">{{ product.name }}</h5>
                <p class="card-text">
                  <small class="text-body-secondary">{{ product.price | currency : "EUR" }}</small>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
      }
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsComponent {
  private productService = inject(ProductService);

  products$ = this.productService.getProducts();
}
