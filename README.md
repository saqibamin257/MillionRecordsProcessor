🚀 MillionRecordsProcessor

A scalable background data processing system built with .NET 8, designed to handle large-volume (million+) records using Hangfire, batch processing, and clean architecture principles.

🎯 Problem Statement

This project simulates a real-world scenario where large datasets must be:

Processed asynchronously
Scheduled reliably
Retried safely on failure
Scaled efficiently

✨ Key Features
⏱ Scheduled Jobs using Hangfire (CRON-based)

🔁 Retry Mechanism
Job-level (Hangfire)
Record-level (custom retry logic)
⚡ Batch Processing with controlled parallelism
🧠 Pipeline-Based Processing
📊 Monitoring via Hangfire Dashboard
🪵 Structured Logging (Serilog)

🧱 Architecture Overview
[ React Dashboard ]
        ↓
     [ API ]
        ↓
[ Application Layer ]
        ↓
     [ Domain ]
        ↓
[ Infrastructure ]
        ↓
[ Worker (Hangfire Server) ]

🔄 System Flow
Hangfire Scheduler
        ↓
Enqueue Job
        ↓
Worker Executes Job
        ↓
ProcessingService (Orchestrator)
        ↓
BatchProcessor
        ↓
Processing Pipeline
        ↓
Database (Records + Logs)

⚙️ Hangfire Workflow (Core Concept)
1. Job Registration
recurringJobManager.AddOrUpdate<ProcessingJobs>(
    "daily-processing-job",
    job => job.StartProcessing(),
    "0 8 * * *"
);

recurringJobManager.AddOrUpdate<ProcessingJobs>(
    "retry-failed-job",
    job => job.RetryFailed(),
    "*/10 * * * *"
);
Jobs are stored in database
They are not executed immediately

2. Scheduling (Internal)
Every minute:
   ↓
Hangfire checks CRON
   ↓
If matched → Job is enqueued

3. Execution
Worker picks job from queue
   ↓
Executes ProcessingJobs
   ↓
Calls ProcessingService

4. Retry Behavior
[AutomaticRetry(Attempts = 3)]
Retries only if job fails after starting
Does NOT run for missed schedules

🔁 Processing Pipeline

Each record is processed through:

RecordContext
     ↓
ValidationStep
     ↓
ProcessingStep
     ↓
PersistenceStep

⚡ Batch Processing Strategy
Data fetched in pages
Processed in batches
Parallel execution using SemaphoreSlim
Failures tracked and retried separately

🧵 Hangfire Queues
[Queue("processing")]
[Queue("retry")]

Queues used:

processing → main jobs
retry → failed record processing

🗄 Data Flow
External API
     ↓
Batch Fetch
     ↓
Processing Pipeline
     ↓
Database Save
     ↓
Failed Logs → Retry Job

⚠️ Important Notes
⏰ Missed Schedule

If the application starts after scheduled time:

Job will NOT run immediately
It will run on the next schedule

🔁 Retry Limitation
Hangfire retry works only after execution starts
Record-level retry is handled separately

🆕 First Run
Retry job executes
No failed records → no action

🛠 Tech Stack
.NET 8
Hangfire
SQL Server
React + TypeScript
Serilog
👨‍💻 Author

Saqib Amin
Backend Engineer | .NET | Scalable Systems 🚀
