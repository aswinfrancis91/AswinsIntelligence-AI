import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    proxy: {
      '/AskAswin': {
        target: 'http://localhost:5257',
        changeOrigin: true,
        secure: false,
      },
      '/ResetConversation': {
        target: 'http://localhost:5257',
        changeOrigin: true,
        secure: false,
      }
    }
  }
})