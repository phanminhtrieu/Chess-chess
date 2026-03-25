# Assignment Module Implementation Summary

This document summarizes the changes made to implement the **Assignment Module** based on the updated `specs/assignment.yaml` specification.

## 1. Domain Model Alignment (Updated)
The domain entities have been strictly aligned with the latest domain specification to ensure consistency across the Modular Monolith:
- **Assignment.cs**: 
  - Reduced fields to: `Id`, `Title`, `DueDate`, `Status`, `CompletedAt`, `CreatedById` (removing legacy `TeacherId`, `StudentId`, `TenantId`, `LearningPlanId`, and `Description`).
  - Managed state machine transitions (`Draft`, `Active`, `Completed`, `Overdue`).
  - Implemented `FinalizeAssignment` logic and `CompletedAt` tracking.
- **AssignmentTask.cs**: 
  - Tracks individual task status (`Pending`, `Completed`).
  - Removed internal `Skipped` status to match spec requirements.
- **LearningTask.cs** (formerly `Task.cs`): 
  - Renamed from `Task` to avoid collisions with `System.Threading.Tasks.Task`.
  - Added `IsPublic` (bool) and `CreatedById` (string).

## 2. CQRS Implementation
Expanded the Command-Query architecture to support separate Frontend and Backoffice layers:
- **Frontend Commands**: 
  - `CreateAssignmentCommand`, `CompleteTaskCommand`, `CompleteAssignmentCommand`.
- **Backoffice Commands**: 
  - `AddTaskToAssignmentCommand`, `ActivateAssignmentCommand`, `FinalizeAssignmentCommand`, `HandleOverdueWorkCommand`.
- **Queries**:
  - **Shared/Frontend**: `GetAssignmentDetailQuery`, `ListAssignmentsQuery`.
  - **Backoffice**: `AssignmentDashboardQuery`, `TaskListInspectionQuery`, `ContentCorrelationViewQuery`.

## 3. Background Processing
- **OverdueMonitorJob**: A background worker registered with **Hangfire** that periodically scans for Active assignments past their DueDate and marks them as Overdue automatically via `MarkAssignmentOverdueCommand`.

## 4. API Layer Separation
Implemented discrete API controllers to manage separate access patterns for students and administrators:

### Frontend API (`api/frontend/assignments`)
- `POST /`: Create assignment (Draft).
- `GET /`: List student assignments (with optional Status filter).
- `GET /{id}`: Detailed assignment and task view.
- `POST /{id}/tasks/{taskId}/complete`: Mark task as done.

### Backoffice API (`api/backoffice/assignments`)
- `POST /`: Administrative assignment creation.
- `POST /{id}/tasks`: Add tasks (only for Draft status).
- `POST /{id}/activate`: Activate assignment for students.
- `POST /{id}/finalize`: Manually finalize completion.
- `POST /{id}/mark-overdue`: Manually trigger overdue state.
- `GET /dashboard`: Grouped status statistics and recent activity.
- `GET /{id}/progress`: Detailed review of assignment progress.
- `GET /{id}/tasks`: Raw inspection of all tasks (timestamps + order).
- `GET /{id}/content-map`: Track tasks back to their source content (Puzzle/Game).

## 5. Database Schema Alignment
- Created and applied `AlignmentWithSpecs` migration.
- Synchronized SQL Server schema by dropping legacy fields and adding completion tracking.
- Unified `CreatedById` identifiers as strings across the module to support various identity sources.

## 6. Technology Stack
- **Backend**: .NET 9, EF Core 9.
- **Background Jobs**: Hangfire (Recurring).
- **Architecture**: Domain-Driven Design (DDD) with CQRS and Modular Monolith pattern.
