const PROXY_CONFIG = {
    "/api/*": {
      target: "http://localhost:33680/",
      changeOrigin: true,
      logLevel: "debug",
      secure: false
    }
  };
  
  module.exports = PROXY_CONFIG;