---
description: Update task status in implementation plan and sync with Notion
---

To use this workflow, specify the task name to mark as completed.

1. **Update Local Plan**:
   - Open `plan/implementation-plan.md`.
   - Find the task item and change `[ ]` to `[x]`.

2. **Phase Completion Check**:
   - Check if all tasks under the current Phase (e.g., Phase 1) are marked as `[x]`.
   - If ANY task in the Phase is still `[ ]`, stop here.

3. **Notion Synchronization**:
// turbo
   - If the Phase is 100% completed, find the corresponding Phase Task in the Notion database `90d7fbf6-2458-82c7-9998-01785f06a359` (linked to project `32e7fbf6-2458-80ce-8451-fb3ea13b2f1d`).
   - Use `API-patch-page` to update its `Status` property to `Done`.
   - Add a comment to the Project Page `32e7fbf6-2458-80ce-8451-fb3ea13b2f1d` celebrating the phase completion.

4. **Summary**:
   - Provide a summary of the local and remote changes made.
