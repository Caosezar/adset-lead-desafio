import { Injectable } from '@angular/core';
import { OpenAPI } from '../api/core/OpenAPI';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {
  private readonly BASE_URL: string;

  constructor() {
    this.BASE_URL = this.determineBaseUrl();
    this.initializeApi();
  }

  private determineBaseUrl(): string {
    if (typeof window !== 'undefined') {
      const hostname = window.location.hostname;
      const protocol = window.location.protocol;

      if (hostname === 'localhost' || hostname === '127.0.0.1') {
        return 'http://localhost:5000';
      }

      return `${protocol}//${hostname}/api`;
    }
    
    return 'http://localhost:5000';
  }

  private initializeApi(): void {
    OpenAPI.BASE = this.BASE_URL;

    OpenAPI.HEADERS = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };

  }
  getBaseUrl(): string {
    return this.BASE_URL;
  }


  buildUrl(path: string): string {
    const cleanPath = path.startsWith('/') ? path : `/${path}`;
    return `${this.BASE_URL}${cleanPath}`;
  }

  buildImageUrl(imagePath: string): string {
    if (!imagePath) return '';
    
    if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
      return imagePath;
    }
    
    if (imagePath.startsWith('/')) {
      return `${this.BASE_URL}${imagePath}`;
    }
    
    return `${this.BASE_URL}/uploads/${imagePath}`;
  }

  updateBaseUrl(baseUrl: string): void {
    OpenAPI.BASE = baseUrl;
  }
}