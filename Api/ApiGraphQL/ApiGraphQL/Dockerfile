FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["ApiGraphQL/ApiGraphQL.csproj", "ApiGraphQL/"]
RUN dotnet restore "ApiGraphQL/ApiGraphQL.csproj"
COPY . .
WORKDIR "/src/ApiGraphQL"
RUN dotnet build "ApiGraphQL.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ApiGraphQL.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ApiGraphQL.dll"]