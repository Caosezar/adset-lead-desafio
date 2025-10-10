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

---

## 🔐 Tecnologias

**Backend:** ASP.NET Core 6.0, Entity Framework Core, SQL Server, Swagger, Repository Pattern, DDD

**Frontend:** Angular 16+, TypeScript, Angular Material, RxJS

---

## 📝 Validações

**Obrigatórios:** Marca, Modelo, Ano (1900-2025), Placa (ABC-1234, ABC1234, ABC1D23), Cor, Preço (>0)

**Opcionais:** KM, Opcionais, Fotos (máx 15, jpeg/png/webp, 5MB cada), Planos (iCarros/WebMotors)

---

**Desenvolvido com ❤️ para o desafio Adset**
