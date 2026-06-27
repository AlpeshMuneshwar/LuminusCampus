# Developer Guide

Quick reference for anyone working on LuminusCampus.

## Project Layout
```
webappi/
├── Controllers/          # Grouped by module code (SIS/, ALM/, STAFF/, FEE/, ADMIN/)
│   ├── AccountController.cs        # Login, logout, access denied
│   ├── SuperAdminController.cs     # Client/module/page management
│   └── SessionController.cs        # Session log viewer
├── Data/                 # All EF entities and DbContext
├── Middleware/
│   ├── DynamicAuthorizationMiddleware.cs  # Checks page-level permissions per request
│   ├── SubscriptionMiddleware.cs          # Verifies module subscription
│   ├── UrlObfuscationMiddleware.cs        # Decodes token URLs to real paths
│   └── SessionTrackingMiddleware.cs       # Tracks active sessions
├── Services/
│   ├── Core/MenuService.cs          # Builds the sidebar menu per user role
│   └── Core/UrlEncryptionService.cs # Encrypts/decrypts URL tokens
├── TagHelpers/           # Custom Razor components (er-input, er-button, er-card, etc.)
├── Views/                # Razor views, organized by module
├── wwwroot/              # CSS, JS, images
└── Program.cs            # Entry point — services, middleware pipeline, DB seeding
```

## Local Setup

### Prerequisites
- .NET 8 SDK
- MySQL Server running locally

### Steps
1. Clone the repo and navigate to the project:
   ```bash
   git clone https://github.com/AlpeshMuneshwar/LuminusCampus.git
   cd LuminusCampus/webappi
   ```

2. Create your config from the example:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```

3. Edit `appsettings.json` with your DB credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=LuminusCampus;User=root;Password=YOUR_PASSWORD;"
     }
   }
   ```

4. Run the app:
   ```bash
   dotnet run
   ```
   On first run, EF migrations create all tables and seed default data (roles, modules, pages, SuperAdmin account).

5. Open `http://localhost:5000` and log in with `superadmin@erp.com` / `Admin@123`.

## Key Concepts

### Permissions flow
Every request goes through this pipeline:
1. `UrlObfuscationMiddleware` — converts token URL to real path
2. `UseAuthentication` / `UseAuthorization` — standard Identity checks
3. `SubscriptionMiddleware` — verifies the user's org has the module activated
4. `DynamicAuthorizationMiddleware` — checks `ErpUserPageRights` for page-level access

### Adding a new page
1. Create the Controller action and View
2. Insert a row into `ErpPages` (or add it in the seed data in `Program.cs`)
3. The page will automatically appear in the menu for users with the right permissions

### Adding a new module
1. Add the module to `ErpModules` seed data
2. Create a folder under `Controllers/` with the module code
3. Add corresponding views under `Views/`
4. Add pages to `ErpPages` linked to the new module

## Deployment

### Build for production
```bash
dotnet publish -c Release -o ./publish
```

### Run on a server
1. Copy the `publish/` folder to your server
2. Create `appsettings.json` on the server with production DB credentials
3. Run:
   ```bash
   dotnet webappi.dll
   ```
4. Use a reverse proxy (Nginx/Apache) to forward traffic from port 80/443 to the app

### With a systemd service (Linux)
```ini
[Unit]
Description=LuminusCampus ERP
After=network.target

[Service]
WorkingDirectory=/var/www/luminuscampus
ExecStart=/usr/bin/dotnet webappi.dll
Restart=always
RestartSec=10
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
```

### With IIS (Windows)
1. Install the .NET 8 Hosting Bundle
2. Create a new site in IIS pointing to the `publish/` folder
3. Set the Application Pool to "No Managed Code"
4. Place `appsettings.json` in the same folder as the DLL

## Files NOT in the repo (gitignored)
| File | Why |
|------|-----|
| `appsettings.json` | Contains real DB password |
| `appsettings.Development.json` | Local dev overrides |
| `Properties/launchSettings.json` | Local ports and environment |
| `bin/`, `obj/` | Build output |
