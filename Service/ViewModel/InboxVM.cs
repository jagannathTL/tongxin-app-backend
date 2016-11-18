using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModel
{
    public class InboxVM
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public string Msg { get; set; }
        public string Url { get; set; }
    }
}
