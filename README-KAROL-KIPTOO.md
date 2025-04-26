
# KejaHUnT

# 🏠 Tenant Property Platform

This microservice manages tenants in a multi-tenant property management system. Each landlord has isolated data, ensuring secure and scalable tenant tracking per property.

## ⚙️ Tech Stack

- ASP.NET Core 8 (Web API)
- Entity Framework Core
- SQL Server / PostgreSQL
- RESTful APIs
- Multi-Tenant architecture 

## 📦 Features

- Add, view, update & delete tenants
- Link tenants to specific properties
- Isolated tenant data 
- Clean architecture (Repositories, DTOs, Services)
- JWT or Cookie-based authentication 

## 🧪 Sample Model

```csharp
public class Tenant {
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public int PropertyId { get; set; }
    public string SaasTenantId { get; set; } // Ensures isolation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

