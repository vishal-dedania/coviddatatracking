using System;
using Structures.Validations;

namespace Structures.ViewModels
{
    [AtLeastOneProperty("County", "State", "City", ErrorMessage = "You must supply at least County or State or City value")]
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
