import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';

@Component({
  selector: 'app-user-audit',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './user-audit.component.html',
  styleUrl: './user-audit.component.scss'
})
export class UserAuditComponent implements OnInit {
  private http = inject(HttpClient);
  private route = inject(ActivatedRoute);
  
  auditData = signal<any>(null);

  ngOnInit(): void {
    const userId = this.route.snapshot.paramMap.get('id')!;
    this.http.get<any>(`http://localhost:5200/api/backoffice/users/${userId}/audit`).subscribe({
      next: (data) => this.auditData.set(data),
      error: (err) => alert('Failed to load audit data: ' + err.message)
    });
  }
}
