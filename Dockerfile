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

ARG REPO=mcr.microsoft.com/dotnet/aspnet
FROM $REPO:3.1-alpine3.13

ENV \
    # Unset ASPNETCORE_URLS from aspnet base image
    ASPNETCORE_URLS= \
    # Disable the invariant mode (set in base image)
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    # Enable correct mode for dotnet watch (only mode supported in a container)
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8 \
    # Skip extraction of XML docs - generally not useful within an image/container - helps performance
    NUGET_XMLDOC_MODE=skip \
    # PowerShell telemetry for docker image usage
    POWERSHELL_DISTRIBUTION_CHANNEL=PSDocker-DotnetCoreSDK-Alpine-3.13

# Add dependencies for disabling invariant mode (set in base image)
RUN apk add --no-cache icu-libs

# Install .NET Core SDK
RUN dotnet_sdk_version=3.1.410 \
    && wget -O dotnet.tar.gz https://dotnetcli.azureedge.net/dotnet/Sdk/$dotnet_sdk_version/dotnet-sdk-$dotnet_sdk_version-linux-musl-x64.tar.gz \
    && dotnet_sha512='d844e044d7dfbca0b69913c3d5a5dde0f46ddf4a43c1e8c2a474dc65c3089521d0e946507ede4654efc4281314360c66f5c477ee90e1e80f30115e7a5aa1b586' \
    && echo "$dotnet_sha512  dotnet.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -C /usr/share/dotnet -oxzf dotnet.tar.gz ./packs ./sdk ./templates ./LICENSE.txt ./ThirdPartyNotices.txt \
    && rm dotnet.tar.gz \
    # Trigger first run experience by running arbitrary cmd
    && dotnet help
    
RUN apk update \
    && apk add --upgrade unzip \
    && apk add busybox-extras \
    && mkdir /temp \
    && cd /temp \
    && dotnet new console -n Everything \
    && cd /temp/Everything \
    && dotnet add package morelinq -v 3.3.2 \
    && dotnet add package Hashids.net -v 1.3.0 \
    && dotnet add package Humanizer -v 2.8.26 \
    && dotnet add package Selenium.WebDriver -v 4.0.0-beta1 \
    && dotnet add package Selenium.Support -v 4.0.0-beta1 \
    && dotnet add package Selenium.WebDriver.ChromeDriver -v 88.0.4324.9600 \
    && dotnet add package Microsoft.Extensions.Configuration.Json -v 3.1.12 \
    && dotnet add package Microsoft.Extensions.Configuration.Binder -v 3.1.12 \
    && dotnet add package AngleSharp -v 0.14.0 \
    && dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 3.1.12 \
    && dotnet add package Microsoft.EntityFrameworkCore.Design -v 3.1.12 \
    && dotnet add package System.Linq.Dynamic.Core -v 1.2.8 \
    && dotnet add package Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite -v 3.1.12 \
    && dotnet add package EFCore.BulkExtensions -v 3.3.1 \
    && dotnet add package ClosedXML -v 0.95.4 \
    && dotnet add package Microsoft.AspNetCore.Mvc.NewtonsoftJson -v 3.1.12 \
    && dotnet add package System.Drawing.Common -v 4.7.2 \
    && dotnet add package Mono.Cecil -v 0.11.3 \
    && dotnet restore  \
    && dotnet build

RUN dotnet tool install --global dotnet-ef --version 3.1.12
ENV PATH="${PATH}:/root/.dotnet/tools"

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