import { Component, inject, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    @if (authService.isAuthenticated()) {
      <div class="layout">
        <nav class="sidebar">
          <div class="logo">
            <h1>LogHub</h1>
          </div>
          <ul class="nav-links">
            <li>
              <a routerLink="/dashboard" routerLinkActive="active">
                <span class="icon">📊</span> Dashboard
              </a>
            </li>
            <li>
              <a routerLink="/logs" routerLinkActive="active">
                <span class="icon">📝</span> Logs
              </a>
            </li>
            <li>
              <a routerLink="/applications" routerLinkActive="active">
                <span class="icon">📱</span> Applications
              </a>
            </li>
          </ul>
          <div class="user-section">
            <div class="user-info">
              <span class="user-name">{{ authService.currentUser()?.name }}</span>
              <span class="user-email">{{ authService.currentUser()?.email }}</span>
            </div>
            <button class="logout-btn" (click)="authService.logout()">Logout</button>
          </div>
        </nav>
        <main class="content">
          <router-outlet></router-outlet>
        </main>
      </div>
    } @else {
      <router-outlet></router-outlet>
    }
  `,
  styles: [`
    .layout {
      display: flex;
      min-height: 100vh;
    }

    .sidebar {
      width: 250px;
      background: #1a1a2e;
      color: white;
      display: flex;
      flex-direction: column;
      position: fixed;
      height: 100vh;
    }

    .logo {
      padding: 1.5rem;
      border-bottom: 1px solid rgba(255,255,255,0.1);
    }

    .logo h1 {
      margin: 0;
      font-size: 1.5rem;
    }

    .nav-links {
      list-style: none;
      padding: 1rem 0;
      margin: 0;
      flex: 1;
    }

    .nav-links li a {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.75rem 1.5rem;
      color: rgba(255,255,255,0.7);
      text-decoration: none;
      transition: all 0.2s;
    }

    .nav-links li a:hover,
    .nav-links li a.active {
      background: rgba(255,255,255,0.1);
      color: white;
    }

    .icon {
      font-size: 1.25rem;
    }

    .user-section {
      padding: 1rem 1.5rem;
      border-top: 1px solid rgba(255,255,255,0.1);
    }

    .user-info {
      margin-bottom: 0.75rem;
    }

    .user-name {
      display: block;
      font-weight: 500;
    }

    .user-email {
      display: block;
      font-size: 0.75rem;
      color: rgba(255,255,255,0.5);
    }

    .logout-btn {
      width: 100%;
      padding: 0.5rem;
      background: rgba(255,255,255,0.1);
      border: none;
      color: white;
      border-radius: 4px;
      cursor: pointer;
    }

    .logout-btn:hover {
      background: rgba(255,255,255,0.2);
    }

    .content {
      flex: 1;
      margin-left: 250px;
      background: #f5f6fa;
      min-height: 100vh;
    }
  `]
})
export class AppComponent {
  authService = inject(AuthService);
}
