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