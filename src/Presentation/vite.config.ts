import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import fs from 'node:fs'
import path from 'node:path'

export default defineConfig({
  plugins: [react()],
  server: {
    port: 3001,
    https: {
      key: fs.readFileSync(path.resolve('./localhost+2-key.pem')),
      cert: fs.readFileSync(path.resolve('./localhost+2.pem')),
    },
    proxy: {
      '/api': {
        target: 'http://localhost:5080',
        changeOrigin: true,
        rewrite: (path) => path.replace(/^\/api/, '/api')
      }
    }
  }
})
