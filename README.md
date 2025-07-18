# Configurable Workflow Engine

## Inspired by Jira: Workflows as Configuration

In powerful systems like Atlassian Jira, workflows are not rigid; they are customizable state machines that adapt to a team's specific process. A task can move from "To Do" to "In Progress" to "Done," or follow a more complex path with reviews, approvals, and quality checks.

This project is inspired by that flexibility. It's a lightweight, API-driven engine built on a simple idea: **what if your business logic was defined by configuration, not by code?**

By defining states and transitions in a simple JSON format, developers can create and modify complex, stateful processes on the fly, without ever needing to write new code or redeploy the application. It's the backend for any system that needs a truly configurable workflow.

---

## Architecture: Clean & Modern

This project uses a **Vertical Slice Architecture**, organizing code by feature for maximum clarity and maintainability.

-   **`/Features`**: The core of the application. All business logic for each API endpoint lives here.
-   **`/Endpoints`**: Defines the API routes and connects them to the correct feature handler.
-   **`/Services`**: Contains shared business logic and validation rules used by multiple features.
-   **`/Infrastructure`**: The data access layer, responsible for saving and retrieving data.
-   **`/Core`**: Shared interfaces (contracts) and application-wide settings.
-   **`/Models`**: The C# representations of our data (e.g., `WorkflowDefinition`, `WorkflowInstance`).

---

## API Endpoints

The engine provides a simple RESTful API to manage and execute workflows.

| Method | Path                                                 | Description                                      |
| :----- | :--------------------------------------------------- | :----------------------------------------------- |
| `POST` | `/api/workflows/definitions`                         | Creates a new workflow template.                 |
| `GET`  | `/api/workflows/definitions`                         | Retrieves all available workflow templates.      |
| `GET`  | `/api/workflows/definitions/{id}`                    | Retrieves a specific workflow template by its ID.|
| `PATCH`| `/api/workflows/definitions/{id}/states`             | Incrementally adds, updates, or removes states.  |
| `PATCH`| `/api/workflows/definitions/{id}/actions`            | Incrementally adds, updates, or removes actions. |
| `POST` | `/api/workflows/instances`                           | Creates a new live instance of a workflow.       |
| `GET`  | `/api/workflows/instances`                           | Retrieves all live workflow instances.           |
| `GET`  | `/api/workflows/instances/{id}`                      | Retrieves a specific workflow instance by its ID.|
| `GET`  | `/api/workflows/instances/{id}/actions`              | Gets the list of available actions for an instance.|
| `POST` | `/api/workflows/instances/{id}/actions`              | Executes an action to move an instance to a new state.|
| `GET`  | `/health`                                            | A simple health check endpoint.                  |

---

## Quick Start

### Prerequisites
- .NET 9 SDK (or later)

### Running the Application
1.  **Clone the repository.**

2.  **Run the application:**
    ```bash
    dotnet run
    ```
- The API will be available at `http://localhost:5000`.
- Interactive API documentation (Swagger UI) is available at `http://localhost:5000/swagger`.

---

## Assumptions & Known Limitations

-   **In-Memory Storage**: The repository uses a simple in-memory dictionary with a JSON file backup. This is great for rapid development but is **not suitable for production**. It does not scale beyond a single instance and is not performant for a large number of workflows.
-   **No Authentication**: The API endpoints are currently open and do not require authentication.
-   **Single-Threaded Assumption**: The repository uses a simple `lock` for basic thread safety, but it has not been hardened for high-concurrency scenarios.
-   **No Database**: There is no persistent database, making data management and backups reliant on the local JSON files.

---

## Roadmap & Future Work

This is the foundational version of the engine. Here are some of the planned improvements:

-   **[ ] Implement API Key Authentication**: Secure the API endpoints to ensure that only authorized clients can interact with the engine.
-   **[ ] Deploy the Application**: Host the service on a public cloud provider and provide a live URL for interaction and development.
-   **[ ] Enhance Validation**: Add more complex validation rules, such as role-based permissions for executing certain actions thorugh api key auth.

