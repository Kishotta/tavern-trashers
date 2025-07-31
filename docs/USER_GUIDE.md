# User Guide

Welcome to Tavern Trashers! This guide will help you get started as a user or developer.

## Running the Application

### Backend
1. Install .NET 8+ and Visual Studio 2022+ or VS Code
2. Navigate to `src/` and run:
   ```sh
   dotnet run --project TavernTrashers.Api/TavernTrashers.Api.csproj
   ```
3. The API will be available at `https://localhost:5001` (default)

### Frontend
1. Install Node.js 18+ and Angular CLI
2. Navigate to `web/tavern-trashers/` and run:
   ```sh
   npm install
   npm start
   ```
3. The app will be available at `http://localhost:4200`

## Features
- Create and manage campaigns
- Manage characters and users
- Roll dice and track results

## API Usage
- See OpenAPI/Swagger docs at `/swagger` when the backend is running

## Troubleshooting
- Ensure all dependencies are installed
- Check logs in the backend and frontend for errors

For more help, open an issue on GitHub.
