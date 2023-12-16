import { AsyncPipe } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgbCollapse } from "@ng-bootstrap/ng-bootstrap";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { map, tap } from "rxjs";
import { AppStore } from "./+state";

@Component({
  standalone: true,
  imports: [RouterModule, AsyncPipe, NgbCollapse],
  selector: "shop-root",
  templateUrl: "./app.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  private appStore = inject(AppStore);
  private oidcSecurityService = inject(OidcSecurityService);

  isMenuCollapsed = true;
  isUserAuthenticated$ = this.oidcSecurityService.isAuthenticated$.pipe(
    map((x) => x.isAuthenticated),
    tap((x) => (x ? this.appStore.loadCartPositions() : {})),
    tap((x) => (x ? this.appStore.loadOrders() : {}))
  );
  userData$ = this.oidcSecurityService.userData$;

  cartPositionCount = this.appStore.cartPositionCount;

  ngOnInit() {
    // Start the authentication flow.
    this.oidcSecurityService
      .checkAuth()
      .subscribe(({ isAuthenticated, userData }) => console.log(isAuthenticated, userData));
  }

  login(): void {
    this.oidcSecurityService.authorize();
  }

  logout(): void {
    this.oidcSecurityService.logoff().subscribe((result) => console.log(result));
  }
}
