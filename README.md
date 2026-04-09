# MillionRecordsProcessor

<div align="center">

**A scalable background processing system built with .NET 10 to handle million+ records using batching, parallel execution, and scheduled jobs via Hangfire.**

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![Hangfire](https://img.shields.io/badge/Hangfire-Background_Jobs-darkgreen?style=flat-square)](https://www.hangfire.io/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-Database-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/en-us/sql-server)
[![React](https://img.shields.io/badge/React-Frontend-61DAFB?style=flat-square&logo=react)](https://react.dev/)
[![Serilog](https://img.shields.io/badge/Serilog-Logging-004880?style=flat-square)](https://serilog.net/)

</div>

---

## Table of Contents

- [Overview](#overview)
- [Processing Flow](#processing-flow)
- [Key Concepts](#key-concepts)
- [Tech Stack](#tech-stack)
- [Getting Started](#getting-started)
- [Important Behavior](#important-behavior)
- [Author](#author)

---

## Overview

**MillionRecordsProcessor** is a production-grade background processing system designed to handle large-scale data workloads with reliability and efficiency. It fetches high-volume data from external sources, processes records in controlled batches with parallelism, persists results into SQL Server, and automatically retries failed records — all orchestrated through Hangfire's job scheduling engine.

### What This System Does

- ✅ Efficiently processes **million+ records** using background jobs, batching, and retry mechanisms
- ✅ Fetches high-volume data from **external API sources**
- ✅ Processes records in **batches with controlled parallelism** (throttling included)
- ✅ Saves results **efficiently into the database**
- ✅ **Retries failed records** independently, without reprocessing successful ones

---

## Processing Flow

The end-to-end data processing pipeline moves through the following stages:

```
                         TRIGGER
                            │
                ┌───────────▼───────────┐
                │      Scheduler        │  ← Cron-based (daily)
                │   Hangfire Server     │    or manual trigger
                └───────────┬───────────┘
                            │
                ┌───────────▼───────────┐
                │     Job Queue         │  ← Enqueued in Hangfire
                │  (Persistent Store)   │    backed by SQL Server
                └───────────┬───────────┘
                            │
                ┌───────────▼───────────┐
                │       Worker          │  ← Dequeues & executes
                │  (Hangfire Worker)    │    with concurrency control
                └───────────┬───────────┘
                            │
               ┌────────────▼────────────┐
               │    Fetch External Data  │  ← Pulls from External API
               │   (ExternalApiSimulator)│    in paginated chunks
               └────────────┬────────────┘
                            │
               ┌────────────▼────────────┐
               │     Batch Splitter      │  ← Divides records
               │                         │    into N-sized batches
               └────────────┬────────────┘
                            │
          ┌─────────────────▼─────────────────────┐
          │           Processing Pipeline          │
          │                                        │
          │  ┌──────────┐  ┌──────────┐  ┌──────┐ │
          │  │ Validate │→ │ Process  │→ │ Save │ │
          │  └──────────┘  └──────────┘  └──────┘ │
          │  (per batch, parallel execution)        │
          └────────┬──────────────────┬────────────┘
                   │                  │
          ┌────────▼────────┐  ┌──────▼──────────┐
          │  ✅ Success     │  │  ❌ Failed       │
          │  Saved to DB    │  │  Marked for      │
          └─────────────────┘  │  Retry           │
                               └──────┬───────────┘
                                      │
                         ┌────────────▼────────────┐
                         │     Retry Job           │  ← Runs every 10 min
                         │  (Independent job)      │    retries only failed
                         └─────────────────────────┘
```

---

## Key Concepts

### 🕐 Scheduled Jobs

- A **daily job** runs on a cron schedule to fetch and process all new data
- A **retry job** runs every **10 minutes** to reprocess only failed records
- Both jobs are registered and managed via Hangfire's dashboard

### 📦 Batch Processing with Throttling

- Incoming records are split into **configurable batch sizes**
- Batches are processed with **controlled parallelism** to avoid resource exhaustion
- Throttling ensures downstream systems (DB, API) are not overwhelmed

### 🔁 Retry Handling

Retry logic operates at two levels:

- **Job-level** — Hangfire retries the entire job if it throws an unhandled exception
- **Record-level** — Individual failed records are flagged in the database and picked up by the dedicated retry job every 10 minutes, without touching records that already succeeded

---

## Tech Stack

| Technology | Role |
|---|---|
| **.NET 10** | Core runtime and application framework |
| **Hangfire** | Background job scheduling, queuing, and dashboard |
| **SQL Server** | Primary data store for records and Hangfire state |
| **React + TypeScript** | Frontend dashboard UI |
| **Serilog** | Structured logging with sink support |
| **EF Core** | ORM for database access and bulk operations |

---

## Getting Started

### Prerequisites

Make sure the following are installed before running the project:

| Requirement | Version | Download |
|---|---|---|
| **.NET SDK** | 10.0+ | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |
| **SQL Server** | 2019+ | [microsoft.com](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| **Node.js** | 18.0+ LTS | [nodejs.org](https://nodejs.org/) |
| **npm** | 9.0+ _(bundled with Node.js)_ | — |

> 💡 Verify your installations: `dotnet --version` · `node --version` · `npm --version`

### Clone the Repository

```bash
git clone https://github.com/saqibamin257/MillionRecordsProcessor.git
cd MillionRecordsProcessor
```

---

### Step 1 — Configure the Database

Update the connection string in `appsettings.json` for **both** `Processor.API` and `Processor.Worker`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=MillionRecordsDb;Trusted_Connection=True;"
}
```
After configuring the connection string in Step 1, the user needs to apply the EF Core migrations to create the database schema. Run the following command in the Package Manager Console in Visual Studio:
powershell
_Update-Database_
Or via the .NET CLI in the terminal, pointing to the project that contains the migrations (Modules.Records based on your solution structure):
bash
_dotnet ef database update --project src/Modules/Modules.Records_

This creates all required tables in SQL Server before the application starts. Only needs to be run once on first setup, or after pulling new commits that include new migrations.


---

### Step 2 — Start the .NET Backend (Multiple Startup Projects)

This solution runs **three .NET projects simultaneously**. The recommended way is to configure Visual Studio to launch them all at once:

1. Right-click the **Solution** in Solution Explorer → _Properties_
2. Go to **Common Properties → Startup Project**
3. Select **Multiple startup projects**
4. Set the following projects to **Start**:

   | Project | Action |
   |---|---|
   | `ExternalApiSimulator` | Start |
   | `Processor.API` | Start |
   | `Processor.Worker` | Start |

5. Click **OK**, then press **F5** (or click ▶ Start)

All three projects will launch together. Visual Studio will open a console/browser window for each.

> 💡 Alternatively, you can start each project manually in separate terminals:
> ```bash
> dotnet run --project ExternalApiSimulator
> dotnet run --project Processor.API
> dotnet run --project Processor.Worker
> ```

---

### Step 3 — Open the Hangfire Dashboard

Once the Worker is running, open the Hangfire dashboard to monitor and manage jobs:

```
http://localhost:xxxx/hangfire
```

> Replace `xxxx` with the port configured in `Processor.Worker`'s `launchSettings.json`.

From the dashboard you can monitor queued jobs, view processing history, trigger jobs manually, and inspect failed jobs with their full stack traces.

#### Triggering the Job Manually _(during development)_

The main processing job is **scheduled to run daily at 08:00 AM**. During development, you don't need to wait — you can trigger it immediately from the Hangfire dashboard:

1. Go to **Recurring Jobs** in the dashboard
2. Find the daily processing job
3. Click **Trigger Now**
---

### Step 4 — Run the React Frontend

The frontend is a **React + TypeScript** application located at `MillionRecordsProcessor.UI/`.

Open a **new terminal** and run the following:

#### 4a. Navigate to the frontend directory

```bash
cd MillionRecordsProcessor.UI
```

#### 4b. Install all dependencies

```bash
npm install
```

> Reads `package.json` and installs all required packages into `node_modules/`. Only needs to be run once, or after pulling new commits that change `package.json`.

#### 4c. Start the development server

```bash
npm run dev
```

The app will be available at:

```
http://localhost:5173
```

> _Vite's default port is `5173`. If already in use, Vite will automatically pick the next available port and print it in the terminal._

#### Environment Variables _(optional)_

If the frontend needs to point to a non-default API URL, create a `.env.local` file inside `MillionRecordsProcessor.UI/`:

```env
VITE_API_BASE_URL=http://localhost:xxxx
```

> `.env.local` is git-ignored by default and will not be committed to the repository.

---

### Running Order Summary

```
  Visual Studio (F5)                          Terminal
  ──────────────────────────────────────      ──────────────────────────
  Multiple Startup Projects configured:       cd MillionRecordsProcessor.UI
                                              npm install
  ┌─ ExternalApiSimulator  → starts           npm run dev
  ├─ Processor.API         → starts
  └─ Processor.Worker      → starts           → React UI ready
                                                http://localhost:5173
  → All three running together
    Hangfire Dashboard:
    http://localhost:xxxx/hangfire
```

---

## Important Behavior

> ⚠️ **Please read before running in production.**

- 🚫 **Missed schedules are _not_ executed automatically** — if the worker is offline when a scheduled job was due, that run is skipped entirely and will not catch up on restart.

- ⏱️ **Retries only occur _after_ a job starts and fails** — the retry mechanism does not compensate for skipped schedules. It only retries records that were _attempted_ and _failed_ during an active run.

- 🔄 **The retry job runs independently every 10 minutes** — it is a separate Hangfire job that queries the database for failed records and reprocesses them in isolation, without interfering with the main daily job.

---

## Author

**Saqib Amin**  
_Backend Engineer · .NET Specialist_

[![GitHub](https://img.shields.io/badge/GitHub-saqibamin257-181717?style=flat-square&logo=github)](https://github.com/saqibamin257)

---

<div align="center">
<sub>Built with ❤️ for scale, reliability, and clean architecture.</sub>
</div>
