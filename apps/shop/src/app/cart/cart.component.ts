import { ChangeDetectionStrategy, Component, TemplateRef, ViewChild, inject } from "@angular/core";
import { AppStore } from "../+state";
import { NgbDatepickerModule, NgbModal } from "@ng-bootstrap/ng-bootstrap";
import { Router } from "@angular/router";

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

  positionsWithProducts = this.appStore.cartPositionsWithProducts;
  positionsLoading = this.appStore.cartPositionsLoading;
  positionCount = this.appStore.cartPositionCount;

  deletePosition(id: number): void {
    this.appStore.deleteCartPositions([id]);
  }

  orderAllPositions(): void {
    this.modalService.open(this.confirm).result.then(
      async () => {
        await this.appStore.orderCartPositions(this.appStore.cartPositionIds());
        this.router.navigate(["orders"]);
      },
      () => {}
    );
  }
}
