using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Records.Domain
{
    public class Record
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }

        public string Status { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        //private Record() { } // Required by EF Core

        public Record(string name, string email)
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Status = "Pending";
        }

        public void MarkProcessed()
        {
            Status = "Processed";
            ProcessedAt = DateTime.UtcNow;
        }

        public void MarkFailed()
        {
            Status = "Failed";
        }
    }
}
