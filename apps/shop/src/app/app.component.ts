import { AsyncPipe } from "@angular/common";
import { ChangeDetectionStrategy, Component, inject } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgbCollapse } from "@ng-bootstrap/ng-bootstrap";
import { OidcSecurityService } from "angular-auth-oidc-client";
import { map } from "rxjs";

@Component({
  standalone: true,
  imports: [RouterModule, AsyncPipe, NgbCollapse],
  selector: "shop-root",
  templateUrl: "./app.component.html",
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AppComponent {
  private oidcSecurityService = inject(OidcSecurityService);

  isMenuCollapsed = true;
  isUserAuthenticated$ = this.oidcSecurityService.isAuthenticated$.pipe(
    map((x) => x.isAuthenticated)
  );
  userData$ = this.oidcSecurityService.userData$;

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
