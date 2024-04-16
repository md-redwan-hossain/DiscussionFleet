FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src


COPY ["DiscussionFleet.Domain/DiscussionFleet.Domain.csproj", "DiscussionFleet.Domain/"]
COPY ["DiscussionFleet.Application/DiscussionFleet.Application.csproj", "DiscussionFleet.Application/"]
COPY ["DiscussionFleet.Infrastructure/DiscussionFleet.Infrastructure.csproj", "DiscussionFleet.Infrastructure/"]
COPY ["DiscussionFleet.Web/DiscussionFleet.Web.csproj", "DiscussionFleet.Web/"]
RUN dotnet restore "DiscussionFleet.Web/DiscussionFleet.Web.csproj"
COPY . .
WORKDIR "/src/DiscussionFleet.Web"
RUN dotnet build "DiscussionFleet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscussionFleet.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscussionFleet.Web.dll"]
