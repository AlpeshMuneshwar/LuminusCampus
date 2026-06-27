# LuminusCampus Overview

LuminusCampus is an ERP platform for educational institutions. It brings student management, academics, finance, staff, and administration under one roof.

### What it does
- Manages student records, attendance, and enrollment
- Handles academic programs, branches, semesters, and academic years
- Tracks staff with departments and designations
- Controls access through a flexible permission system
- Logs every user session for audit purposes

### Tech
- **Framework:** ASP.NET Core 8.0 with Razor Views
- **Database:** MySQL via Entity Framework Core (Pomelo provider)
- **Auth:** ASP.NET Core Identity with custom role hierarchy
- **Frontend:** Bootstrap 5 + custom CSS with light/dark/green themes

### How access works
There are three levels:
1. **SuperAdmin** — full system control, manages clients and modules
2. **Admin** — manages their assigned modules and staff
3. **Staff** — can only access pages explicitly granted to them

Permissions are stored in the `ErpUserPageRights` table. The middleware checks this on every request.

### URL Privacy
Internal URLs are never exposed to the browser. Instead, each page gets a unique token, and the `UrlObfuscationMiddleware` translates tokens back to real paths on the server side.

### Config
All sensitive config (DB credentials, etc.) lives in `appsettings.json`, which is gitignored. Use `appsettings.Example.json` as a template.
