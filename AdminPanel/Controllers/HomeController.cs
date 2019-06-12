using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AdminPanel.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;

namespace AdminPanel.Controllers
{
    public class HomeController : Controller
    {

        public IConfiguration Configuration { get; }
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {

            var _userObj = HttpContext.Session.GetObjectFromJson<User>("_userObj");

            if (_userObj != null)
            {
                List<ReportMapping> _reportList = new List<ReportMapping>();
                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    //SqlDataReader
                    connection.Open();

                    string sql = "select rm.Id,ui.Id,bi.Id bankId,bi.Name BankName,br.Id branchId,br.BranchTitle,ui.FirstName,ui.ContactNo,rm.ReportTitle,rm.ReportUrl from tbl_ReportMappingInfo rm " +
                        "left join tbl_BankInfo bi on rm.BankId=bi.Id " +
                        "left join tbl_BranchInfo br on rm.BranchId = br.Id " +
                        "left join tbl_UserInfo ui on rm.UserId= ui.Id where ui.id='" + _userObj.Id + "' and br.Id='" + _userObj.BranchId + "' "; SqlCommand command = new SqlCommand(sql, connection);
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            ReportMapping _report = new ReportMapping();
                            _report.Id = Convert.ToInt32(dataReader["Id"]);
                            _report.BankId = Convert.ToInt32(dataReader["bankId"]);
                            _report.Bank_VW = Convert.ToString(dataReader["BankName"]);
                            _report.BranchId = Convert.ToInt32(dataReader["branchId"]);
                            _report.Branch_VW = Convert.ToString(dataReader["BranchTitle"]);
                            _report.Username = Convert.ToString(dataReader["FirstName"]);
                            _report.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                            _report.ReportTitle = Convert.ToString(dataReader["ReportTitle"]);
                            _report.ReportUrl = Convert.ToString(dataReader["ReportUrl"]);
                            _reportList.Add(_report);
                        }
                    }
                    connection.Close();
                }

                return View(_reportList);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(LoginViewModel _login)
        {
            if (_login.Email != null && _login.Password != null)
            {
                // 

                string _password = DataManipulationLayer.EncryptData(_login.Password);

                string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                User _user = new User();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string sql = $"Select ui.Id,ui.FirstName,ui.LastName,ui.Email,ui.Usertype,ui.ContactNo,bi.Id BankId,bi.Name,br.Id BranchId, br.BranchTitle from tbl_UserInfo ui " +
                        $"LEFT JOIN tbl_BankInfo bi on bi.Id=ui.BankId " +
                        $"LEFT JOIN tbl_BranchInfo br on br.Id= ui.BranchId Where ui.Email='{_login.Email}' and ui.Password='{_password}' ";
                    SqlCommand command = new SqlCommand(sql, connection);
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            _user.Id = Convert.ToInt32(dataReader["Id"]);
                            _user.FirstName = Convert.ToString(dataReader["FirstName"]);
                            _user.LastName = Convert.ToString(dataReader["LastName"]);
                            _user.Email = Convert.ToString(dataReader["Email"]);
                            _user.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                            _user.BankName_VW = Convert.ToString(dataReader["Name"]);
                            _user.BankId = Convert.ToInt16(dataReader["BankId"]);
                            _user.BranchId = Convert.ToInt16(dataReader["BranchId"]);
                            _user.BranchName_VW = Convert.ToString(dataReader["BranchTitle"]);

                            string _userType = Convert.ToString(dataReader["Usertype"]);
                            if (_userType == "1")
                            {
                                _user.UserType_VW = "Admin";
                                _user.UserType = _userType;
                            }
                            else
                            {
                                _user.UserType_VW = "Normal";
                                _user.UserType = _userType;
                            }
                        }
                    }
                    connection.Close();
                }

                if (_user.Email != null)
                {
                    //var myComplexObject = new MyClass();
                    HttpContext.Session.SetObjectAsJson("_userObj", _user);


                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.error = "Invalid Account";
                    return View();
                }

            }
            else
            {
                ViewBag.error = "Invalid Account";
                return View();
            }
        }



        public IActionResult Registration()
        {
            return View();
        }
    }
}
