## Tech stack
Authentication:
* ASP.NET Core Identity;
* EFCore;
* OpenIdDict.

ApiGateway:
* Ocelot;
* SwaggerForOcelot.

OtherServices:
* ASP.NET Core;
* EFCore;
  * Postgres;
* MassTransit on rabbitMQ:
  * Orchestration sagas;
  * Sync request/response model;
  * Courier – implementation execute/compensate saga transaction;
* AutoMapper;
* Swagger, Ui;
* FluentValidations;
* FluentAssertions;
* XUnit, AutoFixture, Moq.

## Run
* git clone https://github.com/MeitonReen/pet-book-store.git;
* cd pet-book-store/docker-compose;
---
* start docker;
* docker compose build;
* docker compose up;
* start browser;
* http://localhost:8005/swagger;
---
* sign in under pre-installed test account;
* test it.

## Pre-installed test accounts
* `Superuser`-->`KsweJ21)!d`; available permissions: `openid`, `profile`, `default-book-store-resources.crud`;

* `Admin`-->`VAsk23qS(`; available permissions: `openid`, `profile`, `default-admin-resources.crud`; 

* `TestDefaultUser`-->`FS2kaS(2!@`; available permissions: `openid`, `profile`, `default-user-resources.crud`.

## Schemes
[Book store schemes](https://github.com/MeitonReen/pet-book-store/blob/main/BookStoreSchemes_v4.png)

## Registration
* [run](#run);
* http://localhost:7894/account/sign-up;
* register; default permissions: `default-user-resources.crud`;
* http://localhost:8005/swagger;
* select user service open api spec partition;
* authorize with scopes: `openid`, `profile`, `default-book-store-resources.crud`;
* create my profile.

## Authorization service react endpoints
* http://localhost:7894/account/sign-in;
* http://localhost:7894/account/sign-up;
* http://localhost:7894/account/consent.
