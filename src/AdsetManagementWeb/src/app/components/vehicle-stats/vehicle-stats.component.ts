import { Component, Input, Output, EventEmitter } from '@angular/core';

export interface VehicleStats {
  total: number;
  withPhotos: number;
  withoutPhotos: number;
}

@Component({
  selector: 'app-vehicle-stats',
  templateUrl: './vehicle-stats.component.html',
  styleUrls: ['./vehicle-stats.component.css']
})
export class VehicleStatsComponent {
  @Input() vehicleStats: VehicleStats = { total: 0, withPhotos: 0, withoutPhotos: 0 };
  @Input() isLoading: boolean = false;
  
  @Output() exportStock = new EventEmitter<void>();
  @Output() registerVehicle = new EventEmitter<void>();

  onExportStock(): void {
    this.exportStock.emit();
  }

  onRegisterVehicle(): void {
    this.registerVehicle.emit();
  }
  onSaveStats(): void {
    console.log('Salvar estatísticas clicado');
  }
}
