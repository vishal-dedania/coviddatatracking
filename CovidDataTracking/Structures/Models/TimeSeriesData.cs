using System;

namespace Structures.Models
{
    public class TimeSeriesData
    {
        public long LocationUid { get; set; }
        public DateTime Date { get; set; }
        public int Value { get; set; }
        public DateTimeOffset RecordUpdatedOn { get; set; }
    }
}
