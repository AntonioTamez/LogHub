import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApplicationService } from '../../core/services/application.service';
import { Application, CreateApplicationRequest } from '../../core/models/application.model';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="applications">
      <div class="header">
        <h1>Applications</h1>
        <button class="btn" (click)="showCreateModal.set(true)">+ New Application</button>
      </div>

      <div class="apps-grid">
        @for (app of applications(); track app.id) {
          <div class="app-card" [class.inactive]="!app.isActive">
            <div class="app-header">
              <h3>{{ app.name }}</h3>
              <span class="status" [class.active]="app.isActive">
                {{ app.isActive ? 'Active' : 'Inactive' }}
              </span>
            </div>
            <p class="description">{{ app.description || 'No description' }}</p>
            <div class="api-key">
              <label>API Key:</label>
              <code>{{ showApiKey() === app.id ? app.apiKey : '••••••••••••••••' }}</code>
              <button class="btn-icon" (click)="toggleApiKey(app.id)" title="Show/Hide">
                {{ showApiKey() === app.id ? '👁' : '👁‍🗨' }}
              </button>
              <button class="btn-icon" (click)="copyApiKey(app.apiKey)" title="Copy">📋</button>
            </div>
            <div class="app-footer">
              <span class="created">Created: {{ app.createdAt | date:'short' }}</span>
              <div class="actions">
                <button class="btn-small" (click)="regenerateKey(app)">Regenerate Key</button>
                <button class="btn-small btn-toggle" (click)="toggleActive(app)">
                  {{ app.isActive ? 'Deactivate' : 'Activate' }}
                </button>
                <button class="btn-small btn-danger" (click)="deleteApp(app)">Delete</button>
              </div>
            </div>
          </div>
        }
        @if (applications().length === 0) {
          <div class="no-apps">
            <p>No applications yet.</p>
            <p>Create your first application to start logging.</p>
          </div>
        }
      </div>

      <!-- Create Modal -->
      @if (showCreateModal()) {
        <div class="modal-overlay" (click)="showCreateModal.set(false)">
          <div class="modal" (click)="$event.stopPropagation()">
            <h3>Create New Application</h3>
            <form (ngSubmit)="createApplication()">
              <div class="form-group">
                <label>Name</label>
                <input type="text" [(ngModel)]="newApp.name" name="name" required maxlength="100" />
              </div>
              <div class="form-group">
                <label>Description (optional)</label>
                <textarea [(ngModel)]="newApp.description" name="description" rows="3" maxlength="500"></textarea>
              </div>
              <div class="modal-actions">
                <button type="button" class="btn btn-secondary" (click)="showCreateModal.set(false)">Cancel</button>
                <button type="submit" class="btn" [disabled]="!newApp.name">Create</button>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .applications { padding: 1.5rem; }

    .header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 1.5rem;
    }

    h1 { color: #1a1a2e; margin: 0; }

    .btn {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 4px;
      background: #1a1a2e;
      color: white;
      cursor: pointer;
    }

    .btn-secondary { background: #95a5a6; }
    .btn:hover { opacity: 0.9; }

    .apps-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
      gap: 1rem;
    }

    .app-card {
      background: white;
      border-radius: 8px;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .app-card.inactive { opacity: 0.7; }

    .app-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 0.5rem;
    }

    .app-header h3 { margin: 0; color: #1a1a2e; }

    .status {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      background: #e74c3c;
      color: white;
    }

    .status.active { background: #27ae60; }

    .description { color: #666; margin-bottom: 1rem; }

    .api-key {
      background: #f8f9fa;
      padding: 0.75rem;
      border-radius: 4px;
      margin-bottom: 1rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .api-key label { font-weight: 600; font-size: 0.875rem; }
    .api-key code { flex: 1; font-size: 0.75rem; overflow: hidden; text-overflow: ellipsis; }

    .btn-icon {
      background: none;
      border: none;
      cursor: pointer;
      padding: 0.25rem;
      font-size: 1rem;
    }

    .app-footer {
      display: flex;
      justify-content: space-between;
      align-items: center;
      flex-wrap: wrap;
      gap: 0.5rem;
    }

    .created { font-size: 0.75rem; color: #666; }

    .actions { display: flex; gap: 0.5rem; }

    .btn-small {
      padding: 0.25rem 0.5rem;
      font-size: 0.75rem;
      border: 1px solid #ddd;
      background: white;
      border-radius: 4px;
      cursor: pointer;
    }

    .btn-small:hover { background: #f8f9fa; }
    .btn-danger { color: #e74c3c; border-color: #e74c3c; }
    .btn-toggle { color: #f39c12; border-color: #f39c12; }

    .no-apps {
      grid-column: 1 / -1;
      text-align: center;
      padding: 3rem;
      color: #666;
    }

    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background: rgba(0,0,0,0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
    }

    .modal {
      background: white;
      padding: 1.5rem;
      border-radius: 8px;
      width: 90%;
      max-width: 400px;
    }

    .modal h3 { margin: 0 0 1rem 0; }

    .form-group { margin-bottom: 1rem; }
    .form-group label { display: block; margin-bottom: 0.5rem; font-weight: 500; }
    .form-group input, .form-group textarea {
      width: 100%;
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      box-sizing: border-box;
    }

    .modal-actions {
      display: flex;
      justify-content: flex-end;
      gap: 0.5rem;
    }
  `]
})
export class ApplicationsComponent implements OnInit {
  private applicationService = inject(ApplicationService);

  applications = signal<Application[]>([]);
  showCreateModal = signal(false);
  showApiKey = signal<string | null>(null);

  newApp: CreateApplicationRequest = { name: '', description: '' };

  ngOnInit(): void {
    this.loadApplications();
  }

  loadApplications(): void {
    this.applicationService.getApplications().subscribe(apps => this.applications.set(apps));
  }

  createApplication(): void {
    this.applicationService.createApplication(this.newApp).subscribe({
      next: () => {
        this.loadApplications();
        this.showCreateModal.set(false);
        this.newApp = { name: '', description: '' };
      }
    });
  }

  toggleApiKey(id: string): void {
    this.showApiKey.set(this.showApiKey() === id ? null : id);
  }

  copyApiKey(apiKey: string): void {
    navigator.clipboard.writeText(apiKey);
  }

  regenerateKey(app: Application): void {
    if (confirm(`Regenerate API key for "${app.name}"? The old key will stop working.`)) {
      this.applicationService.regenerateApiKey(app.id).subscribe(() => this.loadApplications());
    }
  }

  toggleActive(app: Application): void {
    this.applicationService.updateApplication(app.id, { isActive: !app.isActive })
      .subscribe(() => this.loadApplications());
  }

  deleteApp(app: Application): void {
    if (confirm(`Delete "${app.name}"? This will also delete all its logs.`)) {
      this.applicationService.deleteApplication(app.id).subscribe(() => this.loadApplications());
    }
  }
}
