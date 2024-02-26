# Project Overview

Country list simple app. Used .net core 8, React, consumption of restcountries.com as a data source

## Backend Architecture

The .NET Core API is designed with a clear separation of concerns in mind, following the Repository and Service patterns to provide a clean API surface for the frontend. Dependency Injection (DI) is used extensively to ensure that the components are loosely coupled and easily testable.

## Caching

A caching layer is implemented using IMemoryCache to improve performance by storing frequently accessed data in memory. The CountryCache service abstracts away the caching logic from the rest of the application, providing a simple and cohesive API for caching country data.

## Error Handling

In production, a global error-handling middleware captures unhandled exceptions to provide a consistent response structure for API errors. This allows for better user experience and easier debugging.

## Logging

Serilog is configured for rich logging capabilities. It logs to both the console and to file, ensuring that in a real-world scenario, logs can be persisted and analyzed for issues over time.

## Frontend Integration

The UseSpa middleware is set up to integrate with a React development server, enabling hot-reloading and other development features. This middleware serves static files and proxies requests to the frontend server during development.

## Resilience and Performance

Implementing a resilience policy using something like Polly is essential to handle transient faults and latencies when calling third-party APIs. 

---

# Real-World Considerations

If more time were available or in a production scenario, the following considerations would be made:

## Authentication and Authorization

The application would implement an authentication mechanism, likely JWT tokens or OAuth, to secure the API endpoints. Authorization policies would also be in place to restrict access based on user roles or claims.

## Caching Enhancements

For better cache management, we would integrate a distributed cache like Redis, especially for high-traffic apps. This would also facilitate horizontal scaling of the application.

## API Documentation and Versioning

Swagger would be set up with versioning support to document different versions of the API, making it easier for the front-end developers and API consumers to understand available endpoints and data contracts.

## Monitoring and Observability

Integration with a monitoring system like Application Insights or Prometheus would be added for real-time monitoring, performance metrics, and alerting.

## Containerization

The application would be containerized using Docker for easy deployment, scaling, and to ensure consistency across different environments.

---

# Running the Application Instructions

## Backend Setup:

1. Navigate to the root directory of the .NET Core API.
2. Run `dotnet restore` to install all the necessary NuGet packages.
3. Run `dotnet build` to build the project.
4. Run `dotnet run` to start the API.

## Frontend Setup:

1. Navigate to the Client directory where the React app is located.
2. Run `npm install` or `yarn` to install all the node modules.
3. Run `npm start` or `yarn start` to start the React development server.

## Accessing the Application:

- The React app should now be running on [http://localhost:3000](http://localhost:3000).
- The API endpoints should be accessible in Swagger [http://localhost:5000/api/swagger/index.html](http://localhost:5000/api/swagger/index.html).
- The application should be accessible on [http://localhost:5000/](http://localhost:5000/).

**Note:** Ensure that the ports are not in use and adjust accordingly if necessary. The API might run on a different port if 5000 is occupied; update the proxy configuration in the React app to match the API port.
