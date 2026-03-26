import { Component, inject, signal } from '@angular/core';
import { Router, RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/auth/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './shell.component.html',
  styleUrl: './shell.component.css'
})
export class ShellComponent {
  auth = inject(AuthService);
  router = inject(Router);
  currentUser = this.auth.currentUser;
  isDropdownOpen = false;

  toggleDropdown() {
    this.isDropdownOpen = !this.isDropdownOpen;
  }

  logout() {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  rolePath() {
    return this.currentUser()?.role?.toLowerCase() || 'student';
  }
}
