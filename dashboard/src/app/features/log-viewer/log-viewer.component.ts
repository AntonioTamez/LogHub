import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogService } from '../../core/services/log.service';
import { ApplicationService } from '../../core/services/application.service';
import { LogEntry, LogLevel, LogQueryParams, PagedResponse } from '../../core/models/log-entry.model';
import { Application } from '../../core/models/application.model';

@Component({
  selector: 'app-log-viewer',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './log-viewer.component.html',
  styleUrl: './log-viewer.component.scss'
})
export class LogViewerComponent implements OnInit {
  private logService = inject(LogService);
  private applicationService = inject(ApplicationService);

  applications = signal<Application[]>([]);
  logs = signal<LogEntry[]>([]);
  pagedResponse = signal<PagedResponse<LogEntry> | null>(null);
  loading = signal(false);
  selectedLog = signal<LogEntry | null>(null);
  currentPage = signal(1);

  filters: LogQueryParams = {
    page: 1,
    pageSize: 20,
    sortDescending: true
  };

  ngOnInit(): void {
    this.applicationService.getApplications().subscribe(apps => this.applications.set(apps));
    this.search();
  }

  search(): void {
    this.filters.page = 1;
    this.currentPage.set(1);
    this.loadLogs();
  }

  loadLogs(): void {
    this.loading.set(true);
    this.logService.getLogs(this.filters).subscribe({
      next: (response) => {
        this.logs.set(response.items);
        this.pagedResponse.set(response);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  goToPage(page: number): void {
    this.filters.page = page;
    this.currentPage.set(page);
    this.loadLogs();
  }

  clearFilters(): void {
    this.filters = { page: 1, pageSize: 20, sortDescending: true };
    this.search();
  }

  selectLog(log: LogEntry): void {
    this.selectedLog.set(log);
  }

  getLogLevelName(level: LogLevel): string {
    return ['Trace', 'Debug', 'Info', 'Warning', 'Error', 'Critical'][level] || 'Unknown';
  }
}
