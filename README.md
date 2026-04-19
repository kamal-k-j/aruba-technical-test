# Aruba Technical Test – Senior .NET Backend Developer

A document management system built on a microservices architecture, running on Kubernetes, with a .NET 10 backend, MongoDB as the datastore, and JWT authentication.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [Technology Stack](#technology-stack)
- [Architectural Patterns](#architectural-patterns)
- [Solution Structure](#solution-structure)
- [API Endpoints](#api-endpoints)
- [Document Lifecycle](#document-lifecycle)
- [MongoDB Data Modeling](#mongodb-data-modeling)
- [Kubernetes](#kubernetes)
- [Getting Started](#getting-started)
- [Testing](#testing)
- [Authentication](#authentication)

---

## Architecture Overview

The system is composed of two independent microservices, each with its own solution, Dockerfile, and Kubernetes manifests.

```
aruba-technical-test/
├── document/        # Document Service
├── identity/        # Identity Service
└── k8s/             # Kubernetes manifests
```

### Identity Service

Responsible for authentication and user management. Exposes endpoints for registration, login, and user CRUD operations. Issues JWT tokens consumed by the Document Service to authorize requests.

### Document Service

Responsible for managing commercial documents (Quotes, Proforma Invoices, Sales Orders). Handles the full document lifecycle including state transitions, document generation from other documents, and document linking.

Communication between services is **synchronous via REST**. The Document Service validates JWT tokens issued by the Identity Service on every protected request.

---

## Technology Stack

| Component | Technology |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core Web API |
| Database | MongoDB |
| Authentication | JWT Bearer |
| Mediator / CQRS | MediatR |
| Validation | FluentValidation |
| Mapping | AutoMapper |
| Functional Tests | xUnit + WebApplicationFactory |
| Unit Tests | xUnit |
| Orchestration | Kubernetes |

---

## Architectural Patterns

### Clean Architecture

Each microservice is organized into four distinct layers with strict dependency rules — outer layers depend on inner layers, never the reverse:

```
Api → Application → Domain
Infrastructure → Application → Domain
```

- **Domain** – Core business entities, enums, and domain exceptions. No dependencies on any other layer.
- **Application** – Use cases, commands, queries, validators, and pipeline behaviors. Depends only on Domain.
- **Infrastructure** – MongoDB repositories, JWT token generation, and external service configuration. Implements interfaces defined in Application.
- **Api** – ASP.NET Core controllers, DTOs (Request/Response models), AutoMapper profiles, middleware, and dependency injection wiring.

### CQRS (Command Query Responsibility Segregation)

Read and write operations are strictly separated into **Commands** and **Queries**, each with its own handler. This makes responsibilities explicit, improves testability, and allows independent scaling of read/write paths.

**Commands** (write operations — mutate state):

| Command | Description |
|---|---|
| `InsertDocumentCommand` | Creates a new document |
| `UpdateDocumentCommand` | Updates an existing document |
| `GenerateDocumentCommand` | Generates a new document from an existing one |
| `LinkDocumentCommand` | Links two documents together |
| `SendDocumentCommand` | Transitions document state to Sent |
| `ApproveDocumentCommand` | Transitions document state to Approved |
| `RejectDocumentCommand` | Transitions document state to Rejected |
| `CompleteDocumentCommand` | Transitions document state to Complete |

**Queries** (read operations — no side effects):

| Query | Description |
|---|---|
| `GetAllDocumentsQuery` | Returns all documents |
| `GetDocumentByIdQuery` | Returns a single document by ID |
| `SearchDocumentsQuery` | Returns documents matching search criteria |

### Mediator Pattern

All commands and queries are dispatched through **MediatR**, which decouples the API layer from the Application layer. Controllers never instantiate handlers directly — they send a request object to the mediator, which resolves and invokes the appropriate handler.

This approach also enables **pipeline behaviors**, which are cross-cutting concerns executed automatically before and after every handler:

- **`ValidationBehavior`** – Intercepts every command/query and runs FluentValidation validators before the handler executes. If validation fails, a validation exception is thrown before any business logic runs.
- **`LoggingBehavior`** – Logs the name and execution time of every request, providing observability without polluting handler code.

```
Controller → Mediator → [LoggingBehavior] → [ValidationBehavior] → Handler
```

### SOLID Principles

The codebase applies SOLID principles throughout:

- **Single Responsibility** – Each class has one reason to change. Controllers only handle HTTP concerns, handlers only contain use case logic, repositories only handle data access.
- **Open/Closed** – Pipeline behaviors extend the request processing pipeline without modifying existing handlers.
- **Liskov Substitution** – Repository interfaces (`IDocumentRepository`, `IUserRepository`) are used throughout the Application layer, allowing infrastructure implementations to be swapped or mocked freely.
- **Interface Segregation** – Interfaces are narrow and focused (e.g., `IJwtTokenGenerator` only exposes token generation).
- **Dependency Inversion** – The Application layer depends on abstractions (interfaces), not on concrete MongoDB or JWT implementations, which are injected at runtime.

### Repository Pattern

Data access is abstracted behind repository interfaces defined in the Application/Infrastructure layer. The Application layer depends on `IDocumentRepository` and `IUserRepository` interfaces — concrete MongoDB implementations are registered via Dependency Injection and injected at runtime. This makes unit testing straightforward: repositories are mocked without any database dependency.

---

## Solution Structure

### Identity Service

```
identity/
├── src/
│   ├── Aruba.Identity.Api/
│   │   ├── Auth/
│   │   │   ├── AuthController.cs
│   │   │   ├── Requests/              # LoginRequest, RegisterRequest
│   │   │   └── Responses/             # LoginResponse
│   │   ├── Users/
│   │   │   ├── UsersController.cs
│   │   │   ├── Requests/              # UpdateUserRequest
│   │   │   └── Responses/             # UserResponse
│   │   └── Common/
│   │       ├── Middlewares/           # ExceptionHandlingMiddleware
│   │       └── Mapping/              # ApiMappingProfile (AutoMapper)
│   ├── Aruba.Identity.Application/
│   │   ├── Auth/Commands/             # Login, Register (+ Validators)
│   │   ├── Users/Commands/            # Update, Delete
│   │   ├── Users/Queries/             # GetAll, GetById, GetByEmail, Search
│   │   └── Common/Behaviors/          # LoggingBehavior, ValidationBehavior
│   ├── Aruba.Identity.Domain/
│   │   └── Models/                    # User
│   └── Aruba.Identity.Infrastructure/
│       ├── Auth/                      # JwtTokenGenerator, IJwtTokenGenerator, JwtSettings
│       └── Users/Repository/          # UserRepository, IUserRepository
└── tests/
    ├── Aruba.Identity.UnitTests/
    │   └── Domain/Models/             # UserTests
    └── Aruba.Identity.FunctionalTests/
        ├── Auth/                      # AuthControllerTests
        └── Users/                     # UsersControllerTests
```

### Document Service

```
document/
├── src/
│   ├── Aruba.Document.Api/
│   │   ├── Documents/
│   │   │   ├── DocumentsController.cs
│   │   │   ├── Requests/              # InsertDocumentRequest, UpdateDocumentRequest
│   │   │   └── Responses/             # DocumentResponse
│   │   └── Common/
│   │       ├── Middlewares/           # ExceptionHandlingMiddleware
│   │       └── Mapping/              # ApiMappingProfile (AutoMapper)
│   ├── Aruba.Document.Application/
│   │   ├── Documents/Commands/        # Insert, Update, Approve, Reject, Send, Complete, Generate, Link
│   │   ├── Documents/Queries/         # GetAll, GetById, Search
│   │   └── Common/Behaviors/          # LoggingBehavior, ValidationBehavior
│   ├── Aruba.Document.Domain/
│   │   ├── Models/                    # Document
│   │   └── Enums/                     # DocumentStatus, DocumentType
│   └── Aruba.Document.Infrastructure/
│       └── Documents/Repository/      # DocumentRepository, IDocumentRepository
└── tests/
    ├── Aruba.Document.UnitTests/
    │   └── Domain/Models/             # DocumentTests
    └── Aruba.Document.FunctionalTests/
        └── Documents/                 # DocumentsControllerTests
```

---

## API Endpoints

### Identity Service

| Method | Route | Description | Auth |
|---|---|---|---|
| `POST` | `/api/auth/register` | Register a new user | ❌ |
| `POST` | `/api/auth/login` | Login and obtain a JWT token | ❌ |
| `GET` | `/api/users` | Get all users | ✅ |
| `GET` | `/api/users/{id}` | Get user by ID | ✅ |
| `PUT` | `/api/users/{id}` | Update user | ✅ |
| `DELETE` | `/api/users/{id}` | Delete user | ✅ |

### Document Service

All endpoints require a valid JWT token.

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/documents` | Get all documents |
| `GET` | `/api/documents/{id}` | Get document by ID |
| `GET` | `/api/documents/search` | Search documents by criteria |
| `POST` | `/api/documents` | Create a new document |
| `PUT` | `/api/documents/{id}` | Update a document |
| `POST` | `/api/documents/{id}/generate` | Generate a new document from an existing one |
| `POST` | `/api/documents/{id}/link` | Link two documents together |
| `POST` | `/api/documents/{id}/send` | Transition document state → Sent |
| `POST` | `/api/documents/{id}/approve` | Transition document state → Approved |
| `POST` | `/api/documents/{id}/reject` | Transition document state → Rejected |
| `POST` | `/api/documents/{id}/complete` | Transition document state → Complete |

---

## Document Lifecycle

Documents follow a strict state machine. Invalid transitions are rejected at the domain level with a `DomainException`.

```
Incomplete ──► Complete ──► Sent ──► Approved
                                 └──► Rejected
```

Document generation follows a defined flow:

```
Quote ──► Proforma Invoice ──► Sales Order
```

Each generated document is automatically linked to its source document, preserving traceability across the chain.

---

## MongoDB Data Modeling

MongoDB was chosen as the datastore for its schema flexibility and native support for document-oriented data, which maps naturally to the domain model.

### Main Models

**Document** (base model shared by all document types):

```json
{
  "_id": "ObjectId",
  "type": "Quote | ProformaInvoice | SalesOrder",
  "status": "Incomplete | Complete | Sent | Approved | Rejected",
  "createdAt": "ISODate",
  "updatedAt": "ISODate",
  "linkedDocumentIds": ["ObjectId"],
  "sourceDocumentId": "ObjectId"
}
```

**User**:

```json
{
  "_id": "ObjectId",
  "email": "string",
  "passwordHash": "string",
  "createdAt": "ISODate"
}
```

---

## Kubernetes

Each microservice has its own set of manifests under `k8s/`. MongoDB is deployed as a StatefulSet shared across both services.

```
k8s/
├── document/
│   ├── configmap.yaml     # Non-sensitive application configuration
│   ├── secret.yaml        # JWT secret, MongoDB connection string
│   ├── deployment.yaml    # Application deployment
│   ├── service.yaml       # Internal ClusterIP service
│   └── ingress.yaml       # External HTTP routing
├── identity/
│   ├── configmap.yaml
│   ├── secret.yaml
│   ├── deployment.yaml
│   ├── service.yaml
│   └── ingress.yaml
└── mongodb/
    ├── statefulset.yaml   # MongoDB with persistent storage
    ├── pvc.yaml           # PersistentVolumeClaim for data durability
    ├── service.yaml       # Headless service for StatefulSet DNS resolution
    └── secret.yaml        # MongoDB credentials
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- MongoDB (local or via Docker)
- Docker (optional)

### Configuration

Update `appsettings.Development.json` in both services with your MongoDB connection string and JWT parameters:

```json
{
  "MongoDb": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "aruba"
  },
  "JwtSettings": {
    "Secret": "your-secret-key",
    "Issuer": "aruba-identity",
    "Audience": "aruba-document",
    "ExpiryMinutes": 60
  }
}
```

### Running the services

```bash
# Identity Service (default: https://localhost:5001)
cd identity
dotnet run --project src/Aruba.Identity.Api

# Document Service (default: https://localhost:5002)
cd document
dotnet run --project src/Aruba.Document.Api
```

Swagger UI is available at `/swagger` in development mode and includes the **Authorize 🔒** button to authenticate with a JWT token directly from the interface.

---

## Testing

### Unit Tests

Focused on domain logic and application layer behavior. Dependencies (repositories, services) are mocked. No database or HTTP infrastructure required.

```bash
dotnet test identity/tests/Aruba.Identity.UnitTests
dotnet test document/tests/Aruba.Document.UnitTests
```

Example scenarios covered:

- Valid document creation
- Document generation from a source document
- Invalid state transition throws `DomainException`
- Input validation failures

### Functional Tests

Integration tests that spin up a real in-memory instance of the application using `WebApplicationFactory`. HTTP endpoints are tested end-to-end, including authentication, validation, and persistence (using a test MongoDB instance or in-memory stub). A `TestAuthHandler` bypasses JWT validation to simplify test setup.

```bash
dotnet test identity/tests/Aruba.Identity.FunctionalTests
dotnet test document/tests/Aruba.Document.FunctionalTests
```

---

## Authentication

The Document Service requires a valid JWT token issued by the Identity Service on all protected endpoints.

1. Register via `POST /api/auth/register`
2. Login via `POST /api/auth/login` to receive a JWT token
3. Include the token in all Document Service requests:

```
Authorization: Bearer <token>
```

In Swagger UI, click **Authorize 🔒**, paste the token (without the `Bearer` prefix), and confirm. All subsequent requests will include the token automatically.