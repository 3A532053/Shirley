﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShirleyBook.DataAccess.Data;
using ShirleyBook.DataAccess.Repository.IRepository;
using ShirleyBook.Models;
using ShirleyBook.Models.ViewModels;
using ShirleyBook.Utility;

namespace ShirleyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return View(objCompanyList);
        }
        public IActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company companyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyObj);
            }
        }

        [HttpPost]
        public IActionResult Upsert(Company CompanyObj)
        {
            if (ModelState.IsValid)
            {
                if (CompanyObj.Id == 0)
                {
                    _unitOfWork.Company.Add(CompanyObj);
                }
                else
                {
                    _unitOfWork.Company.Update(CompanyObj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(CompanyObj);
            }

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll() 
        {
            List<Company> objCompanyList = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = objCompanyList });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDelete = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDelete == null) 
            {
                return Json(new { success = false, message = "Error while deleting!" });
            }

            _unitOfWork.Company.Remove(CompanyToBeDelete);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful!" });
        }

        #endregion
    }
}
