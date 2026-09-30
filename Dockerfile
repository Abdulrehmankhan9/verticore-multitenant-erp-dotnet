FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY VertiCore.sln .
COPY VertiCore.API/ VertiCore.API/
COPY VertiCore.Application/ VertiCore.Application/
COPY VertiCore.Domain/ VertiCore.Domain/
COPY VertiCore.Infrastructure/ VertiCore.Infrastructure/

RUN dotnet restore VertiCore.sln

RUN dotnet publish VertiCore.API/VertiCore.API/VertiCore.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "VertiCore.API.dll"]