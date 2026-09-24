# FileProcessing Web App — Deployment Configuration

This document lists the configuration that must be checked/set when deploying the application to another machine or production environment.

> **Security:** Never commit real passwords, API keys, JWT secrets, SMTP passwords, Redis credentials, or production connection strings to GitHub.

## 1. Database Configuration

| Configuration | Development Value | Production Value | Environment Variable / Secret Key | Required |
|---|---|---|---|---|
| Main Database | `localhost / FileProcessing_Web_APP` | Production SQL Server database | `ConnectionStrings__DefaultConnection` | Yes |
| Log Database | Same SQL Server/database in current setup | Production SQL Server database | `LogConnectionString` | Yes |
| Database Encryption | `Encrypt=True` | `Encrypt=True` | Part of connection string | Yes |
| Trusted Certificate | `TrustServerCertificate=True` (development) | Prefer proper trusted certificate; avoid bypass where possible | Part of connection string | Production-dependent |

**Current development connection string:**

```text
Data Source=localhost;Integrated Security=True;Persist Security Info=False;Server=MONTU-KHARB-DES;Encrypt=True;TrustServerCertificate=True;Initial Catalog=FileProcessing_Web_APP
```

**Production:** replace the server, authentication, database name, and other machine-specific settings with production values.

---

## 2. Redis / Distributed Cache

| Configuration | Development | Production | Environment Variable / Secret Key | Required |
|---|---|---|---|---|
| Redis provider | Local Docker Redis or another local Redis instance | Managed Redis service / Redis server | `ConnectionStrings__Redis` | If Redis is enabled |
| Redis connection | Local connection | Managed Redis connection string | `ConnectionStrings__Redis` | If Redis is enabled |
| Redis credentials | Local credentials if configured | Production Redis credentials | `ConnectionStrings__Redis` or secret manager | If required |
| Cache instance name | `FileProcessing:` or project-defined value | Same or environment-specific value | Application configuration | Optional |

**Important:** The real Redis connection string must not be committed to GitHub.

Example secret value:

```text
rediss://default:PASSWORD@xxxxx.upstash.io:6379
```

For ASP.NET Core environment variables, use:

```text
ConnectionStrings__Redis
```

---

## 3. Email / SMTP Configuration

| Configuration | Development Value | Production Value | Environment Variable / Secret Key | Required |
|---|---|---|---|---|
| SMTP Host | `smtp.gmail.com` | SMTP provider host | `EmailSettings__Host` | Yes |
| SMTP Port | `587` | Provider port | `EmailSettings__Port` | Yes |
| Display Name | `Montu kharb` | Application/company display name | `EmailSettings__DisplayName` | Yes |
| SSL / TLS | `true` | `true` / provider requirement | `EmailSettings__UseSSL` | Yes |
| SMTP Username | Not present in shared config | SMTP account username/email | `EmailSettings__Username` | Yes |
| SMTP Password / App Password | Secret | SMTP password/app password | `EmailSettings__Password` | Yes |

**Do not put the Gmail password or App Password in `appsettings.json` or GitHub.**

---

## 4. Application Information

| Configuration | Current Value | Production Value | Environment Variable / Config Key | Required |
|---|---|---|---|---|
| Name | `FileProcessing Web App` | Usually unchanged | `Application__Name` | Yes |
| Version | `1.0.0` | Release version | `Application__Version` | Yes |
| Author | `Montu` | Usually unchanged | `Application__Author` | Optional |
| Description | `Web application for file processing.` | Usually unchanged | `Application__Description` | Optional |
| Copyright | `Copyright © 2026 Montu` | Update as required | `Application__Copyright` | Optional |
| Website | `https://github.com/Montu-Kharb-DES` | Production website/GitHub URL | `Application__Website` | Optional |

---

## 5. Application Base URL

| Configuration | Development Value | Production Value | Environment Variable / Config Key | Required |
|---|---|---|---|---|
| Scheme | `http` | Usually `https` | `App__scheme` | Yes |
| Host | `//localhost:5009/` | Production host/domain | `App__host` | Yes |
| Full Base URL | `http://localhost:5009/` | Example: `https://example.com/` | Derived from scheme + host | Yes |

Example production values:

```text
App__scheme=https
App__host=//example.com/
```

If the application is behind a reverse proxy/load balancer, verify forwarded headers so generated URLs use the external HTTPS scheme/host correctly.

---

## 6. Logging Configuration

| Configuration | Development Value | Production Value | Environment Variable / Config Key | Required |
|---|---|---|---|---|
| Default Log Level | `Information` | `Information` / `Warning` as required | `Logging__LogLevel__Default` | Yes |
| ASP.NET Core Log Level | `Warning` | `Warning` | `Logging__LogLevel__Microsoft.AspNetCore` | Yes |
| Log Storage | Application/database as configured | Production logging destination | Project-specific | Yes |

Current configuration:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

---

## 7. JWT / Authentication Secrets

> Add these entries when JWT authentication/authorization is enabled in the deployed application.

| Configuration | Development | Production | Environment Variable / Secret Key | Required |
|---|---|---|---|---|
| JWT Key | User Secret | Strong production secret | `Jwt__Key` | Yes |
| JWT Issuer | Configured value | Production issuer | `Jwt__Issuer` | Yes |
| JWT Audience | Configured value | Production audience | `Jwt__Audience` | Yes |
| Token Expiration | Project-defined | Production policy | `Jwt__ExpirationMinutes` | As configured |

Never commit the production JWT signing key.

---

## 8. CORS / Frontend Configuration

| Configuration | Development | Production | Environment Variable / Config Key | Required |
|---|---|---|---|---|
| Allowed Frontend Origin | Local Angular URL | Production frontend URL | Project-specific | If CORS is enabled |
| Credentials | Project-defined | Project-defined | Project-specific | If required |
| Allowed Methods | Project-defined | Project-defined | Project-specific | If configured |
| Allowed Headers | Project-defined | Project-defined | Project-specific | If configured |

Example:

```text
https://example.com
```

Do not use `AllowAnyOrigin()` in production unless the application intentionally requires it.

---

## 9. File Storage / Upload Configuration

| Configuration | Development | Production | Environment Variable / Config Key | Required |
|---|---|---|---|---|
| Upload Directory | Local project/server directory | Persistent production storage | Project-specific | Yes |
| Maximum File Size | Project-defined | Production limit | Project-specific | Yes |
| Allowed File Types | Project-defined | Production policy | Project-specific | Yes |
| Persistent Storage | Local disk | Persistent volume / object storage | Project-specific | Yes |

For Docker deployment, ensure uploaded files are not lost when a container is recreated. Use a persistent volume or external/object storage where appropriate.

---

## 10. Docker / Container Configuration

| Configuration | Development | Production | Required |
|---|---|---|---|
| ASP.NET image | Development image | Published production image | Yes |
| Environment | `Development` | `Production` | Yes |
| Exposed HTTP/HTTPS port | Project-defined | Deployment platform/proxy port | Yes |
| Environment variables | Local `.env` / User Secrets | Deployment secrets/environment variables | Yes |
| Volumes | Optional | Persistent data where required | As needed |
| Redis container | Optional for local development | Not required when using managed Redis | As needed |

---

## 11. Production Secrets Checklist

| Secret / Sensitive Value | Where to Store | Commit to GitHub? |
|---|---|---|
| SQL Server password/connection string | Secret manager / deployment environment | **No** |
| Redis connection string/password | Secret manager / deployment environment | **No** |
| SMTP password/App Password | Secret manager / deployment environment | **No** |
| JWT signing key | Secret manager / deployment environment | **No** |
| API keys | Secret manager / deployment environment | **No** |
| OAuth client secrets | Secret manager / deployment environment | **No** |

---

## 12. Local Development Secrets

Each developer/machine must configure its own User Secrets. User Secrets are **not shared through GitHub**.

Example:

```bash
dotnet user-secrets init

dotnet user-secrets set "ConnectionStrings:Redis" "YOUR_REDIS_CONNECTION_STRING"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "Jwt:Key" "YOUR_JWT_SECRET"
dotnet user-secrets set "EmailSettings:Username" "YOUR_EMAIL"
dotnet user-secrets set "EmailSettings:Password" "YOUR_EMAIL_PASSWORD"
```

Check:

```bash
dotnet user-secrets list
```

---

## 13. Production Deployment Checklist

| Step | Check |
|---|---|
| 1 | Set `ASPNETCORE_ENVIRONMENT=Production` |
| 2 | Configure `ConnectionStrings__DefaultConnection` |
| 3 | Configure `LogConnectionString` |
| 4 | Configure Redis using `ConnectionStrings__Redis` if Redis is enabled |
| 5 | Configure SMTP host/port/username/password |
| 6 | Configure `App__scheme` and `App__host` |
| 7 | Configure JWT secrets if authentication is enabled |
| 8 | Configure production CORS/frontend origin |
| 9 | Configure persistent file storage |
| 10 | Configure Docker/container ports and volumes |
| 11 | Verify HTTPS and reverse proxy/forwarded headers |
| 12 | Verify database connectivity |
| 13 | Verify Redis connectivity |
| 14 | Verify signup/email verification flow |
| 15 | Verify logs and error monitoring |
| 16 | Confirm no real secrets are committed to GitHub |

---

## 14. Safe Configuration Pattern

`appsettings.json` should contain only non-secret defaults/placeholders. Secrets should come from User Secrets in development and environment variables/secret management in production.

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": ""
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "DisplayName": "Montu kharb",
    "UseSSL": true
  },
  "App": {
    "scheme": "http",
    "host": "//localhost:5009/"
  },
  "Cache": {
    "Redis": ""
  }
}
```

Production values should be injected through the hosting platform, container environment, or a secret manager rather than hard-coded into the repository.
