import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration } from 'chart.js';
import { LogService } from '../../core/services/log.service';
import { SignalRService } from '../../core/services/signalr.service';
import { AlertService } from '../../core/services/alert.service';
import { DashboardStats, DashboardSummary } from '../../core/models/dashboard.model';
import { LogLevel } from '../../core/models/log-entry.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, BaseChartDirective],
  templateUrl: './dashboard.component.html'
})
export class DashboardComponent implements OnInit {
  private logService = inject(LogService);
  private alertService = inject(AlertService);
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
    this.logService.getSummary().subscribe({
      next: (data) => this.summary.set(data),
      error: () => this.alertService.toast('Failed to load summary', 'error')
    });

    const from = new Date();
    from.setDate(from.getDate() - 7);
    this.logService.getStats(undefined, from.toISOString()).subscribe({
      next: (data) => this.stats.set(data),
      error: () => this.alertService.toast('Failed to load statistics', 'error')
    });
  }

  getLogLevelName(level: LogLevel): string {
    return ['Trace', 'Debug', 'Info', 'Warning', 'Error', 'Critical'][level] || 'Unknown';
  }
}
