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

    public class CovidDailyBreakDownDataResponse
    {
        public SummaryDailyBreakDown Summary { get; set; }
        public IList<CovidRecordsDaily> Records { get; set; }
    }

    public class Summary : MetaData
    {
        public long AverageDailyCases { get; set; }
        public NumberOfCases NumberOfCases { get; set; }
    }

    public class SummaryDailyBreakDown : MetaData
    {
    }

    public class MetaData
    {
        public int CurrentPage { get; set; }
        public long TotalPages { get; set; }
        public int PageSize { get; set; }
        public long TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }

    public class NumberOfCases
    {
        public CasesDetail Minimum { get; set; }
        public CasesDetail Maximum { get; set; }
    }

    public class CovidRecordsDaily : LocationDetail
    {
        public long TodayCases { get; set; }
        public DateTime Date { get; set; }
        public long PreviousDayCases { get; set; }
        public int PercentChange { get; set; }
        public string Trend { get; set; }
        [JsonIgnore] public long TotalCount { get; set; }
    }

    public class CovidRecords : LocationDetail
    {
        public long TotalCasesPerDay { get; set; }
        [JsonIgnore] public long TotalCases { get; set; }
        [JsonIgnore] public long MaxCases { get; set; }
        [JsonIgnore] public DateTime MaxCasesOn { get; set; }
        [JsonIgnore] public long MinCases { get; set; }
        [JsonIgnore] public DateTime MinCasesOn { get; set; }
        [JsonIgnore] public long TotalCount { get; set; }
        public DateTime Date { get; set; }
    }

    public class LocationDetail
    {
        public string County { get; set; }
        public string State { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
    }

    public class CasesDetail
    {
        public long Total { get; set; }
        public DateTime Date { get; set; }
    }
}
