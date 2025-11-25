# MediaAPI
MediaAPI is a full-stack, cloud-deployed media metadata platform featuring a robust .NET backend and a modern React frontend that provides a dynamic, interactive API documentation and testing experience, replacing traditional Swagger UI. The project demonstrates advanced integration, code quality, and user experience best practices.

## 1. Architecture & Tech Stack

- **Backend:**  
  - ASP.NET Core (.NET 8) with modular service layer and dependency injection.
  - RESTful API endpoints for media metadata, TMDB details, posters, and Stremio catalogs.
  - Flexible models for movies/TV, robust serialization, and ProxyResult error handling.

- **Frontend:**  
  - Custom React (TypeScript) application built with Vite, replacing traditional Swagger UI.
  - Dynamic documentation explorer: auto-discovers backend endpoints, displays parameterized forms, and enables live endpoint testing with real-time JSON response rendering.
  - Responsive, interactive UI: expandable endpoint panels, debounced resize logic, SVG favicon, and live URL previews.

## 2. Integrations

- **TMDB API:**  
  - Movie/TV metadata, poster retrieval, and search.
  - Handles both movie and TV results with flexible model binding.

- **MDBList:**  
  - Curated media lists, proxying and aggregating TMDB poster URLs.

- **Stremio Catalogs:**  
  - Custom endpoints for Stremio catalog queries.

## 3. Testing & Code Quality

- **Unit & Integration Tests:**  
  - xUnit suite for backend services/controllers.
  - Mocked external HTTP clients for reliable tests.
  - Integration tests for routing, error handling, and serialization.

- **Error Handling:**  
  - ProxyResult pattern for consistent success/error reporting.
  - ProblemDetails responses for API errors.

## 4. Deployment & CI/CD

- **Backend:**  
  - Automated build and test via GitHub Actions.
  - Vercel workflow (`vercel-deploy.yml`) for ASP.NET Core backend deployed via GitHub Actions.

- **Frontend:**  
  - Fly.io workflow (`fly-deploy.yml`) for React frontend deployed via GitHub Actions.

- **Configuration:**  
  - API keys managed via environment variables and GitHub secrets.

## 6. Notable Features

- Custom React documentation and API explorer, fully replacing Swagger UI.
- Modular, testable backend with clear separation of concerns.
- Secure, automated deployment pipelines for frontend and backend.
- Comprehensive error handling and user feedback in UI and API responses.