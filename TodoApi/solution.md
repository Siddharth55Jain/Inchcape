### 1. Incorrect HTTP Usage

* All endpoints used the `POST` verb, including read, update, and delete operations.

### 2. Tight Coupling

* The controller directly instantiated `TodoService` using `new`, making the application tightly coupled and difficult to test.
* Dependency Injection was not used.

### 3. No Layered Architecture

* Database access, business logic, and controller logic were mixed together.
* The service class contained raw SQL queries.

### 4. SQL Injection Risk

* SQL queries were created using string interpolation.
* This allowed malicious input to modify SQL statements.

### 5. Synchronous Database Operations

* All database operations were synchronous, reducing scalability under concurrent requests.

### 6. Resource Management

* Disposable database resources were not consistently scoped, making resource management less explicit.

### 7. Inconsistent API Design

* The Get endpoint performed two different operations:

  * Returning a single TODO by ID.
  * Returning all TODOs when no ID was supplied.
* These responsibilities were separated into dedicated endpoints.

### 8. Exception Handling

* Internal exception messages were returned directly to the client.

### 9. Missing Abstraction

* Interfaces were not used, making future extension and testing more difficult.

### 10. Limited Testing

* Existing unit tests instantiated concrete services directly and were not aligned with the refactored architecture.

---

# Architectural Decisions

The application was refactored into a layered architecture.

```
Controller
      │
      ▼
Service
      │
      ▼
Repository
      │
      ▼
SQLite Database
```

### Controller

Responsible for:

* Handling HTTP requests
* Returning appropriate HTTP status codes
* Exception handling and logging

### Service

Responsible for:

* Business logic
* Validation
* Coordinating repository calls

### Repository

Responsible for:

* Database access
* Executing SQL queries
* Mapping database records to model objects


---

# Improvements Implemented

* Introduced Dependency Injection.
* Added interfaces for the service and repository layers.
* Implemented Repository Pattern.
* Converted all CRUD operations to asynchronous methods.
* Replaced dynamic SQL with parameterized queries.
* Added a dedicated endpoint to retrieve all TODO items.
* Simplified the Get endpoint to retrieve a single TODO by ID.
* Used appropriate HTTP verbs (`GET`, `POST`, `PUT`, `DELETE`).
* Added structured exception handling with logging.
* Returned generic error messages instead of exposing exception details.
* Scoped all disposable database resources using explicit `using` blocks.
* Improved overall project structure by separating controllers, services, repositories, interfaces, and models.

---

# How to Run

## Prerequisites

* .NET SDK 8.0 (or compatible version)

## Build

```bash
dotnet build
```

## Run

```bash
dotnet run
```

The application automatically creates the SQLite database (`todos.db`) if it does not already exist.

---

# Running Tests

```bash
dotnet test
```

---

# API Documentation

## Create TODO

**POST**

```
/api/Todo/createTodo
```

Request Body

```json
{
  "title": "Buy Milk",
  "description": "From supermarket",
  "isCompleted": false
}
```

---

## Get TODO by Id

**GET**

```
/api/Todo/getTodo?id=1
```

---

## Get All TODOs

**GET**

```
/api/Todo/getAllTodo
```

---

## Update TODO

**PUT**

```
/api/Todo/updateTodo
```

Request Body

```json
{
  "id": 1,
  "title": "Buy Bread",
  "description": "Whole wheat",
  "isCompleted": true
}
```

---

## Delete TODO

**DELETE**

```
/api/Todo/deleteTodo
```

Request Body

```json
{
  "id": 1
}
```

---

# Future Improvements

If additional time were available, the following enhancements would be implemented:

* Introduce a global exception handling middleware.
* Used AWS Secrets Manager to save secrets
* Add pagination, filtering, and sorting for retrieving TODO items.
* Used Redis based caching mechanisms to improve performance for frequently accessed data.
* Add authentication and authorization.
* Add structured logging and application monitoring, like request and response monitoring via jaeger.
* Add Swagger annotations for improved API documentation.
