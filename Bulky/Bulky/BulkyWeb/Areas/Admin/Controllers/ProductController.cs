﻿using Bulky.DataAccess.Data;
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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment WebHostEnvironment)
        {
            _UnitOfWork = unitOfWork;
            _WebHostEnvironment = WebHostEnvironment;

        }
        public IActionResult Index()
        {
            List<Product> objProductlist = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return View(objProductlist);
        }

        public IActionResult Upsert(int? id)
        {

            //ViewBag.CategoryList = CategoryList;
            ProductVM productVM = new() {

                CategoryList = _UnitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                //create
                return View(productVM);
            }
            else
            {
                //update
                productVM.Product = _UnitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _WebHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        //delete old image
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _UnitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _UnitOfWork.Product.Update(productVM.Product);
                }

                _UnitOfWork.Save();
                TempData["success"] = " created successfully";
                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _UnitOfWork.Category.GetAll()
               .Select(u => new SelectListItem
               {
                   Text = u.Name,
                   Value = u.Id.ToString()
               }); 
                return View(productVM);
            }

        }


		#region API CALLS
		[HttpGet]
		public IActionResult GetAll()
		{
			List<Product> objProductList = _UnitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProductList });
		}


		[HttpDelete]
		public IActionResult Delete(int? id)
		{
			var productToBeDeleted = _UnitOfWork.Product.Get(u => u.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			var oldImagePath =
						   Path.Combine(_WebHostEnvironment.WebRootPath,
						   productToBeDeleted.ImageUrl.TrimStart('\\'));

			if (System.IO.File.Exists(oldImagePath))
			{
				System.IO.File.Delete(oldImagePath);
			}

			_UnitOfWork.Product.Remove(productToBeDeleted);
			_UnitOfWork.Save();

			return Json(new { success = true, message = "Delete Successful" });
		}
		#endregion
	}
}
