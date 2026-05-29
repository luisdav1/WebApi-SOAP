# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

ASP.NET Web API 2 + MVC 5 web application written in **VB.NET**, targeting **.NET Framework 4.8.1**. This is a classic (non-SDK-style) MSBuild project that runs under IIS Express. It is the default Visual Studio Web API template, currently containing only scaffold code (`ValuesController`, `HomeController`).

## Build & Run

No `.sln` exists; build the project file directly. Use the Visual Studio Developer command line (MSBuild + IIS Express on PATH).

```powershell
# Restore NuGet packages (packages.config style, restores into .\packages)
nuget restore WebApi\WebApi.vbproj

# Build
msbuild WebApi\WebApi.vbproj /p:Configuration=Debug

# Run under IIS Express (project is configured for https://localhost:44390/)
& "C:\Program Files\IIS Express\iisexpress.exe" /path:"$PWD\WebApi" /port:44390
```

The app is normally launched from Visual Studio (F5). There is **no test project** and no linter configured — correctness is verified by building and exercising endpoints.

## Architecture

Standard ASP.NET request pipeline bootstrapped in [Global.asax.vb](WebApi/Global.asax.vb) `Application_Start`, which wires up four `App_Start` configurators in order:

- [WebApiConfig.vb](WebApi/App_Start/WebApiConfig.vb) — Web API routes. Attribute routing is enabled (`MapHttpAttributeRoutes`) plus the convention route `api/{controller}/{id}`. **API endpoints live under `/api/`.**
- [RouteConfig.vb](WebApi/App_Start/RouteConfig.vb) — MVC routes (the HTML `Home` site).
- [FilterConfig.vb](WebApi/App_Start/FilterConfig.vb) — global MVC filters.
- [BundleConfig.vb](WebApi/App_Start/BundleConfig.vb) — script/CSS bundles (jQuery, Bootstrap, Modernizr).

This means **two parallel stacks coexist**: Web API controllers (inherit `ApiController`, return data) under `Controllers/`, and MVC controllers (inherit `Controller`, return Razor `.vbhtml` views in `Views/`). Add API controllers as `ApiController` subclasses; routing is by HTTP-verb method-name prefix (`GetX`, `PostX`, etc.) unless overridden with attributes.

`Areas/HelpPage/` is the auto-generated API help-page area (browsable docs at `/Help`). It is template boilerplate — generally leave it alone.

The empty `Models/` folder is the intended home for data/DTO classes.

## Conventions

- **Language is VB.NET**, not C#. Match VB syntax (`Imports`, `Public Function ... As`, `<FromBody()>` attributes, `End Class`).
- `OptionStrict` is **Off** and `OptionInfer` is On (set in the `.vbproj`) — implicit conversions compile, but prefer explicit typing in new code.
- Existing scaffold comments are in **Spanish**; follow the user's language for new comments.
- Dependencies are managed via **packages.config + `.\packages`** (old NuGet style), not `PackageReference`. Adding a package means editing both `packages.config` and the `<Reference>`/`<HintPath>` entries in `WebApi.vbproj`.
- JSON serialization uses **Newtonsoft.Json 13.x** (the Web API default formatter).
