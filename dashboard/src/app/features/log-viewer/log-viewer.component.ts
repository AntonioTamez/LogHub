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
  template: `
    <div class="log-viewer">
      <h1>Log Viewer</h1>

      <!-- Filters -->
      <div class="filters">
        <select [(ngModel)]="filters.applicationId" (change)="search()">
          <option value="">All Applications</option>
          @for (app of applications(); track app.id) {
            <option [value]="app.id">{{ app.name }}</option>
          }
        </select>

        <select [(ngModel)]="filters.minLevel" (change)="search()">
          <option [ngValue]="undefined">All Levels</option>
          <option [ngValue]="0">Trace+</option>
          <option [ngValue]="1">Debug+</option>
          <option [ngValue]="2">Info+</option>
          <option [ngValue]="3">Warning+</option>
          <option [ngValue]="4">Error+</option>
          <option [ngValue]="5">Critical</option>
        </select>

        <input
          type="text"
          [(ngModel)]="filters.searchText"
          placeholder="Search logs..."
          (keyup.enter)="search()"
        />

        <button class="btn" (click)="search()">Search</button>
        <button class="btn btn-secondary" (click)="clearFilters()">Clear</button>
      </div>

      <!-- Loading -->
      @if (loading()) {
        <div class="loading">Loading...</div>
      }

      <!-- Logs Table -->
      <div class="logs-table" [class.loading-overlay]="loading()">
        <table>
          <thead>
            <tr>
              <th>Timestamp</th>
              <th>Level</th>
              <th>Application</th>
              <th>Message</th>
            </tr>
          </thead>
          <tbody>
            @for (log of logs(); track log.id) {
              <tr [class]="'level-' + log.level" (click)="selectLog(log)">
                <td class="timestamp">{{ log.timestamp | date:'short' }}</td>
                <td><span class="level-badge">{{ getLogLevelName(log.level) }}</span></td>
                <td>{{ log.applicationName }}</td>
                <td class="message">{{ log.message }}</td>
              </tr>
            }
            @if (logs().length === 0 && !loading()) {
              <tr>
                <td colspan="4" class="no-data">No logs found</td>
              </tr>
            }
          </tbody>
        </table>
      </div>

      <!-- Pagination -->
      <div class="pagination">
        <button [disabled]="!pagedResponse()?.hasPreviousPage" (click)="goToPage(currentPage() - 1)">
          Previous
        </button>
        <span>Page {{ currentPage() }} of {{ pagedResponse()?.totalPages || 1 }}</span>
        <button [disabled]="!pagedResponse()?.hasNextPage" (click)="goToPage(currentPage() + 1)">
          Next
        </button>
      </div>

      <!-- Log Detail Modal -->
      @if (selectedLog()) {
        <div class="modal-overlay" (click)="selectedLog.set(null)">
          <div class="modal" (click)="$event.stopPropagation()">
            <div class="modal-header">
              <h3>Log Details</h3>
              <button class="close-btn" (click)="selectedLog.set(null)">&times;</button>
            </div>
            <div class="modal-body">
              <div class="detail-row">
                <label>ID:</label>
                <span>{{ selectedLog()!.id }}</span>
              </div>
              <div class="detail-row">
                <label>Timestamp:</label>
                <span>{{ selectedLog()!.timestamp | date:'medium' }}</span>
              </div>
              <div class="detail-row">
                <label>Level:</label>
                <span class="level-badge" [class]="'level-' + selectedLog()!.level">
                  {{ getLogLevelName(selectedLog()!.level) }}
                </span>
              </div>
              <div class="detail-row">
                <label>Application:</label>
                <span>{{ selectedLog()!.applicationName }}</span>
              </div>
              <div class="detail-row">
                <label>Message:</label>
                <span class="message-full">{{ selectedLog()!.message }}</span>
              </div>
              @if (selectedLog()!.exception) {
                <div class="detail-row">
                  <label>Exception:</label>
                  <pre>{{ selectedLog()!.exception }}</pre>
                </div>
              }
              @if (selectedLog()!.stackTrace) {
                <div class="detail-row">
                  <label>Stack Trace:</label>
                  <pre>{{ selectedLog()!.stackTrace }}</pre>
                </div>
              }
              @if (selectedLog()!.properties) {
                <div class="detail-row">
                  <label>Properties:</label>
                  <pre>{{ selectedLog()!.properties | json }}</pre>
                </div>
              }
              @if (selectedLog()!.correlationId) {
                <div class="detail-row">
                  <label>Correlation ID:</label>
                  <span>{{ selectedLog()!.correlationId }}</span>
                </div>
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .log-viewer { padding: 1.5rem; }
    h1 { margin-bottom: 1.5rem; color: #1a1a2e; }

    .filters {
      display: flex;
      gap: 0.5rem;
      margin-bottom: 1rem;
      flex-wrap: wrap;
    }

    .filters select, .filters input {
      padding: 0.5rem;
      border: 1px solid #ddd;
      border-radius: 4px;
      font-size: 0.875rem;
    }

    .filters input { flex: 1; min-width: 200px; }

    .btn {
      padding: 0.5rem 1rem;
      border: none;
      border-radius: 4px;
      cursor: pointer;
      background: #1a1a2e;
      color: white;
    }

    .btn-secondary { background: #95a5a6; }
    .btn:hover { opacity: 0.9; }

    .logs-table {
      background: white;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      overflow: hidden;
    }

    .loading-overlay { opacity: 0.5; pointer-events: none; }
    .loading { text-align: center; padding: 2rem; color: #666; }

    table {
      width: 100%;
      border-collapse: collapse;
    }

    th, td {
      padding: 0.75rem;
      text-align: left;
      border-bottom: 1px solid #eee;
    }

    th { background: #f8f9fa; font-weight: 600; color: #333; }
    tbody tr { cursor: pointer; }
    tbody tr:hover { background: #f8f9fa; }

    .timestamp { white-space: nowrap; color: #666; font-size: 0.875rem; }
    .message { max-width: 400px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

    .level-badge {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 500;
    }

    .level-0 .level-badge, .level-1 .level-badge { background: #eee; color: #666; }
    .level-2 .level-badge { background: #d4edda; color: #155724; }
    .level-3 .level-badge { background: #fff3cd; color: #856404; }
    .level-4 .level-badge { background: #f8d7da; color: #721c24; }
    .level-5 .level-badge { background: #721c24; color: white; }

    .no-data { text-align: center; color: #666; padding: 2rem !important; }

    .pagination {
      display: flex;
      justify-content: center;
      align-items: center;
      gap: 1rem;
      padding: 1rem;
    }

    .pagination button {
      padding: 0.5rem 1rem;
      border: 1px solid #ddd;
      background: white;
      border-radius: 4px;
      cursor: pointer;
    }

    .pagination button:disabled { opacity: 0.5; cursor: not-allowed; }

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
      border-radius: 8px;
      width: 90%;
      max-width: 700px;
      max-height: 90vh;
      overflow-y: auto;
    }

    .modal-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 1rem;
      border-bottom: 1px solid #eee;
    }

    .modal-header h3 { margin: 0; }
    .close-btn { background: none; border: none; font-size: 1.5rem; cursor: pointer; }
    .modal-body { padding: 1rem; }

    .detail-row {
      margin-bottom: 1rem;
    }

    .detail-row label {
      display: block;
      font-weight: 600;
      color: #333;
      margin-bottom: 0.25rem;
    }

    .detail-row pre {
      background: #f8f9fa;
      padding: 0.5rem;
      border-radius: 4px;
      overflow-x: auto;
      font-size: 0.875rem;
    }

    .message-full { word-break: break-word; }
  `]
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
