using System;

namespace OCTT.Web.Models
{
    public class PageControl
    {
        private int _currentPage = 1;

        public int CurrentPage
        {
            get { return _currentPage; }
            set => _currentPage = TotalPages < value ? 1 : value;
        }

        public int RecordsPerPage { get; private set; } = 10;

        public int RecordsTotal { get; set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)RecordsTotal / RecordsPerPage); }
        }
    }
}
