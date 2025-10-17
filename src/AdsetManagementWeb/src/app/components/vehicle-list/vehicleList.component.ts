import { Component, OnInit, OnDestroy } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { MatIconRegistry } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { Subscription } from 'rxjs';
import { VehicleListResponse } from '../../api/models/VehicleListResponse';
import { VehicleResponse } from '../../api/models/VehicleResponse';
import { VehicleImageService } from '../../api/services/VehicleImageService';
import { VehicleService } from '../../api/services/VehicleService';
import { VEHICLE_YEARS, VehicleYear } from '../../utils/date.utils';
import { VehicleStats } from '../vehicle-stats/vehicle-stats.component';
import { VehicleEventService } from '../../services/vehicle-event.service';
import { ApiConfigService } from '../../services/api-config.service';

@Component({
  selector: 'app-vehicle-list',
  templateUrl: './vehicleList.component.html',
  styleUrls: ['./vehicleList.component.css']
})
export class VehicleListComponent implements OnInit, OnDestroy {

  filterForm: FormGroup;
  vehicles: VehicleResponse[] = [];
  filteredVehicles: VehicleResponse[] = [];
  vehicleStats: VehicleStats = { total: 0, withPhotos: 0, withoutPhotos: 0 };
  
  private vehicleUpdateSubscription?: Subscription;

  currentPage = 1;
  pageSize = 5;
  totalPages = 1;
  totalItems = 0;

  isModalOpen = false;
  isEditMode = false;
  isLoading = false;
  selectedVehicle: VehicleResponse | undefined;
  filtersExpanded = true;


  hasPendingChanges = false;
  pendingVehicles: VehicleResponse[] = [];
  pendingCount = 0;

  availableYears: VehicleYear[] = VEHICLE_YEARS;


  sortColumn: string = '';
  sortDirection: 'asc' | 'desc' = 'asc';


  isImageViewerOpen = false;
  viewerImages: string[] = [];
  currentImageIndex = 0;


  isOptionsModalOpen = false;
  selectedVehicleForOptions: VehicleResponse | undefined;


  isDrawerOpen = false;
  selectedVehicleForDrawer: VehicleResponse | undefined;

  brands = ['Todos', 'Volkswagen', 'Ford', 'Chevrolet', 'Toyota', 'Honda'];
  priceOptions = ['Todos', '10mil a 50mil', '50mil a 90mil', '+90mil'];
  photoOptions = ['Todos', 'Com fotos', 'Sem fotos'];
  featuresOptions = [
    'Todos',
    'Ar Condicionado',
    'Alarme',
    'Airbag',
    'Freio ABS'
  ];
  colorOptions: string[] = ['Todos'];

  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService,
    private vehicleImageService: VehicleImageService,
    private matIconRegistry: MatIconRegistry,
    private domSanitizer: DomSanitizer,
    private vehicleEventService: VehicleEventService,
    private apiConfig: ApiConfigService,
    private http: HttpClient
  ) {
    this.filterForm = this.fb.group({
      plate: [''],
      brand: [''],
      model: [''],
      yearMin: [null],
      yearMax: [null],
      price: ['Todos'],
      photos: ['Todos'],
      features: ['Todos'],
      color: ['Todos']
    });

    this.matIconRegistry.addSvgIcon(
      'clear-sort',
      this.domSanitizer.bypassSecurityTrustResourceUrl('assets/icons/clear-sort.svg')
    );
    this.matIconRegistry.addSvgIcon(
      'clear-plans',
      this.domSanitizer.bypassSecurityTrustResourceUrl('assets/icons/clear-plans.svg')
    );
  }

  ngOnInit(): void {
    this.loadVehicles();
    this.loadDistinctColors();
    
    this.vehicleUpdateSubscription = this.vehicleEventService.vehicleUpdated$.subscribe(() => {
      this.loadVehicles();
    });
  }

  ngOnDestroy(): void {
    if (this.vehicleUpdateSubscription) {
      this.vehicleUpdateSubscription.unsubscribe();
    }
  }

  loadDistinctColors(): void {
    this.http.get<string[]>(this.apiConfig.buildUrl('/api/vehicle/colors')).subscribe({
      next: (colors: string[]) => {
        this.colorOptions = ['Todos', ...colors];
      },
      error: (error: any) => {
        console.error('Erro ao carregar cores distintas:', error);
      }
    });
  }

  loadVehicles(): void {
    this.isLoading = true;

    this.vehicleService.getApiVehicle(
      undefined,
      undefined,
      undefined,
      undefined,
      undefined,
      undefined,
      undefined,
      this.currentPage,
      this.pageSize
    ).subscribe({
      next: (response: VehicleListResponse) => {
        this.vehicles = response.data || [];
        this.filteredVehicles = [...this.vehicles];
        this.totalItems = response.totalItems || 0;
        this.totalPages = response.totalPages || 1;

        this.calculateStats();

        this.isLoading = false;        
      },
      error: (error: any) => {
        console.error('Erro ao carregar veículos:', error);
        this.isLoading = false;


        this.vehicles = [];
        this.filteredVehicles = [];
        this.totalItems = 0;
        this.calculateStats();
      }
    });
  }

  calculateStats(): void {
    this.vehicleStats.total = this.totalItems;
    this.vehicleStats.withPhotos = this.vehicles.filter((v: VehicleResponse) =>
      v.imagens && v.imagens.length > 0
    ).length;
    this.vehicleStats.withoutPhotos = this.vehicles.filter((v: VehicleResponse) =>
      !v.imagens || v.imagens.length === 0
    ).length;
  }

  onSearch(): void {
    this.isLoading = true;
    const filters = this.filterForm.value;


    setTimeout(() => {

      this.filteredVehicles = this.vehicles.filter(vehicle => {
        return (
          (!filters.plate || (vehicle.placa || '').toLowerCase().includes(filters.plate.toLowerCase())) &&
          (!filters.brand || filters.brand === 'Todos' || (vehicle.marca || '').toLowerCase() === filters.brand.toLowerCase()) &&
          (!filters.model || (vehicle.modelo || '').toLowerCase().includes(filters.model.toLowerCase())) &&
          (!filters.color || filters.color === 'Todos' || (vehicle.cor || '').toLowerCase() === filters.color.toLowerCase()) &&
          (!filters.yearMin || !vehicle.ano || vehicle.ano >= filters.yearMin) &&
          (!filters.yearMax || !vehicle.ano || vehicle.ano <= filters.yearMax) &&
          this.filterByPrice(vehicle, filters.price) &&
          this.filterByPhotos(vehicle, filters.photos) &&
          this.filterByFeatures(vehicle, filters.features)
        );
      });

      this.isLoading = false;      
    }, 500);
  }

  filterByPrice(vehicle: VehicleResponse, priceFilter: string): boolean {
    if (!priceFilter || priceFilter === 'Todos') return true;

    const price = vehicle.preco || 0;

    switch (priceFilter) {
      case '10mil a 50mil':
        return price >= 10000 && price <= 50000;
      case '50mil a 90mil':
        return price > 50000 && price <= 90000;
      case '+90mil':
        return price > 90000;
      default:
        return true;
    }
  }

  filterByPhotos(vehicle: VehicleResponse, photoFilter: string): boolean {
    if (!photoFilter || photoFilter === 'Todos') return true;

    const hasPhotos = !!(vehicle.imagens && vehicle.imagens.length > 0);
    return photoFilter === 'Com fotos' ? hasPhotos : !hasPhotos;
  }

  filterByFeatures(vehicle: VehicleResponse, featureFilter: string): boolean {
    if (!featureFilter || featureFilter === 'Todos') return true;

    const features = this.getFeaturesList(vehicle);
    return features.includes(featureFilter);
  }

  onClearFilters(): void {
    this.isLoading = true;

    this.filterForm.reset({
      plate: '',
      brand: '',
      model: '',
      yearMin: null,
      yearMax: null,
      price: 'Todos',
      photos: 'Todos',
      features: 'Todos',
      color: 'Todos'
    });


    setTimeout(() => {
      this.filteredVehicles = [...this.vehicles];
      this.isLoading = false;
    }, 300);
  }

  onRegisterVehicle(): void {
    this.selectedVehicle = undefined;
    this.isEditMode = false;
    this.isModalOpen = true;
  }

  onEditVehicle(vehicle: VehicleResponse): void {
    this.selectedVehicle = vehicle;
    this.isEditMode = true;
    this.isModalOpen = true;
  }

  onDeleteVehicle(vehicle: VehicleResponse): void {
    if (!vehicle.id) return;

    const confirmDelete = confirm(
      `Tem certeza que deseja excluir o veículo ${vehicle.marca} ${vehicle.modelo} (${vehicle.placa})?`
    );

    if (confirmDelete) {
      this.isLoading = true;

      this.vehicleService.deleteApiVehicle(vehicle.id).subscribe({
        next: () => {          
          this.loadVehicles();
        },
        error: (error) => {
          console.error('Erro ao excluir veículo:', error);
          alert('Erro ao excluir veículo. Tente novamente.');
          this.isLoading = false;
        }
      });
    }
  }

  onVehicleSave(savedVehicle: VehicleResponse): void {    
    this.isModalOpen = false;
    this.selectedVehicle = undefined;


    this.loadVehicles();
  }

  onModalClose(): void {
    this.isModalOpen = false;
    this.selectedVehicle = undefined;
  }

  onExportStock(): void {
    if (this.filteredVehicles.length === 0) {
      alert('Nenhum veículo encontrado para exportar.');
      return;
    }

    
    alert('Feature de exportação será implementada em breve.');
  }

  onSave(): void {
    if (this.pendingVehicles.length === 0) {
      return;
    }


    


    this.pendingVehicles = [];
    this.pendingCount = 0;
    this.hasPendingChanges = false;
    this.loadVehicles();
  }

  isICarrosActive(vehicle: VehicleResponse): boolean {
    return vehicle.pacoteICarros != null && vehicle.pacoteICarros !== '';
  }

  isWebMotorsActive(vehicle: VehicleResponse): boolean {
    return vehicle.pacoteWebMotors != null && vehicle.pacoteWebMotors !== '';
  }

  isPackageSelected(vehicle: VehicleResponse, packageName: string): boolean {
    return vehicle.pacoteICarros === packageName || vehicle.pacoteWebMotors === packageName;
  }

  isBronze(vehicle: VehicleResponse): boolean {
    return this.isPackageSelected(vehicle, 'Bronze');
  }

  isDiamante(vehicle: VehicleResponse): boolean {
    return this.isPackageSelected(vehicle, 'Diamond');
  }

  isPlatinum(vehicle: VehicleResponse): boolean {
    return this.isPackageSelected(vehicle, 'Platinum');
  }

  isBasic(vehicle: VehicleResponse): boolean {
    return this.isPackageSelected(vehicle, 'Basic');
  }

  getFeaturesList(vehicle: VehicleResponse): string[] {
    const features: string[] = [];
    if (vehicle.otherOptions) {
      if (vehicle.otherOptions.arCondicionado) features.push('Ar Condicionado');
      if (vehicle.otherOptions.alarme) features.push('Alarme');
      if (vehicle.otherOptions.airbag) features.push('Airbag');
      if (vehicle.otherOptions.abs) features.push('Freio ABS');
    }
    return features;
  }

  getFeaturesAsString(vehicle: VehicleResponse): string {
    const features = this.getFeaturesList(vehicle);
    return features.length > 0 ? features.join(', ') : 'Nenhuma';
  }

  getVehicleImageUrl(vehicle: VehicleResponse): string {
    if (vehicle.imagens && vehicle.imagens.length > 0) {
      const fileName = vehicle.imagens[0];      
      
      if (fileName && fileName !== '[object Object]') {
        if (fileName.startsWith('http://') || fileName.startsWith('https://')) {          
          return fileName;
        }

        return this.apiConfig.buildImageUrl(fileName);
      }
    }
    return 'https://cdn-icons-png.flaticon.com/512/11696/11696730.png';
  }


  getMainImageUrl(vehicle: VehicleResponse): string {
    return this.getVehicleImageUrl(vehicle);
  }

  formatKilometers(km: number | undefined): string {
    if (!km) return '0';
    return km.toLocaleString('pt-BR');
  }

  formatPrice(price: number | undefined): string {
    if (!price) return '0,00';
    return price.toLocaleString('pt-BR', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  }

  formatPhotoCount(count: number): string {
    return count < 10 ? `0${count}` : `${count}`;
  }

  hasFeatures(vehicle: VehicleResponse): boolean {
    return this.getFeaturesList(vehicle).length > 0;
  }

  getAllFeaturesText(vehicle: VehicleResponse): string {
    const features = this.getFeaturesList(vehicle);
    return features.length > 0 ? features.join(', ') : 'Nenhum opcional';
  }

  openImageGallery(vehicle: VehicleResponse): void {    
  }

  openImageViewer(vehicle: VehicleResponse): void {
    if (vehicle.imagens && vehicle.imagens.length > 0) {
      this.viewerImages = vehicle.imagens.map(img => {
        if (img.startsWith('http://') || img.startsWith('https://')) {
          return img;
        }
        return this.apiConfig.buildImageUrl(img);
      });
    } else {
      this.viewerImages = [this.getMainImageUrl(vehicle)];
    }
    this.currentImageIndex = 0;
    this.isImageViewerOpen = true;
  }

  closeImageViewer(): void {
    this.isImageViewerOpen = false;
    this.viewerImages = [];
    this.currentImageIndex = 0;
  }

  getCurrentViewerImage(): string {
    return this.viewerImages[this.currentImageIndex] || '';
  }

  nextImage(): void {
    if (this.currentImageIndex < this.viewerImages.length - 1) {
      this.currentImageIndex++;
    } else {
      this.currentImageIndex = 0;
    }
  }

  previousImage(): void {
    if (this.currentImageIndex > 0) {
      this.currentImageIndex--;
    } else {
      this.currentImageIndex = this.viewerImages.length - 1;
    }
  }


  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadVehicles();
    }
  }

  onPreviousPage(): void {
    if (this.currentPage > 1) {
      this.onPageChange(this.currentPage - 1);
    }
  }

  onNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.onPageChange(this.currentPage + 1);
    }
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisiblePages = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
    let endPage = Math.min(this.totalPages, startPage + maxVisiblePages - 1);

    if (endPage - startPage + 1 < maxVisiblePages) {
      startPage = Math.max(1, endPage - maxVisiblePages + 1);
    }

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }

    return pages;
  }

  isPreviousDisabled(): boolean {
    return this.currentPage === 1;
  }

  isNextDisabled(): boolean {
    return this.currentPage === this.totalPages || this.totalPages === 0;
  }


  toggleFilters(): void {
    this.filtersExpanded = !this.filtersExpanded;
  }

  sortBy(column: string): void {

    if (this.sortColumn === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {

      this.sortColumn = column;
      this.sortDirection = 'asc';
    }


    this.filteredVehicles.sort((a, b) => {
      let valueA: any;
      let valueB: any;

      switch (column) {
        case 'brand':
          valueA = `${a.marca || ''} ${a.modelo || ''}`.toLowerCase();
          valueB = `${b.marca || ''} ${b.modelo || ''}`.toLowerCase();
          break;
        case 'year':
          valueA = a.ano || 0;
          valueB = b.ano || 0;
          break;
        case 'price':
          valueA = a.preco || 0;
          valueB = b.preco || 0;
          break;
        case 'photos':
          valueA = a.imagens?.length || 0;
          valueB = b.imagens?.length || 0;
          break;
        default:
          return 0;
      }

      if (valueA < valueB) {
        return this.sortDirection === 'asc' ? -1 : 1;
      }
      if (valueA > valueB) {
        return this.sortDirection === 'asc' ? 1 : -1;
      }
      return 0;
    });
  }

  clearSort(): void {
    this.sortColumn = '';
    this.sortDirection = 'asc';
    this.loadVehicles();
  }

  onPageSizeChange(event: Event): void {
    const target = event.target as HTMLSelectElement;
    this.pageSize = Number(target.value);
    this.currentPage = 1;
    this.loadVehicles();
  }

  showVehicleOptions(vehicle: VehicleResponse): void {
    this.selectedVehicleForOptions = vehicle;
    this.isOptionsModalOpen = true;
  }

  closeOptionsModal(): void {
    this.isOptionsModalOpen = false;
    this.selectedVehicleForOptions = undefined;
  }

  toggleDrawer(vehicle: VehicleResponse): void {

    if (this.isDrawerOpen && this.selectedVehicleForDrawer?.id === vehicle.id) {
      this.closeDrawer();
    } else {

      this.selectedVehicleForDrawer = vehicle;
      this.isDrawerOpen = true;
    }
  }

  closeDrawer(): void {
    this.isDrawerOpen = false;
    this.selectedVehicleForDrawer = undefined;
  }

  clearPlans(): void {

    
    this.closeDrawer();
  }
}