# FormCraft — Dynamic Form Builder

A production-style **Dynamic Form Builder** built with **.NET 9 / Blazor Server (Interactive Server)**, **EF Core + SQL Server**, **MediatR (CQRS)**, **FluentValidation**, **AutoMapper**, and **MudBlazor**, following **Clean Architecture** and **DDD** principles.

It provides three primary modules — a **Forms Library**, a drag‑and‑drop **Form Designer**, and a visual **Field Mapping** tool — plus a **dynamic runtime renderer** and a **submission engine**.

---

## Architecture

Clean Architecture with dependencies pointing inward (`Web → Infrastructure → Application → Domain`; `Domain` depends on nothing).

```
src/
  FormBuilder.Domain          Entities, enums, value objects, domain events, repository/service interfaces
  FormBuilder.Application     CQRS (MediatR), DTOs, validators, pipeline behaviors, AutoMapper, DI
  FormBuilder.Infrastructure  EF Core DbContext + configurations, repositories, UnitOfWork,
                              ITableMetadataService (INFORMATION_SCHEMA), seed data, migrations
  FormBuilder.Web             Blazor Server UI (MudBlazor), Minimal API endpoints, runtime renderer
tests/
  FormBuilder.UnitTests       xUnit + FluentAssertions + Moq
db/
  FormBuilder.sql             Idempotent SQL migration script
```

### Key patterns

- **DDD aggregate** — `Form` is the aggregate root and owns the `Page → Row → Column → Field` tree. Invariants (publish lifecycle, "read‑only when published", "≥ 1 page", grid widths) live in the domain.
- **CQRS** — every use case is a MediatR `IRequest` + handler. A `ValidationBehavior` pipeline runs FluentValidation before handlers.
- **Repository + Unit of Work** — repositories expose intent‑revealing methods; `IUnitOfWork.SaveChangesAsync` commits and dispatches domain events via MediatR.
- **Domain events** — raised by aggregates, buffered on `BaseEntity`, published as `DomainEventNotification<T>` after a successful `SaveChanges`.
- **Value objects as JSON** — field validation/appearance/conditional‑logic settings are stored as JSON (`ValidationJson`, `SettingsJson`) so the field schema can evolve without migrations.

---

## Modules

### 1. Forms Library (`/`)
Card grid with search, status chips, per‑form stats (fields / pages / submissions), create & delete dialogs, and a per‑card menu (Designer, Mapping, Publish/Archive, View Live, Delete). Deletes are **soft deletes**.

### 2. Form Designer (`/forms/{id}/designer`)
- **Left** — pages list (add / rename / delete / select).
- **Center** — drag‑and‑drop canvas of rows → columns → fields, with row reordering, delete, and a live field preview.
- **Right** — toolbox tabs: **Fields** (grouped Basic / Choice / Advanced / Static) and **Layout** (1/2/3/4‑column templates on the 12‑grid).
- **Config drawer** — General / Validation / Appearance / Conditional Logic tabs.
- **Preview mode** renders real, interactive controls (no submission).
- **Save** (draft) / **Publish**. Published forms are read‑only; **Edit** reverts to draft.

Both **drag‑and‑drop** and **click‑to‑add** are supported for fields and layouts.

### 3. Field Mapping (`/forms/{id}/mapping`)
- **Left** — searchable form fields. **Right** — pick a target table, then its columns (with PK / type info) read live from `INFORMATION_SCHEMA`.
- **Center** — mapping summary with SVG connectors and per‑row delete.
- Create a mapping by **clicking a field then a column**, or **dragging a field onto a column**.
- Rules enforced server‑side: a field maps to **at most one** column, and the target column **must exist**.

### 4. Runtime renderer (`/f/{slug}`) + submissions
Published forms render dynamically from configuration (no hardcoded controls) using the shared `FieldRenderer`. Submissions are validated (required fields) and stored in `FormSubmissions` / `FormSubmissionValues`.

---

## Running locally

### Prerequisites
- .NET 9 SDK
- SQL Server **LocalDB**

### Database
The app targets `(localdb)\FormBuilderLocalDB` (database `FormBuilderDb`). On this machine the default `MSSQLLocalDB` (SQL 2025 runtime) was broken, so a dedicated instance pinned to the working 2019 runtime is used:

```powershell
sqllocaldb create FormBuilderLocalDB 15.0
sqllocaldb start  FormBuilderLocalDB
```

Change the connection string in `src/FormBuilder.Web/appsettings.json` to use any SQL Server you prefer.

> Migrations **and seed data run automatically on startup**. The seed creates sample business tables (`Customers`, `Leads`, `EventRegistrations`) as mapping targets, a default `DatabaseConnection`, and two example forms.

### Run

```bash
dotnet run --project src/FormBuilder.Web
# then open the printed http URL (e.g. http://localhost:5005)
```

### Test

```bash
dotnet test
```

### Apply migrations manually (optional)

```bash
dotnet ef database update --project src/FormBuilder.Infrastructure --startup-project src/FormBuilder.Infrastructure
# or run the generated idempotent script: db/FormBuilder.sql
```

---

## Minimal API

The Blazor UI calls MediatR directly; the same use cases are also exposed over HTTP:

| Method | Route | Purpose |
|--------|-------|---------|
| GET    | `/api/forms?search=` | List forms |
| POST   | `/api/forms` | Create form |
| PUT/DELETE | `/api/forms/{id}` | Update / soft‑delete |
| POST   | `/api/forms/{id}/publish` · `/archive` | Lifecycle |
| GET/PUT| `/api/forms/{id}/designer` | Load / save layout |
| GET/PUT| `/api/forms/{id}/mappings` (+ `/fields`) | Mappings |
| GET    | `/api/metadata/tables` · `/tables/{t}/columns` | DB metadata |
| POST   | `/api/f/{slug}/submit` | Submit a published form |
| GET    | `/api/forms/{id}/submissions` · `/api/submissions/{id}` | Read submissions |

Known exceptions are mapped to `400` (domain/validation) / `404` (not found) by `ApiExceptionMiddleware`.

---

## Notes
- **AutoMapper 14.0.0** is the latest MIT‑licensed release and carries advisory `GHSA‑rvv3‑g6hj‑g44x`; it is included because the brief requires AutoMapper. Usage is limited to internal DTO projection (no untrusted input), so practical risk is low. Replace with manual mapping to remove the dependency entirely.
- Drag‑and‑drop uses native HTML5 events via Blazor; because the designer is a single interactive component the drag payload is held in component state (no `dataTransfer` round‑tripping).
