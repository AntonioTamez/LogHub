import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ThemeService, Theme } from '../../../core/services/theme.service';

@Component({
  selector: 'app-theme-selector',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './theme-selector.component.html'
})
export class ThemeSelectorComponent {
  themeService = inject(ThemeService);
  isOpen = signal(false);

  toggleDropdown(): void {
    this.isOpen.update(v => !v);
  }

  selectTheme(theme: Theme): void {
    this.themeService.setTheme(theme.id);
    this.isOpen.set(false);
  }

  closeDropdown(): void {
    this.isOpen.set(false);
  }
}
