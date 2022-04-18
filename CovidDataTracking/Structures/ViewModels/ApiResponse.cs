using System;
using System.Collections.Generic;

namespace Structures.ViewModels
{
    public class ApiResponse<T>
    {
        public MetaData MetaData { get; set; }
        public Summary Summary { get; set; }
        public IList<T> Records { get; set; }

        public ApiResponse(IList<T> items, long? count, int pageNumber, int pageSize)
        {
            var totalRecordCount = count ?? 0;
            MetaData = new MetaData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecordCount,
                TotalPages = totalRecordCount > 0
                    ? (long)Math.Ceiling(totalRecordCount / (double)pageSize)
                    : totalRecordCount
            };

            MetaData.HasNext = pageNumber < MetaData.TotalPages;
            MetaData.HasPrevious = MetaData.CurrentPage > 1;

            Records = items;
        }
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
}