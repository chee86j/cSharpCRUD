### cSharpCRUD

## Project Overview

A Full-Stack Task Manager App where users can:

- Register/Login with secure authentication.
- Create, Read, Update, and Delete (CRUD) tasks.
- Categorize and track task progress on a dashboard.
- Visualize progress with charts.

## Tech Stack

### Backend

- C# with ASP.NET Core (.NET 7)
- PostgreSQL with Entity Framework Core (EF Core)
- JWT Authentication with ASP.NET Identity
- Swagger for API documentation

### Frontend

- Next.js (React Framework) with TypeScript
- Material UI (MUI) for responsive UI
- Axios for API calls

## Backend

### Setup Overview

- ASP.NET Core Web API project (`TaskManagerAPI`) targeting .NET 7.
- Connected to PostgreSQL via Entity Framework Core.
- JWT-based Authentication and User Management using ASP.NET Identity.

### Database and Models

- `User` model extends `IdentityUser` for authentication.
- `TaskItem` model for CRUD operations (Title, Description, IsCompleted, Category, CreatedAt, UserId).

### Authentication

- JWT Authentication for secure API access.
- Endpoints:
  - `POST /api/auth/register` → User registration
  - `POST /api/auth/login` → User login (returns JWT)

## Frontend

### Setup Overview

- Next.js project (`task-manager-frontend`) with TypeScript.
- Installed dependencies:
  - Axios for API communication
  - Material UI (MUI) for UI components
  - React Router DOM for routing
