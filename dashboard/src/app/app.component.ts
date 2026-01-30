import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from './core/services/auth.service';
import { ThemeService } from './core/services/theme.service';
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
}
