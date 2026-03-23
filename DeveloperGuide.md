# Developer Guide

Hey, here's the quick rundown for working on LuminusCampus.

### Project Layout
*   `Controllers/`: Logic for different modules.
*   `Data/`: Models and the main DB context.
*   `Middleware/`: Custom logic for URL masking and permissions.
*   `Services/`: Helper code for things like menu generation.

### How it works
*   **Permissions:** Handled in `DynamicAuthorizationMiddleware`. It checks the DB to see if a user has access to a specific page.
*   **URLs:** We don't show real URLs. We use tokens. Check `UrlObfuscationMiddleware` to see how it decodes them.
*   **Adding Pages:** If you add a new page, make sure to add it to the `ErpPages` table so it shows up in the menu and permissions work.

### Local Setup
1.  Make sure MySQL is running.
2.  Update the connection string in `Program.cs`.
3.  Run `dotnet run`. The DB will self-seed on the first run.
