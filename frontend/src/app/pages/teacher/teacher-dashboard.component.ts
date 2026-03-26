import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="max-w-6xl mx-auto">
      <!-- Header Section -->
      <header class="mb-12">
        <h1 class="text-5xl font-headline text-[var(--primary)] tracking-tight mb-2">Curator's Overview</h1>
        <p class="text-[var(--on-surface-variant)] max-w-2xl leading-relaxed">
          Welcome back, Grandmaster. Your students are currently engaged in the <span class="italic font-serif">Sicilian Defense</span> module. Three pupils require immediate strategic intervention.
        </p>
      </header>

      <!-- Bento Grid Layout -->
      <div class="grid grid-cols-12 gap-8">
        <!-- Primary Widget: Critical Attention Required (Span 8) -->
        <section class="col-span-12 lg:col-span-8 bg-[var(--surface-container-low)] rounded-xl p-8 flex flex-col border border-[var(--outline-variant)]/10">
          <div class="flex justify-between items-end mb-8">
            <div>
              <h2 class="text-3xl font-headline text-[var(--primary)] mb-1">Critical Attention Required</h2>
              <p class="text-[10px] text-[var(--on-surface-variant)] font-bold uppercase tracking-widest opacity-60">Overdue Submissions</p>
            </div>
            <button class="text-sm font-semibold text-[var(--primary)] underline decoration-2 underline-offset-4">View All Alerts</button>
          </div>
          
          <div class="space-y-4">
            <div *ngFor="let student of overdueStudents" class="bg-[var(--surface-container-lowest)] p-6 rounded-lg flex items-center justify-between group cursor-pointer border border-transparent hover:border-[var(--outline-variant)]/20 transition-all">
              <div class="flex items-center space-x-5">
                <div class="w-12 h-12 rounded-full overflow-hidden bg-[var(--surface-variant)]">
                  <img [src]="student.avatar" class="w-full h-full object-cover">
                </div>
                <div>
                  <h3 class="text-lg font-headline text-[var(--primary)]">{{ student.name }}</h3>
                  <p class="text-sm text-[var(--on-surface-variant)] leading-relaxed">{{ student.module }} • <span class="text-[var(--error)] font-bold">{{ student.delay }} late</span></p>
                </div>
              </div>
              <button class="opacity-0 group-hover:opacity-100 transition-opacity bg-[var(--primary)] text-white px-4 py-2 rounded-md text-[10px] font-bold tracking-widest uppercase">Send Nudge</button>
            </div>
          </div>
        </section>

        <!-- Secondary Widget: Quick Actions -->
        <section class="col-span-12 lg:col-span-4 flex flex-col space-y-6">
          <div class="bg-[var(--primary)] text-white p-8 rounded-xl h-full flex flex-col justify-between shadow-xl">
            <div>
              <h2 class="text-2xl font-headline mb-2 text-white">Quick Actions</h2>
              <p class="text-white/60 text-sm mb-8">Execute administrative tasks with precision.</p>
            </div>
            <div class="space-y-3">
              <button class="action-btn">
                <span class="font-medium">Create Assignment</span>
                <span class="material-symbols-outlined">edit_document</span>
              </button>
              <button class="action-btn">
                <span class="font-medium">Content Curation</span>
                <span class="material-symbols-outlined">auto_stories</span>
              </button>
              <button class="action-btn">
                <span class="font-medium">Broadcast Class Update</span>
                <span class="material-symbols-outlined">campaign</span>
              </button>
            </div>
          </div>
        </section>

        <!-- Class Performance Overview (Span 6) -->
        <section class="col-span-12 lg:col-span-6 bg-[var(--surface-container-high)] p-8 rounded-xl border border-[var(--outline-variant)]/10">
          <div class="mb-8">
            <h2 class="text-2xl font-headline text-[var(--primary)] mb-1">Performance Overview</h2>
            <p class="text-[10px] text-[var(--on-surface-variant)] font-bold uppercase tracking-widest opacity-60">Global Metrics</p>
          </div>
          <div class="grid grid-cols-2 gap-8">
            <div>
              <div class="text-5xl font-headline text-[var(--primary)] mb-2">92<span class="text-2xl opacity-50">%</span></div>
              <div class="text-sm text-[var(--on-surface-variant)] font-semibold">Engagement</div>
              <div class="mt-4 h-1 w-full bg-[var(--outline-variant)]/30 rounded-full overflow-hidden">
                <div class="bg-[var(--primary)] h-full w-[92%]"></div>
              </div>
            </div>
            <div>
              <div class="text-5xl font-headline text-[var(--primary)] mb-2">78<span class="text-2xl opacity-50">%</span></div>
              <div class="text-sm text-[var(--on-surface-variant)] font-semibold">Completion</div>
              <div class="mt-4 h-1 w-full bg-[var(--outline-variant)]/30 rounded-full overflow-hidden">
                <div class="bg-[var(--primary)] h-full w-[78%]"></div>
              </div>
            </div>
          </div>
          <div class="mt-12 p-6 bg-[var(--surface-container-lowest)] rounded-lg border border-[var(--outline-variant)]/10">
            <div class="flex items-center justify-between mb-2">
              <span class="text-sm font-bold text-[var(--primary)] font-label uppercase tracking-widest opacity-70">ELO GAIN</span>
              <span class="text-sm font-headline italic text-[var(--primary)]">+42 points</span>
            </div>
            <p class="text-xs text-[var(--on-surface-variant)] leading-relaxed">Your class is performing 12% above the institutional average.</p>
          </div>
        </section>

        <!-- Recent Activity (Span 6) -->
        <section class="col-span-12 lg:col-span-6 bg-[var(--surface-container-low)] p-8 rounded-xl border border-[var(--outline-variant)]/10">
          <div class="mb-8 flex justify-between items-center">
            <div>
              <h2 class="text-2xl font-headline text-[var(--primary)] mb-1">Recent Activity</h2>
              <p class="text-[10px] text-[var(--on-surface-variant)] font-bold uppercase tracking-widest opacity-60">Class Log</p>
            </div>
            <span class="material-symbols-outlined text-[var(--primary)] opacity-40">history</span>
          </div>
          <div class="space-y-6">
            <div *ngFor="let log of activityLog" class="flex items-start space-x-4">
              <div class="mt-1.5 w-2 h-2 rounded-full bg-[var(--primary)] shrink-0"></div>
              <div>
                <p class="text-sm leading-relaxed"><span class="font-bold">{{log.student}}</span> {{log.action}} <span class="italic text-[var(--primary)]">{{log.extra}}</span></p>
                <p class="text-[10px] text-[var(--on-surface-variant)] font-bold tracking-widest uppercase opacity-50 mt-1">{{log.time}}</p>
              </div>
            </div>
          </div>
        </section>
      </div>

      <!-- Footer Module -->
      <footer class="mt-16 pt-12 border-t border-[var(--outline-variant)]/15 flex justify-between items-center">
        <div class="flex space-x-12">
          <div>
            <span class="text-[10px] uppercase font-bold text-[var(--on-surface-variant)] opacity-60 block mb-1">Students</span>
            <span class="font-headline text-2xl text-[var(--primary)]">24</span>
          </div>
          <div>
            <span class="text-[10px] uppercase font-bold text-[var(--on-surface-variant)] opacity-60 block mb-1">Hours</span>
            <span class="font-headline text-2xl text-[var(--primary)]">1,420</span>
          </div>
        </div>
        <div class="flex items-center space-x-2 text-[var(--on-surface-variant)] text-xs opacity-60">
          <span class="material-symbols-outlined text-lg">verified</span>
          <span class="uppercase font-bold tracking-widest">Credentials Active</span>
        </div>
      </footer>
    </div>
  `,
  styles: [`
    :host { display: block; }
    .action-btn {
      width: 100%;
      background-color: rgba(255,255,255,0.1);
      border: 1px solid rgba(255,255,255,0.1);
      color: white;
      padding: 1rem;
      border-radius: 0.5rem;
      display: flex;
      align-items: center;
      justify-content: space-between;
      transition: all 0.2s;
    }
    .action-btn:hover { background-color: rgba(255,255,255,0.2); }
  `]
})
export class TeacherDashboardComponent {
  overdueStudents = [
    { name: 'Marcus Holloway', module: 'Tactical Endgames III', delay: '4 days', avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuCE_vTwnBecbLdkN3By0WfQPihKwi1646nG0rhieB3jNUDFz4DxMHNxF0oHZ9VSMNzL39o7AfIAoZK2RuqGmonow-WjMXwOjNWaC1EbaIPaXpPU38cZBmPljYUBMOB8kzeziPS49vPPET7ax6kddg8t1kGyQS8gqh0a-fVMrK4xOTKzxWmr4L89RBNSPUhoFsEi8Mn3VPFajqruaa6pSBI114NjFR-0mglr2P7RMWuXHwpRMIo3HVVLJjQazbDTILqP2KGQ_yqJ' },
    { name: 'Elena Rodriguez', module: 'Opening Principles', delay: '2 days', avatar: 'https://lh3.googleusercontent.com/aida-public/AB6AXuAsHEZw2ayDG2wPAcaLCp5dYgDj1m2De6gt_aXw4yRCcQJj3hk1rPi4XWdECwt1VuCjXvo03RA12TWtP7NfbpZdTRx-XuO5sBrC1Q3y2JN4pJa1pFFnlYzROiqekmK6u20WH1qvicaNgqrfynQiqKyxkYD4h7bGMgwk6_iz_4THOALyVrgsnn6i4LLHWwtl5sYelECM2BFtCx899Mly_Ak8HKM43qNsD0xkU4X3cmOXXpbRzqpsarXwmcUkgihGog0NgW7GlMFU' }
  ];

  activityLog = [
    { student: 'Elena Rodriguez', action: 'submitted', extra: 'Opening Principles (Advanced)', time: '12 minutes ago' },
    { student: 'Marcus Holloway', action: 'imported', extra: 'Sicilian Defense vs AI Level 8', time: '2 hours ago' }
  ];
}
