/*
 * ABOUT THIS FILE
 *
 * This file includes environment-specific configuration values and is used for development. Use
 * a file replacement rule for a specific build target to replace this file with a different one
 * depending on the build configuration, e.g. environment.staging.ts for `ng build -c staging`.
 */
export const environment = {
  production: false,
  shopApiUrl: "https://localhost:7101",
  auth: {
    clientId: "shop",
    authorityUrl: "https://localhost:7001",
    scopes: "openid profile email shop_api",
  },
};
