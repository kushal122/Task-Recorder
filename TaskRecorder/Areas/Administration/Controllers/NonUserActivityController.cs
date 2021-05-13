using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using TaskRecorder.Areas.Administration.Models;
using TaskRecorder.Data;
using TaskRecorder_DataModels.Models;

namespace TaskRecorder.Areas.Administration.Controllers
{
    [Area("Administration")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = "Admin_Schema")]
    public class NonUserActivityController : Controller
    {
        private readonly ConnectionDBContext db;
        public NonUserActivityController(ConnectionDBContext db)
        {
            this.db = db;
        }
        public async Task<IActionResult> ListActivity(int pageNumber = 1)
        {
            return View(await PaginatedList<NonUserActivity>.CreateAsync(db.NonUserActivity, pageNumber, 15));
        }
        public async Task<IActionResult> SearchTerm(string SearchString,int pageNumber=1)
        {
            var user = from u in db.NonUserActivity select u;
            if (!String.IsNullOrWhiteSpace(SearchString))
            {               
                user = user.Where(c => c.IpAddress.Contains(SearchString) || c.Data.Contains(SearchString));
                return View("ListActivity", await PaginatedList<NonUserActivity>.CreateAsync(user, pageNumber, 15));
            }
            ViewBag.Message = "No Record Found";
            return View("ListActivity");
        }
        static private string GetConnectionString()
        {
            return "Data Source=RABIN_RAUT;Initial Catalog=TaskRecorder-DB;user id =sa; password = 3964;MultipleActiveResultSets=true";
        }
        readonly string _connectionString = GetConnectionString();
        public IActionResult GetNonUserData(DateTime? start, DateTime? end)
        {
            using (SqlConnection sql = new SqlConnection(_connectionString))
            {
                string sqlQuery = "select * from [dbo].[NonUserActivity] where ActivityDate between'" + start + "'and'" + end + "'";
                using (SqlCommand cmd = new SqlCommand(sqlQuery, sql))
                {
                    sql.Open();
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    List<NonUserActivity> list = new List<NonUserActivity>();
                    foreach (DataRow item in ds.Tables[0].Rows)
                    {
                        list.Add(new NonUserActivity
                        {
                            Id = Convert.ToInt32(item["Id"]),
                            IpAddress = Convert.ToString(item["IpAddress"]),
                            ActivityDate = Convert.ToDateTime(item["ActivityDate"]),
                            Data = Convert.ToString(item["Data"]),
                            Url = Convert.ToString(item["Url"]),
                        });
                    }
                    sql.Close();
                    ModelState.Clear();
                    return View(list);
                }
            }
        }
    }
}