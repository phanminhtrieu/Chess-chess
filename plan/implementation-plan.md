# Chess Learning Platform — Implementation Plan

This document outlines the step-by-step implementation plan for the **Chess Learning Platform**.

## Phase 1: Foundation & Core Modules (Week 1-2)

### 1.1 Infrastructure Setup
- [ ] Initialize .NET Solution and projects according to the modular monolith architecture.
- [ ] Set up **Entity Framework Core** for each module (Individual DbContexts).
- [ ] Configure **Shared Kernel** (BaseEntity, ICurrentUserService, etc.).
- [ ] Implement **Identity Module**: Register, Login, JWT verification with Role-based access (Admin, Teacher, Student).
- [ ] Set up **Hangfire** for background job management.

### 1.2 Learning & Assignment Modules
- [ ] Implement **Learning Module**: CRUD for Learning Plans and Daily Goals.
- [ ] Implement **Assignment Module**: CRUD for Assignments and AssignmentTasks.
- [ ] Implement Task entity link to ContentId.
- [ ] Unit tests for Learning Plan limits (max 5 active) and Assignment state transitions.

### 1.3 Content Module (Puzzles & CMS)
- [ ] Implement **Content Module**: Support for Puzzle, GameReview, and Custom content types.
- [ ] Implement **Lichess API** sync integration (fetching puzzles).
- [ ] Support local caching of external puzzles.

## Phase 2: Game Import & Progress Tracking (Week 3-4)

### 2.1 Game Library
- [ ] Implement **Game Module**: Integration with Chess.com and Lichess APIs for manual game imports.
- [ ] Create background jobs (`GameImportJob`) to handle large imports asynchronously.
- [ ] Ensure deduplication to prevent importing the same game twice.

### 2.2 Progress & Analytics
- [ ] Implement **Progress Module**: Event-based tracking (PuzzleSolved, TaskCompleted, etc.).
- [ ] Implement `ProgressSummary` calculation (Read model logic).
- [ ] Build Aggregate/Summary API for student dashboards.

## Phase 3: Advanced Features & Polish (Week 5+)

### 3.1 Chess Engine Integration
- [ ] Implement `StockfishProcessService` for local game analysis.
- [ ] Add `GameAnalysisJob` to generate blunder checks and move evaluations.
- [ ] Update frontend/API to display move quality annotations.

### 3.2 UI/UX Development
- [ ] Finalize standard dashboard views for Students and Teachers.
- [ ] Implement interactive Board components for puzzles and game reviews.

---

## Technical Stack
- **Backend**: .NET 8/9, EF Core, SQL Server.
- **Background Jobs**: Hangfire.
- **APIs**: Lichess API, Chess.com API.
- **Engine**: Stockfish.
