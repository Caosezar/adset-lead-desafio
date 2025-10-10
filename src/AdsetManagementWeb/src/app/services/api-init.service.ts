import { Injectable } from '@angular/core';
import { OpenAPI } from '../api/core/OpenAPI';
import { API_CONFIG } from '../config/api.config';

@Injectable({
  providedIn: 'root'
})
export class ApiInitService {
  
  constructor() {
    this.initializeApi();
  }

  private initializeApi(): void {
    OpenAPI.BASE = API_CONFIG.baseUrl;
    
    OpenAPI.WITH_CREDENTIALS = true;
    OpenAPI.CREDENTIALS = 'include';
    
    OpenAPI.HEADERS = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };

    console.log(`API initialized with base URL: ${OpenAPI.BASE}`);
  }

  public updateBaseUrl(newBaseUrl: string): void {
    OpenAPI.BASE = newBaseUrl;
    console.log(`API base URL updated to: ${OpenAPI.BASE}`);
  }

  public getBaseUrl(): string {
    return OpenAPI.BASE;
  }
}
