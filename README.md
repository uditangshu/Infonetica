# Configurable Workflow Engine

A minimal backend service for managing custom workflows as configurable state machines, built with .NET 8 and ASP.NET Core Minimal APIs.

## What it does

This project implements a configurable workflow engine that allows you to:
- Define workflows as state machines with states and transitions
- Start workflow instances from definitions
- Execute actions to move instances between states with full validation
- Inspect and list states, actions, definitions, and running instances

**Core features:**
- State machine validation with comprehensive error handling
- In-memory storage with JSON file persistence
- RESTful API with OpenAPI/Swagger documentation
- Minimal dependencies and clean architecture

## Architecture

The system follows a 3-layer architecture pattern:

```
API Layer (Program.cs)
    ↓
Service Layer (WorkflowService + ValidationService)
    ↓
Repository Layer (InMemoryWorkflowRepository)
    ↓
JSON File Storage
```

- **API Layer**: Minimal API endpoints with OpenAPI documentation
- **Service Layer**: Business logic, validation, and state machine rules
- **Repository Layer**: In-memory storage with JSON file persistence

## Core Concepts

### State
A step in your workflow with the following properties:
- `id`: Unique identifier (required)
- `name`: Display name (required)
- `isInitial`: Whether this is the starting state (exactly one per definition)
- `isFinal`: Whether this is an ending state
- `enabled`: Whether the state is active
- `description`: Optional description

### Action (Transition)
A transition between states with these properties:
- `id`: Unique identifier (required)
- `name`: Display name (required)
- `enabled`: Whether the action can be executed
- `fromStates`: Array of source state IDs (required)
- `toState`: Target state ID (required)
- `description`: Optional description

### Workflow Definition
A complete workflow blueprint containing:
- `id`: Unique identifier (required)
- `name`: Display name
- `description`: Optional description
- `states`: Array of State objects (required)
- `actions`: Array of Action objects (required)
- `createdAt`: Creation timestamp
- `version`: Version number

### Workflow Instance
A running workflow with:
- `id`: Unique identifier (auto-generated)
- `definitionId`: Reference to workflow definition
- `currentState`: Current state ID
- `history`: Array of state transitions
- `createdAt`: Creation timestamp
- `updatedAt`: Last update timestamp
- `status`: Active, Completed, or Cancelled

## Quick Start

### Prerequisites
- .NET 8 SDK
- Any code editor (VS Code, Visual Studio, etc.)

### Run the Application
```bash
# Clone or download the project
cd ConfigurableWorkflowEngine

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# API available at: https://localhost:5001
# Swagger UI: https://localhost:5001/swagger
```

### Test the API

1. **Create a workflow definition**:
```bash
curl -X POST https://localhost:5001/api/workflows/definitions \
  -H "Content-Type: application/json" \
  -d @sample-workflow.json
```

2. **Start a workflow instance**:
```bash
curl -X POST https://localhost:5001/api/workflows/instances \
  -H "Content-Type: application/json" \
  -d '{"definitionId": "modern-agile-workflow"}'
```

3. **Execute an action**:
```bash
curl -X POST https://localhost:5001/api/workflows/instances/{instance-id}/actions \
  -H "Content-Type: application/json" \
  -d '{"actionId": "start_work", "executedBy": "john.doe"}'
```

## API Endpoints

### Workflow Definitions
- `POST /api/workflows/definitions` - Create workflow definition
- `GET /api/workflows/definitions/{id}` - Get workflow definition
- `GET /api/workflows/definitions` - List all definitions

### Workflow Instances
- `POST /api/workflows/instances` - Start new workflow instance
- `GET /api/workflows/instances/{id}` - Get instance with state and history
- `GET /api/workflows/instances` - List all instances
- `GET /api/workflows/definitions/{definitionId}/instances` - Get instances by definition

### Workflow Actions
- `POST /api/workflows/instances/{instanceId}/actions` - Execute action
- `GET /api/workflows/instances/{instanceId}/actions` - Get available actions

### Utility
- `GET /health` - Health check endpoint
- `GET /swagger` - API documentation

## Sample Workflow

The included `sample-workflow.json` demonstrates a modern agile workflow:

**States**: To Do → In Progress → QA Ready → Testing → Done
**Alternative paths**: Blocked, Cancelled, Reopen

**Key transitions**:
- Start Work: To Do → In Progress
- Ready for QA: In Progress → QA Ready
- Pass Testing: Testing → Done
- Fail Testing: Testing → In Progress
- Block/Unblock: Various states ↔ Blocked

## Validation Rules

The system enforces these validation rules:

### Definition Validation
- ✅ Unique definition and state/action IDs
- ✅ Exactly one initial state required
- ✅ All action state references must exist
- ✅ Required fields must be present

### Runtime Validation
- ✅ Instance can only be created from valid definition
- ✅ Actions can only be executed from valid source states
- ✅ Disabled actions/states cannot be used
- ✅ Final states cannot execute actions
- ✅ Comprehensive error messages for failures

## Implementation Details

### Technology Stack
- **Framework**: .NET 8 with Minimal APIs
- **Serialization**: System.Text.Json
- **Documentation**: Swagger/OpenAPI
- **Storage**: In-memory with JSON backup

### Key Design Decisions

1. **Minimal APIs**: Chosen for simplicity and performance
2. **In-memory storage**: Fast access with JSON persistence for durability
3. **Service layer**: Clean separation of concerns and testability
4. **Comprehensive validation**: Prevents invalid state transitions
5. **Structured error handling**: Helpful error messages for debugging

### File Structure
```
ConfigurableWorkflowEngine/
├── Models/
│   ├── State.cs
│   ├── Action.cs
│   ├── WorkflowDefinition.cs
│   ├── WorkflowInstance.cs
│   └── DTOs.cs
├── Repository/
│   ├── IWorkflowRepository.cs
│   └── InMemoryWorkflowRepository.cs
├── Services/
│   ├── IWorkflowService.cs
│   ├── WorkflowService.cs
│   ├── IWorkflowValidationService.cs
│   └── WorkflowValidationService.cs
├── Program.cs
├── sample-workflow.json
└── README.md
```

## Assumptions & Limitations

### Assumptions Made
- Single-tenant system (no authentication required)
- Synchronous processing sufficient for demo
- JSON files adequate for persistence
- UTF-8 encoding for all text
- No concurrent access from multiple clients

### Known Limitations
- No thread-safety for concurrent requests
- In-memory storage limits scalability
- No database persistence
- No rollback or undo functionality
- No workflow versioning
- No real-time notifications

### Future Enhancements
If given more time, these features would be added:
- Database persistence (Entity Framework)
- Authentication and authorization
- Concurrent access handling
- Workflow versioning and migration
- Real-time notifications (SignalR)
- Bulk operations
- Advanced validation rules
- Audit logging
- Performance monitoring

## Development Notes

This implementation took approximately 2 hours and focuses on:
- ✅ **Correctness**: All state machine rules properly enforced
- ✅ **Clarity**: Clean code structure with meaningful names
- ✅ **Maintainability**: Extensible design with proper separation of concerns
- ✅ **Pragmatism**: Appropriate abstractions without over-engineering

The code prioritizes correctness and clarity over premature optimization, making it easy to understand and extend for future requirements.

## Testing

While unit tests aren't included due to time constraints, the system can be tested via:
1. Swagger UI at `/swagger`
2. curl commands (examples above)
3. Postman collection (import OpenAPI spec)
4. Health check endpoint at `/health`

The validation system provides comprehensive error messages to help identify and fix issues quickly.

---

**Note**: This is a demonstration implementation built for evaluation purposes. Production use would require additional hardening, security measures, and scalability considerations.

