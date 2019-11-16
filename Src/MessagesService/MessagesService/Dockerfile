FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["MessagesService/MessagesService.csproj", "MessagesService/"]
RUN dotnet restore "MessagesService/MessagesService.csproj"
COPY . .
WORKDIR "/src/MessagesService"
RUN dotnet build "MessagesService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessagesService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessagesService.dll"]