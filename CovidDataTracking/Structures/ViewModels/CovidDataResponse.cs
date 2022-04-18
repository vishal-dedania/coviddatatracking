using System;

namespace Structures.ViewModels
{
    public class CovidDataResponse : BaseViewModel
    {
        public string County { get; set; }
        public string State { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public long AverageDailyCases { get; set; }
        public NumberOfCases NumberOfCases { get; set; }
    }

    public class NumberOfCases
    {
        public CasesDetail Minimum { get; set; }
        public CasesDetail Maximum { get; set; }
    }

    public class CasesDetail
    {
        public long Total { get; set; }
        public DateTime Date { get; set; }
    }
}
