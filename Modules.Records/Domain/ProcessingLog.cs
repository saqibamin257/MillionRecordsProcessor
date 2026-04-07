using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Records.Domain
{
    public class ProcessingLog
    {
        public int Id { get; private set; }
        public Guid RecordId { get; private set; }
        public string Status { get; private set; }
        public int RetryCount { get; private set; }
        public string ErrorMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private ProcessingLog() { }

        public ProcessingLog(Guid recordId, string error)
        {
            RecordId = recordId;
            Status = "Failed";
            RetryCount = 0;
            ErrorMessage = error;
            CreatedAt = DateTime.UtcNow;
        }

        public void IncrementRetry()
        {
            RetryCount++;
        }
        public void MarkResolved()
        {
            Status = "Resolved";
        }
    }
}
