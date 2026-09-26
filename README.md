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
- Next.js 16 (React 19, TypeScript) — active application at `verticore-web/`
- ASP.NET Razor Pages — retained as the legacy UI in `VertiCore.Web/`

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
├── VertiCore.Web/             ← Legacy ASP.NET Razor Pages UI
└── verticore-web/             ← Active Next.js frontend
```

## Features
- [x] Multi-tenant data isolation
- [x] Role-based access (TenantAdmin, Manager, Staff)
- [x] JWT authentication (register, login, team invitations)
- [x] Forgot/reset password with expiring, single-use reset tokens
- [x] Client management (CRUD)
- [x] Invoice creation, status updates, and tenant-scoped PDF export
- [x] Role-specific dashboards for Tenant Admin/Manager and Staff
- [x] Staff task assignment, assigned-task list, and status updates
- [x] Global case-insensitive unique user email and duplicate-account checks
- [x] Audit logging
- [x] Email service (welcome, invitation, and password-reset links)
- [x] Next.js frontend (landing, auth, clients, invoices, users, tasks, dashboard, audit)
- [ ] Deployment

## Progress
- [x] Phase 0 — Solution setup & Clean Architecture
- [x] Phase 1 — Domain layer (Entities, Enums, ValueObjects, Events)
- [x] Phase 2 — Application layer (Services, DTOs, Validators, Mappings)
- [x] Phase 3 — Infrastructure layer (Repositories, JWT, PDF, Email)
- [x] Phase 4 — API layer (Controllers, Middleware, Extensions)
- [x] Phase 5 — Next.js Frontend
- [ ] Phase 6 — Deploy

## Frontend Routes
- `/` — public landing page
- `/login`, `/register` — authentication and tenant registration
- `/forgot-password`, `/reset-password` — password recovery
- `/dashboard` — role-specific overview
- `/clients` — client create, edit, and delete
- `/invoices`, `/invoices/new` — invoice list, status, PDF, and creation
- `/tasks` — task assignment for managers and status updates for staff
- `/users` — team list and invitations
- `/audit` — tenant audit log (Tenant Admin)

The Razor Pages app is retained for reference; the Next.js app is the active frontend.

## Local Development
Run the frontend from the repository root:

```powershell
npm.cmd --prefix .\verticore-web run dev -- --hostname 0.0.0.0 --port 3000
```

Run the API using its HTTPS profile from the repository root:

```powershell
dotnet run --project .\VertiCore.API\VertiCore.API\VertiCore.API.csproj --launch-profile https
```

The API expects PostgreSQL configuration in `VertiCore.API/VertiCore.API/appsettings.json` and local email credentials in .NET User Secrets. The development frontend URL is `http://localhost:3000`.

Apply pending database migrations from the repository root:

```powershell
dotnet ef database update --project .\VertiCore.Infrastructure\VertiCore.Infrastructure\VertiCore.Infrastructure.csproj --startup-project .\VertiCore.API\VertiCore.API\VertiCore.API.csproj
```

The unique-email migration stops if existing duplicate emails are found; resolve those accounts before applying it.

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