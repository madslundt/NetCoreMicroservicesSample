#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["ReviewsService/ReviewsService.csproj", "ReviewsService/"]
RUN dotnet restore "ReviewsService/ReviewsService.csproj"
COPY . .
WORKDIR "/src/ReviewsService"
RUN dotnet build "ReviewsService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReviewsService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReviewsService.dll"]