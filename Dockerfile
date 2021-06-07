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

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
COPY source dest


RUN dotnet restore

RUN dotnet build

CMD dotnet $mainDll

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