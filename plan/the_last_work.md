# Role-Based UI Implementation Plan

Follow Stitch designs (Chess Web Dashboard PRD) to build role-aware pages in the Angular frontend and seed all user roles in the backend.

## Background

- **Frontend**: Angular 19, lazy-loaded pages, [AuthService](file:///d:/Chess%20chess/frontend/src/app/core/auth/auth.service.ts#6-97) signal (`currentUser.role`), JWT stored in `localStorage`.
- **Backend**: .NET 8 modular monolith, ASP.NET Identity, JWT auth. [UserProfileDto](file:///d:/Chess%20chess/backend/src/Modules/Identity/ChessLearning.Modules.Identity/Application/Users/UserDtos.cs#5-6) already returns `Role`. No data seeding exists yet.
- **Roles identified**: `Student`, `Teacher`, `Admin`.

## User Review Required

> [!IMPORTANT]
> **Role assignment on registration**: The current `RegisterUserCommand` does not accept a role parameter yet. The plan adds a `Role Selection` screen *after* registration (post-login, first-time prompt). Please confirm this is preferable to choosing the role *during* registration.

> [!NOTE]
> Seeded user passwords will be `Test1234` (meets the current 4-char minimum policy). All seed users are for development only.

---

## Proposed Changes

#### 1. Backend – User Seeding [DONE]

#### [MODIFY] [Program.cs](file:///d:/Chess%20chess/backend/src/ChessLearning.Api/Program.cs)
Add a `SeedDataAsync` call in `app.Run()` block that seeds roles (`Student`, `Teacher`, `Admin`) and three demo users:

| DisplayName       | Email                   | Role    | Password  |
|-------------------|-------------------------|---------|-----------|
| Curator Ivory     | teacher@chess.local     | Teacher | Test1234  |
| Scholar Novak     | student@chess.local     | Student | Test1234  |
| Admin Root        | admin@chess.local       | Admin   | Test1234  |

#### [NEW] [IdentitySeedData.cs](file:///d:/Chess%20chess/backend/src/Modules/Identity/ChessLearning.Modules.Identity/Infrastructure/IdentitySeedData.cs)
Static helper class `IdentitySeedData.SeedAsync(IServiceProvider)` using `RoleManager<IdentityRole<Guid>>` and `UserManager<User>`. Guards against re-seeding (checks `roleManager.RoleExistsAsync`).

---

### 2. Backend – Role in Registration [DONE]

#### [MODIFY] [RegisterUserCommand.cs](file:///d:/Chess%20chess/backend/src/Modules/Identity/ChessLearning.Modules.Identity/Application/Users/Commands/RegisterUserCommand.cs)
Removed default "Student" role assignment to enforce post-login role selection.

---

### 3. Frontend – Core Auth & Routing [DONE]

#### [MODIFY] [auth.service.ts](file:///d:/Chess%20chess/frontend/src/app/core/auth/auth.service.ts)
- Expose `role` from `currentUser` signal as a computed `userRole`.
- Add `isTeacher`, `isStudent` helpers.
- After login, navigate to `/select-role` if `role` is null/empty, otherwise to `/dashboard`.

#### [MODIFY] [auth.guard.ts](file:///d:/Chess%20chess/frontend/src/app/core/auth/auth.guard.ts)
- Update guard to support `data.roles` array on routes, rejecting unauthorized access.

#### [MODIFY] [app.routes.ts](file:///d:/Chess%20chess/frontend/src/app/app.routes.ts)
Add new lazy routes:

```
/select-role        → RoleSelectionComponent   (authenticated, no role required)
/dashboard          → redirects based on role
/student/dashboard  → StudentDashboardComponent  (role: Student)
/teacher/dashboard  → TeacherDashboardComponent  (role: Teacher)
/teacher/assignments/create → AssignmentCreateComponent (role: Teacher)
```

---

### 4. Frontend – Shared Shell Layout [DONE]

#### [NEW] [shell/](file:///d:/Chess%20chess/frontend/src/app/pages/shell/)
`ShellComponent` wraps all authenticated routes with:
- **Left sidebar** (Material Icons): Dashboard, Assignments, Puzzles, Progress, Settings (links differ by role)
- **Top navbar**: App name ("Archivist Portal"), user avatar/name, logout
- **Design tokens**: implemented via Tailwind CDN and root variables.

---

### 5. Frontend – Role Selection Page [DONE]

#### [NEW] [pages/role-selection/](file:///d:/Chess%20chess/frontend/src/app/pages/role-selection/)
`RoleSelectionComponent`:
- Two premium cards: **"I'm a Student"** / **"I'm a Teacher"**
- On select → `POST /api/frontend/users/set-role` → navigate to appropriate dashboard.

---

### 6. Frontend – Student Portal Dashboard [DONE]

#### [NEW] [pages/student/dashboard/](file:///d:/Chess%20chess/frontend/src/app/pages/student/)
`StudentDashboardComponent` (matches Stitch "Student Portal Dashboard v3"):
- **Bento Grid Layout**: Urgent Banner, Daily Protocol, Tactical Drill, Progress.

---

### 7. Frontend – Teacher Portal Dashboard [DONE]

#### [NEW] [pages/teacher/dashboard/](file:///d:/Chess%20chess/frontend/src/app/pages/teacher/)
`TeacherDashboardComponent` (matches Stitch "Teacher Portal Dashboard v3"):
- **Bento Grid Layout**: Critical Attention list, Quick Actions, Performance stats.

---

### 8. Frontend – Assignment Creation Page [PARTIAL] [TODO: Connect to Backend]

#### [NEW] [pages/teacher/assignment-create/](file:///d:/Chess%20chess/frontend/src/app/pages/teacher/)
`AssignmentCreateComponent` (matches Stitch "Assignment Creation Draft"):
- **Initial form and UI implemented**. Needs backend connection for persistence.

---

### 9. Backend – Set Role Endpoint [DONE]

#### [NEW] [SetRoleCommand.cs](file:///d:/Chess%20chess/backend/src/Modules/Identity/ChessLearning.Modules.Identity/Application/Users/Commands/SetRoleCommand.cs)
Command handler that assigns roles; now **idempotent** to allow multiple calls.

---

## Progress Update - March 27, 2026

### 1. UI & Design System (Modern Archivist)
- **Tailwind Integration**: Configured Tailwind Play CDN in `index.html` to enable immediate rendering of sophisticated layout utilities (`grid-cols-12`, `gap-8`, `shadow-xl`) without build configuration hurdles.
- **Layout Overhaul**: 
    - Resolved **sidebar/header overlap** by strictly defining z-indices and applying a fixed `ml-64` margin to the main content canvas.
    - Updated **Shell Header** and **Sidebar** with high-fidelity "Grandmaster Ivory" typography (Noto Serif/Manrope).
    - Fixed **Font Rendering**: Explicitly defined `font-headline`, `font-body`, and `font-label` fallbacks to ensure consistency across browsers.
- **Bento Grid Implementation**: 
    - Full implementation of the multi-column dashboard layouts for both Student and Teacher views.
    - Dynamic visual feedback for "Urgent" tasks using `error-container` and `primary` accent levels.
- **Decorative Elements**: Added large background glyphs (♙/♔) to the Role Selection screen for a premium "Boutique" feel.

### 2. Auth Flow & State Stabilization
- **Eliminated Race Conditions**: 
    - `AuthService.login()` now waits for the full user profile to be fetched before completing, ensuring the application enters the dashboard with a known role.
    - `AuthService.setRole()` is now synchronous with the profile refresh, preventing UI flickers where the role appeared missing for a split second.
- **Improved AuthGuard**: 
    - The guard now observes an `onReady()` state, effectively waiting for background profile verification (e.g., on page refresh) before determining if a fallback to `/select-role` is necessary.
- **Backend Idempotency**: 
    - Updated `SetRoleCommandHandler` to safely check for existing roles. This resolved the "User already has an assigned role" crash when users re-selected or refreshed the role selection page.
    - Prepared for **multiple role support** by allowing cumulative role assignments instead of exclusive locks.

---
4. **Guard**: Manually navigate to `/teacher/dashboard` while logged in as Student → should redirect to `/student/dashboard`
