# 🛒 retail-compare

A full-stack price tracking system built with **.NET 8 MAUI** and **ASP.NET Core Web API**. The platform monitors retail prices across stores, supports offline-first mobile browsing, and sends push alerts on price drops.

---

## 🏗️ System Architecture
# 🛒 retail-compare

A full-stack price tracking system built with **.NET 8 MAUI** and **ASP.NET Core Web API**. The platform monitors retail prices across stores, supports offline-first mobile browsing, and sends push alerts on price drops.

---

## 🏗️ System Architecture

```text
                                  +-----------------------+
                                  |   Web Scraper Agent   |
                                  +-----------+-----------+
                                              |
                                              | POST /api/ingestion/scrape
                                              v
+-----------------------+         +-----------+-----------+
|   .NET MAUI Mobile    | <-----> |   ASP.NET Core Web API   | <-----> [ PostgreSQL / SQL Server ]
|   (Android / iOS)     |  HTTP   +-----------+-----------+
+-----------+-----------+                     |
            |                                 | Price Drop Event
            | (Push Alerts)                   v
            +-------------------- [ Firebase Cloud Messaging ]
```
## ⚡ Solution Layout

The repository uses a single Visual Studio solution structure:

- **`RetailCompare.API`** *(ASP.NET Core Web API)*: Database migrations, business logic, ingestion pipelines, and REST endpoints.
- **`RetailCompare.App`** *(.NET MAUI)*: Cross-platform mobile app with offline caching and Firebase Cloud Messaging listeners.
- **`RetailCompare.Shared`** *(Class Library)*: Shared Data Transfer Objects (DTOs) used across client and server.

---

## ✨ Core Features

- **📱 .NET MAUI Client:**
  - Real-time connectivity awareness with a top banner on network loss.
  - Offline-first caching powered by `SQLite`.
  - Push notification listeners for routing users to price drop screens.

- **⚡ ASP.NET Core API:**
  - Batch ingestion pipeline for scraped prices.
  - Price drop detection engine comparing current prices against user watchlists.
  - Firebase Cloud Messaging (FCM v1) integration.

---

## 🛠️ Tech Stack

- **Mobile:** .NET 10 MAUI, C#, XAML, AppShell, SQLite, `Plugin.FirebasePushNotifications`
- **Backend:** ASP.NET Core 8, Entity Framework Core
- **Database:** PostgreSQL / SQL Server
- **Messaging:** Firebase Cloud Messaging (FCM v1 API)

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2026 (with MAUI workload enabled)
- Android SDK / Emulator or iOS Simulator

### Quick Start

1. **Clone the repository:**
   git clone [https://github.com/LwandiNxumalo/retail-compare.git](https://github.com/LwandiNxumalo/retail-compare.git)
   cd retail-compare
