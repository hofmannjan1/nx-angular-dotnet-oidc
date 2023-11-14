import { ChangeDetectionStrategy, Component } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
  selector: "shop-products",
  standalone: true,
  imports: [CommonModule],
  template: `<p>products works!</p>`,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProductsComponent {}
