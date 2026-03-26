import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-list.component.html',
  styleUrl: './user-list.component.scss'
})
export class UserListComponent implements OnInit {
  private http = inject(HttpClient);
  users = signal<any[]>([]);

  ngOnInit(): void {
    this.http.get<any[]>('http://localhost:5200/api/backoffice/users').subscribe({
      next: (data) => this.users.set(data),
      error: (err) => console.error(err)
    });
  }
}
