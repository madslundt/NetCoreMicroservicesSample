FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["Users.Service/Users.Service.csproj", "Users.Service/"]
RUN dotnet restore "Users.Service/Users.Service.csproj"
COPY . .
WORKDIR "/src/Users.Service"
RUN dotnet build "Users.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Users.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Users.Service.dll"]