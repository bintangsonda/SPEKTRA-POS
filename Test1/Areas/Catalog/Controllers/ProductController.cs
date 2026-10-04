using AppCode;
using General.Catalog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace SPOS.Web.UI.Areas.Catalog.Controllers
{
    [Area("Catalog")]
    public class ProductController : IDS.Web.UI.Controllers.MenuController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private string sUser;
        private string sUserGroup;
        private string sBranchCode;
        private string sHO;
        private readonly IWebHostEnvironment _env;

        public ProductController(IHttpContextAccessor contextAccessor, IWebHostEnvironment env) : base(contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _env = env;
            sUser = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_ID);
            sUserGroup = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_GROUP_CODE);
            sBranchCode = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_CODE);
            sHO = _contextAccessor?.HttpContext?.Session.GetString(IDS.Tool.GlobalVariable.SESSION_USER_BRANCH_HO_STATUS);
            _env = env;
        }
        public ActionResult Index()
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.CreateAccess == -1 || AccessLevel.EditAccess == -1 || AccessLevel.DeleteAccess == -1)
            {
                return RedirectToAction("Index", "Main", new { Area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["FormAction"] = 1;
            ViewData["BranchList"] = new SelectList(IDS.GeneralTable.Branch.GetBranchForDatasource(), "Value", "Text", sBranchCode.ToString());
            ViewData["CategoriesList"] = new SelectList(General.Catalog.Categories.GetCboCategories (), "Value", "Text", sBranchCode.ToString());


            ViewBag.UserMenu = MainMenu;

            return View();
        }

        public JsonResult GetData(string BranchFilter,string CategoriesFilter)
        {
            if (sUser == null)
                throw new Exception("You do not have access to access this page");

            JsonResult result = new JsonResult(new { });

            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var orderColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{orderColumnIndex}][name]"].FirstOrDefault();
                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int totalRecords = 0; // Total keseluruhan data
                int totalRecordsShowing = 0; // Total data setelah filter / search

                List<Product> datalist = Product.GetData(BranchFilter, CategoriesFilter,"");
                if (datalist != null)
                {
                    totalRecords = datalist.Count;


                    // Sorting    
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        datalist = datalist.AsQueryable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                    }

                    // Search    
                    if (!string.IsNullOrEmpty(searchValue))
                    {
                        string searchValueLower = searchValue.ToLower();

                        datalist = datalist.Where(x => x.ProdCode.ToLower().Contains(searchValueLower) ||
                                                 x.Branch.ToLower().Contains(searchValueLower) ||
                                                 x.CategoriesCode.ToString().Contains(searchValueLower) ||
                                                 x.ProdName.ToString().Contains(searchValueLower) ||
                                                 x.Description.ToString().Contains(searchValueLower) ||                                              
                                                 x.OperatorID.ToString().ToLower().Contains(searchValueLower) ||
                                                 x.LastUpdate.ToString(IDS.Tool.GlobalVariable.DEFAULT_DATETIME_FORMAT).ToLower().Contains(searchValueLower)
                                                 ).ToList();
                    }

                    totalRecordsShowing = datalist.Count();

                    // Paging
                    if (pageSize > 0)
                        datalist = datalist.Skip(skip).Take(pageSize).ToList();

                    // Returning Json Data
                    result = this.Json(new { draw = draw, recordsFiltered = totalRecordsShowing, recordsTotal = totalRecords, data = datalist });
                }
            }
            catch
            {
                throw;
            }

            return result;
        }

        [HttpPost]
        public async Task<ActionResult> Create(
        IFormFile ImageFile,
        string ProdCode,
        string Branch,
        string CategoriesCode,
        string ProdName,
        string Description,
        string Status,
        decimal Harga,
        string VarianProduct,
        int FormAction)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["MessageFromExcel"] = "";

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }
                try
                {
                   
                    Product dataModal =new Product();
                    dataModal.ProdCode = ProdCode;
                    dataModal.Branch = Branch;
                    dataModal.CategoriesCode = CategoriesCode;
                    dataModal.ProdName = ProdName;
                    dataModal.Description = Description;
                    dataModal.Status = (Status == "true" || Status == "True" || Status == "1") ? true : false;
                    dataModal.Harga = Harga;
                    dataModal.OperatorID = currentUser;
                    if (!string.IsNullOrEmpty(VarianProduct))
                    {
                        var variantList = JsonConvert.DeserializeObject<List<VariantHeader>>(VarianProduct);
                        dataModal.VarianProduct = variantList;
                    }
                    string folderName = ProdCode + "-" + Branch + "-" + CategoriesCode;
                    string fileName = "";
                    if (ImageFile != null)
                    {
                        string originalExt = Path.GetExtension(ImageFile.FileName);
                        fileName = ImageFile.FileName.ToString() + originalExt;
                        string imageUrl = $"../../Uploads/ProductIMG/{folderName}/{fileName}";

                        dataModal.Image = imageUrl;
                    }

                    int result = dataModal.InsUpDel((int)FormAction);

                    if (result > 0)
                    {

                        if (ImageFile != null && ImageFile.Length > 0)
                        {
                            // Folder dasar: wwwroot/Uploads/ProductIMG
                            string baseFolder = Path.Combine(_env.WebRootPath, "Uploads", "ProductIMG");

                            // Nama folder produk

                            // Path folder produk
                            string productFolder = Path.Combine(baseFolder, folderName);

                            // 1. Create folder jika belum ada
                            if (!Directory.Exists(productFolder))
                            {
                                Directory.CreateDirectory(productFolder);
                            }
                            else
                            {
                                // 2. Hapus file lama dalam folder
                                foreach (var file in Directory.GetFiles(productFolder))
                                {
                                    System.IO.File.Delete(file);
                                }
                            }

                            // 3. Generate nama file unik berdasarkan file asli
                        

                            // Full path simpan file
                            string filePath = Path.Combine(productFolder, fileName);

                            // 4. Save file fisik ke folder
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await ImageFile.CopyToAsync(stream);
                            }

                            
                        }

                        if ((int)FormAction == 1)
                        {
                            if (string.IsNullOrWhiteSpace(dataModal.ProdCode))
                                return Json(new { msg = "Failed, data is empty!", success = 0 });
                            else
                                return Json(new { msg = "New Product has been save. Product with Code: " + dataModal.ProdCode+", Outlet: "+dataModal.Branch+", Category: ", success = 1, sno = dataModal.ProdCode });
                        }
                        else
                        {
                            return Json(new { msg = "Edit Product has been save.", success = 1 });
                        }
                    }
                    else
                        throw new Exception("Error");
                }
                catch (Exception ex)
                {
                    return Json(new { msg = ex.Message, success = 0 });
                }
            }
            else
            {
                return Json(new { msg = "Failed!, Not Valid Mode!", success = 0 });
            }
        }

        public ActionResult Delete(Product DataModal)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ModelState.Clear();
            if (DataModal != null)
            {
                if (ModelState.IsValid)
                {
                    string branch = sBranchCode.ToString();
                    string currentUser = sUser as string;

                    if (string.IsNullOrWhiteSpace(currentUser))
                    {
                        return Json("SessionTimeOut");
                    }

                    try
                    {
                        DataModal.OperatorID = currentUser;
                        DataModal.ProdName = "Empty";
                        DataModal.Status = false;
                        DataModal.Description = "Empty";
                        DataModal.Harga = 0;

                        int result = DataModal.InsUpDel(3);
                        if (result < 1)
                        {
                            return Json(new { msg = "Failed to Delete Product Code: " + DataModal.ProdCode + ", Outlet: " + DataModal.Branch + ", Category: ", success = 0 });
                        }
                        string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "ProductIMG");
                        string productFolder = Path.Combine(rootPath, DataModal.ProdCode+"-"+DataModal.Branch);
                        if (Directory.Exists(productFolder))
                        {
                            // 2. Hapus semua file di dalam folder
                            var files = Directory.GetFiles(productFolder);
                            foreach (var file in files)
                            {
                                System.IO.File.Delete(file);
                            }

                            // 3. Hapus foldernya
                            Directory.Delete(productFolder);
                        }
                        return Json(new { msg = "success Delete Data", success = 1 });
                    }
                    catch (Exception ex)
                    {
                        return Json(new { msg = ex.Message, success = 0 });
                    }
                }
                else
                {
                    return Json(new { msg = "Failed!, Not Valid Mode!", success = 0 });
                }
            }
            else
            {
                return Json(new { msg = "Failed!, Not Valid Mode!" });
            }
        }

        public ActionResult Edit(string ProdCode, string Branch)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(sUserGroup ?? "", this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.EditAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;

            ViewData["FormAction"] = 2;
            Product data = Product.GetDataEditProduct(ProdCode, Branch);
            if (data != null)
            {
                return Json(data);
            }
            return null;
        }

        public JsonResult GetDataVarianProduct()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            list = Product.GetDataVarianProduct();
            return Json(list);
        }

        public async Task<ActionResult> Duplicate(string ProdCode,string Outlet, string OutletTo)
        {
            if (sUser == null)
                return RedirectToAction("index", "Main", new { area = "" });

            IDS.Web.UI.Models.GroupAccessLevel AccessLevel = IDS.Web.UI.Models.GroupAccessLevel.GetFormGroupAccess(Convert.ToString(sUserGroup), this.ControllerContext.RouteData.Values["controller"].ToString());

            if (AccessLevel.ReadAccess == -1 || AccessLevel.CreateAccess == 0)
            {
                return RedirectToAction("error403", "error", new { area = "" });
            }

            ViewData["Page.Insert"] = AccessLevel.CreateAccess;
            ViewData["Page.Edit"] = AccessLevel.EditAccess;
            ViewData["Page.Delete"] = AccessLevel.DeleteAccess;
            ViewData["MessageFromExcel"] = "";

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                string currentUser = sUser as string;

                if (string.IsNullOrWhiteSpace(currentUser))
                {
                    return Json("SessionTimeOut");
                }
                try
                {

                    Product dataModal = new Product();
                    int result = dataModal.DuplicateProduct(ProdCode,Outlet,OutletTo, currentUser);

                    if (result>0)
                    {
                        return Json(new { msg = "Duplicate Success, Product with Code: " + ProdCode + ", Outlet: " + Outlet + ", to Outlet: " + OutletTo, success = 1 });
                    }
                    else
                    {
                        return Json(new { msg = "Duplicate Success to Outlet"+OutletTo, success = 1 });
                    }
                    
                }
                catch (Exception ex)
                {
                    return Json(new { msg = ex.Message, success = 0 });
                }
            }
            else
            {
                return Json(new { msg = "Failed!, Not Valid Mode!", success = 0 });
            }
        }
    }
}
