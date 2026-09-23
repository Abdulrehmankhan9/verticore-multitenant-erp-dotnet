# VertiCore — Multi-Tenant Business ERP (.NET)

## What is VertiCore?
A multi-tenant business management system built with ASP.NET Core 8 and Next.js.
One platform, multiple businesses, completely isolated data.

## Business Problem
Small-medium businesses manage clients, invoices, and payments
through Excel and Word. VertiCore replaces this with a structured,
role-based, real-time platform.

## Tech Stack

**Backend:**
- ASP.NET Core 8 (Web API)
- Entity Framework Core
- PostgreSQL (Npgsql)
- JWT Authentication
- AutoMapper
- FluentValidation
- QuestPDF (Invoice PDF generation)
- BCrypt (Password hashing)

**Frontend:**
- Next.js (React) — in progress

**Architecture:**
- Clean Architecture
- Repository Pattern
- Multi-tenant middleware

## Architecture

```
VertiCore/
├── VertiCore.Domain/          ← Entities, Enums, ValueObjects, Events
├── VertiCore.Application/     ← Services, DTOs, Interfaces, Validators
├── VertiCore.Infrastructure/  ← Database, Repositories, JWT, PDF, Email
├── VertiCore.API/             ← Controllers, Middleware, Extensions
└── verticore-web/             ← Next.js Frontend (coming soon)
```

## Features
- [x] Multi-tenant data isolation
- [x] Role-based access (TenantAdmin, Manager, Staff)
- [x] JWT Authentication (Register, Login, Invite User)
- [x] Client management (CRUD)
- [x] Invoice & billing with PDF export
- [x] Dashboard & reports
- [x] Audit logging
- [x] Email service (User invitations)
- [ ] Next.js Frontend
- [ ] Deployment

## Progress
- [x] Phase 0 — Solution setup & Clean Architecture
- [x] Phase 1 — Domain layer (Entities, Enums, ValueObjects, Events)
- [x] Phase 2 — Application layer (Services, DTOs, Validators, Mappings)
- [x] Phase 3 — Infrastructure layer (Repositories, JWT, PDF, Email)
- [x] Phase 4 — API layer (Controllers, Middleware, Extensions)
- [ ] Phase 5 — Next.js Frontend
- [ ] Phase 6 — Deploy

## Complete Structure

```
VertiCore/
│
├── VertiCore.Domain/
│   ├── Common/
│   │   └── BaseEntity.cs
│   ├── Entities/
│   │   ├── AuditLog.cs
│   │   ├── Client.cs
│   │   ├── Invoice.cs
│   │   ├── InvoiceItem.cs
│   │   ├── Tenant.cs
│   │   ├── User.cs
│   │   └── UserInvitation.cs
│   ├── Enums/
│   │   ├── InvoiceStatus.cs
│   │   └── UserRole.cs
│   ├── Events/
│   │   ├── ClientCreatedEvent.cs
│   │   └── InvoiceCreatedEvent.cs
│   ├── Exceptions/
│   │   ├── ClientNotFoundException.cs
│   │   ├── InvalidCredentialsException.cs
│   │   ├── InvoiceNotFoundException.cs
│   │   └── TenantNotFoundException.cs
│   └── ValueObjects/
│       ├── Email.cs
│       └── Money.cs
│
├── VertiCore.Application/
│   ├── DTOs/
│   │   ├── Auth/
│   │   ├── Client/
│   │   ├── Dashboard/
│   │   ├── Invoice/
│   │   └── User/
│   ├── Exceptions/
│   │   └── ValidationException.cs
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── IRepository.cs
│   │   │   ├── IClientRepository.cs
│   │   │   ├── IInvoiceRepository.cs
│   │   │   ├── IInvoiceItemRepository.cs
│   │   │   └── IUserRepository.cs
│   │   └── Services/
│   │       ├── IAuthService.cs
│   │       ├── IClientService.cs
│   │       ├── IInvoiceService.cs
│   │       ├── IUserService.cs
│   │       ├── IDashboardService.cs
│   │       ├── IAuditLogService.cs
│   │       ├── ICurrentTenantService.cs
│   │       ├── IEmailService.cs
│   │       ├── IJwtService.cs
│   │       └── IPdfService.cs
│   ├── Mappings/
│   │   ├── AuditLogMappingProfile.cs
│   │   ├── ClientMappingProfile.cs
│   │   └── InvoiceMappingProfile.cs
│   ├── Services/
│   │   ├── AuthService.cs
│   │   ├── ClientService.cs
│   │   ├── InvoiceService.cs
│   │   ├── UserService.cs
│   │   ├── DashboardService.cs
│   │   ├── AuditLogService.cs
│   │   └── CurrentTenantService.cs
│   └── Validators/
│       ├── Auth/
│       ├── Client/
│       └── Invoice/
│
├── VertiCore.Infrastructure/
│   ├── Data/
│   │   └── AppDbContext.cs
│   ├── Migrations/
│   ├── Repositories/
│   │   ├── BaseRepository.cs
│   │   ├── ClientRepository.cs
│   │   ├── InvoiceRepository.cs
│   │   ├── InvoiceItemRepository.cs
│   │   └── UserRepository.cs
│   └── Services/
│       ├── JwtService.cs
│       ├── PdfService.cs
│       └── EmailService.cs
│
├── VertiCore.API/
│   ├── Controllers/
│   │   ├── AuthController.cs
│   │   ├── ClientController.cs
│   │   ├── InvoiceController.cs
│   │   ├── UserController.cs
│   │   ├── DashboardController.cs
│   │   └── AuditLogController.cs
│   ├── Extensions/
│   │   ├── ServiceCollectionExtensions.cs
│   │   └── ApplicationBuilderExtensions.cs
│   ├── Filters/
│   │   └── ValidationFilter.cs
│   ├── Middleware/
│   │   ├── ExceptionMiddleware.cs
│   │   └── TenantMiddleware.cs
│   └── Program.cs
│
└── verticore-web/             ← Next.js (coming soon)
```