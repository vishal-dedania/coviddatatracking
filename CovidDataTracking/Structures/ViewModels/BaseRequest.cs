using System.ComponentModel.DataAnnotations;

namespace Structures.ViewModels
{
    public class BaseRequest
    {
        [Range(1, 20)]
        public int PageSize { get; set; } = 10;

        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "PageNumber must be greater than zero.")]
        public int PageNumber { get; set; } = 1;
    }
}