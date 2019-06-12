using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AdminPanel.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AdminPanel.Controllers
{
    public class BankController : Controller
    {
        public IConfiguration Configuration { get; }
        public BankController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: Bank
        public ActionResult Index()
        {
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
            //return View(teacherList);
            return View(_bankList);
        }


        // GET: Bank/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bank/Create
        [HttpPost]
        public ActionResult Create(Bank _objOfBank)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        string sql = $"Insert Into tbl_BankInfo (Name, Address, ContactNo) Values ('{_objOfBank.BankName}', '{_objOfBank.Address}','{_objOfBank.ContactNo}')";
                        using (SqlCommand command = new SqlCommand(sql, connection))
                        {
                            command.CommandType = CommandType.Text;
                            connection.Open();
                            command.ExecuteNonQuery();
                            connection.Close();
                        }
                        ViewBag.successMessage = "Data saved successfully";
                        ModelState.Clear();
                        return View();
                    }
                }
                else
                    return View();

                //return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }


        public IActionResult Update(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            Bank _bank = new Bank();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select * from tbl_BankInfo where Id='{id}'";
                SqlCommand command = new SqlCommand(sql, connection);
                connection.Open();
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        _bank.Id = Convert.ToInt32(dataReader["Id"]);
                        _bank.BankName = Convert.ToString(dataReader["Name"]);
                        _bank.Address = Convert.ToString(dataReader["Address"]);
                        _bank.ContactNo = Convert.ToString(dataReader["ContactNo"]);

                    }
                }
                connection.Close();
            }
            return View(_bank);
        }

        // POST: Bank/Update/1
        [HttpPost]
        [ActionName("Update")]
        public IActionResult Update_Post(Bank _bank)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Update tbl_BankInfo SET Name='{_bank.BankName}', Address='{_bank.Address}',ContactNo='{_bank.ContactNo}' Where Id='{_bank.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "Bank");
        }

        // POST: Bank/Delete/1
        [HttpPost]
        public IActionResult Delete(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Delete From tbl_BankInfo Where Id='{id}'";
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
            return RedirectToAction("Index", "Bank");
        }
    }
}