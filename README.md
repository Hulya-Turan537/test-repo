# TrafficFineManagement

Trafik Cezası Yönetim ve Onay Modülü — Junior .NET Developer case çalışması.

Traffic Fine Management and Approval Module built as a .NET 9 MVC web application.

## Tech stack

- .NET 9 ASP.NET Core MVC
- Entity Framework Core 9 + SQL Server
- FluentValidation

## Prerequisites

- .NET SDK 9.0+
- SQL Server (LocalDB or full instance)

## Getting started

```powershell
dotnet restore
dotnet build TrafficFineManagement.sln
dotnet run --project TrafficFineManagement
```

Update the SQL Server connection string in `TrafficFineManagement/appsettings.json` before applying EF Core migrations (added in later issues).

## Domain (from case)

### Vehicles (`Araç`)

Required fields: plate (`Plaka`), vehicle type (`Araç tipi`), brand/model (`Marka / Model`).

Vehicle types: Binek, Çekici, Dorse, Kiralık Araç.

### Approval workflow (`Onay süreci`)

`Yeni` → `Yönetici Onayı` → `Finans Onayı` → `Tamamlandı`

A fine can also be rejected (`Reddedildi`) with a required rejection reason.

### Audit trail (`Onay geçmişi`)

Each approval/rejection records: actor, timestamp, action type, description/reason, previous state, new state.

## Issue-driven delivery

Feature work is tracked as GitHub Issues. Implement one issue per branch (`feature/issue-N-...`).

## License

Private case study / educational use.
