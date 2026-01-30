/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        // Theme-aware colors using CSS variables
        theme: {
          primary: 'var(--color-primary)',
          secondary: 'var(--color-secondary)',
          accent: 'var(--color-accent)',
          bg: 'var(--color-background)',
          surface: 'var(--color-surface)',
          card: 'var(--color-card)',
          border: 'var(--color-border)',
          text: 'var(--color-text)',
          'text-dim': 'var(--color-text-dim)',
          'text-comment': 'var(--color-text-comment)',
          success: 'var(--color-success)',
          warning: 'var(--color-warning)',
          error: 'var(--color-error)',
          info: 'var(--color-info)',
        },
        // Legacy colors (kept for backwards compatibility during transition)
        cyber: {
          bg: 'var(--color-background)',
          surface: 'var(--color-surface)',
          card: 'var(--color-card)',
          border: 'var(--color-border)',
          'border-glow': 'var(--color-primary)',
        },
        neon: {
          green: 'var(--color-primary)',
          cyan: 'var(--color-secondary)',
          pink: 'var(--color-accent)',
          orange: '#ff6b35',
          yellow: 'var(--color-warning)',
          red: 'var(--color-error)',
          purple: 'var(--color-accent)',
        },
        terminal: {
          text: 'var(--color-text)',
          dim: 'var(--color-text-dim)',
          comment: 'var(--color-text-comment)',
        }
      },
      fontFamily: {
        'mono': ['"JetBrains Mono"', '"Fira Code"', 'Consolas', 'monospace'],
        'display': ['"Orbitron"', '"Rajdhani"', 'sans-serif'],
      },
      animation: {
        'pulse-glow': 'pulse-glow 2s ease-in-out infinite',
        'scan': 'scan 8s linear infinite',
        'flicker': 'flicker 0.15s infinite',
        'typing': 'typing 0.8s steps(20) forwards',
        'blink': 'blink 1s step-end infinite',
        'slide-up': 'slide-up 0.4s ease-out forwards',
        'slide-in': 'slide-in 0.3s ease-out forwards',
        'fade-in': 'fade-in 0.3s ease-out forwards',
        'glow-pulse': 'glow-pulse 3s ease-in-out infinite',
      },
      keyframes: {
        'pulse-glow': {
          '0%, 100%': { boxShadow: '0 0 5px var(--color-primary), 0 0 10px var(--color-primary)' },
          '50%': { boxShadow: '0 0 10px var(--color-primary), 0 0 20px var(--color-primary), 0 0 30px var(--color-primary)' },
        },
        'scan': {
          '0%': { transform: 'translateY(-100%)' },
          '100%': { transform: 'translateY(100%)' },
        },
        'flicker': {
          '0%, 100%': { opacity: '1' },
          '50%': { opacity: '0.8' },
        },
        'typing': {
          'from': { width: '0' },
          'to': { width: '100%' },
        },
        'blink': {
          '0%, 100%': { opacity: '1' },
          '50%': { opacity: '0' },
        },
        'slide-up': {
          'from': { opacity: '0', transform: 'translateY(20px)' },
          'to': { opacity: '1', transform: 'translateY(0)' },
        },
        'slide-in': {
          'from': { opacity: '0', transform: 'translateX(-10px)' },
          'to': { opacity: '1', transform: 'translateX(0)' },
        },
        'fade-in': {
          'from': { opacity: '0' },
          'to': { opacity: '1' },
        },
        'glow-pulse': {
          '0%, 100%': { filter: 'brightness(1)' },
          '50%': { filter: 'brightness(1.2)' },
        },
      },
      backgroundImage: {
        'grid-pattern': 'linear-gradient(var(--color-primary-10) 1px, transparent 1px), linear-gradient(90deg, var(--color-primary-10) 1px, transparent 1px)',
        'noise': "url(\"data:image/svg+xml,%3Csvg viewBox='0 0 200 200' xmlns='http://www.w3.org/2000/svg'%3E%3Cfilter id='noise'%3E%3CfeTurbulence type='fractalNoise' baseFrequency='0.9' numOctaves='4' stitchTiles='stitch'/%3E%3C/filter%3E%3Crect width='100%25' height='100%25' filter='url(%23noise)'/%3E%3C/svg%3E\")",
      },
      boxShadow: {
        'neon': '0 0 5px var(--color-primary), 0 0 20px var(--color-primary)',
        'neon-lg': '0 0 10px var(--color-primary), 0 0 40px var(--color-primary)',
        'inner-glow': 'inset 0 0 20px color-mix(in srgb, var(--color-primary) 10%, transparent)',
      },
    },
  },
  plugins: [],
}
