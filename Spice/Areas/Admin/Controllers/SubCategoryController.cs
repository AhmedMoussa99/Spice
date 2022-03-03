using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        [TempData]
        public string StatusMessage { get; set; }
        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;

        }
        public async Task<IActionResult> Index()
        {
            return View(await _db.SubCategory.Include(x=>x.Category).ToListAsync());
        }
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                SubCategory = new Models.SubCategory()
            };
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
             
               var doesSubCategoryExsits = _db.SubCategory.Include(x => x.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if(doesSubCategoryExsits.Count()> 0)
                {
                    StatusMessage = "Error : Sub Category Exists Under" + doesSubCategoryExsits.First().Category.Name + "Category. Please Use Another Name";
                }
                else
                {
                    _db.SubCategory.Add(model.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelvm = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                SubCategory = new Models.SubCategory(),
                StatusMessage = StatusMessage
               
            };
            return View(modelvm);
        }
        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int Id)
        {
            List<SubCategory> SubCategories = new List<SubCategory>();
            SubCategories = await(from SubCategory in _db.SubCategory
                             where SubCategory.CategoryId == Id
                             select SubCategory).ToListAsync();
            return Json(new SelectList(SubCategories, "Id", "Name"));
        }
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == 0)
            {
                return NotFound();
            }
            var SubCategory = await _db.SubCategory.SingleOrDefaultAsync(x=>x.Id==Id);
            if (SubCategory == null)
            {
                return NotFound();
            }
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                SubCategory = SubCategory
            };
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {

                var doesSubCategoryExsits = _db.SubCategory.Include(x => x.Category).Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExsits.Count() > 0)
                {
                    StatusMessage = "Error : Sub Category Exists Under" + doesSubCategoryExsits.First().Category.Name + "Category. Please Use Another Name";
                }
                else
                {
                    var subCatFromDb = await _db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }

            SubCategoryAndCategoryViewModel modelvm = new SubCategoryAndCategoryViewModel
            {
                CategoryList = await _db.Category.ToListAsync(),
                SubCategoryList = await _db.SubCategory.OrderBy(x => x.Name).Select(x => x.Name).Distinct().ToListAsync(),
                SubCategory = new Models.SubCategory(),
                StatusMessage = StatusMessage

            };
            return View(modelvm);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = await _db.SubCategory.Include(s => s.Category).SingleOrDefaultAsync(m => m.Id == id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subCategory = await _db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);
            _db.SubCategory.Remove(subCategory);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}