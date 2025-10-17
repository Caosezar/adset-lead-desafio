# 🚀 Guia de Primeira Execução - Sistema de Gerenciamento de Veículos Adset

> **📝 Nota:** Esta documentação foi gerada com assistência de IA para facilitar o setup inicial do projeto.

---

## ⚠️ Notas Importantes

### 🎨 Sobre o Layout e Ícones
Durante o desenvolvimento, encontrei limitações com o arquivo Adobe Illustrator:
- Alguns ícones **não existiam** ou **não puderam ser exportados**
- Substituí por ícones equivalentes do Angular Material
- A estrutura visual foi mantida fiel ao layout original na medida do possível, achei um tanto confuso.

### 🤔 Sobre Funcionalidades Não Especificadas
Funcionalidades não detalhadas foram implementadas usando:
- **Bom senso** e **melhores práticas**
- **Consistência** com o sistema
- Foco na **experiência do usuário**

---

## 📋 Pré-requisitos

### Backend
- Visual Studio 2022+ Community
- .NET 6.0 SDK+
- SQL Server 2012+ Express (ou LocalDB)

### Frontend
- Node.js v16.x+
- Angular CLI 12.x+
- VS Code (recomendado)

---

## 🗄️ Setup Rápido

### 1. Banco de Dados

Edite `appsettings.json` no projeto **AdsetManagement.API**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=AdsetManagementDb;Trusted_Connection=true;TrustServerCertificate=true"
  }
}
```

### 2. Executar Migrations

**Package Manager Console (Visual Studio):**
```powershell
Update-Database
```

**Ou Command Line:**
```bash
cd src/AdsetManagement.API
dotnet ef database update --project ../AdsetManagement.Infrastructure/AdsetManagement.Infrastructure.csproj
```

---

## 🔧 Backend

```bash
dotnet restore
```

No Visual Studio: defina **AdsetManagement.API** como projeto de inicialização e pressione **F5**.

Acesse: **http://localhost:5000/swagger**

---

## 🎨 Frontend

```bash
cd src/AdsetManagementWeb
npm install
ng serve
```

Acesse: **http://localhost:4200**

---

## 🧪 Teste Básico

1. Clique em **"Registrar Veículo"**
2. Preencha os dados obrigatórios
3. Adicione imagens (máximo 15)
4. Selecione planos (iCarros/WebMotors)
5. Salve

**Funcionalidades:** CRUD completo, upload de imagens, filtros (placa, marca, ano, preço, fotos, cor), ordenação, drawer de planos, paginação

---

## 🐛 Problemas Comuns

- **SQL Server não conecta:** Verifique se está rodando e execute migrations
- **Porta 5000 ocupada:** `netstat -ano | findstr :5000` → `taskkill /PID [processo] /F`
- **Frontend não compila:** `rm -rf node_modules && npm install`
- **API não conecta:** Confirme backend rodando e porta em `environment.ts`
- **Migrations falham:** Recrie com `dotnet ef migrations add InitialCreate`
- **Imagens 404:** Verifique pasta `wwwroot/uploads/vehicles` e `app.UseStaticFiles()`

---

## 📁 Estrutura

```
src/
├── AdsetManagement.API/           # API REST
├── AdsetManagement.Application/   # Lógica de negócio
├── AdsetManagement.Domain/        # Entidades e interfaces
├── AdsetManagement.Infrastructure/# Repositórios e migrations
└── AdsetManagementWeb/            # Angular frontend
```

---

## 🎯 Endpoints

- `GET/POST/PUT/DELETE /api/Vehicle` - CRUD de veículos
- `POST/GET/DELETE /api/vehicle/{id}/images` - Gerenciamento de imagens
- `GET /uploads/vehicles/{id}/{file}` - Servir arquivos
- `GET /api/vehicle/colors` - Obter cores distintas (otimizado para filtros)

**Swagger UI:** http://localhost:5000/swagger

**Exemplos de Requisições:**

```bash
# Criar veículo
POST http://localhost:5000/api/vehicle
Content-Type: application/json

{
  "marca": "Toyota",
  "modelo": "Corolla",
  "ano": "2023",
  "placa": "ABC1234",
  "cor": "Prata",
  "preco": 95000,
  "km": 15000,
  "pacoteICarros": "Platinum",
  "pacoteWebMotors": "Básico",
  "otherOptions": {
    "arCondicionado": true,
    "alarme": true,
    "airbag": true,
    "abs": true
  }

# Upload de imagens
POST http://localhost:5000/api/vehicle/1/images
Content-Type: multipart/form-data

Images: [file1.jpg, file2.jpg, file3.jpg]

# Buscar veículos com filtros
GET http://localhost:5000/api/vehicle?marca=Toyota&anoMin=2020&page=1&pageSize=10
```

---

## 🔐 Tecnologias

**Backend:** ASP.NET Core 6.0, Entity Framework Core, SQL Server, Swagger, Repository Pattern, DDD

**Frontend:** Angular 16+, TypeScript, Angular Material, RxJS

---

## 🏗️ Arquitetura e Padrões Utilizados

### 📋 OpenAPI Specification

O projeto utiliza **OpenAPI 3.0** para definição e documentação da API:

#### **Geração de Serviços Frontend**

Os serviços TypeScript do frontend foram gerados automaticamente a partir da especificação OpenAPI:

```bash
# Localização do arquivo spec
src/AdesetManagement.Spec/openApi.yaml

# Comando de geração (executado automaticamente no build)
ng-openapi-gen --input ./openApi.yaml --output ./src/app/api
```

**Benefícios:**
- ✅ Tipos TypeScript fortemente tipados
- ✅ Sincronização automática entre backend e frontend
- ✅ Redução de erros em tempo de desenvolvimento
- ✅ Documentação viva e atualizada

**Serviços Gerados:**
```typescript
// Exemplo de serviço gerado
src/AdsetManagementWeb/src/app/api/
├── models/
│   ├── VehicleResponse.ts
│   ├── CreateVehicleRequest.ts
│   ├── UpdateVehicleRequest.ts
│   └── ...
├── services/
│   ├── VehicleService.ts
│   └── VehicleImageService.ts
└── base-service.ts
```

**Uso no Componente:**
```typescript
import { VehicleService } from '../../api/services/VehicleService';
import { CreateVehicleRequest } from '../../api/models/CreateVehicleRequest';

constructor(private vehicleService: VehicleService) {}

// Tipos já inferidos automaticamente
createVehicle(data: CreateVehicleRequest) {
  this.vehicleService.postApiVehicle({ body: data }).subscribe({
    next: (response) => console.log('Veículo criado:', response),
    error: (error) => console.error('Erro:', error)
  });
}
```

---

### 🎯 Input e Output Properties

O Angular utiliza **decorators** `@Input()` e `@Output()` para comunicação entre componentes pai e filho.

#### **@Input() - Recebendo Dados do Pai**

**Exemplo: Vehicle Modal Component**

```typescript
// vehicleModal.component.ts
import { Component, Input, OnInit } from '@angular/core';

export class VehicleModalComponent implements OnInit {
  // Recebe o veículo selecionado do componente pai
  @Input() vehicle?: VehicleResponse;
  
  // Recebe se está em modo de edição
  @Input() isEditMode: boolean = false;

  ngOnInit() {
    if (this.isEditMode && this.vehicle) {
      // Preenche o formulário com dados do veículo
      this.populateForm(this.vehicle);
    }
  }
}
```

**Uso no Template Pai:**
```html
<!-- vehicleList.component.html -->
<app-vehicle-modal
  [vehicle]="selectedVehicle"
  [isEditMode]="isEditMode">
</app-vehicle-modal>
```

#### **@Output() - Enviando Eventos para o Pai**

**Exemplo: Vehicle Modal Component**

```typescript
// vehicleModal.component.ts
import { Component, Output, EventEmitter } from '@angular/core';

export class VehicleModalComponent {
  // Emite evento quando veículo é salvo
  @Output() save = new EventEmitter<VehicleResponse>();
  
  // Emite evento quando modal é fechado
  @Output() close = new EventEmitter<void>();
  
  // Emite evento para atualizar lista
  @Output() refreshList = new EventEmitter<void>();

  onSaveVehicle() {
    // Salva o veículo...
    this.vehicleService.postApiVehicle(vehicleData).subscribe({
      next: (savedVehicle) => {
        // Emite o veículo salvo para o pai
        this.save.emit(savedVehicle);
        
        // Solicita atualização da lista
        this.refreshList.emit();
        
        // Fecha o modal
        this.close.emit();
      }
    });
  }
}
```

**Escuta no Componente Pai:**
```html
<!-- vehicleList.component.html -->
<app-vehicle-modal
  *ngIf="isModalOpen"
  [vehicle]="selectedVehicle"
  [isEditMode]="isEditMode"
  (save)="onVehicleSave($event)"
  (close)="onModalClose()"
  (refreshList)="loadVehicles()">
</app-vehicle-modal>
```

```typescript
// vehicleList.component.ts
export class VehicleListComponent {
  onVehicleSave(savedVehicle: VehicleResponse): void {
    console.log('Veículo salvo:', savedVehicle);
    this.isModalOpen = false;
    this.loadVehicles(); // Atualiza a listagem
  }
  
  onModalClose(): void {
    this.isModalOpen = false;
    this.selectedVehicle = undefined;
  }
}
```

#### **@Input() com Setter - Validação e Transformação**

```typescript
export class VehicleStatsComponent {
  private _vehicleStats!: VehicleStats;
  
  @Input()
  set vehicleStats(value: VehicleStats) {
    // Valida ou transforma dados antes de usar
    this._vehicleStats = value;
    this.updateCharts(); // Atualiza gráficos quando dados mudam
  }
  
  get vehicleStats(): VehicleStats {
    return this._vehicleStats;
  }
}
```

#### **@Output() com Dados Customizados**

```typescript
// Definindo interface para o evento
export interface VehicleFilterEvent {
  filters: VehicleFilterRequest;
  timestamp: Date;
}

export class VehicleFilterComponent {
  @Output() filterApplied = new EventEmitter<VehicleFilterEvent>();
  
  applyFilters(filters: VehicleFilterRequest) {
    this.filterApplied.emit({
      filters: filters,
      timestamp: new Date()
    });
  }
}
```

**Fluxo Completo de Comunicação:**

```
┌─────────────────────────────────────────────────────┐
│         VehicleListComponent (Pai)                  │
│                                                     │
│  selectedVehicle: VehicleResponse ──┐              │
│  isEditMode: boolean ───────────────┼──@Input()──┐ │
│                                     │             │ │
│  onVehicleSave(vehicle) ◄───────────┼──@Output() │ │
│  onModalClose() ◄───────────────────┼──(save)    │ │
│  loadVehicles() ◄───────────────────┘──(close)   │ │
│                                        (refresh)  │ │
└───────────────────────────────────────────────────┼─┘
                                                    │
                                                    ▼
                ┌─────────────────────────────────────┐
                │   VehicleModalComponent (Filho)     │
                │                                     │
                │  @Input() vehicle                   │
                │  @Input() isEditMode                │
                │                                     │
                │  @Output() save                     │
                │  @Output() close                    │
                │  @Output() refreshList              │
                └─────────────────────────────────────┘
```

---

## 📝 Validações

**Obrigatórios:** Marca, Modelo, Ano (1900-2025), Placa (ABC-1234, ABC1234, ABC1D23), Cor, Preço (>0)

**Opcionais:** KM, Opcionais, Fotos (máx 15, jpeg/png/webp, 5MB cada), Planos (iCarros/WebMotors)

---

**Desenvolvido com ❤️ para o desafio Adset**
