# Base stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /out

# Development stage
FROM base AS development
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=build /out .
ENTRYPOINT ["dotnet", "EventSphere.Api.dll"]

# Production stage
FROM base AS production
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /out .
ENTRYPOINT ["dotnet", "EventSphere.Api.dll"]