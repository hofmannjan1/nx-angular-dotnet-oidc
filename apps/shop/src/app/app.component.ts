import { Component } from "@angular/core";
import { RouterModule } from "@angular/router";

@Component({
  standalone: true,
  imports: [RouterModule],
  selector: "shop-root",
  template: ` <router-outlet /> `,
})
export class AppComponent {}
