import { ChangeDetectionStrategy, Component } from "@angular/core";
import { CommonModule } from "@angular/common";

@Component({
  selector: "shop-orders",
  standalone: true,
  imports: [CommonModule],
  template: `<p>orders works!</p>`,
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class OrdersComponent {}
