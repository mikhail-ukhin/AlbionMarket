using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlbionMarket.Services
{
    public class WorkerStateService
    {
        public bool ScanInProgress { get; set; }

        public int ItemsFound { get; set; }

        public DateTime? LastScanStart { get; set; }

        public DateTime? LastScanFinished { get; set; }

        public int ItemsFoundLastScan { get; set; }
    }
}
