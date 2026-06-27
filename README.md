# LuminusCampus

**LuminusCampus** is a secure, multi-module ERP system built with ASP.NET Core 8.0 and Entity Framework Core. Designed for schools, colleges, and offices that need centralized management of students, staff, finance, and academics.

## Features
- **Multi-Module**: Student Info, Academics, Finance, Exams, Staff, Transport, Communication
- **Role-Based Access**: SuperAdmin → Admin → Staff hierarchy with page-level permissions
- **URL Privacy**: Token-based URL obfuscation hides internal routes
- **Session Tracking**: Logs user sessions with IP, browser, and device info
- **Auto Seeding**: Database seeds itself on first run — roles, modules, pages, and default SuperAdmin

## Tech Stack
| Layer | Technology |
|-------|-----------|
| Backend | ASP.NET Core 8.0 |
| ORM | Entity Framework Core 8.0 |
| Database | MySQL (via Pomelo) |
| Auth | ASP.NET Core Identity |
| Frontend | Razor Views, Bootstrap 5, Custom CSS |

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL Server (5.7+ or 8.x)

## Quick Start
```bash
# 1. Clone the repo
git clone https://github.com/AlpeshMuneshwar/LuminusCampus.git
cd LuminusCampus

# 2. Set up your config
cd webappi
cp appsettings.Example.json appsettings.json
```

Open `appsettings.json` and fill in your MySQL credentials:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=LuminusCampus;User=root;Password=YOUR_PASSWORD;"
  }
}
```

```bash
# 3. Run the app
dotnet run
```

The app will:
- Auto-create the database via EF migrations
- Seed default roles, modules, pages, and a SuperAdmin account
- Start listening on `http://localhost:5000` (or the port shown in terminal)

## Default Login
| Role | Email | Password |
|------|-------|----------|
| SuperAdmin | superadmin@erp.com | Admin@123 |

> Change this password immediately in production.

## Project Structure
```
webappi/
├── Controllers/       # Route handlers grouped by module (SIS, ALM, STAFF, etc.)
├── Data/              # EF models, DbContext, and entity definitions
├── Middleware/         # URL obfuscation, auth checks, subscription enforcement
├── Migrations/        # EF Core migration files
├── Models/            # ViewModels for forms and dashboards
├── Services/          # Menu builder, URL encryption
├── TagHelpers/        # Custom Razor tag helpers (er-input, er-button, etc.)
├── ViewComponents/    # Sidebar component
├── Views/             # Razor views organized by module
├── wwwroot/           # Static assets (CSS, JS, images)
├── Program.cs         # App entry point, service config, and DB seeding
└── appsettings.Example.json  # Template config (safe to commit)
```

## Guides
- [Guide.md](Guide.md) — Project overview
- [DeveloperGuide.md](DeveloperGuide.md) — How to work on this codebase
- [UserGuide.md](UserGuide.md) — End-user instructions
- [ScalabilityGuide.md](ScalabilityGuide.md) — Scaling considerations

## Deployment
See the [Developer Guide](DeveloperGuide.md#deployment) for production deployment steps.

## License
This project is proprietary. All rights reserved.
