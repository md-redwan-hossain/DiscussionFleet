FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/DiscussionFleet.Domain/*.csproj", "DiscussionFleet.Domain/"]
COPY ["src/DiscussionFleet.Application/*.csproj", "DiscussionFleet.Application/"]
COPY ["src/DiscussionFleet.Infrastructure/*.csproj", "DiscussionFleet.Infrastructure/"]
COPY ["src/DiscussionFleet.EmailWorker/*.csproj", "DiscussionFleet.EmailWorker/"]
RUN dotnet restore "DiscussionFleet.EmailWorker/DiscussionFleet.EmailWorker.csproj"
COPY . .
WORKDIR "/src/DiscussionFleet.EmailWorker"
RUN dotnet build "DiscussionFleet.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscussionFleet.EmailWorker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscussionFleet.EmailWorker.dll"]