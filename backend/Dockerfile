# Stage 1: Build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
#huong dan luu va tat: 
#luu: ctrl + O -> ctrl + X -> enterx
# Copy everything and restore
COPY src/ ./src/
COPY *.sln ./
RUN dotnet restore

# Publish the API
RUN dotnet publish src/ChessLearning.Api/ChessLearning.Api.csproj -c Release -o /out

# Stage 2: Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ChessLearning.Api.dll"]
