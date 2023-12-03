import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AppStore } from "../+state";

@Component({
  selector: "shop-cart",
  standalone: true,
  imports: [CommonModule],
  templateUrl: "cart.component.html",
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartComponent {
  private appStore = inject(AppStore);

  cartPositionsWithProducts = this.appStore.cartPositionsWithProducts;
  cartPositionsLoading = this.appStore.cartPositionsLoading;

  deletePosition(id: number) {
    this.appStore.deleteCartPositions([id]);
  }
}
