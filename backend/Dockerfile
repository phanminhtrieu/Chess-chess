# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY *.sln ./
COPY src/ChessLearning.Api/*.csproj ./src/ChessLearning.Api/
COPY src/ChessLearning.Shared/*.csproj ./src/ChessLearning.Shared/

# Copy all module csproj files
COPY src/Modules/Assignment/ChessLearning.Modules.Assignment/*.csproj ./src/Modules/Assignment/ChessLearning.Modules.Assignment/
COPY src/Modules/Content/ChessLearning.Modules.Content/*.csproj ./src/Modules/Content/ChessLearning.Modules.Content/
COPY src/Modules/Game/ChessLearning.Modules.Game/*.csproj ./src/Modules/Game/ChessLearning.Modules.Game/
COPY src/Modules/Identity/ChessLearning.Modules.Identity/*.csproj ./src/Modules/Identity/ChessLearning.Modules.Identity/
COPY src/Modules/Learning/ChessLearning.Modules.Learning/*.csproj ./src/Modules/Learning/ChessLearning.Modules.Learning/
COPY src/Modules/Progress/ChessLearning.Modules.Progress/*.csproj ./src/Modules/Progress/ChessLearning.Modules.Progress/

RUN dotnet restore

# Copy the remaining files and build the app
COPY src/ ./src/
RUN dotnet publish src/ChessLearning.Api/ChessLearning.Api.csproj -c Release -o /out

# Use the runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .

# Expose the port the app runs on
EXPOSE 8080
ENTRYPOINT ["dotnet", "ChessLearning.Api.dll"]

