using Structures.Models;
using System.Collections.Generic;

namespace Parsers
{
    public interface IParseCovid19CsvFile
    {
        (List<LocationData>, List<TimeSeriesData>) Parse(string fileLocation);
    }
}