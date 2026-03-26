import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink, Router } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-user-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './user-detail.component.html',
  styleUrl: './user-detail.component.scss'
})
export class UserDetailComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  
  userId!: string;
  userForm = signal<any>(null);

  ngOnInit(): void {
    this.userId = this.route.snapshot.paramMap.get('id')!;
    
    // We fetch users and find this one, since the spec didn't outline a single user endpoint
    this.http.get<any[]>('http://localhost:5200/api/backoffice/users').subscribe(users => {
      const user = users.find(u => u.id === this.userId);
      if (user) {
        this.userForm.set(this.fb.group({
          displayName: [user.displayName, Validators.required],
          role: [user.role, Validators.required]
        }));
      }
    });
  }

  onSave() {
    this.http.put(`http://localhost:5200/api/backoffice/users/${this.userId}`, this.userForm().value).subscribe({
      next: () => alert('User updated successfully'),
      error: err => alert('Failed to update user')
    });
  }

  revokeSessions() {
    if (confirm('Are you sure you want to revoke all sessions for this user?')) {
      this.http.post(`http://localhost:5200/api/backoffice/users/${this.userId}/revoke-sessions`, {}).subscribe({
        next: () => alert('Sessions revoked'),
        error: () => alert('Failed to revoke sessions')
      });
    }
  }

  deactivateUser() {
    if (confirm('Are you sure you want to deactivate this user? They will not be able to log in.')) {
      this.http.post(`http://localhost:5200/api/backoffice/users/${this.userId}/deactivate`, {}).subscribe({
        next: () => {
          alert('User deactivated');
          this.router.navigate(['/users']);
        },
        error: () => alert('Failed to deactivate user')
      });
    }
  }
}
