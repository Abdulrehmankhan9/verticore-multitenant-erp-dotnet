# VertiCore — Multi-Tenant Business ERP (.NET)

## What is VertiCore?
A multi-tenant business management system built with ASP.NET Core.
One platform, multiple businesses, completely isolated data.

## Business Problem
Small-medium businesses manage clients, invoices, and payments 
through Excel and Word. VertiCore replaces this with a structured, 
role-based, real-time platform.

## Tech Stack
- ASP.NET Core 8 (Web API)
- ASP.NET Razor Pages (Frontend)
- Entity Framework Core
- SQL Server
- JWT Authentication
- Clean Architecture

## Architecture
- Domain — Entities, Enums, Interfaces
- Application — Business Logic, Services, DTOs
- Infrastructure — Database, Repositories
- API — Controllers, Middleware
- Web — Razor Pages UI

## Features
- [ ] Multi-tenant data isolation
- [ ] Role-based access (SuperAdmin, TenantAdmin, Manager, Staff)
- [ ] Client management
- [ ] Invoice & billing with PDF export
- [ ] Dashboard & reports
- [ ] Audit logging

## Progress
- [x] Phase 0 — Solution setup complete
- [x] Phase 1 — Domain layer (Entities + Enums complete)
- [ ] Phase 2 — Auth system
- [ ] Phase 3 — Core modules
- [ ] Phase 4 — Advanced features
- [ ] Phase 5 — Polish & deploy

## VertiCore — Complete Scaffold Structure

```
Solution 'VertiCore'/
│
├── VertiCore.Domain/
│   ├── Entities/
│   ├── Enums/
│   └── Interfaces/
│
├── VertiCore.Application/
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── Client/
│   │   ├── Invoice/
│   │   └── Dashboard/
│   ├── Services/
│   └── Interfaces/
│
├── VertiCore.Infrastructure/
│   ├── Data/
│   │   └── Migrations/
│   ├── Repositories/
│   └── Services/
│
├── VertiCore.API/
│   ├── Controllers/
│   ├── Middleware/
│   └── Extensions/
│
└── VertiCore.Web/
    ├── Pages/
    │   ├── Auth/
    │   ├── Dashboard/
    │   ├── Clients/
    │   └── Invoices/
    └── wwwroot/
```