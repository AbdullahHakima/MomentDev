namespace Bloggi.Helpers
{
    public class Pagination<T> where T:class 
    {
        public int PageSize { get; set; } = 10;
        public int PageCount { get; set; }
        public int PageIndex { get; set; }
        public IEnumerable<T> Data { get; set;}
        public Pagination(IEnumerable<T> data,int pageCount,int pageIndex,int pageSize)
        {
            PageSize = pageSize;
            Data= data;
            PageCount= pageCount;
            PageIndex= pageIndex;
        }
    }
}
