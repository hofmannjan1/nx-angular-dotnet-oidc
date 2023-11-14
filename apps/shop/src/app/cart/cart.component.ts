import { ChangeDetectionStrategy, Component } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
  selector: "shop-cart",
  standalone: true,
  imports: [CommonModule],
  template: `<p>cart works!</p>`,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartComponent {}
