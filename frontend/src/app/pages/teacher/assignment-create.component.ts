import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormArray } from '@angular/forms';

@Component({
  selector: 'app-assignment-create',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="assignment-create-page fadeIn">
      <header class="page-header sticky-top">
        <div class="header-left">
          <h1 class="page-title">Curate New Assignment</h1>
          <p class="subtitle">Design a tactical study sequence for your scholars.</p>
        </div>
        <div class="header-right">
          <button class="draft-btn">Save Draft</button>
          <button (click)="onSubmit()" class="submit-btn primary">Design Protocol</button>
        </div>
      </header>

      <div class="curator-grid">
        <!-- Sidebar: Task Sequence -->
        <aside class="sequence-panel">
          <header class="panel-header">
            <h3 class="panel-title">Task Sequence</h3>
          </header>

          <div class="sequence-list">
            <div *ngFor="let task of tasks.controls; let i = index" class="task-card">
              <span class="task-num">0{{ i + 1 }}</span>
              <div class="task-info">
                <span class="task-type">{{ task.value.type }}</span>
                <span class="task-name">{{ task.value.name }}</span>
              </div>
              <button (click)="removeTask(i)" class="remove-btn">×</button>
            </div>

            <button (click)="addTask()" class="add-task-btn">+ Add Sequence Fragment</button>
          </div>
        </aside>

        <!-- Main Content: Content Selection -->
        <main class="content-curation">
          <section class="selection-section">
            <h3 class="section-title">Recommended Fragments</h3>
            <div class="selection-grid">
              <div (click)="addFragment('Puzzle Suite', 'Sicilian Tactics')" class="fragment-card">
                <span class="fragment-icon">♞</span>
                <span class="fragment-name">Sicilian Master Pack</span>
                <span class="fragment-stats">15 puzzles</span>
              </div>
              <div (click)="addFragment('Game Review', 'Morphy 1858 Opera')" class="fragment-card">
                <span class="fragment-icon">♛</span>
                <span class="fragment-name">Opera House Masterpiece</span>
                <span class="fragment-stats">Lesson</span>
              </div>
            </div>
          </section>

          <section class="selection-section">
            <h3 class="section-title">From Your Collection</h3>
            <div class="selection-grid">
              <div (click)="addFragment('Tactical Drills', 'Endgame Puzzles')" class="fragment-card dimmed">
                <span class="fragment-icon">♜</span>
                <span class="fragment-name">The Final Frontier</span>
                <span class="fragment-stats">22 puzzles</span>
              </div>
            </div>
          </section>
        </main>
      </div>
    </div>
  `,
  styles: [`
    .assignment-create-page { animation: fadeIn 0.4s ease-out; height: 100%; display: flex; flex-direction: column; }

    .sticky-top {
      position: sticky; top: 0; background: #0b0d10; z-index: 10;
      padding-bottom: 2rem; border-bottom: 1px solid rgba(255, 255, 255, 0.05);
      display: flex; justify-content: space-between; align-items: flex-end;
    }

    .page-title { font-family: 'Noto Serif', serif; font-size: 2.2rem; margin-bottom: 0.5rem; color: #f1f5f9; }

    .subtitle { color: rgba(148, 163, 184, 0.6); font-size: 1rem; }

    .header-right { display: flex; gap: 1.5rem; }

    .draft-btn { background: transparent; color: #94a3b8; border: 1px solid rgba(255, 255, 255, 0.1); padding: 0.8rem 1.8rem; border-radius: 8px; cursor: pointer; font-weight: 600; }

    .submit-btn.primary { background: #d1b48c; color: #020617; border: none; padding: 0.8rem 2.2rem; border-radius: 8px; font-weight: 700; cursor: pointer; box-shadow: 0 4px 15px rgba(209, 180, 140, 0.2); }

    .curator-grid { display: grid; grid-template-columns: 350px 1fr; gap: 4rem; padding-top: 3rem; flex: 1; }

    .sequence-panel { display: flex; flex-direction: column; gap: 2rem; }

    .panel-title { font-size: 1.1rem; text-transform: uppercase; letter-spacing: 0.1rem; color: #d1b48c; font-weight: 700; }

    .sequence-list { display: flex; flex-direction: column; gap: 1.2rem; }

    .task-card {
      background: #121418; border: 1px solid rgba(255, 255, 255, 0.04); padding: 1.5rem; border-radius: 12px;
      display: flex; align-items: center; gap: 1.5rem; position: relative;
    }

    .task-num { color: rgba(209, 180, 140, 0.3); font-family: 'Noto Serif', serif; font-size: 1.1rem; font-weight: 700; }

    .task-info { display: flex; flex-direction: column; gap: 0.2rem; }

    .task-type { color: rgba(148, 163, 184, 0.5); font-size: 0.75rem; text-transform: uppercase; font-weight: 700; }

    .task-name { color: #f1f5f9; font-size: 0.95rem; font-weight: 500; }

    .remove-btn { position: absolute; top: 1rem; right: 1rem; background: transparent; border: none; color: #ef4444; font-size: 1.2rem; cursor: pointer; opacity: 0; transition: opacity 0.2s; }

    .task-card:hover .remove-btn { opacity: 0.6; }

    .add-task-btn {
      background: rgba(255, 255, 255, 0.02); border: 1px dashed rgba(255, 255, 255, 0.1); color: rgba(255, 255, 255, 0.4);
      padding: 1.5rem; border-radius: 12px; cursor: pointer; font-weight: 500; transition: all 0.2s;
    }

    .add-task-btn:hover { background: rgba(209, 180, 140, 0.04); border-color: rgba(209, 180, 140, 0.2); color: #d1b48c; }

    .content-curation { display: flex; flex-direction: column; gap: 4.5rem; }

    .section-title { font-family: 'Noto Serif', serif; font-size: 1.4rem; color: #f1f5f9; margin-bottom: 2rem; opacity: 0.8; }

    .selection-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1.5rem; }

    .fragment-card {
      background: #121418; border: 1px solid rgba(255, 255, 255, 0.04); padding: 2.2rem; border-radius: 16px;
      cursor: pointer; transition: all 0.3s cubic-bezier(0.2, 0, 0.2, 1);
      display: flex; flex-direction: column; gap: 0.8rem;
    }

    .fragment-card:hover { transform: translateY(-5px); border-color: rgba(209, 180, 140, 0.2); background: rgba(209, 180, 140, 0.02); }

    .fragment-icon { font-size: 1.8rem; color: #d1b48c; margin-bottom: 0.5rem; }

    .fragment-name { color: #f1f5f9; font-weight: 600; font-size: 1.05rem; }

    .fragment-stats { color: rgba(148, 163, 184, 0.5); font-size: 0.85rem; font-weight: 500; }

    .dimmed { opacity: 0.4; }

    @keyframes fadeIn { from { opacity: 0; transform: translateY(15px); } to { opacity: 1; transform: translateY(0); } }
  `]
})
export class AssignmentCreateComponent {
  tasks: FormArray;

  constructor(private fb: FormBuilder) {
    this.tasks = this.fb.array([
      this.fb.group({ type: 'Puzzle Suite', name: 'Tactical Foundation' })
    ]);
  }

  addTask() {
    this.tasks.push(this.fb.group({ type: 'New Task', name: 'Click to configure' }));
  }

  addFragment(type: string, name: string) {
    this.tasks.push(this.fb.group({ type, name }));
  }

  removeTask(index: number) {
    this.tasks.removeAt(index);
  }

  onSubmit() {
    console.log('Protocol Sequence:', this.tasks.value);
    alert('Assignment protocol sequence designed.');
  }
}
