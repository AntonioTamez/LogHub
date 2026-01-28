import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { LogService } from '../../core/services/log.service';
import { SignalRService } from '../../core/services/signalr.service';
import { DashboardStats, DashboardSummary } from '../../core/models/dashboard.model';
import { LogLevel } from '../../core/models/log-entry.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  template: `
    <div class="dashboard">
      <h1>Dashboard</h1>

      <!-- Summary Cards -->
      <div class="summary-cards">
        <div class="card">
          <div class="card-value">{{ summary()?.totalApplications || 0 }}</div>
          <div class="card-label">Applications</div>
        </div>
        <div class="card">
          <div class="card-value">{{ summary()?.logsLast24Hours || 0 }}</div>
          <div class="card-label">Logs (24h)</div>
        </div>
        <div class="card card-warning">
          <div class="card-value">{{ summary()?.warningsLast24Hours || 0 }}</div>
          <div class="card-label">Warnings (24h)</div>
        </div>
        <div class="card card-error">
          <div class="card-value">{{ summary()?.errorsLast24Hours || 0 }}</div>
          <div class="card-label">Errors (24h)</div>
        </div>
      </div>

      <!-- Charts -->
      <div class="charts-row">
        <div class="chart-container">
          <h3>Logs by Level</h3>
          @if (barChartData()) {
            <canvas baseChart
              [type]="'bar'"
              [data]="barChartData()!"
              [options]="barChartOptions">
            </canvas>
          }
        </div>

        <div class="chart-container">
          <h3>Logs by Application</h3>
          @if (pieChartData()) {
            <canvas baseChart
              [type]="'pie'"
              [data]="pieChartData()!"
              [options]="pieChartOptions">
            </canvas>
          }
        </div>
      </div>

      <div class="chart-container full-width">
        <h3>Logs Over Time</h3>
        @if (lineChartData()) {
          <canvas baseChart
            [type]="'line'"
            [data]="lineChartData()!"
            [options]="lineChartOptions">
          </canvas>
        }
      </div>

      <!-- Real-time Logs -->
      <div class="realtime-logs">
        <h3>
          Real-time Logs
          <span class="connection-status" [class.connected]="signalRService.connectionState() === 'Connected'">
            {{ signalRService.connectionState() === 'Connected' ? 'Connected' : 'Disconnected' }}
          </span>
        </h3>
        <div class="log-list">
          @for (log of signalRService.recentLogs().slice(0, 10); track log.id) {
            <div class="log-item" [class]="'level-' + log.level">
              <span class="log-level">{{ getLogLevelName(log.level) }}</span>
              <span class="log-message">{{ log.message }}</span>
              <span class="log-time">{{ log.timestamp | date:'HH:mm:ss' }}</span>
            </div>
          }
          @if (signalRService.recentLogs().length === 0) {
            <div class="no-logs">Waiting for logs...</div>
          }
        </div>
      </div>
    </div>
  `,
  styles: [`
    .dashboard { padding: 1.5rem; }
    h1 { margin-bottom: 1.5rem; color: #1a1a2e; }

    .summary-cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
      margin-bottom: 2rem;
    }

    .card {
      background: #fff;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      text-align: center;
    }

    .card-value { font-size: 2.5rem; font-weight: bold; color: #1a1a2e; }
    .card-label { color: #666; margin-top: 0.5rem; }
    .card-warning .card-value { color: #f39c12; }
    .card-error .card-value { color: #e74c3c; }

    .charts-row {
      display: grid;
      grid-template-columns: 1fr 1fr;
      gap: 1rem;
      margin-bottom: 1rem;
    }

    .chart-container {
      background: #fff;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
    }

    .chart-container.full-width { grid-column: 1 / -1; }
    .chart-container h3 { margin: 0 0 1rem 0; color: #333; }

    .realtime-logs {
      background: #fff;
      padding: 1.5rem;
      border-radius: 8px;
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      margin-top: 1rem;
    }

    .realtime-logs h3 {
      margin: 0 0 1rem 0;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }

    .connection-status {
      font-size: 0.75rem;
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      background: #e74c3c;
      color: white;
    }

    .connection-status.connected { background: #27ae60; }

    .log-list { max-height: 300px; overflow-y: auto; }

    .log-item {
      display: flex;
      align-items: center;
      gap: 1rem;
      padding: 0.5rem;
      border-bottom: 1px solid #eee;
    }

    .log-level {
      padding: 0.25rem 0.5rem;
      border-radius: 4px;
      font-size: 0.75rem;
      font-weight: 500;
      min-width: 80px;
      text-align: center;
    }

    .level-0 .log-level, .level-1 .log-level { background: #eee; color: #666; }
    .level-2 .log-level { background: #d4edda; color: #155724; }
    .level-3 .log-level { background: #fff3cd; color: #856404; }
    .level-4 .log-level { background: #f8d7da; color: #721c24; }
    .level-5 .log-level { background: #721c24; color: white; }

    .log-message { flex: 1; overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }
    .log-time { color: #666; font-size: 0.875rem; }
    .no-logs { text-align: center; color: #666; padding: 2rem; }

    @media (max-width: 768px) {
      .charts-row { grid-template-columns: 1fr; }
    }
  `]
})
export class DashboardComponent implements OnInit {
  private logService = inject(LogService);
  signalRService = inject(SignalRService);

  summary = signal<DashboardSummary | null>(null);
  stats = signal<DashboardStats | null>(null);

  barChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { display: false } }
  };

  pieChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { position: 'right' } }
  };

  lineChartOptions: ChartConfiguration['options'] = {
    responsive: true,
    plugins: { legend: { display: false } }
  };

  barChartData = computed(() => {
    const s = this.stats();
    if (!s) return null;
    return {
      labels: ['Trace', 'Debug', 'Info', 'Warning', 'Error', 'Critical'],
      datasets: [{
        data: [s.traceCount, s.debugCount, s.informationCount, s.warningCount, s.errorCount, s.criticalCount],
        backgroundColor: ['#bdc3c7', '#95a5a6', '#27ae60', '#f39c12', '#e74c3c', '#8e44ad']
      }]
    };
  });

  pieChartData = computed(() => {
    const s = this.stats();
    if (!s || Object.keys(s.logsByApplication).length === 0) return null;
    return {
      labels: Object.keys(s.logsByApplication),
      datasets: [{
        data: Object.values(s.logsByApplication),
        backgroundColor: ['#3498db', '#e74c3c', '#2ecc71', '#f39c12', '#9b59b6', '#1abc9c']
      }]
    };
  });

  lineChartData = computed(() => {
    const s = this.stats();
    if (!s || Object.keys(s.logsByHour).length === 0) return null;
    const entries = Object.entries(s.logsByHour).slice(-24);
    return {
      labels: entries.map(([k]) => k.split(' ')[1] || k),
      datasets: [{
        data: entries.map(([, v]) => v),
        borderColor: '#3498db',
        backgroundColor: 'rgba(52, 152, 219, 0.1)',
        fill: true,
        tension: 0.4
      }]
    };
  });

  ngOnInit(): void {
    this.loadData();
    this.signalRService.connect();
  }

  loadData(): void {
    this.logService.getSummary().subscribe(data => this.summary.set(data));

    const from = new Date();
    from.setDate(from.getDate() - 7);
    this.logService.getStats(undefined, from.toISOString()).subscribe(data => this.stats.set(data));
  }

  getLogLevelName(level: LogLevel): string {
    return ['Trace', 'Debug', 'Info', 'Warning', 'Error', 'Critical'][level] || 'Unknown';
  }
}
