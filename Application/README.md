# File-Processing-Web-App
# File Processing & Conversion System

## 1. Project Overview

**Project Name:** File Processing & Conversion System

**Goal:** Build an industry-ready document and file processing platform
similar in capability to online PDF/file tools such as iLovePDF.

The system will support:

-   PDF manipulation
-   PDF conversion
-   Office document conversion
-   Image processing
-   OCR
-   File upload/download
-   Background processing
-   Temporary file lifecycle
-   Cloud object storage
-   Authentication and user history
-   Scalable worker processing

### Core Technology

  Layer                          Technology
  ------------------------------ --------------------------------
  Frontend                       Angular
  Backend                        ASP.NET Core Web API / .NET 10
  Architecture                   Clean Architecture
  Database                       SQL Server
  ORM                            Entity Framework Core
  Cache                          Redis
  Background Jobs                Hangfire
  Object Storage                 Azure Blob Storage / Amazon S3
  Local Dev Storage              MinIO
  PDF Generation                 QuestPDF
  PDF Reading/Extraction         PdfPig
  PDF Compression/Optimization   Ghostscript
  Office → PDF                   LibreOffice
  Image Processing               ImageSharp
  Advanced Image Processing      Magick.NET
  OCR                            Tesseract
  Logging                        Serilog
  Validation                     FluentValidation
  Testing                        xUnit + integration tests
  API Documentation              OpenAPI / Swagger
  Containerization               Docker + Docker Compose
  Reverse Proxy                  Nginx

------------------------------------------------------------------------

# 2. Architectural Goal

The application should not be designed as a collection of controllers
that directly call third-party libraries.

The architecture should be:

``` text
Angular
   |
   v
ASP.NET Core API
   |
   v
Application
   |
   +------------------+
   |                  |
   v                  v
Domain           Infrastructure
                      |
        +-------------+-------------+
        |             |             |
        v             v             v
    PDF Engine    Office Engine   Image/OCR
        |             |             |
        +-------------+-------------+
                      |
                      v
               Object Storage
                      |
                      v
                  SQL Server
```

The main principle is:

> **Application defines what the system needs. Infrastructure defines
> how external libraries/tools perform it.**

For example:

``` text
Application
    |
    +-- IPdfCompressionService
              ^
              |
Infrastructure
    |
    +-- GhostscriptPdfCompressionService
```

The Application layer must not depend directly on Ghostscript.

------------------------------------------------------------------------

# 3. Solution Structure

The system follows a **Modular Monolith architecture**.

The application is deployed as one backend application initially, but the business capabilities are isolated into independent modules. Each module owns its own Domain, Application and module-specific Infrastructure.

Cross-cutting technical capabilities such as database access, object storage, Redis, Hangfire and logging are placed in **Shared Infrastructure**.

EF Core migrations are maintained in a **separate Migration project**.

```text
FileProcessingSystem/
│
├── application/
│   │
│   ├── API/
│   │   └── FileProcessingSystem.API/
│   │
│   ├── Modules/
│   │   │
│   │   ├── FileManagement/
│   │   │   ├── Domain/
│   │   │   │   ├── Entities/
│   │   │   │   ├── Enums/
│   │   │   │   └── ValueObjects/
│   │   │   │
│   │   │   ├── Application/
│   │   │   │   ├── Abstractions/
│   │   │   │   ├── Features/
│   │   │   │   ├── DTOs/
│   │   │   │   └── Validators/
│   │   │   │
│   │   │   └── Infrastructure/
│   │   │       ├── Persistence/
│   │   │       └── Services/
│   │   │
│   │   ├── PDF/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   │   ├── Abstractions/
│   │   │   │   ├── Features/
│   │   │   │   ├── DTOs/
│   │   │   │   └── Validators/
│   │   │   └── Infrastructure/
│   │   │       ├── PdfPig/
│   │   │       ├── QuestPDF/
│   │   │       └── Ghostscript/
│   │   │
│   │   ├── Conversion/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   │   ├── Abstractions/
│   │   │   │   ├── Features/
│   │   │   │   ├── DTOs/
│   │   │   │   └── Validators/
│   │   │   └── Infrastructure/
│   │   │       └── LibreOffice/
│   │   │
│   │   ├── ImageProcessing/
│   │   │   ├── Domain/
│   │   │   ├── Application/
│   │   │   │   ├── Abstractions/
│   │   │   │   ├── Features/
│   │   │   │   ├── DTOs/
│   │   │   │   └── Validators/
│   │   │   └── Infrastructure/
│   │   │       ├── ImageSharp/
│   │   │       └── MagickNET/
│   │   │
│   │   └── OCR/
│   │       ├── Domain/
│   │       ├── Application/
│   │       │   ├── Abstractions/
│   │       │   ├── Features/
│   │       │   ├── DTOs/
│   │       │   └── Validators/
│   │       └── Infrastructure/
│   │           └── Tesseract/
│   │
│   ├── Shared/
│   │   │
│   │   ├── CommonLibrary/
│   │   │   ├── Results/
│   │   │   ├── Exceptions/
│   │   │   ├── Extensions/
│   │   │   ├── Constants/
│   │   │   └── Helpers/
│   │   │
│   │   └── Infrastructure/
│   │       ├── Persistence/
│   │       ├── Storage/
│   │       │   ├── Local/
│   │       │   ├── MinIO/
│   │       │   ├── AzureBlob/
│   │       │   └── S3/
│   │       ├── Redis/
│   │       ├── Hangfire/
│   │       ├── Logging/
│   │       └── Security/
│   │
│   └── Migration/
│       └── FileProcessingSystem.Migration/
│           ├── Migrations/
│           └── Program.cs
│
├── frontend/
│   └── file-processing-system-ui/
│
├── tests/
│   ├── FileProcessingSystem.UnitTests/
│   └── FileProcessingSystem.IntegrationTests/
│
├── docker/
│   ├── api/
│   ├── worker/
│   └── nginx/
│
├── docs/
│   ├── architecture/
│   ├── api/
│   └── deployment/
│
├── docker-compose.yml
├── Directory.Build.props
├── Directory.Packages.props
└── README.md
```

## 3.1 Why Modular Monolith?

A Modular Monolith gives the project strong module boundaries without the operational complexity of microservices.

```text
                    File Processing System
                             │
                 ┌───────────┴───────────┐
                 │     ASP.NET Core      │
                 │      Application      │
                 └───────────┬───────────┘
                             │
        ┌────────────────────┼────────────────────┐
        │                    │                    │
        ▼                    ▼                    ▼
 File Management           PDF               Conversion
        │                    │                    │
        └──────────────┬─────┴──────────────┬─────┘
                       │                    │
                       ▼                    ▼
                Image Processing          OCR
                       │
                       └──────────┬──────────┘
                                  │
                                  ▼
                       Shared Infrastructure
```

Each module has a clear responsibility and should communicate through application contracts rather than directly accessing another module's internal implementation.

### Benefits

- Easier development than microservices
- One deployment unit
- Simple local development
- Clear business boundaries
- Independent module ownership
- Easier testing
- Easier future extraction into microservices if required
- Shared infrastructure without duplicating technical code

---

# 4. Module Boundaries

## 4.1 File Management Module

Responsibility:

```text
Upload
Download
Delete
File validation
File metadata
File lifecycle
File ownership
Temporary file management
```

Structure:

```text
FileManagement/
├── Domain/
├── Application/
└── Infrastructure/
```

This module owns file-related business rules.

It should not know how Azure Blob, S3 or MinIO works internally.

It uses:

```csharp
IFileStorageService
```

which is provided by Shared Infrastructure.

---

## 4.2 PDF Module

Responsibility:

```text
Merge
Split
Remove pages
Extract pages
Reorder pages
Rotate
Watermark
Page numbers
Protect
Unlock with valid credentials
Compress
Extract text
Render pages
PDF metadata
```

Structure:

```text
PDF/
├── Domain/
├── Application/
│   ├── Abstractions/
│   ├── Features/
│   ├── DTOs/
│   └── Validators/
│
└── Infrastructure/
    ├── PdfPig/
    ├── QuestPDF/
    └── Ghostscript/
```

The PDF module owns the PDF business/use-case logic.

Third-party PDF libraries stay inside:

```text
PDF.Infrastructure
```

---

## 4.3 Conversion Module

Responsibility:

```text
Word → PDF
Excel → PDF
PowerPoint → PDF
PDF → Word
PDF → Excel
PDF → PowerPoint
HTML → PDF
```

Structure:

```text
Conversion/
├── Domain/
├── Application/
└── Infrastructure/
    └── LibreOffice/
```

The Application layer defines:

```csharp
public interface IOfficeToPdfService
{
    Task<Stream> ConvertAsync(
        Stream input,
        string fileName,
        CancellationToken cancellationToken);
}
```

Infrastructure implements it using LibreOffice.

---

## 4.4 Image Processing Module

Responsibility:

```text
Resize
Compress
Crop
Rotate
Convert
Watermark
Thumbnail
Image → PDF
PDF → Image where applicable
```

Structure:

```text
ImageProcessing/
├── Domain/
├── Application/
└── Infrastructure/
    ├── ImageSharp/
    └── MagickNET/
```

Use ImageSharp as the normal .NET image-processing engine.

Use Magick.NET only where advanced format support or processing requirements justify it.

---

## 4.5 OCR Module

Responsibility:

```text
Image → Text
PDF → Text
Scanned PDF → Searchable PDF
OCR processing
Language configuration
OCR result handling
```

Structure:

```text
OCR/
├── Domain/
├── Application/
└── Infrastructure/
    └── Tesseract/
```

The OCR engine itself remains an infrastructure concern.

---

# 5. Shared Infrastructure

Shared Infrastructure contains technical services that are used by multiple modules.

```text
Shared/
└── Infrastructure/
    ├── Persistence/
    ├── Storage/
    ├── Redis/
    ├── Hangfire/
    ├── Logging/
    └── Security/
```

## 5.1 Persistence

Contains:

```text
EF Core
SQL Server
DbContext
Common persistence configuration
Transaction infrastructure
```

The shared persistence layer should provide the technical database infrastructure.

Module-specific entities/configurations should remain associated with their respective module.

Recommended direction:

```text
Module Domain
      ↓
Shared Persistence
      ↓
SQL Server
```

---

## 5.2 Storage

```text
Shared.Infrastructure/
└── Storage/
    ├── Local/
    ├── MinIO/
    ├── AzureBlob/
    └── S3/
```

Common abstraction:

```csharp
public interface IFileStorageService
{
    Task<StoredFileResult> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken);
}
```

Modules use the abstraction.

They do not directly reference:

```text
Azure.Storage.Blobs
Amazon.S3
MinIO SDK
```

---

## 5.3 Redis

Shared Redis infrastructure handles:

```text
Caching
Rate limiting state
Short-lived job state
Distributed coordination
```

It must not be used as the primary storage for uploaded files.

```text
SQL Server
    ↓
Persistent metadata

Redis
    ↓
Temporary/cache state

Object Storage
    ↓
Actual files
```

---

## 5.4 Hangfire

Hangfire is shared because multiple modules can create processing jobs.

```text
PDF Module
     │
Conversion Module
     │
Image Module
     │
OCR Module
     │
     ▼
Shared Hangfire Infrastructure
     │
     ▼
Background Worker
```

Example:

```csharp
BackgroundJob.Enqueue(() =>
    pdfCompressionService.CompressAsync(jobId));
```

The actual processing implementation remains inside the owning module.

---

## 5.5 Logging

Use:

```text
Serilog
```

as shared infrastructure.

Every module should produce structured logs containing useful context such as:

```text
CorrelationId
JobId
UserId
Module
Operation
Duration
Status
Error
```

---

# 6. Separate Migration Project

EF Core migrations should be isolated from the runtime API and Infrastructure projects.

```text
application/
└── Migration/
    └── FileProcessingSystem.Migration/
        ├── Migrations/
        └── Program.cs
```

Purpose:

```text
Runtime application
       ≠
Database migration tooling
```

The migration project references the required Infrastructure and DbContext components but is not part of the API request-processing pipeline.

Typical workflow:

```text
Change Entity
    ↓
Create Migration
    ↓
Migration Project
    ↓
SQL Server
```

Example:

```bash
dotnet ef migrations add InitialCreate \
  --project application/Migration/FileProcessingSystem.Migration \
  --startup-project application/API/FileProcessingSystem.API
```

Database update:

```bash
dotnet ef database update \
  --project application/Migration/FileProcessingSystem.Migration \
  --startup-project application/API/FileProcessingSystem.API
```

---

# 7. Dependency Rules

The modular monolith must enforce dependency direction.

```text
API
 │
 ▼
Application
 │
 ▼
Domain
```

Infrastructure implements Application abstractions:

```text
Application
    ↑
    │
Infrastructure
```

Modules should not depend on another module's Infrastructure.

### Correct

```text
PDF.Application
      ↓
PDF abstraction

PDF.Infrastructure
      ↓
PDF third-party library
```

### Incorrect

```text
PDF.Application
      ↓
Conversion.Infrastructure
      ↓
LibreOffice
```

Instead, cross-module communication should use contracts/application abstractions.

---

# 8. Module Communication

When one module needs another module:

```text
PDF
 │
 ▼
Application Contract
 │
 ▼
Other Module
```

Do not access:

```text
OtherModule.Infrastructure
OtherModule.DbContext
OtherModule.Repository
OtherModule.InternalService
```

Example:

```text
Conversion Module
       |
       | needs file
       v
IFileStorageService
       |
       v
Shared Infrastructure
       |
       v
Azure Blob / S3 / MinIO
```

---

# 9. Backend Request Flow

For a PDF compression request:

```text
Angular
   |
   | POST /api/pdf/compress
   v
API Controller
   |
   v
PDF Application
   |
   v
PDF Compression Use Case
   |
   +------> File Management / Storage abstraction
   |
   +------> Shared Hangfire
   |
   v
PDF Infrastructure
   |
   v
Ghostscript
   |
   v
Output Stream
   |
   v
Shared Storage
   |
   v
Azure Blob / S3 / MinIO
   |
   v
SQL Server metadata
```

The API never directly invokes Ghostscript.

---

# 10. Frontend Architecture

Angular remains a separate frontend application:

```text
frontend/
└── file-processing-system-ui/
```

Recommended:

```text
src/app/
├── core/
│   ├── auth/
│   ├── guards/
│   ├── interceptors/
│   ├── services/
│   └── models/
│
├── shared/
│   ├── components/
│   ├── directives/
│   ├── pipes/
│   └── models/
│
├── features/
│   ├── home/
│   ├── file-management/
│   ├── pdf/
│   │   ├── merge/
│   │   ├── split/
│   │   ├── compress/
│   │   ├── rotate/
│   │   └── watermark/
│   │
│   ├── conversion/
│   ├── images/
│   ├── ocr/
│   ├── jobs/
│   └── account/
│
├── app.routes.ts
└── app.config.ts
```

Angular feature boundaries should mirror backend business modules where practical.

```text
Angular PDF Feature
        ↓
PDF API
        ↓
PDF Module
```

---

# 11. Database Architecture

Use a single SQL Server database initially.

```text
SQL Server
    |
    +-- File Management tables
    +-- PDF/processing tables
    +-- Conversion/job tables
    +-- User/account tables
    +-- Audit tables
```

Recommended core tables:

```text
Users
Files
FileVersions
StorageObjects
ProcessingJobs
ProcessingJobFiles
UserOperationHistory
AuditLogs
```

The database stores metadata.

Large binary files are stored in object storage.

```text
SQL Server
-------------------------
FileId
UserId
StorageKey
FileName
Size
ContentType
Status
CreatedAt
ExpiresAt

        +

Object Storage
-------------------------
actual PDF/DOCX/XLSX/JPG/etc.
```

---

# 12. Cloud Storage Strategy

## Local Development

```text
MinIO
```

## Production

Choose one managed provider:

```text
Azure Blob Storage
```

or:

```text
Amazon S3
```

Recommended Azure-oriented deployment:

```text
Angular
   ↓
Azure Static Web Apps / CDN
   ↓
ASP.NET Core API
   ↓
Background Workers
   ↓
Azure Blob Storage
   ↓
Azure SQL
   ↓
Redis
```

The storage abstraction allows the provider to change without changing module business logic.

---

# 13. Final Modular Monolith Architecture

```text
┌─────────────────────────────────────────────────────────────┐
│                       Angular Frontend                      │
│                                                             │
│ File | PDF | Conversion | Image | OCR | Jobs | Account      │
└─────────────────────────────┬───────────────────────────────┘
                              │ HTTPS
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  ASP.NET Core API                           │
│                                                             │
│ Controllers | Auth | Middleware | OpenAPI | DI              │
└─────────────────────────────┬───────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    MODULAR MONOLITH                         │
│                                                             │
│  ┌────────────┐  ┌────────┐  ┌────────────┐                 │
│  │   File     │  │  PDF   │  │ Conversion │                 │
│  │ Management │  │ Module │  │   Module   │                 │
│  └────────────┘  └────────┘  └────────────┘                 │
│                                                             │
│  ┌────────────────┐              ┌────────────┐             │
│  │ Image          │              │    OCR     │             │
│  │ Processing     │              │   Module   │             │
│  └────────────────┘              └────────────┘             │
└─────────────────────────────┬───────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  SHARED INFRASTRUCTURE                      │
│                                                             │
│ EF Core | Storage | Redis | Hangfire | Serilog | Security   │
└──────────────┬────────────────┬────────────────┬────────────┘
               │                │                │
               ▼                ▼                ▼
          SQL Server       Redis/Queue       Blob/S3/MinIO
               │                                 │
               │                                 │
               │                           Actual Files
               │
          Metadata/State
```

## Core principle

```text
                    MODULAR MONOLITH
                          │
          ┌───────────────┼────────────────┐
          │               │                │
          ▼               ▼                ▼
       Modules      Shared Infrastructure  Migration
          │               │                │
          │               │                └── Separate project
          │               │
          │               ├── SQL Server
          │               ├── Storage
          │               ├── Redis
          │               ├── Hangfire
          │               └── Serilog
          │
          ├── File Management
          ├── PDF
          ├── Conversion
          ├── Image Processing
          └── OCR
```

This structure keeps the application as a **single deployable monolith today**, while maintaining clear module boundaries so individual modules can be extracted into separate services later if scale or organizational requirements justify microservices.


# 4. Backend Project Responsibilities

## 4.1 API

Project:

``` text
FileProcessingSystem.API
```

Responsibilities:

-   HTTP endpoints
-   Controllers
-   Authentication configuration
-   Middleware
-   Exception handling
-   API versioning
-   Swagger/OpenAPI
-   Dependency Injection registration
-   Request/response models where appropriate

Example:

``` text
API/
├── Controllers/
│   ├── FilesController.cs
│   ├── PdfController.cs
│   ├── ConversionController.cs
│   ├── ImageController.cs
│   ├── OcrController.cs
│   └── JobsController.cs
│
├── Middleware/
│   ├── ExceptionHandlingMiddleware.cs
│   └── CorrelationIdMiddleware.cs
│
├── Extensions/
│   ├── ServiceCollectionExtensions.cs
│   └── ApplicationBuilderExtensions.cs
│
└── Program.cs
```

Controller example:

``` csharp
[ApiController]
[Route("api/pdf")]
public sealed class PdfController : ControllerBase
{
    private readonly IPdfMergeService _pdfMergeService;

    public PdfController(IPdfMergeService pdfMergeService)
    {
        _pdfMergeService = pdfMergeService;
    }

    [HttpPost("merge")]
    public async Task<IActionResult> Merge(
        [FromForm] List<IFormFile> files,
        CancellationToken cancellationToken)
    {
        var result = await _pdfMergeService
            .MergeAsync(files, cancellationToken);

        return File(
            result.Content,
            result.ContentType,
            result.FileName);
    }
}
```

The controller should remain thin.

------------------------------------------------------------------------

# 5. Domain Project

Project:

``` text
FileProcessingSystem.Domain
```

Domain contains business concepts that should not depend on ASP.NET, EF
Core, Redis, cloud SDKs or PDF libraries.

Recommended:

``` text
Domain/
├── Entities/
│   ├── FileRecord.cs
│   ├── ProcessingJob.cs
│   ├── ProcessingOperation.cs
│   ├── UserFile.cs
│   └── StoredFile.cs
│
├── Enums/
│   ├── FileStatus.cs
│   ├── JobStatus.cs
│   ├── FileOperationType.cs
│   └── StorageProvider.cs
│
├── ValueObjects/
│   ├── FileSize.cs
│   └── FileIdentifier.cs
│
└── Constants/
```

Example:

``` csharp
public enum ProcessingJobStatus
{
    Queued,
    Processing,
    Completed,
    Failed,
    Cancelled
}
```

------------------------------------------------------------------------

# 6. Application Project

Project:

``` text
FileProcessingSystem.Application
```

This is the main business/use-case layer.

Recommended structure:

``` text
Application/
├── Abstractions/
│   ├── Storage/
│   │   └── IFileStorageService.cs
│   │
│   ├── PDF/
│   │   ├── IPdfMergeService.cs
│   │   ├── IPdfSplitService.cs
│   │   ├── IPdfCompressionService.cs
│   │   ├── IPdfPageService.cs
│   │   ├── IPdfWatermarkService.cs
│   │   ├── IPdfProtectionService.cs
│   │   ├── IPdfRenderingService.cs
│   │   └── IPdfTextExtractionService.cs
│   │
│   ├── Conversion/
│   │   ├── IOfficeToPdfService.cs
│   │   ├── IPdfToWordService.cs
│   │   ├── IPdfToExcelService.cs
│   │   └── IPdfToPowerPointService.cs
│   │
│   ├── Image/
│   │   └── IImageProcessingService.cs
│   │
│   ├── OCR/
│   │   └── IOcrService.cs
│   │
│   └── Jobs/
│       └── IProcessingJobService.cs
│
├── Features/
│   ├── Files/
│   ├── PDF/
│   ├── Conversion/
│   ├── Images/
│   ├── OCR/
│   └── Jobs/
│
├── DTOs/
├── Validators/
└── DependencyInjection.cs
```

------------------------------------------------------------------------

# 7. CommonLibrary

Project:

``` text
FileProcessingSystem.CommonLibrary
```

This project should contain genuinely shared technical utilities.

Recommended:

``` text
CommonLibrary/
├── Results/
├── Exceptions/
├── Extensions/
├── Constants/
├── Helpers/
├── Security/
├── Serialization/
└── Time/
```

Examples:

``` text
Result<T>
Error
PagedResult<T>
DateTime extensions
String extensions
File-name sanitization
Common exception types
```

Do not turn CommonLibrary into a dumping ground.

------------------------------------------------------------------------

# 8. Infrastructure

Project:

``` text
FileProcessingSystem.Infrastructure
```

This project contains all external implementations.

``` text
Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs
│   ├── Configurations/
│   └── Repositories/
│
├── Storage/
│   ├── Local/
│   ├── AzureBlob/
│   ├── S3/
│   └── Minio/
│
├── PDF/
│   ├── PdfPig/
│   ├── QuestPdf/
│   └── Ghostscript/
│
├── Office/
│   └── LibreOffice/
│
├── Images/
│   ├── ImageSharp/
│   └── MagickNet/
│
├── OCR/
│   └── Tesseract/
│
├── BackgroundJobs/
│   └── Hangfire/
│
├── Caching/
│   └── Redis/
│
├── Logging/
│   └── Serilog/
│
└── DependencyInjection.cs
```

------------------------------------------------------------------------

# 9. Migration Project

Project:

``` text
FileProcessingSystem.Infrastructure.Migration
```

Keep EF Core migrations separate from the runtime Infrastructure
project.

``` text
Migration/
├── Migrations/
│   ├── 202608290001_InitialCreate.cs
│   ├── 202608290001_InitialCreate.Designer.cs
│   └── AppDbContextModelSnapshot.cs
│
├── Program.cs
└── FileProcessingSystem.Infrastructure.Migration.csproj
```

This follows the existing project approach where the migration project
is under:

``` text
Infrastructure/
└── Migration/
```

------------------------------------------------------------------------

# 10. Frontend --- Angular

Project:

``` text
frontend/file-processing-system-ui
```

Recommended Angular structure:

``` text
src/
├── app/
│   │
│   ├── core/
│   │   ├── auth/
│   │   ├── guards/
│   │   ├── interceptors/
│   │   ├── services/
│   │   └── models/
│   │
│   ├── shared/
│   │   ├── components/
│   │   ├── directives/
│   │   ├── pipes/
│   │   └── models/
│   │
│   ├── features/
│   │   ├── home/
│   │   ├── pdf/
│   │   │   ├── merge/
│   │   │   ├── split/
│   │   │   ├── compress/
│   │   │   ├── rotate/
│   │   │   ├── watermark/
│   │   │   └── protect/
│   │   │
│   │   ├── conversion/
│   │   ├── images/
│   │   ├── ocr/
│   │   ├── jobs/
│   │   └── account/
│   │
│   ├── app.routes.ts
│   └── app.config.ts
│
└── assets/
```

Angular should communicate only with the ASP.NET API.

``` text
Angular Service
      |
      v
HTTP API
      |
      v
ASP.NET Controller
```

------------------------------------------------------------------------

# 11. File Processing Pipeline

The standard pipeline should be:

``` text
User
  |
  v
Angular
  |
  | multipart/form-data
  v
ASP.NET API
  |
  v
File Validation
  |
  +---- Invalid ---> 400
  |
  v
Object Storage
  |
  v
Create Processing Job
  |
  v
Hangfire
  |
  v
Worker
  |
  v
Processing Engine
  |
  v
Output Object Storage
  |
  v
Update Job
  |
  v
Angular
  |
  v
Download
```

------------------------------------------------------------------------

# 12. File Storage Architecture

Do not couple the application to a physical disk.

Create:

``` csharp
public interface IFileStorageService
{
    Task<StoredFileResult> UploadAsync(
        Stream stream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(
        string storageKey,
        CancellationToken cancellationToken);

    Task DeleteAsync(
        string storageKey,
        CancellationToken cancellationToken);
}
```

Implementations:

``` text
IFileStorageService
        |
        +-- LocalFileStorageService
        +-- MinioFileStorageService
        +-- AzureBlobStorageService
        +-- S3FileStorageService
```

## Development

Use:

``` text
MinIO
```

because it provides S3-compatible object storage locally.

## Production

Recommended primary choice:

``` text
Azure Blob Storage
```

or:

``` text
Amazon S3
```

depending on deployment ecosystem.

The Application layer should not know which provider is being used.

------------------------------------------------------------------------

# 13. SQL Server Responsibility

SQL Server stores metadata and application state.

Do not use SQL Server as the default place for large uploaded files.

Recommended database:

``` text
Users
Files
FileVersions
ProcessingJobs
ProcessingJobFiles
Operations
UserOperationHistory
StorageObjects
ApiKeys
RefreshTokens
AuditLogs
```

Example:

``` text
Files
--------------------------------
Id
UserId
OriginalFileName
StoredFileName
ContentType
Extension
SizeInBytes
StorageKey
StorageProvider
Status
CreatedAt
ExpiresAt
```

The actual file:

``` text
Azure Blob / S3 / MinIO
```

The database stores:

``` text
storageKey
```

------------------------------------------------------------------------

# 14. PDF Technology Strategy

## 14.1 PdfPig

Use for:

-   Text extraction
-   Reading PDF structure
-   Page inspection
-   PDF metadata
-   Text analysis

Example:

``` csharp
using UglyToad.PdfPig;

using var document = PdfDocument.Open(stream);

foreach (var page in document.GetPages())
{
    var text = page.Text;
}
```

Do not use PdfPig as the only PDF engine for every operation.

------------------------------------------------------------------------

# 15. QuestPDF

Use for:

-   Creating new PDFs
-   Reports
-   Generated documents
-   Images to PDF
-   Application-generated PDF output

Example:

``` csharp
Document.Create(document =>
{
    document.Page(page =>
    {
        page.Content()
            .Text("Generated PDF");
    });
})
.GeneratePdf(outputStream);
```

Responsibility:

``` text
QuestPDF
    =
PDF Generation
```

Not:

``` text
QuestPDF
    =
Every PDF manipulation operation
```

------------------------------------------------------------------------

# 16. Ghostscript

Use for:

-   PDF compression
-   PDF optimization
-   PDF rendering-related workflows where appropriate
-   PDF compatibility transformations

Architecture:

``` text
IPdfCompressionService
        |
        v
GhostscriptPdfCompressionService
        |
        v
Ghostscript executable
        |
        v
Optimized PDF
```

The Ghostscript process should execute inside a controlled worker
environment.

Never allow arbitrary command-line arguments from the user to reach the
process.

------------------------------------------------------------------------

# 17. LibreOffice

Use for:

``` text
DOC
DOCX
XLS
XLSX
PPT
PPTX
        |
        v
LibreOffice headless
        |
        v
PDF
```

Architecture:

``` text
IOfficeToPdfService
        |
        v
LibreOfficeService
```

Run LibreOffice only in the worker/container environment.

The API should never directly expose operating-system process execution
to the client.

------------------------------------------------------------------------

# 18. ImageSharp

Use ImageSharp as the default .NET image processing library.

Operations:

``` text
Resize
Crop
Rotate
Compress
Format conversion
Thumbnail
Watermark
```

Example:

``` csharp
using var image = await Image.LoadAsync(inputStream);

image.Mutate(x =>
    x.Resize(1200, 0));

await image.SaveAsync(outputStream, cancellationToken);
```

------------------------------------------------------------------------

# 19. Magick.NET

Use when ImageSharp does not provide the required advanced format or
operation.

Typical responsibilities:

``` text
Advanced image formats
Advanced transformations
Image metadata handling
Specialized conversion
```

Do not automatically send every image through both ImageSharp and
Magick.NET.

------------------------------------------------------------------------

# 20. OCR

Recommended engine:

``` text
Tesseract
```

Pipeline:

``` text
Scanned PDF
    |
    v
PDF Renderer
    |
    v
Page Image
    |
    v
Tesseract
    |
    v
Recognized Text
```

For searchable PDF:

``` text
Scanned PDF
    |
    v
Render pages
    |
    v
OCR
    |
    v
Text + original page image
    |
    v
Searchable PDF
```

------------------------------------------------------------------------

# 21. Background Processing

Large operations should not execute inside a normal HTTP request.

Use:

``` text
ASP.NET Core
      |
      v
Create Job
      |
      v
Hangfire
      |
      v
Worker
      |
      v
Processing
```

Example:

``` csharp
BackgroundJob.Enqueue(() =>
    pdfCompressionService.CompressAsync(jobId));
```

Job states:

``` text
Queued
   ↓
Processing
   ↓
Completed
```

Failure:

``` text
Processing
   ↓
Failed
```

The API should return a job identifier for long-running work.

------------------------------------------------------------------------

# 22. Redis

Redis should be used for:

-   Distributed cache
-   Rate limiting state
-   Temporary job progress
-   Short-lived data
-   Distributed coordination where required

Do not store large uploaded PDFs in Redis.

Correct separation:

``` text
SQL Server
    ↓
Persistent metadata

Redis
    ↓
Temporary/cache data

Blob/S3
    ↓
Actual files
```

------------------------------------------------------------------------

# 23. API Design

Recommended endpoints:

``` text
/api/files
/api/jobs

/api/pdf/merge
/api/pdf/split
/api/pdf/compress
/api/pdf/rotate
/api/pdf/watermark
/api/pdf/protect
/api/pdf/unlock
/api/pdf/extract-pages
/api/pdf/remove-pages
/api/pdf/extract-text
/api/pdf/render

/api/conversion/word-to-pdf
/api/conversion/excel-to-pdf
/api/conversion/powerpoint-to-pdf
/api/conversion/pdf-to-word
/api/conversion/pdf-to-excel

/api/images/resize
/api/images/compress
/api/images/convert
/api/images/crop
/api/images/rotate

/api/ocr/image
/api/ocr/pdf
```

------------------------------------------------------------------------

# 24. File Upload Security

Never trust:

``` text
FileName
Extension
Content-Type
```

from the client.

Validate:

``` text
Extension
+
Content-Type
+
Magic bytes/file signature
+
Maximum file size
+
Maximum page count
+
Processing limits
```

Also:

-   Generate server-side storage keys.
-   Sanitize original file names.
-   Never use user-provided paths.
-   Store uploads outside the web root.
-   Never execute uploaded files.
-   Apply request size limits.
-   Use cancellation tokens.
-   Clean temporary files.
-   Scan uploads with an antivirus service when required by deployment
    policy.

------------------------------------------------------------------------

# 25. File Lifecycle

Recommended lifecycle:

``` text
Uploaded
   |
   v
Stored
   |
   v
Queued
   |
   v
Processing
   |
   +---- Failed
   |
   v
Completed
   |
   v
Available for Download
   |
   v
Expired
   |
   v
Deleted
```

Files should have an expiration policy.

Example:

``` text
Anonymous files:
24 hours

Authenticated temporary files:
configurable retention

User-owned permanent files:
until user deletes them
```

Retention should be configurable.

------------------------------------------------------------------------

# 26. Anonymous vs Authenticated Users

Support both.

## Anonymous

``` text
Upload
 ↓
Process
 ↓
Download
 ↓
Automatic expiration
```

No permanent file history is required.

## Authenticated

``` text
Upload
 ↓
Process
 ↓
File History
 ↓
Download
 ↓
Manage Files
```

User ownership must be enforced server-side.

------------------------------------------------------------------------

# 27. Operation Model

Every operation should be represented consistently.

Example:

``` csharp
public enum FileOperationType
{
    MergePdf,
    SplitPdf,
    CompressPdf,
    RotatePdf,
    WatermarkPdf,
    ProtectPdf,
    OfficeToPdf,
    PdfToWord,
    PdfToImage,
    ImageToPdf,
    ResizeImage,
    CompressImage,
    Ocr
}
```

This allows:

``` text
ProcessingJob
      |
      +-- OperationType
      +-- Status
      +-- Progress
      +-- InputFiles
      +-- OutputFiles
      +-- Error
```

------------------------------------------------------------------------

# 28. Processing Job Design

Example:

``` text
ProcessingJobs
--------------------------------
Id
UserId
OperationType
Status
Progress
StartedAt
CompletedAt
ErrorMessage
CreatedAt
```

Input/output relation:

``` text
ProcessingJob
      |
      +-- Input File 1
      +-- Input File 2
      |
      +-- Output File
```

This allows merge/split/conversion operations to use different numbers
of input and output files.

------------------------------------------------------------------------

# 29. Worker Architecture

For production, separate API and workers.

``` text
                  Load Balancer
                       |
             +---------+---------+
             |                   |
             v                   v
          API 1               API 2
             |                   |
             +---------+---------+
                       |
                       v
                    Queue
                       |
              +--------+--------+
              |        |        |
              v        v        v
           Worker1  Worker2  Worker3
              |        |        |
              +--------+--------+
                       |
                       v
                 Object Storage
```

This allows the processing capacity to scale independently from the API.

------------------------------------------------------------------------

# 30. Docker Architecture

Development:

``` text
Docker Compose
│
├── Angular
├── ASP.NET API
├── Worker
├── SQL Server
├── Redis
├── MinIO
├── Hangfire Dashboard
└── Nginx
```

Production:

``` text
Nginx / Cloud Load Balancer
          |
          v
     ASP.NET API
          |
          v
       Queue
          |
          v
       Workers
          |
    +-----+------+
    |            |
    v            v
 SQL Server   Blob/S3
    |
    v
  Redis
```

------------------------------------------------------------------------

# 31. Cloud Recommendation

## Recommended Azure stack

Because the backend is ASP.NET, Azure is a strong production option:

``` text
Angular
   ↓
Azure Static Web Apps / CDN
   ↓
ASP.NET Core API
   ↓
Azure App Service / Container Apps / AKS
   ↓
Queue / Hangfire
   ↓
Worker containers
   ↓
Azure Blob Storage
   ↓
Azure SQL Database
   ↓
Azure Cache for Redis
```

Alternative AWS:

``` text
CloudFront
   ↓
S3 / Angular
   ↓
ECS / EKS
   ↓
SQS
   ↓
Worker
   ↓
S3
   ↓
RDS SQL Server
   ↓
ElastiCache
```

Start with a simpler managed deployment and move to Kubernetes only when
actual scale requires it.

------------------------------------------------------------------------

# 32. Recommended Development Environment

Local:

``` text
Angular
ASP.NET Core
SQL Server
Redis
MinIO
Hangfire
LibreOffice
Ghostscript
Tesseract
Docker Compose
```

Example:

``` text
docker compose up -d
```

The application should be able to run without cloud credentials during
local development.

------------------------------------------------------------------------

# 33. NuGet Package Categories

Do not blindly install every package. Add packages only where the
feature requires them.

### Core

``` text
Microsoft.EntityFrameworkCore
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Design
```

### Validation

``` text
FluentValidation
FluentValidation.DependencyInjectionExtensions
```

### PDF

``` text
PdfPig
QuestPDF
```

Ghostscript and LibreOffice are external executables rather than normal
NuGet-only dependencies.

### Background

``` text
Hangfire.Core
Hangfire.SqlServer
```

### Redis

``` text
StackExchange.Redis
```

or the ASP.NET distributed-cache integration where appropriate.

### Image

``` text
SixLabors.ImageSharp
Magick.NET
```

### OCR

Use an appropriate Tesseract .NET wrapper and keep the native OCR
runtime available inside the worker container.

### Logging

``` text
Serilog.AspNetCore
```

------------------------------------------------------------------------

# 34. Testing Strategy

## Unit Tests

Test:

``` text
Application services
Validators
Business rules
File naming
Job state transitions
Metadata processing
```

Example:

``` text
MergePdfServiceTests
CompressPdfServiceTests
FileValidationServiceTests
ProcessingJobServiceTests
```

## Integration Tests

Test:

``` text
API
SQL Server
Storage
Redis
Background jobs
```

Example:

``` text
POST /api/pdf/merge
      |
      v
Application
      |
      v
Infrastructure
      |
      v
Output file
```

Do not make unit tests depend on real cloud storage.

------------------------------------------------------------------------

# 35. Observability

Use structured logging.

Recommended:

``` text
Serilog
```

Every processing job should have:

``` text
CorrelationId
JobId
UserId
Operation
InputSize
OutputSize
Duration
Status
Error
```

Example:

``` text
JobId=abc123
Operation=CompressPdf
Status=Completed
DurationMs=8420
InputSize=18MB
OutputSize=4MB
```

Production monitoring should include:

``` text
API latency
API error rate
Queue length
Worker utilization
Processing duration
Storage failures
Database failures
Failed jobs
Disk/temp usage
```

------------------------------------------------------------------------

# 36. PDF Feature Roadmap

## Phase 1 --- Foundation

``` text
File Upload
File Download
File Delete
File Validation
Storage abstraction
SQL metadata
Job model
```

## Phase 2 --- Basic PDF

``` text
Merge
Split
Extract Pages
Remove Pages
Reorder Pages
Rotate
```

## Phase 3 --- PDF Optimization

``` text
Compress
Watermark
Page Numbers
Protect
Unlock with valid credentials
```

## Phase 4 --- Image

``` text
JPG → PDF
PDF → JPG
Resize
Compress
Crop
Rotate
Convert
```

## Phase 5 --- Office

``` text
Word → PDF
Excel → PDF
PowerPoint → PDF
```

## Phase 6 --- OCR

``` text
Image → Text
PDF → Text
Scanned PDF → Searchable PDF
```

## Phase 7 --- Advanced

``` text
PDF → Word
PDF → Excel
PDF → PowerPoint
PDF/A
Repair
Redaction
Compare
Forms
Digital signatures
```

------------------------------------------------------------------------

# 37. Important Architecture Rules

### Rule 1

Controllers should not contain processing logic.

### Rule 2

Application should not reference third-party processing libraries
directly.

### Rule 3

Infrastructure owns external tools.

### Rule 4

Large files should use streaming wherever practical.

### Rule 5

Actual files belong in object storage, not normal SQL rows.

### Rule 6

Metadata belongs in SQL Server.

### Rule 7

Redis is not file storage.

### Rule 8

Long-running operations belong in background workers.

### Rule 9

API and Worker should be independently scalable.

### Rule 10

Every processing operation should have a consistent Job model.

### Rule 11

All temporary files require an expiration/cleanup strategy.

### Rule 12

External executables must run with controlled arguments and restricted
permissions.

------------------------------------------------------------------------

# 38. Final Recommended Architecture

``` text
┌──────────────────────────────────────────────────────────────┐
│                         ANGULAR UI                            │
│                                                              │
│  PDF Tools | Conversion | Images | OCR | Account | History  │
└─────────────────────────────┬────────────────────────────────┘
                              │ HTTPS
                              ▼
┌──────────────────────────────────────────────────────────────┐
│                    ASP.NET CORE API                          │
│                                                              │
│ Controllers | Auth | Validation | Middleware | Swagger       │
└─────────────────────────────┬────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────┐
│                     APPLICATION                              │
│                                                              │
│ PDF | Conversion | Images | OCR | Files | Jobs | Business   │
│ Rules + Interfaces                                           │
└──────────────┬─────────────────────────────┬─────────────────┘
               │                             │
               ▼                             ▼
┌──────────────────────────┐      ┌────────────────────────────┐
│      INFRASTRUCTURE      │      │       BACKGROUND           │
│                          │      │          WORKERS            │
│ EF Core                  │      │                            │
│ Blob/S3/MinIO            │      │ Hangfire                   │
│ PdfPig                   │      │ PDF Processing             │
│ QuestPDF                 │      │ Office Conversion           │
│ Ghostscript              │      │ Image Processing            │
│ LibreOffice              │      │ OCR                         │
│ ImageSharp               │      │                            │
│ Magick.NET               │      └──────────────┬─────────────┘
│ Tesseract                │                     │
│ Redis                    │                     │
│ Serilog                  │                     │
└────────────┬─────────────┘                     │
             │                                   │
             ▼                                   ▼
┌──────────────────────┐             ┌─────────────────────────┐
│     SQL SERVER       │             │    OBJECT STORAGE       │
│                      │             │                         │
│ Metadata             │             │ Azure Blob / S3         │
│ Users                │             │ MinIO (local)            │
│ Jobs                 │             │                         │
│ File records         │             │ Actual files             │
│ Audit                │             │                         │
└──────────────────────┘             └─────────────────────────┘
```

------------------------------------------------------------------------

# 39. Target Outcome

The finished project should demonstrate these industry-level concepts:

``` text
Clean Architecture
Dependency Injection
SOLID
Generic abstractions
ASP.NET Core Web API
Angular
EF Core
SQL Server
Object Storage
Cloud Architecture
File Streaming
PDF Processing
Office Conversion
Image Processing
OCR
Background Jobs
Redis
Distributed Processing
Docker
Docker Compose
Logging
Monitoring
Authentication
Authorization
Testing
CI/CD
Cloud Deployment
```

The key design decision is to make the system **engine-independent**:

``` text
Application
    |
    +-- IPdfProcessor
    |
    +-- IFileStorageService
    |
    +-- IOfficeConverter
    |
    +-- IImageProcessor
    |
    +-- IOcrService
    |
    +-- IJobService
             |
             v
       Infrastructure
             |
   +---------+----------+
   |         |          |
 PdfPig   Ghostscript  LibreOffice
 QuestPDF ImageSharp   Tesseract
          Azure Blob    MinIO/S3
```

This makes the project easier to test, replace, scale, containerize, and
deploy to cloud infrastructure.
