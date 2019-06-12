using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace AdminPanel.Controllers
{
    public class ReportController : Controller
    {
        public IConfiguration Configuration { get; }
        public ReportController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: Report
        public ActionResult Index()
        {
            List<ReportMapping> _reportList = new List<ReportMapping>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "select rm.Id,bi.Id bankId,bi.Name BankName,br.Id branchId,br.BranchTitle,ui.FirstName,ui.ContactNo,rm.ReportTitle,rm.ReportUrl from tbl_ReportMappingInfo rm " +
                    "left join tbl_BankInfo bi on rm.BankId=bi.Id " +
                    "left join tbl_BranchInfo br on rm.BranchId = br.Id " +
                    "left join tbl_UserInfo ui on rm.UserId= ui.Id"; SqlCommand command = new SqlCommand(sql, connection);
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
            //return View();
        }

        // GET: Report/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Report/Create
        public ActionResult Create()
        {
            // Getting Bank Data From BankTable
            List<Bank> _bankList = new List<Bank>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "Select * From tbl_BankInfo"; SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Bank _bank = new Bank();
                        _bank.Id = Convert.ToInt32(dataReader["Id"]);
                        _bank.BankName = Convert.ToString(dataReader["Name"]);
                        _bank.Address = Convert.ToString(dataReader["Address"]);
                        _bank.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                        _bankList.Add(_bank);
                    }
                }
                connection.Close();
            }

            // Insert first default item
            _bankList.Insert(0, new Bank { Id = 0, BankName = "Select" });

            // Assiging bank List to ViewBag.ListOfBank
            ViewBag.ListOfBank = _bankList;
            return View();
        }

        // POST: Report/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ReportMapping _report)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        int _selectedValueBank = _report.BankId;
                        int _selectedValueBrnach = _report.BranchId;
                        int _selectedValueUserType = _report.UserId;

                        _report.CreatedBy = "Admin";
                        _report.CreatedDate = DateTime.Now;

                        string sql = $"Insert Into tbl_ReportMappingInfo (BankId, BranchId, UserId,ReportTitle,ReportUrl,CreatedBy,CreatedDate) Values ('{_report.BankId}', '{_report.BranchId}','{_report.UserId}','{_report.ReportTitle}','{_report.ReportUrl}','{_report.CreatedBy}','{_report.CreatedDate}')";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandType = CommandType.Text;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        return RedirectToAction("Index");
                    }
                }
                else
                    return View();

                //return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
                //return View();
            }
        }

        // GET: Report/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            ReportMapping _report = new ReportMapping();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"select rm.Id,bi.Id bankId,bi.Name Bank,br.Id branchId,br.BranchTitle,ui.FirstName,ui.ContactNo,rm.ReportTitle,rm.ReportUrl from tbl_ReportMappingInfo rm " +
                    $"left join tbl_BankInfo bi on rm.BankId=bi.Id " +
                    $"left join tbl_BranchInfo br on rm.BranchId = br.Id " +
                    $"left join tbl_UserInfo ui on rm.UserId= ui.Id Where rm.Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        _report.Id = Convert.ToInt32(dataReader["Id"]);
                        _report.BankId = Convert.ToInt32(dataReader["bankId"]);
                        _report.Bank_VW = Convert.ToString(dataReader["Bank"]);
                        _report.BranchId = Convert.ToInt32(dataReader["branchId"]);
                        _report.Branch_VW = Convert.ToString(dataReader["BranchTitle"]);
                        _report.Username = Convert.ToString(dataReader["FirstName"]);
                        _report.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                        _report.ReportTitle = Convert.ToString(dataReader["ReportTitle"]);
                        _report.ReportUrl = Convert.ToString(dataReader["ReportUrl"]);
                    }
                }
                connection.Close();
            }
            return View(_report);
        }

        // POST: Report/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ReportMapping _report)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                _report.UpdatedBy = "Admin";
                _report.UpdatedDate = DateTime.Now;

                string sql = $"Update tbl_ReportMappingInfo SET ReportTitle='{_report.ReportTitle}', ReportUrl='{_report.ReportUrl}',UpdateBy='{_report.UpdatedBy}',UpdateDate='{_report.UpdatedDate}' Where Id='{_report.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "Report");
        }

        // GET: Report/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Report/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}