# Nx full-stack Angular + .NET workspace with OIDC

## Quick Start

Angular Shop application

- Run `npx nx serve shop` to start the Angular Shop application. Navigate to http://localhost:5201/.

Shop API

- Run `npx nx serve shop-api` to start the Shop Web API. Navigate to https://localhost:7101/.

Identity Server application

- Run `npx nx serve identity-server` to start the Identity Server application. Navigate to https://localhost:7001/.
- Run `npx nx build identity-server` to build the Identity Server application.
- Run `npx nx publish identity-server` to publish the Identity Server application into the `dist/publish/apps/identity-server` folder. The executable can also be run on the local machine using `./IdentityServer`.

All applications

- Run `npx nx run-many --target=serve --all` to start all applications at once.
