using Modules.Records.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Modules.Processing.Application.Context
{
    public class RecordContext
    {
        public Record Record { get; init; }

        public bool HasFailed { get; private set; }
        public string? Error { get; private set; }

        public void Fail(string error)
        {
            HasFailed = true;
            Error = error;
        }
    }
}
