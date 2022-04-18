using AppLogs;
using CsvHelper;
using Structures.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;

namespace Parsers
{
    public class ParseCovid19CsvFile : IParseCovid19CsvFile
    {
        public (List<LocationData>, List<TimeSeriesData>) Parse(string fileLocation)
        {
            var timeStamp = DateTimeOffset.Now;
            var locationData = new List<LocationData>();
            var timeSeriesData = new List<TimeSeriesData>();

            try
            {
                using var reader = new StreamReader(fileLocation);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

                //using dynamic as list of columns in csv are dynamic based on everyday data points
                var records = csv.GetRecords<dynamic>();
                foreach (ExpandoObject data in records)
                {
                    var record = new Dictionary<string, object>(data);
                    var locationUid = long.Parse(record["UID"].ToString() ?? "-1");
                    locationData.Add(new LocationData
                    {
                        Uid = locationUid,
                        Iso2 = record["iso2"].ToString(),
                        Iso3 = record["iso3"].ToString(),
                        Code3 = record["code3"].ToString(),
                        FIPS = record["FIPS"].ToString(),
                        Admin2 = record["Admin2"].ToString(),
                        ProvinceState = record["Province_State"].ToString(),
                        CountryRegion = record["Country_Region"].ToString(),
                        Lat = float.Parse(record["Lat"].ToString() ?? "0"),
                        Long = float.Parse(record["Long_"].ToString() ?? "0"),
                        CombinedKey = record["Combined_Key"].ToString(),
                        CreatedOn = timeStamp
                    });

                    var timeSeriesStartDate = new DateTime(2020, 1, 22);
                    while (timeSeriesStartDate < DateTime.UtcNow.AddDays(-1))
                    {
                        timeSeriesData.Add(new TimeSeriesData
                        {
                            LocationUid = locationUid,
                            Date = timeSeriesStartDate,
                            Value = int.Parse(record[timeSeriesStartDate.ToString("M/d/yy")].ToString() ?? "0"),
                            RecordUpdatedOn = timeStamp
                        });

                        timeSeriesStartDate = timeSeriesStartDate.AddDays(1);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

            return (locationData, timeSeriesData);
        }
    }
}
