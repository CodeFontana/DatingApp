# Dating App
Based on Udemy course, "*Build an app with ASPNET Core and ~~Angular~~ from scratch*" by Neil Cummings, with a ***Major Twist*** -- the client-side application is built entirely using ASP.NET Core ***Blazor WebAssembly***.  
  
This project is all about learning ASP.NET Core Web API, ASP.NET Core Blazor WebAssembly, Entity Framework and MudBlazor.  It is an undertaking for both learning, personal growth, and evaluation of this technology stack for real-world uses.

## Project Features
* Common features
  * .NET Generic Host for Configuration, Logging and Dependency Injection container
    * Modern Compositional programming model
    * Loosely-coupled dependencies using Interfaces
  * Top-level statements to reduce Program.cs ceremony
  * Global-using Statements to reduce boilerplate in every code file
  * File-scoped namespaces to flatten horizontal code width
* API features
  * User management and authentication with ASP.NET Core Identity with JWT Bearer Tokens
    * Generates JWT with user's roles and claims.
    * User authorization via role management and authorization policies
  * Response Caching
  * Response Sorting and Pagination
  * API Versioning
  * OpenAPI specification with Swagger
    * Support for JWT Bearer Tokens
    * Support API versioning
  * API health checks, including SQL database health check (https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks)
  * Exception Handling Middleware
  * User activity monitoring with Service Filters
  * IP Rate Limiting (https://github.com/stefanprodan/AspNetCoreRateLimit)
  * Logging with ConsoleLogger project (github.com/CodeFontana/ConsoleLogger)
  * Automated Database Migration with Seeding of demo data to get started
* Client features
  * Reactive and Responsive Client UI using MudBlazor (https://www.mudblazor.com | https://github.com/MudBlazor/MudBlazor)
  * Simple UI animations using Animate.css (https://animate.style | https://github.com/animate-css/animate.css)
  * API-based user authentication using JWT
  * Request Sorting and Pagination of API data
  * Local caching of API data
  * Automatic Loading Spinner during API requests (https://stackoverflow.com/questions/56604886/blazor-display-wait-or-spinner-on-api-call/69182425#69182425)
* Class Library / Entity Framework Core features
  * One to One, One to Many and Many to Many relationships between EF Core Entities
  * Data Annotations and Custom Attributes for Entity Models for avoiding common EF Core pitfalls in database design
  * Employment of Split Queryies to eliminate cartesian explosion problem and allowing the app to scale with production data

## Roadmap
* Finish client implementation of Messaging feature
* Add Chat feature based on SignalR
* Implement Unit-of-Work pattern for EF
* Implement Public vs Private User Interests

## Great Articles
https://dev.to/dotnet/why-build-single-page-apps-in-blazor-103m

https://chrissainty.com/what-is-blazor-and-why-is-it-so-exciting/
