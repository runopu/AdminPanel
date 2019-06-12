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
    public class UserController : Controller
    {
        public IConfiguration Configuration { get; }
        public UserController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // GET: User
        public ActionResult Index()
        {
            List<User> _UserList = new List<User>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "Select ui.Id,ui.FirstName,ui.LastName,ui.Email,ui.Usertype," +
                    "ui.ContactNo,bi.Id BankId,br.Id BranchId,bi.Name,br.BranchTitle from tbl_UserInfo ui " +
                    "LEFT JOIN tbl_BankInfo bi on bi.Id=ui.BankId " +
                    "LEFT JOIN tbl_BranchInfo br on br.Id= ui.BranchId"; SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User _user = new User();
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
                        //_user.UserType_VW = Convert.ToString(dataReader["Usertype"]);
                        _UserList.Add(_user);
                    }
                }
                connection.Close();
            }
            return View(_UserList);
        }

        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
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

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User _user)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        int _selectedValueBank = _user.BankId;
                        int _selectedValueBrnach = _user.BranchId;
                        string _selectedValueUserType = _user.UserType;
                        string _password = DataManipulationLayer.EncryptData(_user.Password);
                        _user.CreatedBy = "Admin";
                        _user.CreatedDate = DateTime.Now;

                        string sql = $"Insert Into tbl_UserInfo (FirstName, LastName, Email,ContactNo,BankId,BranchId,Usertype,Password,CreatedBy,CreatedDate) Values ('{_user.FirstName}', '{_user.LastName}','{_user.Email}','{_user.ContactNo}','{_user.BankId}','{_user.BranchId}','{_user.UserType}','{_password}','{_user.CreatedBy}','{_user.CreatedDate}')";
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

        // GET: User/Edit/5
        public ActionResult Edit(int id)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            User _user = new User();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = $"Select ui.Id,ui.FirstName,ui.LastName,ui.Email,ui.Usertype,ui.ContactNo,bi.Id BankId,bi.Name,br.Id BranchId, br.BranchTitle from tbl_UserInfo ui " +
                    $"LEFT JOIN tbl_BankInfo bi on bi.Id=ui.BankId " +
                    $"LEFT JOIN tbl_BranchInfo br on br.Id= ui.BranchId Where ui.Id='{id}'";
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
            return View(_user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User _user)
        {
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                _user.UpdatedBy = "Admin";
                _user.UpdatedDate = DateTime.Now;

                string sql = $"Update tbl_UserInfo SET FirstName='{_user.FirstName}', LastName='{_user.LastName}',Email='{_user.Email}',ContactNo='{_user.ContactNo}',Usertype='{_user.UserType}',UpdateBy='{_user.UpdatedBy}',UpdateDate='{_user.UpdatedDate}' Where Id='{_user.Id}'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            //return RedirectToAction("Index");
            return RedirectToAction("Index", "User");
        }

        // GET: User/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: User/Delete/5
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


        public JsonResult GetBranchBySelectingBank(int _bankId)
        {
            List<Branch> _branchList = new List<Branch>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "Select * From tbl_BranchInfo Where BankId='" + _bankId + "'"; SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        Branch _branch = new Branch();
                        _branch.Id = Convert.ToInt32(dataReader["Id"]);
                        _branch.BranchTitle = Convert.ToString(dataReader["BranchTitle"]);
                        _branchList.Add(_branch);
                    }
                }
                connection.Close();
            }
            _branchList.Insert(0, new Branch { Id = 0, BranchTitle = "Select" });
            return Json(new SelectList(_branchList, "Id", "BranchTitle"));
        }

        public JsonResult GetUserListBySelectingBranch(int _branchId)
        {
            List<User> _userList = new List<User>();
            string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //SqlDataReader
                connection.Open();

                string sql = "select ui.Id,ISNULL(ui.FirstName,'') + ' ' + ISNULL(ui.LastName,'')+ '--'+ISNULL(ui.ContactNo,'') as Name " +
                    "from tbl_UserInfo ui " +
                    "left join tbl_BranchInfo br on ui.BranchId = br.Id Where br.Id='" + _branchId + "'"; SqlCommand command = new SqlCommand(sql, connection);
                using (SqlDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        User _user = new User();
                        _user.Id = Convert.ToInt32(dataReader["Id"]);
                        _user.FirstName = Convert.ToString(dataReader["Name"]);
                        _userList.Add(_user);
                    }
                }
                connection.Close();
            }
            _userList.Insert(0, new User { Id = 0, FirstName = "Select" });
            return Json(new SelectList(_userList, "Id", "FirstName"));
        }

    }
}