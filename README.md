# LuminusCampus

**LuminusCampus** is a secure, multi-module ERP system built with **ASP.NET Core 8.0** and **Entity Framework Core**. It is designed to streamline school and office management through a modern, role-based architecture.

## Key Features
*   **Multi-Module Architecture**: Support for Student Info (SIS), Academics, Finance, Exams, and Staff Management.
*   **Granular RBAC**: Flexible role-based access control with page-level permissions.
*   **Privacy-First Navigation**: URL obfuscation and token-based routing.
*   **Audit & Session Tracking**: Built-in monitoring for user sessions and security logging.
*   **Clean Code**: Human-written, optimized codebase with minimal documentation.

## Getting Started
1.  Clone the repository.
2.  Rename `webappi/appsettings.Example.json` to `webappi/appsettings.json`.
3.  Update the `DefaultConnection` string with your MySQL credentials.
4.  Run `dotnet run`.

For more details, see the included guides:
*   [Guide.md](Guide.md)
*   [DeveloperGuide.md](DeveloperGuide.md)
*   [UserGuide.md](UserGuide.md)
*   [ScalabilityGuide.md](ScalabilityGuide.md)
