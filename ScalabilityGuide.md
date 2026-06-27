# Scalability Guide

Things to consider when LuminusCampus needs to handle more users, more data, or more modules.

## Database

### Indexing
These columns are queried on almost every request — make sure they're indexed:
- `ErpUserPageRights.UserId` + `ErpUserPageRights.PageId` (composite)
- `ErpSubscriptions.UserId` + `ErpSubscriptions.ModuleId` (composite)
- `ErpPages.ModuleId`
- `ErpPages.UrlToken`
- `UserSessions.UserId`

### Connection pooling
The default MySQL connection pool works fine for small setups. If you're hitting 100+ concurrent users, increase `MaxPoolSize` in the connection string:
```
Server=...;Database=...;MaxPoolSize=200;
```

### Read replicas
For heavy reporting or analytics, consider pointing read-only queries to a MySQL replica to keep the primary database responsive.

## Caching

### Menu and permissions
Right now, `MenuService` hits the database on every page load. For better performance:
- Cache user permissions in memory (or Redis) with a short TTL (5–10 minutes)
- Invalidate the cache when an Admin changes permissions
- This alone can cut DB queries by 60–70% under load

### Static assets
Use response caching headers or a CDN for CSS, JS, and images. The `wwwroot/` files don't change often.

## Code structure

### Keep modules isolated
Each module already has its own controller folder (`SIS/`, `ALM/`, `STAFF/`, etc.). As you add more:
- Keep this pattern — don't mix module logic
- Create dedicated services for complex business rules
- Avoid putting too much logic directly in controllers

### Background jobs
If you need scheduled tasks (report generation, data cleanup, email notifications), use a library like Hangfire rather than putting them in the request pipeline.

## Infrastructure

### Reverse proxy
In production, always run behind Nginx or Apache. Let the proxy handle SSL, compression, and static file serving.

### Horizontal scaling
The app is mostly stateless (auth is cookie-based via ASP.NET Identity). To scale horizontally:
- Use a shared session store (Redis) or switch to JWT tokens
- Use a shared data protection key store so cookies work across instances
- Put a load balancer in front

### Monitoring
Add health checks and structured logging early. ASP.NET Core has built-in support:
```csharp
builder.Services.AddHealthChecks()
    .AddMySql(connectionString);
```
Then monitor `/health` from your infrastructure.
