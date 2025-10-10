export interface ApiConfig {
  baseUrl: string;
  timeout: number;
  retryAttempts: number;
}

export const getApiConfig = (): ApiConfig => {
  // Check if running in browser
  if (typeof window !== 'undefined') {
    const hostname = window.location.hostname;
    const protocol = window.location.protocol;
    
    // Development configuration
    if (hostname === 'localhost' || hostname === '127.0.0.1') {
      return {
        baseUrl: 'http://localhost:5000',
        timeout: 30000,
        retryAttempts: 3
      };
    }
    
    // Production configuration
    return {
      baseUrl: `${protocol}//${hostname}/api`,
      timeout: 30000,
      retryAttempts: 3
    };
  }
  
  // Server-side rendering or other environments
  return {
    baseUrl: 'http://localhost:5000',
    timeout: 30000,
    retryAttempts: 3
  };
};

export const API_CONFIG = getApiConfig();
