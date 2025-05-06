# ToDo API

A minimal REST API for managing ToDo tasks built with **.NET 8**, **PostgreSQL**, and **Docker**. Supports full CRUD operations, task status updates, and filtering by date. Includes unit and integration tests with xUnit.

## ✅ Features

- Create, read, update, and delete (CRUD) ToDo tasks
- Filter tasks for **today**, **next day**, or **this week**
- Mark tasks as done
- Update task completion percentage
- Data persistence using **PostgreSQL**
- Validation using Data Annotations
- Unit and integration tests with **xUnit**
- Docker support for easy deployment
- Follows **Conventional Commits**
- Fully commented and ready to compile

---

## 🚀 Technologies Used

- [.NET 8 Minimal API](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL](https://www.postgresql.org/)
- [xUnit](https://xunit.net/)
- [Docker](https://www.docker.com/)
- [Docker Compose (optional)](https://docs.docker.com/compose/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## 🧪 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [PostgreSQL](https://www.postgresql.org/) (if not using Docker Compose)

---

## 🐳 Run with Docker

```bash
docker build -t todo-api .
docker run -p 8080:80 todo-api
```
Using docker compose
```bash
docker-compose up --build
```

🛠 Run Locally (Without Docker)
```bash
dotnet restore
dotnet ef database update
dotnet run --project ToDoApi
```
API will run on: http://localhost:5000 (or specified port)

📌 API Endpoints
Method	Endpoint	Description
GET	/todoitems	Get all tasks
GET	/todoitems/{id}	Get task by ID
GET	/todoitems/today	Get tasks for today
GET	/todoitems/nextday	Get tasks for tomorrow
GET	/todoitems/thisweek	Get tasks for this week
POST	/todoitems	Create a new task
PUT	/todoitems/{id}	Update task
DELETE	/todoitems/{id}	Delete task
PATCH	/todoitems/{id}	Set task completion to 100%
PATCH	/todoitems/{id}/done	Mark task as done (IsDone = true)
✅ Validation Rules

    Title: required, 3–100 characters

    Expiry: required, must be a future date

    CompletePercent: 0–100 only

    Description: optional

🧪 Run Tests
```bash
dotnet test
```

