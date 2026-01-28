import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { LogEntry, LogQueryParams, PagedResponse } from '../models/log-entry.model';
import { DashboardStats, DashboardSummary } from '../models/dashboard.model';

@Injectable({
  providedIn: 'root'
})
export class LogService {
  private http = inject(HttpClient);

  getLogs(params: LogQueryParams): Observable<PagedResponse<LogEntry>> {
    let httpParams = new HttpParams();

    if (params.applicationId) httpParams = httpParams.set('applicationId', params.applicationId);
    if (params.minLevel !== undefined) httpParams = httpParams.set('minLevel', params.minLevel.toString());
    if (params.maxLevel !== undefined) httpParams = httpParams.set('maxLevel', params.maxLevel.toString());
    if (params.searchText) httpParams = httpParams.set('searchText', params.searchText);
    if (params.correlationId) httpParams = httpParams.set('correlationId', params.correlationId);
    if (params.from) httpParams = httpParams.set('from', params.from);
    if (params.to) httpParams = httpParams.set('to', params.to);
    if (params.page) httpParams = httpParams.set('page', params.page.toString());
    if (params.pageSize) httpParams = httpParams.set('pageSize', params.pageSize.toString());
    if (params.sortBy) httpParams = httpParams.set('sortBy', params.sortBy);
    if (params.sortDescending !== undefined) httpParams = httpParams.set('sortDescending', params.sortDescending.toString());

    return this.http.get<PagedResponse<LogEntry>>(`${environment.apiUrl}/logs`, { params: httpParams });
  }

  getLogById(id: string): Observable<LogEntry> {
    return this.http.get<LogEntry>(`${environment.apiUrl}/logs/${id}`);
  }

  getStats(applicationId?: string, from?: string, to?: string): Observable<DashboardStats> {
    let httpParams = new HttpParams();
    if (applicationId) httpParams = httpParams.set('applicationId', applicationId);
    if (from) httpParams = httpParams.set('from', from);
    if (to) httpParams = httpParams.set('to', to);

    return this.http.get<DashboardStats>(`${environment.apiUrl}/dashboard/stats`, { params: httpParams });
  }

  getSummary(): Observable<DashboardSummary> {
    return this.http.get<DashboardSummary>(`${environment.apiUrl}/dashboard/summary`);
  }
}
