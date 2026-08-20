# TrafficFineManagement

Trafik Cezası Yönetim ve Onay Modülü — Junior .NET Developer case çalışması.

.NET 9 MVC web app for registering vehicles, recording traffic fines, and running a two-step approval workflow with an audit trail.

## Tech stack

- .NET 9 ASP.NET Core MVC
- Entity Framework Core 9 (Code First) + SQL Server / LocalDB
- FluentValidation
- Bootstrap 5, vanilla JavaScript

## Prerequisites

- .NET SDK 9.0+
- SQL Server LocalDB (Visual Studio / SQL Server Express) or a full SQL Server instance
- `dotnet-ef` tool (optional, only if you recreate migrations): `dotnet tool install --global dotnet-ef --version 9.0.8`

## How to run

From the repository root:

```powershell
dotnet restore
dotnet ef database update --project TrafficFineManagement --startup-project TrafficFineManagement
dotnet run --project TrafficFineManagement
```

Watch mode:

```powershell
dotnet watch --project TrafficFineManagement
```

Default URLs (`Properties/launchSettings.json`):

- HTTP: http://localhost:5277
- HTTPS: https://localhost:7277

The home page redirects to `/Dashboard`.

Connection string lives in `TrafficFineManagement/appsettings.json` (`DefaultConnection`). The default uses LocalDB and contains no secrets:

```
Server=(localdb)\mssqllocaldb;Database=TrafficFineManagement;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True
```

## Demo roles

There is no full Identity login. Use the **Aktif rol** dropdown in the navbar:

| Role | User shown in history | Can act when status is |
|------|------------------------|-------------------------|
| Yönetici | Ayşe Yılmaz (Yönetici) | Yeni, Yönetici Onayı |
| Finans | Mehmet Kaya (Finans) | Finans Onayı |

Switching role is stored in session and returns you to the same page.

## Workflow

Happy path:

`Yeni` → (Yönetici onaylar) → `Finans Onayı` → (Finans onaylar) → `Tamamlandı`

- A new fine starts as **Yeni** (manager queue). **Yönetici Onayı** is the same manager stage if a record is ever left in that status.
- Reject from the current stage sets **Reddedildi**. The Bootstrap modal requires a reason (min. 3 characters); cancel does not change status.
- **Tamamlandı** and **Reddedildi** are terminal: no edit, approve, or reject.

```
Yeni / Yönetici Onayı  --Yönetici Onayla-->  Finans Onayı  --Finans Onayla-->  Tamamlandı
         \                                      \
          \--Reddet--> Reddedildi                \--Reddet--> Reddedildi
```

## Business rules

### Vehicles

- Plate, type, and brand/model are required.
- Types: Binek, Çekici, Dorse, Kiralık Araç.
- Plate must look like a Turkish plate (example: `34 ABC 123`) and is unique.
- A vehicle with fines cannot be deleted.

### Fines

- Linked vehicle, amount (> 0), and date (not in the future) are required.
- List filters: plate and status.
- Closed records are read-only.

### Audit trail

Each create/approve/reject writes: actor, timestamp, action type, description or rejection reason, previous status, new status. History is shown on the fine details page and cannot be edited.

## Project layout

```
TrafficFineManagement.sln
TrafficFineManagement/
  Controllers/     MVC endpoints
  Data/            FineDbContext + migrations
  Models/          Entities and enums
  Services/        Approval workflow + current user
  Validators/      FluentValidation rules
  Views/           Razor UI
```

## Validation and errors

- Server: FluentValidation + ModelState, Turkish messages, TempData alerts.
- Client: jQuery unobtrusive validation on forms; reject modal validated in `wwwroot/js/site.js`.
- Unknown ids return 404. Illegal workflow moves return a friendly error and do not change status.

## License

Private case study / educational use.
