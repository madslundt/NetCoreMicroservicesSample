FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ./ .
RUN dotnet restore "DataModel/DataModel.csproj"
RUN dotnet restore "UsersService/UsersService.csproj"

WORKDIR "/src/DataModel"
RUN dotnet tool install -g dotnet-ef
# RUN "$HOME/.dotnet/tools/dotnet-ef" database update

WORKDIR "/src/UsersService"
RUN dotnet build "UsersService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UsersService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UsersService.dll"]