# Chess Learning Platform — Architecture Design

A hybrid chess learning platform built as a **Modular Monolith** with **Clean Architecture (lightweight)**. Supports both teacher-driven learning and self-directed learning. Tenant-ready from day one.

---

## Design Principles

- **Modular Monolith**: Each domain module is independently structured inside one deployable app. Modules communicate via application-layer interfaces (no direct DB cross-joins between modules).
- **Clean Architecture (lightweight)**: Each module has `Domain → Application → Infrastructure` layers. Skip unnecessary abstractions.
- **Tenant-ready**: `TenantId` (nullable) added to all user-owned entities, allowing future multi-tenant migration.
- **No over-engineering**: CQRS is applied at the pattern level (separate Query/Command handlers) without a full MediatR pipeline unless warranted.

---

## 1. Domain Modules

```
┌──────────────────────────────────────────────────┐
│                   Modules                         │
│                                                   │
│  Identity  │  Learning  │  Assignment  │ Content  │
│            │            │              │          │
│  Game      │  Progress  │              │          │
└──────────────────────────────────────────────────┘
```

### Module Responsibilities

| Module | Responsibility |
|---|---|
| **Identity** | Users, roles (Admin/Teacher/Student), JWT auth, refresh tokens |
| **Learning** | Learning plans, daily goals, self-learning tracking |
| **Assignment** | Teacher-created assignments, tasks, student progress on tasks |
| **Content** | Puzzles (cached from Lichess), game reviews, custom text content |
| **Game** | Imported games (Chess.com / Lichess), personal game library |
| **Progress** | Event-based progress tracking, aggregation, dashboard stats |

---

## 2. Entities & Aggregates

### Identity Module

```
User (Aggregate Root)
├── Id, Email, PasswordHash, DisplayName
├── TenantId (nullable)
├── CreatedAt, UpdatedAt
└── Roles: [Admin, Teacher, Student] (many-to-many via UserRoles)

RefreshToken
├── Id, UserId, Token, ExpiresAt, RevokedAt
```

> ℹ️ Standard ASP.NET Identity extended with `TenantId` and `DisplayName`. Roles are string-based (Admin, Teacher, Student).

---

### Learning Module

```
LearningPlan (Aggregate Root)
├── Id, UserId, TenantId
├── Title, Description
├── StartDate, EndDate
├── Status: [Active, Paused, Completed, Archived]
└── DailyGoals: List<DailyGoal>

DailyGoal (Entity, child of LearningPlan)
├── Id, LearningPlanId
├── Date, TargetMinutes, TargetTaskCount
└── IsCompleted
```

**Rules:**
- A user may have at most **5 active** Learning Plans at once (enforced in Application layer).
- A Learning Plan is **optional** — users can use the platform without one.

---

### Assignment Module

```
Assignment (Aggregate Root)
├── Id, TeacherId, StudentId
├── TenantId, LearningPlanId (nullable)
├── Title, Description
├── DueDate, Status: [Draft, Active, Completed, Overdue]
└── AssignmentTasks: List<AssignmentTask>

AssignmentTask (Entity, child of Assignment)
├── Id, AssignmentId, TaskId (→ Content.Task)
├── Order, Status: [Pending, Completed, Skipped]
└── CompletedAt (nullable)

Task (Aggregate Root — shared reference)
├── Id, ContentId (→ Content.Content)
├── ContentType: [Puzzle, GameReview, Custom]
├── Title, Instructions
├── CreatedById, IsPublic
└── CreatedAt
```

**Rules:**
- Assignment **may or may not** be linked to a `LearningPlanId`.
- `Task` is a thin wrapper: it holds instructions, not the content itself.
- `Task.ContentId` references the **Content module** across boundaries (by ID only, no direct navigation).

---

### Content Module

```
Content (Aggregate Root)
├── Id, Type: [Puzzle, GameReview, Custom]
├── Title, Body (JSON or text)
├── Source: [User, System, External]
├── CreatedById (nullable for system content)
├── TenantId (nullable)
└── CreatedAt

Puzzle (Value Object / child of Content)
├── Id, ContentId
├── ExternalId, ExternalSource: [Lichess]
├── FEN, SolutionMoves (JSON array)
├── Theme, Rating
└── LastSyncedAt

PuzzleAttempt (Aggregate Root)
├── Id, UserId, PuzzleId
├── IsSuccess, TimeSpentSeconds
└── AttemptedAt
```

**Rules:**
- Puzzles are always **cached locally** after fetching from Lichess.
- Admin and Teacher can create content; System creates imported content.
- `PuzzleAttempt` is an independent aggregate — no cascade from `Puzzle`.

---

### Game Module

```
ImportedGame (Aggregate Root)
├── Id, UserId, TenantId
├── Source: [Chesscom, Lichess]
├── ExternalId (unique per source+user)
├── PGN (raw), White, Black, Result
├── PlayedAt, ImportedAt
└── LinkedTaskId (nullable → Assignment.Task)

GameImportJob (Aggregate Root)
├── Id, UserId, Source
├── Status: [Pending, Running, Completed, Failed]
├── ScheduledAt, StartedAt, CompletedAt
├── GamesImported, ErrorMessage
└── IsAutoSync
```

**Rules:**
- `(UserId, Source, ExternalId)` is unique — prevents duplicate imports.
- Phase 1: manual import only. Phase 2: periodic auto-sync via `IsAutoSync=true` on job.
- `LinkedTaskId` is optional and set by assignment rules ("review your last 3 games").

---

### Progress Module

```
ProgressEvent (Aggregate Root)
├── Id, UserId, TenantId
├── Type: [PuzzleSolved, TaskCompleted, GameImported,
│          GameAnalyzed, DailyGoalAchieved, StreakMaintained]
├── Value (numeric, e.g. 1 for completed or accuracy %)
├── Metadata (JSON — stores context like PuzzleId, TaskId)
└── Timestamp

ProgressSummary (Read Model — computed)
├── UserId, PeriodStart, PeriodEnd
├── TotalPuzzlesSolved, PuzzleAccuracy
├── TotalTasksCompleted, TotalGamesImported
├── CurrentStreak, LongestStreak
└── TotalTimeSpentMinutes
```

**Rules:**
- Only `ProgressEvent` is written to. `ProgressSummary` is computed on-demand (or cached periodically).
- Events are immutable — never updated, only appended.

---

## 3. Key Relationships

### LearningPlan ↔ Assignment

```
LearningPlan (optional)
    │
    └─── Assignment ──── AssignmentTask ──── Task ──── Content
         (many per plan)  (ordered list)     (type)    (data)

A LearningPlan has zero or more Assignments.
An Assignment belongs to zero or one LearningPlan.
An Assignment always belongs to one Student.
```

### Task ↔ Content

```
Task
  └── ContentType: Puzzle → Content (type=Puzzle) → Puzzle (detail)
  └── ContentType: GameReview → Content (type=GameReview)
  └── ContentType: Custom → Content (type=Custom, body=text)
```

> Tasks are instructions + a reference. Content is the actual data. This allows reusing one `Content` item across many `Task`s.

---

## 4. Database Schema (High-Level)

### Identity

```sql
Users           (Id, Email, PasswordHash, DisplayName, TenantId, CreatedAt)
Roles           (Id, Name)
UserRoles       (UserId, RoleId)
RefreshTokens   (Id, UserId, Token, ExpiresAt, RevokedAt)
```

### Learning

```sql
LearningPlans   (Id, UserId, TenantId, Title, Description, StartDate, EndDate, Status, CreatedAt)
DailyGoals      (Id, LearningPlanId, Date, TargetMinutes, TargetTaskCount, IsCompleted)
```

### Assignment

```sql
Tasks           (Id, ContentId, ContentType, Title, Instructions, CreatedById, IsPublic, CreatedAt)
Assignments     (Id, TeacherId, StudentId, LearningPlanId NULL, TenantId, Title, Description, DueDate, Status, CreatedAt)
AssignmentTasks (Id, AssignmentId, TaskId, Order, Status, CompletedAt)
```

### Content

```sql
Contents        (Id, Type, Title, Body, Source, CreatedById NULL, TenantId NULL, CreatedAt)
Puzzles         (Id, ContentId, ExternalId, ExternalSource, FEN, SolutionMoves, Theme, Rating, LastSyncedAt)
PuzzleAttempts  (Id, UserId, PuzzleId, IsSuccess, TimeSpentSeconds, AttemptedAt)
```
> Index: `Puzzles(ExternalId, ExternalSource)` UNIQUE

### Game

```sql
ImportedGames   (Id, UserId, TenantId, Source, ExternalId, PGN, White, Black, Result, PlayedAt, ImportedAt, LinkedTaskId NULL)
GameImportJobs  (Id, UserId, Source, Status, IsAutoSync, ScheduledAt, StartedAt, CompletedAt, GamesImported, ErrorMessage)
```
> Index: `ImportedGames(UserId, Source, ExternalId)` UNIQUE

### Progress

```sql
ProgressEvents  (Id, UserId, TenantId, Type, Value, Metadata, Timestamp)
```
> Index: `ProgressEvents(UserId, Timestamp)` for range queries

---

## 5. REST API Endpoints

### Auth
```
POST   /api/auth/register
POST   /api/auth/login
POST   /api/auth/refresh
POST   /api/auth/logout
```

### Learning Plans
```
GET    /api/learning-plans              # My plans
POST   /api/learning-plans
GET    /api/learning-plans/{id}
PUT    /api/learning-plans/{id}
DELETE /api/learning-plans/{id}
GET    /api/learning-plans/{id}/daily-goals
POST   /api/learning-plans/{id}/daily-goals
PUT    /api/learning-plans/{id}/daily-goals/{goalId}
```

### Assignments
```
GET    /api/assignments                 # My assignments (teacher view or student view)
POST   /api/assignments                 # Teacher creates
GET    /api/assignments/{id}
PUT    /api/assignments/{id}
DELETE /api/assignments/{id}
GET    /api/assignments/{id}/tasks
POST   /api/assignments/{id}/tasks/{taskId}/complete   # Student marks complete
```

### Tasks & Content
```
GET    /api/tasks                       # My tasks (public + mine)
POST   /api/tasks
GET    /api/tasks/{id}
PUT    /api/tasks/{id}
DELETE /api/tasks/{id}

GET    /api/content                     # With ?type=Puzzle|GameReview|Custom
POST   /api/content
GET    /api/content/{id}
PUT    /api/content/{id}
DELETE /api/content/{id}
```

### Puzzles
```
GET    /api/puzzles                     # With filters: ?theme=&rating=
GET    /api/puzzles/{id}
POST   /api/puzzles/{id}/attempts       # Submit attempt
GET    /api/puzzles/{id}/attempts/me    # My attempts on a puzzle
```

### Games
```
GET    /api/games                       # My game library
GET    /api/games/{id}
POST   /api/games/import                # Manual import (body: source, username or token)
DELETE /api/games/{id}
GET    /api/games/import-jobs           # Status of import jobs
POST   /api/games/import-jobs/{id}/cancel
```

### Progress
```
GET    /api/progress/summary            # My summary stats (streak, totals)
GET    /api/progress/events             # Raw events with pagination
GET    /api/progress/dashboard          # Aggregated chart data (by day/week)
```

### Admin
```
GET    /api/admin/users
PUT    /api/admin/users/{id}/roles
GET    /api/admin/content               # Manage all content
GET    /api/admin/progress/reports      # Platform-wide stats
```

---

## 6. Background Jobs

### Technology: Hangfire (recommended)
- Persistent job storage backed by SQL Server (same DB)
- Dashboard UI at `/admin/hangfire`
- Simple setup for 1–3 dev team

### Jobs

#### `GameImportJob` (Phase 1 — triggered manually)
```
Trigger:    User hits POST /api/games/import
Queue:      default
Steps:
  1. Fetch games from Chess.com or Lichess API (paginated)
  2. Deduplicate by (UserId, Source, ExternalId)
  3. Store new games in ImportedGames table
  4. Update GameImportJob record (status, count)
  5. Fire ProgressEvent: GameImported
```

#### `GameAutoSyncJob` (Phase 2 — recurring)
```
Trigger:    Hangfire recurring job — daily at midnight
Queue:      background
Steps:
  1. Find all users with IsAutoSync=true
  2. Run GameImportJob per user
```

#### `PuzzleSyncJob` (Ongoing)
```
Trigger:    Hangfire recurring — weekly
Queue:      background
Steps:
  1. Fetch new puzzles from Lichess puzzle API
  2. Deduplicate by ExternalId
  3. Store/update Puzzles + Contents
```

#### `GameAnalysisJob` (Phase 2 — Stockfish)
```
Trigger:    User requests analysis (POST /api/games/{id}/analyze)
Queue:      analysis (separate queue with limited concurrency)
Steps:
  1. Load PGN from ImportedGames
  2. Call IChessEngineService.AnalyzeGameAsync(pgn)
  3. Store results in AnalysisResults table (future)
  4. Fire ProgressEvent: GameAnalyzed
```

#### `ProgressAggregationJob` (Ongoing)
```
Trigger:    Hangfire recurring — daily
Queue:      background
Steps:
  1. Aggregate ProgressEvents from yesterday
  2. Upsert into ProgressSummary (or materialized view)
```

---

## 7. Chess Engine Abstraction (Stockfish)

Defined in `Shared.Application` so any module can depend on it.

```csharp
// Shared.Application/Contracts/IChessEngineService.cs

public interface IChessEngineService
{
    Task<PositionAnalysis> AnalyzePositionAsync(string fen, AnalysisOptions options, CancellationToken ct = default);
    Task<GameAnalysis> AnalyzeGameAsync(string pgn, AnalysisOptions options, CancellationToken ct = default);
    Task<bool> IsHealthyAsync(CancellationToken ct = default);
}

public record AnalysisOptions(int Depth = 20, int MultiPv = 3, int TimeLimitMs = 5000);

public record PositionAnalysis(
    string Fen,
    double Evaluation,      // centipawns
    string[] BestMoves,     // UCI notation
    bool IsMate,
    int? MateIn
);

public record GameAnalysis(
    string Pgn,
    List<MoveAnalysis> Moves,
    string Summary          // "3 blunders, 2 mistakes"
);

public record MoveAnalysis(
    int MoveNumber,
    string Move,
    double EvalBefore,
    double EvalAfter,
    MoveQuality Quality     // Best, Good, Inaccuracy, Mistake, Blunder
);

public enum MoveQuality { Best, Good, Inaccuracy, Mistake, Blunder }
```

**Implementation options:**

| Option | Phase | Notes |
|---|---|---|
| `StockfishProcessService` | Phase 2 | Spawns local Stockfish process; simplest |
| `StockfishHttpService` | Phase 2 | Calls a self-hosted Stockfish REST wrapper |
| `NullChessEngineService` | Phase 1 | No-op stub, returns empty results |

Register in DI:
```csharp
// Phase 1
services.AddSingleton<IChessEngineService, NullChessEngineService>();

// Phase 2
services.AddSingleton<IChessEngineService, StockfishProcessService>();
```

---

## 8. Project Folder Structure

```
ChessLearning/
│
├── src/
│   │
│   ├── ChessLearning.Api/                         # ASP.NET Web API host
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── LearningPlansController.cs
│   │   │   ├── AssignmentsController.cs
│   │   │   ├── PuzzlesController.cs
│   │   │   ├── GamesController.cs
│   │   │   └── ProgressController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── TenantResolutionMiddleware.cs
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   ├── ChessLearning.Shared/                      # Shared kernel (no business logic)
│   │   ├── Domain/
│   │   │   ├── BaseEntity.cs                      # Id, CreatedAt, UpdatedAt
│   │   │   └── AggregateRoot.cs
│   │   ├── Application/
│   │   │   ├── Contracts/
│   │   │   │   ├── IChessEngineService.cs         # Engine abstraction
│   │   │   │   ├── ICurrentUserService.cs
│   │   │   │   └── ITenantService.cs
│   │   │   └── Models/
│   │   │       └── PagedResult.cs
│   │   └── Infrastructure/
│   │       ├── CurrentUserService.cs
│   │       └── DateTimeService.cs
│   │
│   └── Modules/
│       │
│       ├── Identity/
│       │   ├── Domain/
│       │   │   ├── User.cs
│       │   │   └── RefreshToken.cs
│       │   ├── Application/
│       │   │   ├── Commands/
│       │   │   │   ├── RegisterCommand.cs
│       │   │   │   ├── LoginCommand.cs
│       │   │   │   └── RefreshTokenCommand.cs
│       │   │   └── Services/
│       │   │       └── ITokenService.cs
│       │   └── Infrastructure/
│       │       ├── IdentityDbContext.cs
│       │       ├── TokenService.cs
│       │       └── IdentityModuleRegistration.cs
│       │
│       ├── Learning/
│       │   ├── Domain/
│       │   │   ├── LearningPlan.cs
│       │   │   └── DailyGoal.cs
│       │   ├── Application/
│       │   │   ├── Commands/
│       │   │   │   ├── CreateLearningPlanCommand.cs
│       │   │   │   └── UpdateDailyGoalCommand.cs
│       │   │   └── Queries/
│       │   │       └── GetMyLearningPlansQuery.cs
│       │   └── Infrastructure/
│       │       ├── LearningDbContext.cs
│       │       └── LearningModuleRegistration.cs
│       │
│       ├── Assignment/
│       │   ├── Domain/
│       │   │   ├── Assignment.cs
│       │   │   ├── AssignmentTask.cs
│       │   │   └── Task.cs
│       │   ├── Application/
│       │   │   ├── Commands/
│       │   │   │   ├── CreateAssignmentCommand.cs
│       │   │   │   └── CompleteAssignmentTaskCommand.cs
│       │   │   └── Queries/
│       │   │       └── GetMyAssignmentsQuery.cs
│       │   └── Infrastructure/
│       │       ├── AssignmentDbContext.cs
│       │       └── AssignmentModuleRegistration.cs
│       │
│       ├── Content/
│       │   ├── Domain/
│       │   │   ├── Content.cs
│       │   │   ├── Puzzle.cs
│       │   │   └── PuzzleAttempt.cs
│       │   ├── Application/
│       │   │   ├── Commands/
│       │   │   │   └── SubmitPuzzleAttemptCommand.cs
│       │   │   ├── Queries/
│       │   │   │   └── GetPuzzlesQuery.cs
│       │   │   └── Services/
│       │   │       └── IPuzzleProviderService.cs    # Lichess abstraction
│       │   └── Infrastructure/
│       │       ├── ContentDbContext.cs
│       │       ├── LichessPuzzleProvider.cs
│       │       └── ContentModuleRegistration.cs
│       │
│       ├── Game/
│       │   ├── Domain/
│       │   │   ├── ImportedGame.cs
│       │   │   └── GameImportJob.cs
│       │   ├── Application/
│       │   │   ├── Commands/
│       │   │   │   └── ImportGamesCommand.cs
│       │   │   ├── Queries/
│       │   │   │   └── GetMyGamesQuery.cs
│       │   │   └── Services/
│       │   │       └── IGameImportService.cs       # Chess.com / Lichess abstraction
│       │   └── Infrastructure/
│       │       ├── GameDbContext.cs
│       │       ├── ChessComImporter.cs
│       │       ├── LichessImporter.cs
│       │       └── GameModuleRegistration.cs
│       │
│       └── Progress/
│           ├── Domain/
│           │   ├── ProgressEvent.cs
│           │   └── ProgressSummary.cs             # Read model
│           ├── Application/
│           │   ├── Commands/
│           │   │   └── RecordProgressEventCommand.cs
│           │   └── Queries/
│           │       ├── GetProgressSummaryQuery.cs
│           │       └── GetDashboardDataQuery.cs
│           └── Infrastructure/
│               ├── ProgressDbContext.cs
│               └── ProgressModuleRegistration.cs
│
├── tests/
│   ├── ChessLearning.UnitTests/
│   │   ├── Learning/
│   │   ├── Assignment/
│   │   ├── Content/
│   │   └── Progress/
│   └── ChessLearning.IntegrationTests/
│       └── Api/
│
└── ChessLearning.sln
```

---

## Module Registration Pattern

Each module self-registers in `Program.cs`:

```csharp
// Program.cs
builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddLearningModule(builder.Configuration)
    .AddAssignmentModule(builder.Configuration)
    .AddContentModule(builder.Configuration)
    .AddGameModule(builder.Configuration)
    .AddProgressModule(builder.Configuration);
```

Each `*ModuleRegistration.cs` wires up its own `DbContext`, handlers, and services — keeping modules self-contained.

---

## Cross-Module Communication Rules

> [!IMPORTANT]
> Modules **never** share a `DbContext` or reference each other's entity types. Cross-module references use **IDs only**.

| From | To | Method |
|---|---|---|
| Assignment | Content | `ContentId` reference on `Task` |
| Game | Progress | Fire `ProgressEvent` via `IProgressService` interface |
| Content | Progress | Fire `ProgressEvent` after `PuzzleAttempt` |
| Assignment | Progress | Fire `ProgressEvent` after `AssignmentTask` completed |

---

## Open Questions / Future Decisions

| Topic | Decision Needed |
|---|---|
| Game analysis storage | New `AnalysisResults` table or extend `ImportedGames` with JSON column? |
| ProgressSummary caching | In-memory, Redis, or DB materialized table? |
| Admin Angular app scope | Full CMS for content, or just user management + reports? |
| Phase 2 auto-sync | Store Chess.com/Lichess API tokens per user — encryption strategy? |
