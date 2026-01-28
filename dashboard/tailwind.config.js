/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#1a1a2e',
          dark: '#16213e',
        }
      }
    },
  },
  plugins: [],
}
