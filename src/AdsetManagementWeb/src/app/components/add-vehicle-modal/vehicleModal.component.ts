import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { OtherOptionsRequest } from '../../api/models/OtherOptionsRequest';
import { UpdateVehicleRequest } from '../../api/models/UpdateVehicleRequest';
import { VehicleResponse } from '../../api/models/VehicleResponse';
import { VehicleImageService } from '../../api/services/VehicleImageService';
import { VehicleService } from '../../api/services/VehicleService';

@Component({
  selector: 'app-vehicle-modal',
  templateUrl: './vehicleModal.component.html',
  styleUrls: ['./vehicleModal.component.css']
})
export class VehicleModalComponent implements OnInit {
  @Input() vehicle?: VehicleResponse;
  @Input() isEditMode: boolean = false;
  @Output() save = new EventEmitter<VehicleResponse>();
  @Output() close = new EventEmitter<void>();
  @Output() refreshList = new EventEmitter<void>();

  vehicleForm!: FormGroup;
  isLoading = false;
  selectedImages: File[] = [];
  imagePreviewUrls: string[] = [];
  existingImages: string[] = [];


  brandOptions = [
    'Audi', 'BMW', 'Chevrolet', 'Fiat', 'Ford', 'Honda', 'Hyundai',
    'Jeep', 'Kia', 'Mercedes-Benz', 'Nissan', 'Peugeot', 'Renault',
    'Toyota', 'Volkswagen', 'Volvo'
  ];

  colorOptions = [
    'Azul', 'Branco', 'Cinza', 'Prata', 'Preto', 'Vermelho',
    'Verde', 'Amarelo', 'Bege', 'Marrom', 'Rosa', 'Roxo'
  ];

  fuelOptions = [
    'Flex', 'Gasolina', 'Etanol', 'Diesel', 'Elétrico', 'Híbrido'
  ];

  transmissionOptions = [
    'Manual', 'Automático', 'CVT', 'Semi-automático'
  ];


  availableFeatures = [
    { key: 'arCondicionado', label: 'Ar Condicionado' },
    { key: 'direcaoHidraulica', label: 'Direção Hidráulica' },
    { key: 'vidroEletrico', label: 'Vidro Elétrico' },
    { key: 'travaEletrica', label: 'Trava Elétrica' },
    { key: 'alarme', label: 'Alarme' },
    { key: 'som', label: 'Som' },
    { key: 'rodaLigaLeve', label: 'Roda Liga Leve' },
    { key: 'airbag', label: 'Airbag' },
    { key: 'abs', label: 'ABS' },
    { key: 'gps', label: 'GPS' },
    { key: 'camera', label: 'Câmera de Ré' },
    { key: 'bluetooth', label: 'Bluetooth' },
    { key: 'usb', label: 'USB' },
    { key: 'bancoCouro', label: 'Banco de Couro' },
    { key: 'tetoSolar', label: 'Teto Solar' }
  ];

  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService,
    private vehicleImageService: VehicleImageService,
    private http: HttpClient
  ) { }

  ngOnInit(): void {
    this.initializeForm();

    if (this.isEditMode && this.vehicle) {
      this.populateForm();
      this.loadExistingImages();
    }
  }

  private initializeForm(): void {
    this.vehicleForm = this.fb.group({
      placa: ['', [Validators.required, Validators.pattern(/^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$/)]],
      marca: ['', Validators.required],
      modelo: ['', Validators.required],
      ano: ['', [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      cor: ['', Validators.required],
      km: ['', [Validators.required, Validators.min(0)]],
      preco: ['', [Validators.required, Validators.min(0)]],

      arCondicionado: [false],
      direcaoHidraulica: [false],
      vidroEletrico: [false],
      travaEletrica: [false],
      alarme: [false],
      som: [false],
      rodaLigaLeve: [false],
      airbag: [false],
      abs: [false],
      gps: [false],
      camera: [false],
      bluetooth: [false],
      usb: [false],
      bancoCouro: [false],
      tetoSolar: [false]
    });
  }

  private populateForm(): void {
    if (!this.vehicle) return;

    this.vehicleForm.patchValue({
      placa: this.vehicle.placa,
      marca: this.vehicle.marca,
      modelo: this.vehicle.modelo,
      ano: this.vehicle.ano,
      cor: this.vehicle.cor,
      km: this.vehicle.km,
      preco: this.vehicle.preco
    });


    if (this.vehicle.otherOptions) {
      this.availableFeatures.forEach(feature => {
        const value = (this.vehicle!.otherOptions as any)?.[feature.key] || false;
        this.vehicleForm.patchValue({ [feature.key]: value });
      });
    }
  }

  private loadExistingImages(): void {
    if (!this.vehicle?.imagens) return;

    this.existingImages = this.vehicle.imagens;
  }

  onImageSelect(event: any): void {
    const files = event.target.files;
    if (!files) return;

    this.selectedImages = Array.from(files);
    this.imagePreviewUrls = [];


    for (let file of this.selectedImages) {
      const reader = new FileReader();
      reader.onload = (e: any) => {
        this.imagePreviewUrls.push(e.target.result);
      };
      reader.readAsDataURL(file);
    }
  }

  removeImage(index: number, isExisting: boolean = false): void {
    if (isExisting) {
      this.existingImages.splice(index, 1);
    } else {
      this.selectedImages.splice(index, 1);
      this.imagePreviewUrls.splice(index, 1);
    }
  }

  async onSave(): Promise<void> {
    if (this.vehicleForm.invalid) {
      this.markFormGroupTouched();
      return;
    }

    this.isLoading = true;

    try {
      const formValue = this.vehicleForm.value;
      let savedVehicle: VehicleResponse;

      if (this.isEditMode && this.vehicle) {

        const otherOptions: OtherOptionsRequest = {};
        this.availableFeatures.forEach(feature => {
          (otherOptions as any)[feature.key] = formValue[feature.key] || false;
        });

        const updateRequest: UpdateVehicleRequest = {
          placa: formValue.placa,
          marca: formValue.marca,
          modelo: formValue.modelo,
          ano: formValue.ano,
          cor: formValue.cor,
          km: formValue.km,
          preco: formValue.preco,
          otherOptions: otherOptions
        };

        console.log('Updating vehicle:', updateRequest);
        savedVehicle = await this.vehicleService.putApiVehicle(this.vehicle.id!, updateRequest).toPromise();


        if (this.selectedImages.length > 0 && savedVehicle.id) {
          console.log('Uploading images for vehicle:', savedVehicle.id);
          await this.uploadImages(savedVehicle.id);
        }
      } else {
        
        const formData = new FormData();
        
        
        formData.append('Marca', formValue.marca || '');
        formData.append('Modelo', formValue.modelo || '');
        formData.append('Ano', (formValue.ano || '').toString());
        formData.append('Placa', formValue.placa || '');
        formData.append('Cor', formValue.cor || '');
        formData.append('Preco', (parseFloat(formValue.preco) || 0).toString());
        
        
        formData.append('Km', (formValue.km || '').toString());
        
        
        formData.append('ArCondicionado', formValue.arCondicionado ? 'true' : '');
        formData.append('Alarme', formValue.alarme ? 'true' : '');
        formData.append('Airbag', formValue.airbag ? 'true' : '');
        formData.append('ABS', formValue.abs ? 'true' : '');
        
        
        formData.append('PacoteICarros', formValue.pacoteICarros || '');
        formData.append('PacoteWebMotors', formValue.pacoteWebMotors || '');

        
        if (this.selectedImages.length > 0) {
          this.selectedImages.forEach((image) => {
            formData.append('Images', image, image.name);
          });
        }
        
        
        savedVehicle = await this.http.post<VehicleResponse>('http://localhost:5000/api/Vehicle/with-images', formData).toPromise();
      }

      console.log('Vehicle saved successfully:', savedVehicle);
      this.save.emit(savedVehicle);
      
      // Emitir evento para refresh da lista após criação/edição
      this.refreshList.emit();

    } catch (error: any) {
      console.error('Error saving vehicle:', error);
      console.error('Error response:', error.error);
      console.error('Error status:', error.status);
      alert('Erro ao salvar veículo: ' + (error.error?.title || error.message || 'Erro desconhecido'));
    } finally {
      this.isLoading = false;
    }
  }

  private async uploadImages(vehicleId: number): Promise<void> {
    try {
      for (const image of this.selectedImages) {
        const formData = {
          Images: [image]
        };

        await this.vehicleImageService.postApiVehicleImages(vehicleId, formData).toPromise();
      }
    } catch (error) {
      console.error('Error uploading images:', error);
    }
  }

  private markFormGroupTouched(): void {
    Object.keys(this.vehicleForm.controls).forEach(key => {
      const control = this.vehicleForm.get(key);
      control?.markAsTouched();
    });
  }

  onCancel(): void {
    this.close.emit();
  }


  getFieldError(fieldName: string): string {
    const control = this.vehicleForm.get(fieldName);
    if (control?.errors && control.touched) {
      if (control.errors['required']) return `${fieldName} é obrigatório`;
      if (control.errors['pattern']) return `${fieldName} inválido`;
      if (control.errors['min']) return `${fieldName} deve ser maior que ${control.errors['min'].min}`;
      if (control.errors['max']) return `${fieldName} deve ser menor que ${control.errors['max'].max}`;
    }
    return '';
  }

  hasFieldError(fieldName: string): boolean {
    const control = this.vehicleForm.get(fieldName);
    return !!(control?.errors && control.touched);
  }
}