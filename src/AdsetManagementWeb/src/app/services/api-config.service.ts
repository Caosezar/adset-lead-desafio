import { Injectable } from '@angular/core';
import { OpenAPI } from '../api/core/OpenAPI';

@Injectable({
  providedIn: 'root'
})
export class ApiConfigService {

  constructor() {
    this.initializeApi();
  }

  private initializeApi(): void {

    OpenAPI.BASE = 'http://localhost:5000';


    OpenAPI.HEADERS = {
      'Content-Type': 'application/json',
      'Accept': 'application/json'
    };
  }

  updateBaseUrl(baseUrl: string): void {
    OpenAPI.BASE = baseUrl;
    console.log('Base URL atualizada para:', baseUrl);
  }
}