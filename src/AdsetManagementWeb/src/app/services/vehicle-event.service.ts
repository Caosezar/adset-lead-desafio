import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class VehicleEventService {
  private vehicleUpdatedSource = new Subject<void>();
  
  vehicleUpdated$ = this.vehicleUpdatedSource.asObservable();

  notifyVehicleUpdated(): void {
    this.vehicleUpdatedSource.next();
  }
}
