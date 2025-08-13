using System.ComponentModel.DataAnnotations;

namespace EcommerceApp.Application.DTOs.Response
{
    public class PagedResult<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalRecords { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public PagedResult()
        {
        }

        public PagedResult(List<T> data, int count, int pageNumber, int pageSize)
        {
            Data = data;
            TotalRecords = count;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            HasNextPage = PageNumber < TotalPages;
            HasPreviousPage = PageNumber > 1;
        }
    }
}