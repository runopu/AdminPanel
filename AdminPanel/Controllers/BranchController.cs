using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AdminPanel.Controllers
{
    public class BranchController : Controller
    {
        public IConfiguration Configuration { get; }
        public BranchController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Index()
        {
            List<Branch> _branchList = new List<Branch>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "Select Br.Id,Br.BankId,Ba.Name,Br.BranchTitle,Br.BranchAddress,Br.BranchManager,Br.ContactNo From tbl_BranchInfo Br left join tbl_BankInfo Ba on Br.BankId=Ba.Id"; SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Branch _branch = new Branch();
                        _branch.Id = Convert.ToInt32(dataReader["Id"]);
                        _branch.BankId = Convert.ToInt32(dataReader["BankId"]);
                        _branch.BankName_VW = Convert.ToString(dataReader["Name"]);
                        _branch.BranchTitle = Convert.ToString(dataReader["BranchTitle"]);
                        _branch.BranchAddress = Convert.ToString(dataReader["BranchAddress"]);
                        _branch.BranchManager = Convert.ToString(dataReader["BranchManager"]);
                        _branch.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                        _branchList.Add(_branch);
                    }
                }
                connection.Close();
            }
            //return View(_bankList);
            return View(_branchList);
        }

        [HttpGet]
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

        [HttpPost]
        public ActionResult Create(Branch _branch)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        int _selectedValue = _branch.BankId;
                        _branch.CreatedBy = "Admin";
                        _branch.CreateDate = DateTime.Now;

                        string sql = $"Insert Into tbl_BranchInfo (BankId, BranchTitle, BranchAddress,ContactNo,BranchManager,CreatedBy,CreatedDate) Values ('{_selectedValue}', '{_branch.BranchTitle}','{_branch.BranchAddress}','{_branch.ContactNo}','{_branch.BranchManager}','{_branch.CreatedBy}','{_branch.CreateDate}')";
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

        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            Branch _branch = new Branch();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select Br.Id,Br.BankId,Ba.Name,Br.BranchTitle,Br.BranchAddress,Br.BranchManager,Br.ContactNo From tbl_BranchInfo Br left join tbl_BankInfo Ba on Br.BankId=Ba.Id Where Br.Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        _branch.Id = Convert.ToInt32(dataReader["Id"]);
                        _branch.BankId = Convert.ToInt32(dataReader["BankId"]);
                        _branch.BankName_VW = Convert.ToString(dataReader["Name"]);
                        _branch.BranchTitle = Convert.ToString(dataReader["BranchTitle"]);
                        _branch.BranchAddress = Convert.ToString(dataReader["BranchAddress"]);
                        _branch.BranchManager = Convert.ToString(dataReader["BranchManager"]);
                        _branch.ContactNo = Convert.ToString(dataReader["ContactNo"]);
                    }
                }
                connection.Close();
            }
            return View(_branch);
        }

        [HttpPost]
        [ActionName("Update")]
        public IActionResult Update_Post(Branch _branch)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                _branch.UpdateBy = "Admin";
                _branch.UpdateDate = DateTime.Now;

                string sql = $"Update tbl_BranchInfo SET BranchTitle='{_branch.BranchTitle}', BranchAddress='{_branch.BranchAddress}',ContactNo='{_branch.ContactNo}',BranchManager='{_branch.BranchManager}',UpdateBy='{_branch.UpdateBy}',UpdateDate='{_branch.UpdateDate}' Where Id='{_branch.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "Branch");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From tbl_BranchInfo Where Id='{id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        ViewBag.Result = "Operation got error:" + ex.Message;
                    }
                    connection.Close();
                }
            }
            return RedirectToAction("Index", "Branch");
        }
    }
}