using System;

namespace Structures.ViewModels
{
    public class CovidDataRequest : BaseRequest
    {
        public string County { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
