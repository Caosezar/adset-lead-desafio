import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CreateVehicleRequest } from '../../api/models/CreateVehicleRequest';
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
  existingImages: Array<{id: number, url: string}> = []; 
  removedImageIds: number[] = []; 


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
    { key: 'alarme', label: 'Alarme' },
    { key: 'airbag', label: 'Airbag' },
    { key: 'abs', label: 'ABS' }
  ];

  
  iCarrosPlans = [
    { value: '', label: 'Nenhum' },
    { value: 'Bronze', label: 'Bronze' },
    { value: 'Diamante', label: 'Diamante' },
    { value: 'Platinum', label: 'Platinum' }
  ];

  webMotorsPlans = [
    { value: '', label: 'Nenhum' },
    { value: 'Básico', label: 'Básico' }
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
      placa: ['', [Validators.required, Validators.pattern(/^[A-Z]{3}-?\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$/)]],
      marca: ['', Validators.required],
      modelo: ['', Validators.required],
      ano: ['', [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      cor: ['', Validators.required],
      km: ['', [Validators.required, Validators.min(0)]],
      preco: ['', [Validators.required, Validators.min(0)]],

      
      arCondicionado: [false],
      alarme: [false],
      airbag: [false],
      abs: [false],

      
      pacoteICarros: [''],
      pacoteWebMotors: ['']
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
      preco: this.vehicle.preco,
      pacoteICarros: this.vehicle.pacoteICarros || '',
      pacoteWebMotors: this.vehicle.pacoteWebMotors || ''
    });

    
    if (this.vehicle.otherOptions) {
      this.availableFeatures.forEach(feature => {
        const value = (this.vehicle!.otherOptions as any)?.[feature.key] || false;
        this.vehicleForm.patchValue({ [feature.key]: value });
      });
    }
  }

  private async loadExistingImages(): Promise<void> {
    if (!this.vehicle?.id) {
      this.existingImages = [];
      return;
    }

    try {
      
      const images = await this.vehicleImageService.getApiVehicleImages(this.vehicle.id).toPromise();
      
      this.existingImages = (images || []).map(img => ({
        id: img.id || 0,
        url: img.imageUrl || ''
      }));
      
      console.log('[loadExistingImages] Loaded images with IDs:', this.existingImages);
    } catch (error) {
      console.error('[loadExistingImages] Error loading images:', error);
      this.existingImages = [];
    }
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

  async removeImage(index: number, isExisting: boolean = false): Promise<void> {
    if (isExisting) {
      
      const image = this.existingImages[index];
      console.log(`[removeImage] Removing existing image at index ${index}:`, image);
      
      
      this.removedImageIds.push(image.id);
      this.existingImages.splice(index, 1);
      
      
      if (this.isEditMode && this.vehicle?.id) {
        await this.deleteImageFromServer(image.id);
      }
    } else {
      
      console.log(`[removeImage] Removing new image at index ${index}`);
      this.selectedImages.splice(index, 1);
      this.imagePreviewUrls.splice(index, 1);
    }
  }

  
  private async deleteImageFromServer(imageId: number): Promise<void> {
    try {
      if (!this.vehicle?.id) return;

      console.log(`[deleteImageFromServer] Deleting image ID ${imageId} from vehicle ${this.vehicle.id}`);
      
      
      await this.vehicleImageService.deleteApiVehicleImages(this.vehicle.id, imageId).toPromise();
      
      console.log(`[deleteImageFromServer] Image deleted successfully`);
    } catch (error) {
      console.error('[deleteImageFromServer] Error deleting image:', error);
      alert('Erro ao remover imagem. Por favor, tente novamente.');
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

      
      const otherOptions: OtherOptionsRequest = {};
      this.availableFeatures.forEach(feature => {
        (otherOptions as any)[feature.key] = formValue[feature.key] || false;
      });

      if (this.isEditMode && this.vehicle) {
        
        const updateRequest: UpdateVehicleRequest = {
          placa: formValue.placa,
          marca: formValue.marca,
          modelo: formValue.modelo,
          ano: formValue.ano?.toString() || '',
          cor: formValue.cor,
          km: formValue.km,
          preco: parseFloat(formValue.preco) || 0,
          otherOptions: otherOptions,
          pacoteICarros: formValue.pacoteICarros || null,
          pacoteWebMotors: formValue.pacoteWebMotors || null
        };

        console.log('Updating vehicle:', updateRequest);
        savedVehicle = await this.vehicleService.putApiVehicle(this.vehicle.id!, updateRequest).toPromise();

        
        if (this.selectedImages.length > 0 && savedVehicle.id) {
          console.log('Uploading images for vehicle:', savedVehicle.id);
          await this.uploadImages(savedVehicle.id);
        }
      } else {
        
        const createRequest: CreateVehicleRequest = {
          placa: formValue.placa,
          marca: formValue.marca,
          modelo: formValue.modelo,
          ano: formValue.ano?.toString() || '',
          cor: formValue.cor,
          km: formValue.km,
          preco: parseFloat(formValue.preco) || 0,
          otherOptions: otherOptions,
          pacoteICarros: formValue.pacoteICarros || null,
          pacoteWebMotors: formValue.pacoteWebMotors || null
        };

        console.log('Creating vehicle:', createRequest);
        savedVehicle = await this.vehicleService.postApiVehicle(createRequest).toPromise();
        
        
        if (this.selectedImages.length > 0 && savedVehicle.id) {
          console.log(`Uploading ${this.selectedImages.length} images for vehicle ${savedVehicle.id}`);
          await this.uploadImages(savedVehicle.id);
          
          
          const updatedVehicle = await this.vehicleService.getApiVehicle1(savedVehicle.id).toPromise();
          savedVehicle = updatedVehicle;
        }
      }

      console.log('Vehicle saved successfully:', savedVehicle);
      this.save.emit(savedVehicle);
      
      
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
      console.log(`[uploadImages] Starting upload for vehicle ${vehicleId}`);
      console.log(`[uploadImages] Number of selected images: ${this.selectedImages.length}`);
      
      
      this.selectedImages.forEach((img, index) => {
        console.log(`[uploadImages] Image ${index}: ${img.name}, Size: ${img.size}, Type: ${img.type}`);
      });
      
      
      const formData = new FormData();
      this.selectedImages.forEach((image) => {
        formData.append('Images', image, image.name);
      });

      console.log('[uploadImages] FormData created with', this.selectedImages.length, 'images');
      
      
      const result = await this.http.post<any>(
        `http://localhost:5000/api/vehicle/${vehicleId}/images`,
        formData
      ).toPromise();
      
      console.log('[uploadImages] Upload response:', result);
    } catch (error) {
      console.error('[uploadImages] Error uploading images:', error);
      throw error;
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