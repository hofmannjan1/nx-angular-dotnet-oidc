/*
 * ABOUT THIS FILE
 *
 * This file is responsible for bootstrapping an instance of the Angular application and rendering
 * the standalone AppComponent as the application's root component.
 */
import { bootstrapApplication } from "@angular/platform-browser";
import { appConfig } from "./app/app.config";
import { AppComponent } from "./app/app.component";

bootstrapApplication(AppComponent, appConfig).catch((err) => console.error(err));
