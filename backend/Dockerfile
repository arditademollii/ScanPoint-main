# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore ScanPoint/ScanPoint.csproj
RUN dotnet publish ScanPoint/ScanPoint.csproj -c Release -o /app/out

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "ScanPoint.dll"]