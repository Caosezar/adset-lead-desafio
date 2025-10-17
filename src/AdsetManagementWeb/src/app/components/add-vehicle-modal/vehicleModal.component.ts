import { HttpClient } from '@angular/common/http';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CreateVehicleRequest } from '../../api/models/CreateVehicleRequest';
import { OtherOptionsRequest } from '../../api/models/OtherOptionsRequest';
import { UpdateVehicleRequest } from '../../api/models/UpdateVehicleRequest';
import { VehicleResponse } from '../../api/models/VehicleResponse';
import { VehicleImageService } from '../../api/services/VehicleImageService';
import { VehicleService } from '../../api/services/VehicleService';
import { VehicleEventService } from '../../services/vehicle-event.service';
import { ApiConfigService } from '../../services/api-config.service';

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
  existingImages: Array<{ id: number, url: string }> = [];
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
    private http: HttpClient,
    private vehicleEventService: VehicleEventService,
    private apiConfig: ApiConfigService
  ) { }

  async ngOnInit(): Promise<void> {
    this.initializeForm();

    if (this.isEditMode && this.vehicle) {
      this.populateForm();
      await this.loadExistingImages();
    }
  }

  private initializeForm(): void {
    this.vehicleForm = this.fb.group({
      placa: ['', [Validators.required, Validators.pattern(/^[A-Z]{3}-?\d{4}$|^[A-Z]{3}\d[A-Z]\d{2}$/)]],
      marca: ['', Validators.required],
      modelo: ['', Validators.required],
      ano: ['', [Validators.required, Validators.min(1900), Validators.max(new Date().getFullYear() + 1)]],
      cor: ['', Validators.required],
      km: ['', [Validators.min(0)]],
      preco: ['', [Validators.required, Validators.min(0.01)]],


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

      this.existingImages = (images || []).map(img => {
        const imageUrl = img.imageUrl || '';
        const fullUrl = this.apiConfig.buildImageUrl(imageUrl);
        return {
          id: img.id || 0,
          url: fullUrl
        };
      });
      
    } catch (error) {
      this.existingImages = [];
    }
  }

  onImageSelect(event: any): void {
    const files = event.target.files;
    if (!files) return;

    const MAX_IMAGES = 15;
    const currentTotalImages = this.existingImages.length + this.selectedImages.length;
    const availableSlots = MAX_IMAGES - this.existingImages.length;

    if (files.length > availableSlots) {
      alert(`Você pode adicionar no máximo ${availableSlots} imagem(ns). Limite total: ${MAX_IMAGES} imagens por veículo.`);
      event.target.value = '';
      return;
    }

    this.selectedImages = Array.from(files);
    console.log('Imagens selecionadas:', this.selectedImages.length, this.selectedImages);
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


      this.removedImageIds.push(image.id);
      this.existingImages.splice(index, 1);


      if (this.isEditMode && this.vehicle?.id) {
        await this.deleteImageFromServer(image.id);
      }
    } else {
      this.selectedImages.splice(index, 1);
      this.imagePreviewUrls.splice(index, 1);
    }
  }


  private async deleteImageFromServer(imageId: number): Promise<void> {
    try {
      if (!this.vehicle?.id) return;


      await this.vehicleImageService.deleteApiVehicleImages(this.vehicle.id, imageId).toPromise();
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
        // Atualiza o veículo usando o endpoint normal
        const updateRequest: UpdateVehicleRequest = {
          placa: formValue.placa,
          marca: formValue.marca,
          modelo: formValue.modelo,
          ano: formValue.ano?.toString() || '',
          cor: formValue.cor,
          km: formValue.km,
          preco: parseFloat(formValue.preco),
          otherOptions: otherOptions,
          pacoteICarros: formValue.pacoteICarros || null,
          pacoteWebMotors: formValue.pacoteWebMotors || null
        };
        savedVehicle = await this.vehicleService.putApiVehicle(this.vehicle.id!, updateRequest).toPromise();
        
        // Se houver novas imagens, faz upload separado (mesmo fluxo do Create)
        if (this.selectedImages.length > 0 && savedVehicle.id) {
          console.log('Fazendo upload de', this.selectedImages.length, 'imagem(ns)...');
          await this.uploadImages(savedVehicle.id);
          
          // Recarrega o veículo com as novas imagens
          const updatedVehicle = await this.vehicleService.getApiVehicle1(savedVehicle.id).toPromise();
          savedVehicle = updatedVehicle;
        }
      } else {

        const createRequest: CreateVehicleRequest = {
          placa: formValue.placa,
          marca: formValue.marca,
          modelo: formValue.modelo,
          ano: formValue.ano?.toString() || '',
          cor: formValue.cor,
          km: formValue.km,
          preco: parseFloat(formValue.preco),
          otherOptions: otherOptions,
          pacoteICarros: formValue.pacoteICarros || null,
          pacoteWebMotors: formValue.pacoteWebMotors || null
        };
        savedVehicle = await this.vehicleService.postApiVehicle(createRequest).toPromise();


        if (this.selectedImages.length > 0 && savedVehicle.id) {
          await this.uploadImages(savedVehicle.id);


          const updatedVehicle = await this.vehicleService.getApiVehicle1(savedVehicle.id).toPromise();
          savedVehicle = updatedVehicle;
        }
      }
      this.save.emit(savedVehicle);

      // Notifica que o veículo foi atualizado/criado
      this.vehicleEventService.notifyVehicleUpdated();

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
      console.log('[uploadImages] Iniciando upload de imagens para veículo:', vehicleId);
      console.log('[uploadImages] selectedImages.length:', this.selectedImages.length);
      
      if (this.selectedImages.length === 0) {
        console.warn('[uploadImages] Nenhuma imagem selecionada!');
        return;
      }

      const formData = new FormData();
      this.selectedImages.forEach((image, index) => {
        console.log(`[uploadImages] Adicionando imagem ${index + 1}:`, {
          name: image.name,
          size: image.size,
          type: image.type
        });
        formData.append('Images', image, image.name);
      });

      // Verificar conteúdo do FormData
      console.log('[uploadImages] Conteúdo do FormData:');
      formData.forEach((value, key) => {
        if (value instanceof File) {
          console.log(`  ${key}: [File] ${value.name} (${value.size} bytes)`);
        } else {
          console.log(`  ${key}: ${value}`);
        }
      });

      const uploadUrl = this.apiConfig.buildUrl(`/api/vehicle/${vehicleId}/images`);
      console.log('[uploadImages] Fazendo POST para:', uploadUrl);
      const result = await this.http.post<any>(uploadUrl, formData).toPromise();
      
      console.log('[uploadImages] Upload concluído com sucesso:', result);
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
      if (control.errors['required']) {
        if (fieldName === 'preco') return 'Preço é obrigatório';
        return `${fieldName} é obrigatório`;
      }
      if (control.errors['pattern']) return `${fieldName} inválido`;
      if (control.errors['min']) {
        if (fieldName === 'preco') return 'Preço deve ser maior que R$ 0,00';
        return `${fieldName} deve ser maior que ${control.errors['min'].min}`;
      }
      if (control.errors['max']) return `${fieldName} deve ser menor que ${control.errors['max'].max}`;
    }
    return '';
  }

  hasFieldError(fieldName: string): boolean {
    const control = this.vehicleForm.get(fieldName);
    return !!(control?.errors && control.touched);
  }
}