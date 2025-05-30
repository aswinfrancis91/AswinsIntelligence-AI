import { defineConfig, loadEnv } from 'vite'
import react from '@vitejs/plugin-react'

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), '');
  return {
    plugins: [react()],
    server: {
      port: parseInt(env.VITE_PORT),
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
        },
        '/GenerateGraph': {
          target: 'http://localhost:5257',
          changeOrigin: true,
          secure: false,
        }
      }
    }
  }
})