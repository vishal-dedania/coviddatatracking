using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Structures.ViewModels
{
    public class CovidDataResponse
    {
        public Summary Summary { get; set; }
        public IList<CovidRecords> Records { get; set; }
    }

    public class Summary
    {
        public long AverageDailyCases { get; set; }
        public NumberOfCases NumberOfCases { get; set; }
    }

    public class NumberOfCases
    {
        public CasesDetail Minimum { get; set; }
        public CasesDetail Maximum { get; set; }
    }

    public class CovidRecords
    {
        public string County { get; set; }
        public string State { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public long TotalCases { get; set; }
        public DateTime Date { get; set; }
    }

    public class CasesDetail
    {
        public long Total { get; set; }
        public DateTime Date { get; set; }
    }
}
