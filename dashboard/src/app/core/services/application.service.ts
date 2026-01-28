import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Application, CreateApplicationRequest, UpdateApplicationRequest } from '../models/application.model';

@Injectable({
  providedIn: 'root'
})
export class ApplicationService {
  private http = inject(HttpClient);

  getApplications(): Observable<Application[]> {
    return this.http.get<Application[]>(`${environment.apiUrl}/applications`);
  }

  getApplication(id: string): Observable<Application> {
    return this.http.get<Application>(`${environment.apiUrl}/applications/${id}`);
  }

  createApplication(request: CreateApplicationRequest): Observable<Application> {
    return this.http.post<Application>(`${environment.apiUrl}/applications`, request);
  }

  updateApplication(id: string, request: UpdateApplicationRequest): Observable<Application> {
    return this.http.put<Application>(`${environment.apiUrl}/applications/${id}`, request);
  }

  deleteApplication(id: string): Observable<void> {
    return this.http.delete<void>(`${environment.apiUrl}/applications/${id}`);
  }

  regenerateApiKey(id: string): Observable<{ newApiKey: string }> {
    return this.http.post<{ newApiKey: string }>(`${environment.apiUrl}/applications/${id}/regenerate-key`, {});
  }
}
