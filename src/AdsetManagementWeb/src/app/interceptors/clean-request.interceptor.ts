import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class CleanRequestInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (req.body instanceof FormData) {
      return next.handle(req);
    }
    
    if ((req.method === 'POST' || req.method === 'PUT') && req.body && typeof req.body === 'object' && !Array.isArray(req.body)) {
      const cleanedBody = this.removeEmptyFields(req.body);
      const clonedRequest = req.clone({ body: cleanedBody });
      return next.handle(clonedRequest);
    }
    
    return next.handle(req);
  }

  private removeEmptyFields(obj: any): any {
    const cleaned: any = {};
    
    Object.keys(obj).forEach(key => {
      const value = obj[key];

      if (value !== null && value !== undefined && value !== '' &&
          !(Array.isArray(value) && value.length === 0)) {

        if (typeof value === 'object' && !Array.isArray(value)) {
          const cleanedNestedObj = this.removeEmptyFields(value);
          
          if (Object.keys(cleanedNestedObj).length > 0) {
            cleaned[key] = cleanedNestedObj;
          }
        } else {
          cleaned[key] = value;
        }
      }
    });
    
    return cleaned;
  }
}