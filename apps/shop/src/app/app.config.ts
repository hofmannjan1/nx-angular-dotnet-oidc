/**
 * ABOUT THIS FILE
 *
 * This files includes the initial application configuration. This is a lightweight alternative to
 * the app.module.ts file which would be used for an Angular application without standalone
 * components.
 */
import { ApplicationConfig } from "@angular/core";
import { provideRouter } from "@angular/router";
import { provideHttpClient, withInterceptors } from "@angular/common/http";
import { authInterceptor, provideAuth } from "angular-auth-oidc-client";
import { appRoutes } from "./app.routes";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(appRoutes),
    provideHttpClient(withInterceptors([authInterceptor()])),
    provideAuth({
      config: {
        postLoginRoute: "/orders",
        // TODO forbiddenRoute: '',
        // TODO unauthorizedRoute: '',
        // Redirect urls for the identity server
        redirectUrl: window.location.origin,
        postLogoutRedirectUri: window.location.origin,
        // Url of the identity server
        authority: "https://localhost:7001",
        // Id of the client application
        clientId: "shop",
        // Scopes requested by the client
        scope: "openid profile email",
        // Use authorization code flow + PKCE
        responseType: "code",
        // Renew client tokens when expired using the refresh token
        silentRenew: true,
        useRefreshToken: true,
        // Urls where the authInterceptor adds the access token to the request
        secureRoutes: ["https://localhost:7101"],
      },
    }),
  ],
};
