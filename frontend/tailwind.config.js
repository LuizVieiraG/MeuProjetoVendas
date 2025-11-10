/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'gamer-purple': {
          50: '#f3e8ff',
          100: '#e9d5ff',
          200: '#d8b4fe',
          300: '#c084fc',
          400: '#a855f7',
          500: '#9333ea',
          600: '#7e22ce',
          700: '#6b21a8',
          800: '#581c87',
          900: '#4c1d95',
        },
        'gamer-dark': {
          50: '#1a1a1a',
          100: '#0f0f0f',
          200: '#000000',
        }
      },
      boxShadow: {
        'gamer-purple': '0 0 20px rgba(147, 51, 234, 0.5)',
        'gamer-purple-lg': '0 0 30px rgba(147, 51, 234, 0.7)',
        'gamer-glow': '0 0 15px rgba(147, 51, 234, 0.4), 0 0 30px rgba(147, 51, 234, 0.2)',
      },
      fontFamily: {
        'gamer': ['Orbitron', 'sans-serif'],
      },
    },
  },
  plugins: [],
}


