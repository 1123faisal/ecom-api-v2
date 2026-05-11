FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ../EcomApi.Domain/EcomApi.Domain.csproj         EcomApi.Domain/
COPY ../EcomApi.Application/EcomApi.Application.csproj   EcomApi.Application/
COPY ../EcomApi.Infrastructure/EcomApi.Infrastructure.csproj EcomApi.Infrastructure/
COPY EcomApi.API.csproj                              EcomApi.API/

RUN dotnet restore EcomApi.API/EcomApi.API.csproj

COPY ../EcomApi.Domain         EcomApi.Domain/
COPY ../EcomApi.Application    EcomApi.Application/
COPY ../EcomApi.Infrastructure EcomApi.Infrastructure/
COPY .                         EcomApi.API/

RUN dotnet publish EcomApi.API/EcomApi.API.csproj \
    -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "EcomApi.API.dll"]