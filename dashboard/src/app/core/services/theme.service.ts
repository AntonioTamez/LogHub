import { Injectable, signal, effect } from '@angular/core';

export interface Theme {
  id: string;
  name: string;
  description: string;
  colors: {
    primary: string;
    secondary: string;
    accent: string;
    background: string;
    surface: string;
    card: string;
    border: string;
    text: string;
    textDim: string;
    textComment: string;
    success: string;
    warning: string;
    error: string;
    info: string;
  };
}

export const THEMES: Theme[] = [
  {
    id: 'cyberpunk',
    name: 'Cyberpunk',
    description: 'Neon green terminal aesthetic',
    colors: {
      primary: '#00ff95',
      secondary: '#00d4ff',
      accent: '#b84dff',
      background: '#0a0a0f',
      surface: '#12121a',
      card: '#1a1a24',
      border: '#2a2a3a',
      text: '#e0e0e0',
      textDim: '#6a6a7a',
      textComment: '#5a5a6a',
      success: '#00ff95',
      warning: '#ffd93d',
      error: '#ff3366',
      info: '#00d4ff',
    }
  },
  {
    id: 'matrix',
    name: 'Matrix',
    description: 'Classic green on black',
    colors: {
      primary: '#00ff41',
      secondary: '#008f11',
      accent: '#00ff41',
      background: '#0d0d0d',
      surface: '#0a0a0a',
      card: '#111111',
      border: '#1a3a1a',
      text: '#00ff41',
      textDim: '#00aa2a',
      textComment: '#006622',
      success: '#00ff41',
      warning: '#aaff00',
      error: '#ff0040',
      info: '#00ff41',
    }
  },
  {
    id: 'synthwave',
    name: 'Synthwave',
    description: 'Retro 80s pink & purple vibes',
    colors: {
      primary: '#ff2975',
      secondary: '#f222ff',
      accent: '#00d9ff',
      background: '#1a1025',
      surface: '#241734',
      card: '#2d1f42',
      border: '#4a2c6a',
      text: '#eee5ff',
      textDim: '#9d8abf',
      textComment: '#6b5a8a',
      success: '#00ff9f',
      warning: '#ffe66d',
      error: '#ff2975',
      info: '#00d9ff',
    }
  },
  {
    id: 'arctic',
    name: 'Arctic',
    description: 'Cool blue frost theme',
    colors: {
      primary: '#5ccfe6',
      secondary: '#73d0ff',
      accent: '#d4bfff',
      background: '#0f1419',
      surface: '#151b22',
      card: '#1c242d',
      border: '#2d3640',
      text: '#e6e6e6',
      textDim: '#8a9199',
      textComment: '#5c6773',
      success: '#87d96c',
      warning: '#ffcc66',
      error: '#f27983',
      info: '#5ccfe6',
    }
  },
  {
    id: 'amber',
    name: 'Amber Terminal',
    description: 'Retro amber monochrome',
    colors: {
      primary: '#ffb000',
      secondary: '#ff8c00',
      accent: '#ffd700',
      background: '#1a1400',
      surface: '#221a00',
      card: '#2a2000',
      border: '#3d3000',
      text: '#ffcc00',
      textDim: '#b38f00',
      textComment: '#806600',
      success: '#00ff00',
      warning: '#ffb000',
      error: '#ff4400',
      info: '#ffcc00',
    }
  },
  {
    id: 'dracula',
    name: 'Dracula',
    description: 'Popular dark theme with purple accents',
    colors: {
      primary: '#bd93f9',
      secondary: '#ff79c6',
      accent: '#8be9fd',
      background: '#282a36',
      surface: '#21222c',
      card: '#2d2f3d',
      border: '#44475a',
      text: '#f8f8f2',
      textDim: '#a9adc1',
      textComment: '#6272a4',
      success: '#50fa7b',
      warning: '#f1fa8c',
      error: '#ff5555',
      info: '#8be9fd',
    }
  },
  {
    id: 'monokai',
    name: 'Monokai',
    description: 'Classic code editor theme',
    colors: {
      primary: '#a6e22e',
      secondary: '#66d9ef',
      accent: '#f92672',
      background: '#1e1e1e',
      surface: '#252526',
      card: '#2d2d2d',
      border: '#3e3e3e',
      text: '#f8f8f2',
      textDim: '#a0a0a0',
      textComment: '#75715e',
      success: '#a6e22e',
      warning: '#e6db74',
      error: '#f92672',
      info: '#66d9ef',
    }
  },
  {
    id: 'nord',
    name: 'Nord',
    description: 'Arctic, north-bluish color palette',
    colors: {
      primary: '#88c0d0',
      secondary: '#81a1c1',
      accent: '#b48ead',
      background: '#2e3440',
      surface: '#3b4252',
      card: '#434c5e',
      border: '#4c566a',
      text: '#eceff4',
      textDim: '#d8dee9',
      textComment: '#616e88',
      success: '#a3be8c',
      warning: '#ebcb8b',
      error: '#bf616a',
      info: '#88c0d0',
    }
  }
];

const THEME_STORAGE_KEY = 'loghub_theme';
const DEFAULT_THEME_ID = 'cyberpunk';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentThemeId = signal<string>(DEFAULT_THEME_ID);

  readonly currentTheme = signal<Theme>(THEMES[0]);
  readonly availableThemes = THEMES;

  constructor() {
    this.loadSavedTheme();

    effect(() => {
      const theme = this.currentTheme();
      this.applyTheme(theme);
    });
  }

  private loadSavedTheme(): void {
    const savedThemeId = localStorage.getItem(THEME_STORAGE_KEY);
    if (savedThemeId) {
      const theme = THEMES.find(t => t.id === savedThemeId);
      if (theme) {
        this.currentThemeId.set(theme.id);
        this.currentTheme.set(theme);
      }
    }
  }

  setTheme(themeId: string): void {
    const theme = THEMES.find(t => t.id === themeId);
    if (theme) {
      this.currentThemeId.set(theme.id);
      this.currentTheme.set(theme);
      localStorage.setItem(THEME_STORAGE_KEY, themeId);
    }
  }

  private applyTheme(theme: Theme): void {
    const root = document.documentElement;

    root.style.setProperty('--color-primary', theme.colors.primary);
    root.style.setProperty('--color-secondary', theme.colors.secondary);
    root.style.setProperty('--color-accent', theme.colors.accent);
    root.style.setProperty('--color-background', theme.colors.background);
    root.style.setProperty('--color-surface', theme.colors.surface);
    root.style.setProperty('--color-card', theme.colors.card);
    root.style.setProperty('--color-border', theme.colors.border);
    root.style.setProperty('--color-text', theme.colors.text);
    root.style.setProperty('--color-text-dim', theme.colors.textDim);
    root.style.setProperty('--color-text-comment', theme.colors.textComment);
    root.style.setProperty('--color-success', theme.colors.success);
    root.style.setProperty('--color-warning', theme.colors.warning);
    root.style.setProperty('--color-error', theme.colors.error);
    root.style.setProperty('--color-info', theme.colors.info);

    // Update body background
    document.body.style.backgroundColor = theme.colors.background;
    document.body.style.color = theme.colors.text;
  }
}
