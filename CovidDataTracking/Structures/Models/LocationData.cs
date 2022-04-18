using System;

namespace Structures.Models
{
    public class LocationData
    {
        public long Uid { get; set; }
        public string Iso2 { get; set; }
        public string Iso3 { get; set; }
        public string Code3 { get; set; }
        public string FIPS { get; set; }
        public string Admin2 { get; set; }
        public string ProvinceState { get; set; }
        public string CountryRegion { get; set; }
        public float Lat { get; set; }
        public float Long { get; set; }
        public string CombinedKey { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}
