import react from '@vitejs/plugin-react'
import { defineConfig, loadEnv } from 'vite'

export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [react()],
    server:{
      port: parseInt(env.VITE_PORT),
      proxy: {
        // "todoapiservice" is the name of the API in AppHost.cs.
        '/api': {
          target: 
            process.env.TODOAPISERVICE_HTTPS ||
            process.env.TODOAPISERVICE_HTTP,
          changeOrigin: true,
          secure: false
        }
      }
    },
    build:{
      outDir: "dist",
      rollupOptions: {
        input: "./index.html"
      }
    }
  }
})