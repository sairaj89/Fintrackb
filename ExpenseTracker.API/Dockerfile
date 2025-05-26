# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application and build
COPY . . 
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Use the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the build result from the previous stage
COPY --from=build /app/publish .

# Expose the port (change if your app uses a different one)
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "ExpenseTracker.API.dll"]
