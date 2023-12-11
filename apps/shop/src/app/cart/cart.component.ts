import {
  ChangeDetectionStrategy,
  Component,
  Signal,
  TemplateRef,
  ViewChild,
  computed,
  inject,
} from "@angular/core";
import { NgbDatepickerModule, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Router } from "@angular/router";
import { AppStore } from "../+state";
import { CartViewModel } from "./cart.viewmodel";

@Component({
  selector: "shop-cart",
  standalone: true,
  imports: [NgbDatepickerModule],
  templateUrl: "cart.component.html",
  styles: [],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CartComponent {
  private appStore = inject(AppStore);
  private router = inject(Router);
  private modalService = inject(NgbModal);

  @ViewChild("confirm") confirm?: TemplateRef<any>;

  viewModel: Signal<CartViewModel> = computed(() => ({
    positionsWithProducts: this.appStore.cartPositionsWithProducts(),
    positionsLoading: this.appStore.cartPositionsLoading(),
    positionIds: this.appStore.cartPositionIds(),
    positionCount: this.appStore.cartPositionCount(),
    productCount: Object.values(this.appStore.cartPositions()).reduce(
      (acc, x) => acc + x.quantity,
      0
    )
  }));

  deletePositions(ids: number[]): void {
    this.appStore.deleteCartPositions(ids);
  }

  order(): void {
    this.modalService.open(this.confirm).result.then(
      async () => {
        await this.appStore.orderCart();
        this.router.navigate(["orders"]);
      },
      () => {}
    );
  }
}
