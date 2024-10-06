FROM mcr.microsoft.com/dotnet/aspnet:8.0.4-jammy AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.204-jammy AS prepare-restore-files
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet tool install --global --no-cache dotnet-subset 
WORKDIR /src
COPY . .
RUN dotnet subset restore SmartCoinOS.sln --root-directory /src --output /src/restore_subset/

FROM mcr.microsoft.com/dotnet/sdk:8.0.204-jammy AS publish
ARG TARGETARCH
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --from=prepare-restore-files /src/restore_subset .
RUN dotnet restore "SmartCoinOS.sln" -a "$TARGETARCH"
COPY . .
WORKDIR "/src/ClientPortal.Api"
RUN dotnet publish "ClientPortal.Api.csproj" --no-restore -a "$TARGETARCH"  -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID  
ENTRYPOINT ["dotnet", "ClientPortal.Api.dll"]
