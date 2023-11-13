import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";
import { NgbCollapse } from "@ng-bootstrap/ng-bootstrap";

@Component({
  standalone: true,
  imports: [RouterModule, NgbCollapse],
  selector: "shop-root",
  templateUrl: "./app.component.html",
})
export class AppComponent {
  isMenuCollapsed = true;
}
