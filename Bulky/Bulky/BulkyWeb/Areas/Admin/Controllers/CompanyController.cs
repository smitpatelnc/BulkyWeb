using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;

        }
        public IActionResult Index()
        {
            List<Company> objCompanylist = _UnitOfWork.Company.GetAll().ToList();

            return View(objCompanylist);
        }

        public IActionResult Upsert(int? id)
        {


            if (id == null || id == 0)
            {
                //create
                return View(new Company());
            }
            else
            {
                //update
                Company companyObj = _UnitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company companyObj)
        {
            if (ModelState.IsValid)
            {

                if (companyObj.Id == 0)
                {
                    _UnitOfWork.Company.Add(companyObj);
                }
                else
                {
                    _UnitOfWork.Company.Update(companyObj);
                }

                _UnitOfWork.Save();
                TempData["success"] = " created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(companyObj);
            }
        }


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Company> objCompanyList = _UnitOfWork.Company.GetAll().ToList();
			return Json(new { data = objCompanyList });
		}


		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var companyToBeDeleted = _UnitOfWork.Company.Get(u => u.Id == id);
			if (companyToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			_UnitOfWork.Company.Remove(companyToBeDeleted);
			_UnitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}
		#endregion
	}
}
