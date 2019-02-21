using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassActivity2.Models;
using ClassActivity2.ViewModels;
using System.Data;
using System.IO;


namespace ClassActivity2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        /******************************************************************/
        /*The following actions are here to service the Advanced Report view*/
        /******************************************************************/

        //This action returns the Advanced view with its associated ViewModel
        //This action will be called for the initial loading of the Advanced view

            [HttpGet]
        public ActionResult Advanced()
        {
            AdvancedVM vm = new AdvancedVM();

            //to retrieve a list of all the employess to populate the dropdown
            vm.lgemployees = GetEmployees(0);

            //Set default values for the FROM and TO dates
            vm.DateFrom = new DateTime(2011, 01, 1);
            vm.DateTo = new DateTime(2011, 12, 31);

            return View(vm);

        }

        private SelectList GetEmployees(int selected)
        {
            using (HardwareDBEntities db = new HardwareDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //Create a SelectListItem for each employee record in the DB
                //Value is set to the primary key of the record and Text is set to the Name of the employee

                var employees = db.lgemployees.Select(x => new SelectListItem
                {
                    Value = x.emp_num.ToString(),
                    Text = x.emp_fname
                }).ToList();

                //If selected pearameter has a value, configure the SelectList so that the apporiate item is preselected
                if (selected == 0)
                    return new SelectList(employees, "Value", "Text");
                else
                    return new SelectList(employees, "Value", "Text", selected);
            }
        }
        //to process the given criteria and return report data based on the criteria

        [HttpPost]
        public ActionResult Advanced(AdvancedVM vm)
        {
            using (HardwareDBEntities db = new HardwareDBEntities())
            {
                db.Configuration.ProxyCreationEnabled = false;

                //to retrieve a list of the employees so as to populate  the dropdown
                vm.lgemployees = GetEmployees(vm.Selectedemp_num);

                //to get the full details of the selected employee so that it can displayed on the view
                vm.employee = db.lgemployees.Where(x => x.emp_num == vm.Selectedemp_num).FirstOrDefault();

                //to get all the invoice details that adheres to the entered criteria
                //For each of the results, load data into a new ReportRecord object
                var temp = db.lginvoices.Include("lgcustomer").ToList();
                var date = temp.Where(pp => pp.employee_id == vm.employee.emp_num
                    && pp.inv_DATETIME >= vm.DateFrom
                    && pp.inv_DATETIME <= vm.DateTo).ToList();

                var list = date.ToList().Select(rr => new ReportRecord
                {
                    INV_Date = rr.inv_DATETIME.ToString(),
                    Amount = Convert.ToDouble(rr.inv_total),
                    //Customer = db.lgcustomers.Where(pp => pp.cust_code == rr.cust_code).Select(x => x.cust_fname + " " + x.cust_lname).FirstOrDefault(),
                    Customer = rr.lgcustomer.cust_fname,
                    Employee_id = Convert.ToInt32(rr.employee_id)
                });

                vm.results = list.GroupBy(g => g.Customer).ToList();

                //Load the list of ReportRecords returned by the above query into a new dictionary grouped by Employee
                //This will be used to generate the chart on the View through the MicroSoft Charts helper
                vm.chartData = list.GroupBy(g => g.Customer).ToDictionary(g => g.Key, g => g.Sum(v => v.Amount));

                //Store the chartData dictionary in temporary data so that it can be accessed by the EmployeeOrdersChart action resonsible for generating the chart
                TempData["chartData"] = vm.chartData;
                TempData["records"] = list.ToList();
                TempData["employee"] = vm.employee;

                return View(vm);

            }
        }

        public ActionResult EmployeeSalesChart()
        {
            //Load the chartData from temporary memory
            var data = TempData["chartData"];

            //Return the EmployeeOrdersChart temporary view, pass through the required chartData
            return View(TempData["chartData"]);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}