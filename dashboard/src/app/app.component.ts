import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { ThemeService } from './core/services/theme.service';
import { AlertService } from './core/services/alert.service';
import { ThemeSelectorComponent } from './shared/components/theme-selector/theme-selector.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, ThemeSelectorComponent],
  templateUrl: './app.component.html'
})
export class AppComponent {
  authService = inject(AuthService);
  themeService = inject(ThemeService);
  private alertService = inject(AlertService);

  async logout(): Promise<void> {
    const confirmed = await this.alertService.confirm(
      'Disconnect Session',
      'Are you sure you want to log out?',
      'Logout',
      'Cancel'
    );

    if (confirmed) {
      this.authService.logout();
      this.alertService.toast('Session terminated', 'info');
    }
  }
}
