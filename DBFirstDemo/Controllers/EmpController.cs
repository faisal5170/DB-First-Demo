using DBFirstDemo.Models;
using DBFirstDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DBFirstDemo.Controllers
{
    public class EmpController : Controller
    {
        private static readonly DemoDBEntities db = new DemoDBEntities();
        // GET: Emp
        public ActionResult Index()
        {
            //==> using Linq
            //return View(db.Emps.Select(Emp => new EmpViewModel
            //{
            //    Age = Emp.Age,
            //    Country = Emp.Country,
            //    ID = Emp.ID,
            //    LastModified = Emp.LastModified,
            //    Name = Emp.Name,
            //    Profile = Emp.Profile
            //}).ToList());

            //==>Calling StoreProcedure
            var data = db.GetAllEmployees().Select(Emp => new EmpViewModel
            {
                Age = Emp.Age,
                Country = Emp.Country,
                ID = Emp.ID,
                LastModified = Emp.LastModified,
                Name = Emp.Name,
                Profile = Emp.Profile
            }).ToList();
            return View(data);
        }

        // [Route("Emp/Employee/{EID?}")]
        // [Route("Employee/{EID}/edit")]
        public ActionResult AddUpdateEmployee(int? EID)
        {
            Emp empModel = new Emp();
            if (EID.HasValue)
                empModel = db.Emps.Find(EID);
            return View(empModel);
        }

        [HttpPost]
        public ActionResult AddUpdateEmployee(Emp model)
        {
            if (ModelState.IsValid)
            {
                if (model.ID == 0)
                    db.Emps.Add(model);
                else
                {
                    var oldData = db.Emps.Find(model.ID);
                    oldData.Country = model.Country;
                    oldData.Age = model.Age;
                    oldData.Name = model.Name;
                    oldData.Profile = model.Profile;
                }
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult DeleteEmployee(int EID)
        {
            var empModel = db.Emps.Find(EID);
            db.Emps.Remove(empModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetAllEmployees()
        {
            try
            {
                IList<ListedbutNoLocationViewModel> ItemList;
                ItemList = new List<ListedbutNoLocationViewModel>();
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                SqlConnection connectIt = new SqlConnection(ConfigurationManager.ConnectionStrings["tempConnection"].ConnectionString);
                connectIt.Open();
                SqlCommand objCommand = new SqlCommand
                {
                    Connection = connectIt,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "GetAllEmployees"
                };
                //objCommand.Parameters.AddWithValue("@Orderby", Order);
                SqlDataAdapter da = new SqlDataAdapter
                {
                    SelectCommand = objCommand
                };
                objCommand.CommandTimeout = 300;
                objCommand.ExecuteNonQuery();
                da.Fill(ds);

                connectIt.Close();
                connectIt.Dispose();
                objCommand.Dispose();

                ListedbutNoLocationViewModel item;
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    item = new ListedbutNoLocationViewModel
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Age = Convert.ToInt32(dr["Age"]),
                        Name = dr["Name"].ToString(),
                        Profile = dr["Profile"].ToString(),
                        Country = dr["Country"].ToString(),
                        LastModified = Convert.ToDateTime(dr["LastModified"])
                    };
                    ItemList.Add(item);
                }
                return View(ItemList);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}