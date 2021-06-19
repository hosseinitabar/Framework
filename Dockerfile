# mcr.microsoft.com/dotnet/runtime-deps:3.1 => https://hub.docker.com/_/microsoft-dotnet-runtime-deps/
# This image contains the native dependencies needed by .NET. It does not include .NET. It is for self-contained applications.

# mcr.microsoft.com/dotnet/aspnet:3.1 => https://hub.docker.com/_/microsoft-dotnet-aspnet/
# This image contains the ASP.NET Core and .NET runtimes and libraries and is optimized for running ASP.NET Core apps in production.

# mcr.microsoft.com/dotnet/runtime:3.1 => https://hub.docker.com/_/microsoft-dotnet-runtime/
# This image contains the .NET runtimes and libraries and is optimized for running .NET apps in production.

# mcr.microsoft.com/dotnet/sdk:3.1 => https://hub.docker.com/_/microsoft-dotnet-sdk
# This image contains the .NET SDK which is comprised of three parts:
# 
# .NET CLI
# .NET runtime
# ASP.NET Core
#
# Use this image for your development process (developing, building and testing applications).

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS three

RUN apk update
RUN apk add --no-cache git
RUN apk add --upgrade unzip
WORKDIR /HolismDotNet
RUN git clone https://github.com/HolismDotNet/Framework
RUN git clone https://github.com/HolismDotNet/Api 
WORKDIR /temp
RUN dotnet new console -n Everything
WORKDIR /temp/Everything
RUN dotnet add package morelinq -v 3.3.2
RUN dotnet add package Hashids.net -v 1.3.0
RUN dotnet add package Humanizer -v 2.8.26
RUN dotnet add package Selenium.WebDriver -v 4.0.0-beta1
RUN dotnet add package Selenium.Support -v 4.0.0-beta1
RUN dotnet add package Selenium.WebDriver.ChromeDriver -v 88.0.4324.9600
RUN dotnet add package Microsoft.Extensions.Configuration.Json -v 3.1.12
RUN dotnet add package Microsoft.Extensions.Configuration.Binder -v 3.1.12
RUN dotnet add package AngleSharp -v 0.14.0
RUN dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 3.1.12
RUN dotnet add package System.Linq.Dynamic.Core -v 1.2.8
RUN dotnet add package Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite -v 3.1.12
RUN dotnet add package EFCore.BulkExtensions -v 3.3.1
RUN dotnet add package ClosedXML -v 0.95.4
RUN dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson -v 3.1.12
RUN dotnet add package System.Drawing.Common -v 4.7.2
RUN dotnet add package Mono.Cecil -v 0.11.3

RUN dotnet restore
RUN dotnet build

# https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/Dockerfile

# Todo: R&D about docker URL context
# https://docs.docker.com/engine/reference/builder/#usage

# Can I use git submodules and direct git cloning on each "image" to reduce dependency on the infra? Developer just clones "X" and runs "./Setup.sh". Then this happens:
# docker command fetchs a Dockerfile from github
# docker command builds an image from .NET SDK
# docker builds a SQL Server Express container
# docker gets Framework, Quality, Api, WebUi, and any other required dependency (or maybe I should publish Framework, Quality, Api, WebUi, etc to NuGet; or even publish only with one version => replace=true)

# https://docs.docker.com/glossary/

# https://docs.microsoft.com/en-us/windows-server/get-started/getting-started-with-nano-server

# docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=developer' -e 'MSSQL_PID=Express' -p 1433:1433 -d mcr.microsoft.com/mssql/server:2019-latest-ubuntu