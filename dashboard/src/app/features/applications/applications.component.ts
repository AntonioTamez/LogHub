import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApplicationService } from '../../core/services/application.service';
import { AlertService } from '../../core/services/alert.service';
import { Application, CreateApplicationRequest } from '../../core/models/application.model';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './applications.component.html'
})
export class ApplicationsComponent implements OnInit {
  private applicationService = inject(ApplicationService);
  private alertService = inject(AlertService);

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
        this.alertService.toast('Application created successfully', 'success');
      },
      error: (err) => {
        this.alertService.error('Creation Failed', err.error?.error || 'Failed to create application');
      }
    });
  }

  toggleApiKey(id: string): void {
    this.showApiKey.set(this.showApiKey() === id ? null : id);
  }

  copyApiKey(apiKey: string): void {
    navigator.clipboard.writeText(apiKey);
    this.alertService.toast('API key copied to clipboard', 'success');
  }

  async regenerateKey(app: Application): Promise<void> {
    const confirmed = await this.alertService.confirmDanger(
      'Regenerate API Key',
      `Regenerate API key for "${app.name}"? The old key will stop working immediately.`,
      'Regenerate',
      'Cancel'
    );

    if (confirmed) {
      this.applicationService.regenerateApiKey(app.id).subscribe({
        next: () => {
          this.loadApplications();
          this.alertService.toast('API key regenerated', 'success');
        },
        error: () => {
          this.alertService.error('Error', 'Failed to regenerate API key');
        }
      });
    }
  }

  toggleActive(app: Application): void {
    this.applicationService.updateApplication(app.id, { isActive: !app.isActive })
      .subscribe({
        next: () => {
          this.loadApplications();
          this.alertService.toast(
            app.isActive ? 'Application disabled' : 'Application enabled',
            app.isActive ? 'warning' : 'success'
          );
        },
        error: () => {
          this.alertService.error('Error', 'Failed to update application');
        }
      });
  }

  async deleteApp(app: Application): Promise<void> {
    const confirmed = await this.alertService.confirmDanger(
      'Delete Application',
      `Delete "${app.name}"? This will permanently delete all its logs and cannot be undone.`,
      'Delete',
      'Cancel'
    );

    if (confirmed) {
      this.applicationService.deleteApplication(app.id).subscribe({
        next: () => {
          this.loadApplications();
          this.alertService.toast('Application deleted', 'success');
        },
        error: () => {
          this.alertService.error('Error', 'Failed to delete application');
        }
      });
    }
  }
}
