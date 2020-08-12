FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Src/ReviewsService/ReviewsService.csproj", "Src/ReviewsService/"]
COPY ["Src/ReviewsService/DataModel/DataModel.csproj", "Src/ReviewsService/DataModel/"]
COPY ["Src/Events/Events.csproj", "Src/Events/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "Src/ReviewsService/ReviewsService.csproj"
COPY . .
WORKDIR "/src/Src/ReviewsService"
RUN dotnet build "ReviewsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReviewsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewsService.dll"]