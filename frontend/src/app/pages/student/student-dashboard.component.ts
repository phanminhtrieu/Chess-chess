import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="max-w-6xl mx-auto">
      <!-- Header Section -->
      <header class="mb-16">
        <h1 class="font-headline text-5xl font-bold text-[var(--primary)] mb-4 tracking-tight">Archival Overview</h1>
        <p class="text-[var(--on-surface-variant)] max-w-2xl leading-relaxed">
          Welcome back, Scholar. Your training curriculum requires attention. Review your active learning plans and resolve pending tactical drills.
        </p>
      </header>

      <!-- Bento Grid Layout -->
      <div class="grid grid-cols-12 gap-8">
        <!-- Primary Widget: Continue Learning (Hero Span) -->
        <section class="col-span-12 lg:col-span-8 bg-[var(--error-container)] rounded-xl p-8 relative overflow-hidden group border border-[var(--error)]/10">
          <div class="relative z-10 flex flex-col h-full justify-between">
            <div>
              <span class="bg-[var(--error)] text-white px-3 py-1 rounded-sm text-[10px] font-bold tracking-widest uppercase inline-block mb-6">Urgent: Overdue</span>
              <h2 class="font-headline text-4xl text-[var(--on-error-container)] font-bold mb-2">Endgame Refinement: Lucena Position</h2>
              <p class="text-[var(--on-error-container)]/80 max-w-md font-medium">This assignment was due 2 days ago. Master the critical rook and pawn endgames to secure your promotion to Expert rank.</p>
            </div>
            <div class="mt-12">
              <button class="bg-[var(--primary)] text-white px-8 py-3 rounded-md font-bold tracking-wide hover:opacity-90 transition-all flex items-center space-x-2">
                <span>Resume Assignment</span>
                <span class="material-symbols-outlined text-sm">arrow_forward</span>
              </button>
            </div>
          </div>
          <div class="absolute right-0 bottom-0 top-0 w-1/3 opacity-10 group-hover:opacity-20 transition-opacity">
            <span class="material-symbols-outlined text-[12rem] absolute -right-8 -bottom-8">history_edu</span>
          </div>
        </section>

        <!-- Secondary Widget: Daily Goal Progress -->
        <section class="col-span-12 lg:col-span-4 bg-[var(--surface-container)] rounded-xl p-8 flex flex-col border-l-4 border-[var(--primary)] h-full">
          <div class="flex justify-between items-start mb-8">
            <h3 class="font-headline text-2xl font-bold">Daily Protocol</h3>
            <span class="font-headline text-xl italic text-[var(--on-surface-variant)]">2/4</span>
          </div>
          <div class="space-y-4 flex-1">
            <div *ngFor="let task of dailyTasks" class="flex items-center space-x-4 p-3 bg-[var(--surface-container-lowest)] rounded cursor-pointer group border border-transparent hover:border-[var(--outline-variant)]/30 transition-all">
              <input type="checkbox" [checked]="task.done" class="w-5 h-5 accent-[var(--primary)]">
              <span class="text-[var(--on-surface-variant)] font-medium group-hover:text-[var(--primary)] transition-colors">{{ task.label }}</span>
            </div>
          </div>
        </section>

        <!-- Tactical Drill Widget -->
        <section class="col-span-12 lg:col-span-5 bg-[var(--surface-container-low)] rounded-xl p-8 overflow-hidden relative border border-[var(--outline-variant)]/10">
          <div class="flex items-center justify-between mb-6">
            <h3 class="font-headline text-2xl font-bold">Tactical Drill</h3>
            <span class="text-[10px] font-bold text-[var(--on-surface-variant)] bg-[var(--surface-container-highest)] px-2 py-1 rounded">DAILY PUZZLE</span>
          </div>
          <div class="aspect-square w-full bg-[var(--primary-container)] rounded-lg flex items-center justify-center relative group overflow-hidden">
            <img src="https://lh3.googleusercontent.com/aida-public/AB6AXuAkwQ9x-GKDLyPbcilZPd-_8V0k_Nl5e-NHjH0FNFXEKffxlFQfN_NOucBtO8QhtRfD5NQyOy2ATheLczhcnlnH9SO02TEs0NC5z0YjqPlke0TfOlRQ1064herVTgI6-p0G9ATYjApQADow6ljcI8tfsKShFJno9VebephNROflKu6JwbmpL0zlQ9iJrjyXKRiID2UkAESJHZk46JNdDjbPUoTED1RYmO9hsUsD8KhoJXlvOX3DcQ7HDB0b8lGMR3_ilNTo6Acl" 
                 class="w-full h-full object-cover opacity-60 group-hover:scale-105 transition-transform duration-700">
            <div class="absolute inset-0 flex flex-col items-center justify-center p-6 text-center">
              <span class="material-symbols-outlined text-white mb-2 text-4xl">lightbulb</span>
              <p class="text-white font-headline text-xl mb-4">White to move and win material.</p>
              <button class="bg-white text-[var(--primary)] px-6 py-2 rounded-md font-bold text-sm hover:shadow-lg transition-all">Solve on Lichess</button>
            </div>
          </div>
        </section>

        <!-- Progress Summary & Active Plans Container -->
        <div class="col-span-12 lg:col-span-7 grid grid-cols-1 gap-8">
          <!-- Progress Summary -->
          <section class="bg-[var(--surface-container-lowest)] rounded-xl p-8 shadow-[0_20px_40px_rgba(27,28,24,0.04)] border border-[var(--outline-variant)]/10">
            <div class="flex items-center justify-between mb-8">
              <h3 class="font-headline text-2xl font-bold">Progress Summary</h3>
              <button class="text-[var(--primary)] text-sm font-bold flex items-center space-x-1 underline">Detailed Analytics</button>
            </div>
            <div class="grid grid-cols-3 gap-8">
              <div class="text-center">
                <div class="font-headline text-4xl font-bold text-[var(--primary)] mb-1">1,284</div>
                <div class="text-[10px] uppercase tracking-widest text-[var(--on-surface-variant)] font-bold">Puzzles</div>
              </div>
              <div class="text-center">
                <div class="font-headline text-4xl font-bold text-[var(--primary)] mb-1">82%</div>
                <div class="text-[10px] uppercase tracking-widest text-[var(--on-surface-variant)] font-bold">Rate</div>
              </div>
              <div class="text-center">
                <div class="font-headline text-4xl font-bold text-[var(--primary)] mb-1">2,140</div>
                <div class="text-[10px] uppercase tracking-widest text-[var(--on-surface-variant)] font-bold">ELO</div>
              </div>
            </div>
          </section>

          <!-- Active Learning Plans -->
          <section class="bg-[var(--surface-container)] rounded-xl p-8 border border-[var(--outline-variant)]/10">
            <h3 class="font-headline text-2xl font-bold mb-6">Active Learning Plans</h3>
            <ul class="space-y-3">
              <li *ngFor="let plan of learningPlans" class="flex items-center justify-between p-4 bg-[var(--surface-container-lowest)] rounded group hover:bg-[var(--primary)] hover:text-white transition-all cursor-pointer">
                <div class="flex items-center space-x-4">
                  <span class="material-symbols-outlined">{{ plan.icon }}</span>
                  <span class="font-medium">{{ plan.title }}</span>
                </div>
                <span class="text-xs font-bold opacity-60 group-hover:opacity-100">{{ plan.progress }}%</span>
              </li>
            </ul>
          </section>
        </div>

        <!-- Recent Activity Stream -->
        <section class="col-span-12 bg-[var(--surface)] p-8 border-t border-[var(--outline-variant)]/15 mt-8">
          <div class="max-w-4xl mx-auto">
            <h3 class="font-headline text-3xl font-bold mb-10 text-center">Chronological Activity</h3>
            <div class="space-y-12 relative before:absolute before:left-[1.65rem] before:top-4 before:bottom-4 before:w-[1px] before:bg-[var(--outline-variant)]/30">
              <div class="relative flex space-x-8 items-start">
                <div class="z-10 w-14 h-14 rounded-full bg-[var(--primary)] flex items-center justify-center text-white flex-shrink-0 shadow-lg">
                  <span class="material-symbols-outlined">check_circle</span>
                </div>
                <div class="pt-2">
                  <time class="text-[10px] font-bold uppercase tracking-widest text-[var(--on-surface-variant)] mb-1 block opacity-60">2 Hours Ago</time>
                  <h4 class="font-headline text-xl font-bold mb-2">Tactical Mastery Achieved</h4>
                  <p class="text-[var(--on-surface-variant)] leading-relaxed font-medium">Completed 15 advanced tactical motifs including 'The Windmill'. Streak maintained for 4 days.</p>
                </div>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  `,
  styles: [`
    :host { display: block; }
  `]
})
export class StudentDashboardComponent {
  dailyTasks = [
    { label: 'Solve 10 Tactical Puzzles', done: true },
    { label: 'Analyze 1 Rapid Game', done: true },
    { label: 'Read 1 Lesson Module', done: false },
    { label: 'Play 2 Blitz Rounds', done: false }
  ];

  learningPlans = [
    { title: 'Mastering the Sicilian Defense', icon: 'menu_book', progress: 65 },
    { title: 'Calculation Fundamentals', icon: 'psychology', progress: 12 },
    { title: 'Minor Piece Endgames', icon: 'grid_view', progress: 40 }
  ];
}
