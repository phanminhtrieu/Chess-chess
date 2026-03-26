import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-role-selection',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './role-selection.component.html',
  styleUrl: './role-selection.component.css'
})
export class RoleSelectionComponent {
  auth = inject(AuthService);
  router = inject(Router);

  selectRole(role: string) {
    this.auth.setRole(role).subscribe({
      next: () => {
        const path = role === 'Teacher' ? '/teacher/dashboard' : '/student/dashboard';
        this.router.navigate([path]);
      }
    });
  }
}
