using ClassActivity2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClassActivity2.ViewModels
{
    public class AdvancedVM
    {
        //Fields for report criteria

        public IEnumerable<SelectListItem> lgemployees { get; set; }
        public int Selectedemp_num { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }


        //Fields to report data
        public lgemployee employee { get; set; }
        public List<IGrouping<string,ReportRecord>> results { get; set; }
        public Dictionary<string, double> chartData { get; set; }
    }

    public class ReportRecord
    {
        public string INV_Date { get; set; }
        public double Amount { get; set; }
        public string Customer { get; set; }
        public int Employee_id { get; set; }
    }

}