# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ExpenseTracker.API/ExpenseTracker.API.csproj ./ExpenseTracker.API/
RUN dotnet restore ./ExpenseTracker.API/ExpenseTracker.API.csproj

# Copy entire project
COPY . .

# Publish the project
RUN dotnet publish ./ExpenseTracker.API/ExpenseTracker.API.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 80
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
