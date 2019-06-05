const PROXY_CONFIG = {
    "/api/*": {
      target: "http://localhost:5821/",
      changeOrigin: true,
      logLevel: "debug",
      secure: false
    }
  };
  
  module.exports = PROXY_CONFIG;