import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ApplicationService } from '../../core/services/application.service';
import { Application, CreateApplicationRequest } from '../../core/models/application.model';

@Component({
  selector: 'app-applications',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './applications.component.html'
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
