# Scalability Guide

If you're planning to scale this up, here are a few things to keep in mind:

### Database
*   **Indexing:** Make sure you've got indexes on `UserId`, `PageId`, and `ModuleId` in the rights/subscription tables.
*   **Caching:** The current menu service hits the DB every time. Adding a simple Redis or memory cache for user permissions would help a lot.

### Code
*   **Separation:** As you add more modules, keep them in separate folders to avoid the controllers getting too messy.
*   **Services:** Try to pull more logic out of the controllers and into specific services.

### Infrastructure
*   **Statelessness:** The app is mostly stateless (outside of standard auth), so you should be able to run it behind a load balancer easily enough.
