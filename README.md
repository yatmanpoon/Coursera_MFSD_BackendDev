# Microsoft Copilot Back-end API Development Demo

A simple web project that demonstrates how to use Microsoft Copilot to construct the back-end API functions.

## Overview

This project demonstrates how to:
- use Copilot to scaffold the project setup
- generate CRUD endpoints for specific actions towards the model
- identify and fix bugs with Copilot
- implement middlewares for logging, error-handling and authenication functions

## Tech Stack

- C#
- .NET 10
- ASP.NET Core Web API
- OpenAPI / Swagger-style endpoint docs
- JSON-based REST API
- In-memory user data store
- Health checks and CORS middleware

## Project Structure

- `UserManagementAPI/Program.cs` - application startup, middleware, authentication checks, CORS, health endpoints, and controller mapping
- `UserManagementAPI/Controllers/UsersController.cs` - CRUD API endpoints for managing users
- `UserManagementAPI/Models/User.cs` - user model and validation-related data model
- `UserManagementAPI/appsettings.json` - application configuration, including the required `Authentication:Token`
- `UserManagementAPI/UserManagementAPI.http` - sample HTTP requests for testing the API
- `UserManagementAPI/Properties/launchSettings.json` - local startup profiles for development

## How to Run

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
2. Clone the repository.
3. Open `UserManagementAPI/appsettings.json` and update the `Authentication:Token` value to the token you want to use for API requests.
4. From the repository root, run:

   ```bash
   dotnet run --project UserManagementAPI/UserManagementAPI.csproj
   ```

5. The API will start in the terminal and display the local HTTPS URL.
6. Send requests with the `Authorization: Bearer <token>` header, for example:

   ```bash
   curl -H "Authorization: Bearer change-me-production-token" https://localhost:<port>/api/users
   ```

   Replace `<port>` with the port shown by the app output.

## Course

This is the final assignment for the Coursera course: Microsoft Full-Stack Developer Professional Certificate - Back-End Development with .NET

## License

This project is for educational purposes.