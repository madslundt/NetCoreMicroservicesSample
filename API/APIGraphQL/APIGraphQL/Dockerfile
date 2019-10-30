FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["APIGraphQL.csproj", ""]
RUN dotnet restore "./APIGraphQL.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "APIGraphQL.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "APIGraphQL.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "APIGraphQL.dll"]