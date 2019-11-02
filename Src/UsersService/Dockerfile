FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["UsersService/UsersService.csproj", "UsersService/"]
COPY ["DataModel/DataModel.csproj", "DataModel/"]
RUN dotnet restore "UsersService/UsersService.csproj"
COPY . .
WORKDIR "/src/UsersService"
RUN dotnet build "UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsersService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]