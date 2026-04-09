<!--
  Sync Impact Report:
  - Version change: 0.0.0 → 1.0.0
  - List of modified principles:
    - [PRINCIPLE_1_NAME] → I. Code Quality & Architecture
    - [PRINCIPLE_2_NAME] → II. Testing Excellence
    - [PRINCIPLE_3_NAME] → III. UX Consistency & "Modern Archivist" Design
    - [PRINCIPLE_4_NAME] → IV. Performance First
    - [PRINCIPLE_5_NAME] → V. Documentation & Traceability
  - Added sections: Governance
  - Removed sections: None
  - Templates requiring updates: ✅ plan-template.md, ✅ spec-template.md, ✅ tasks-template.md (already generic)
  - Follow-up TODOs: None
-->

# Chess Learning Platform Constitution

## Core Principles

### I. Code Quality & Architecture
Implement features following Clean Architecture patterns with strictly defined domain boundaries.
- **Rules**:
  - Keep the domain logic decoupled from external frameworks and UI.
  - Use strongly typed contracts (TypeScript/C#) for all inter-module communication.
  - Favor composition over inheritance; ensure components are focused and reusable.
- **Rationale**: Ensures long-term maintainability and allows the system to evolve without massive refactors.

### II. Testing Excellence (NON-NEGOTIABLE)
Deliver reliable software through a test-driven approach and comprehensive safe-guards.
- **Rules**:
  - TDD is mandatory for domain logic: tests must be written and observed failing before implementation.
  - Maintain high coverage (80%+) for domain services and API logic.
  - Critical user journeys must be protected by automated integration and E2E tests.
- **Rationale**: Provides the confidence to iterate quickly and refactor safely.

### III. UX Consistency & "Modern Archivist" Design
Provide a premium, cohesive experience that balances sophistication with functionality.
- **Rules**:
  - Strictly adhere to the "Modern Archivist" design system tokens (Bento grids, glassmorphism, Outfit/Inter typography).
  - Interfaces MUST be responsive and functional across all device types (mobile, tablet, desktop).
  - Support both Light and Dark modes with curated, harmonious color palettes.
- **Rationale**: A high-end visual experience builds user trust and distinguishes the platform in a competitive market.

### IV. Performance First
Ensure the platform feels alive and responsive through aggressive optimization.
- **Rules**:
  - Direct user interactions (e.g., chess move inputs) must respond in under 100ms.
  - Target a First Contentful Paint (FCP) of <1.5s on 4G network conditions.
  - Use lazy-loading and efficient asset management to minimize bundle sizes.
- **Rationale**: Speed is a feature; performance bottlenecks directly correlate with user drop-off in educational tools.

### V. Documentation & Traceability
Maintain a clear "source of truth" across code, specifications, and project management.
- **Rules**:
  - Every feature MUST have an associated spec.md and plan.md in the `.specify/` system.
  - Task progress in tasks.md MUST be synchronized with Notion project tracking.
  - Document "why" in code comments for complex logic, not just "what".
- **Rationale**: Eliminates ambiguity and ensures seamless collaboration between AI agents and human developers.

## Governance

The Constitution is the supreme governing document of this repository. All implementations, refactors, and contributions MUST comply with its principles.

- **Amendment Process**: Amendments require a version bump (Semantic Versioning) and a Sync Impact Report documenting the changes.
- **Enforcement**: PR reviews and CI gates (where possible) should validate compliance with these principles.
- **Review Frequency**: The Constitution should be reviewed at the start of every major milestone to ensure it still reflects project needs.

**Version**: 1.0.0 | **Ratified**: 2026-03-27 | **Last Amended**: 2026-03-27
