# Weather App

## Overview

This is a full-stack weather application built using **C# (.NET Core) for the backend** and **React with TypeScript for the frontend**. The application integrates with the **OpenWeather API** to fetch weather data, implements **JWT authentication**, and follows the **CQRS pattern** with **Clean Architecture**.

## Features

- **User Authentication** (Register & Login with JWT)
- **Weather Forecast Data Fetching** (Using OpenWeather API)
- **CRUD Operations** for Weather Data
- **Unit & Integration Testing** (xUnit, Moq, and WebApplicationFactory)
- **Swagger API Documentation**

---

## Tech Stack

### Backend (C# .NET Core)

- **ASP.NET Core Web API**
- **Entity Framework Core** (EF Core)
- **CQRS with MediatR**
- **JWT Authentication**
- **SQLite Database** (or In-Memory for testing)
- **Moq & xUnit** for unit testing
- **Integration Testing with WebApplicationFactory**

### Frontend (React + TypeScript)

- **React with Vite**
- **TypeScript**
- **Material-UI (MUI) Components**
- **Fetch API for HTTP Requests**
- **React Router** (for navigation)

---

## Installation & Setup

### Prerequisites

- .NET 8 or later
- Node.js 18+
- SQL Server (if using a real database)

### Backend Setup

1. **Clone the repository**
   ```sh
   git clone https://github.com/RajaGC1989/WeatherApp.git
   cd weather-app/backend
   ```
2. **Install dependencies**
   ```sh
   dotnet restore
   ```
3. **Update appsettings.json** (Set OpenWeather API Key & JWT Secret Key)
4. **Run database migrations** (if using MSSQL)
   ```sh
   dotnet ef database update
   ```
5. **Run the backend**
   ```sh
   dotnet run
   ```
6. **Access API Swagger Docs** at [http://localhost:7103/swagger](http://localhost:7103/swagger)

### Frontend Setup

1. **Navigate to the frontend folder**
   ```sh
   cd ../frontend
   ```
2. **Install dependencies**
   ```sh
   npm install
   ```
3. **Start the React App**
   ```sh
   npm run dev
   ```
4. **Open in browser:** [http://localhost:5173](http://localhost:5173)

---

## Running Tests

### Unit Tests (Backend)

```sh
dotnet test
```

### Integration Tests (Backend)

```sh
dotnet test --filter Category=Integration
```

### Frontend Tests (If Implemented)

- need to Implement

---

## API Endpoints

### **Auth Controller** (`/api/auth`)

- `POST /register` - Register a new user
- `POST /login` - Authenticate user & get JWT token

### **Weather Controller** (`/api/weather`)

- `GET /` - Get all saved weather data
- `GET /{city}` - Get weather by city
- `GET /fetch/{city}` - Fetch weather from OpenWeather API and store it
- `PUT /{city}` - Update weather for a city
- `DELETE /{city}` - Delete weather record

---

## Folder Structure

```
/weather-app
│── backend (ASP.NET Core API)
│   ├── Application (Business logic & services)
│   ├── Domain (Entities & Models)
│   ├── Infrastructure (EF Core, Repositories)
│   ├── API (Controllers & Configurations)
│   ├── Tests (Unit & Integration Tests)
│
│── frontend (React + TypeScript)
│   ├── src
│   │   ├── components (Reusable UI Components)
│   │   ├── pages (Page Components)
│   │   ├── services (API Calls & Fetch Logic)
│   │   ├── context (Global State & Auth)
│   │   ├── App.tsx (Main App Component)
│   │   ├── main.tsx (Entry File)
```

---

## Future Enhancements

- Implement Infinite Scrolling in Grid** (MUI DataGrid) **
- Implement **Caching** (e.g., Redis) to reduce API calls
- Add **Role-based Authentication** (Admin/User)
- Implement **WebSockets** for real-time weather updates
- Improve **UI/UX** with better visualization

---

## License

This project is open-source and available under the MIT License.

---

## Contact

For any queries or contributions, reach out to:

- **Your Name**: Raja
- **GitHub**: https://github.com/RajaGC1989/WeatherApp
