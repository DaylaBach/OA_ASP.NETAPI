using Business.Product;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;

namespace Business.Category
{
    public class CategoryHandler : ICategoryHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<CategoryHandler> _logger;

        public CategoryHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<CategoryHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> getAllCategory(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Category.ToListAsync();
                if (model.PageSize.HasValue && model.PageNumber.HasValue)
                {
                    if (model.PageSize <= 0)
                    {
                        model.PageSize = 0;
                    }

                    int excludeRows = (model.PageNumber.Value - 1) * (model.PageSize.Value);
                    if (excludeRows <= 0)
                    {
                        excludeRows = 0;
                    }
                    data = data.Skip(excludeRows).Take(model.PageSize.Value).ToList();
                }
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Category, CategoryCreateModel>(data);
                return new ResponseObject<List<CategoryCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> getCategoryById(Guid? categoryId)
        {
            try
            {
                if (categoryId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường categoryId không được để trống!");
                }

                var data = await _myDbContext.Category.FirstOrDefaultAsync(x => x.CategoryId.Equals(categoryId));
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin danh mục!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Category, CategoryCreateModel>(data);
                return new ResponseObject<CategoryCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> searchCategory(PageModel model, string name, bool status)
        {
            int bitStatus;
            if (status == true)
            {
                bitStatus = 1;
            }
            else
            {
                bitStatus = 0;
            }
            try
            {
                string sql = "SELECT * FROM categories WHERE 1=1";
                if (name != null)
                {
                    sql += " AND lower(categoryName) LIKE N'%" + name.ToLower() + "%'";
                }
                if (bitStatus >= 0 && bitStatus < 2)
                {
                    sql += " AND status = " + bitStatus;
                }

                var data = await _myDbContext.Category.FromSqlRaw(sql).ToListAsync();
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tìm thấy danh mục!");
                }

                if (model.PageSize.HasValue && model.PageNumber.HasValue)
                {
                    if (model.PageSize <= 0)
                    {
                        model.PageSize = 0;
                    }

                    int excludeRows = (model.PageNumber.Value - 1) * (model.PageSize.Value);
                    if (excludeRows <= 0)
                    {
                        excludeRows = 0;
                    }
                    data = data.Skip(excludeRows).Take(model.PageSize.Value).ToList();
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Category, CategoryCreateModel>(data);
                return new ResponseObject<List<CategoryCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> CreateCategory(CategoryCreateModel CategoryModel)
        {
            try
            {
                var validation = new ValidationCategoryModel();
                var result = await validation.ValidateAsync(CategoryModel);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }
                if (CategoryModel.Image != null)
                {
                    /// Convert the base64 string back to bytes
                    byte[] imageBytes = Convert.FromBase64String(CategoryModel.Image);

                    // Save the file to a location or process it as needed.
                    // For example, you can save it to the "wwwroot/images" folder:
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_image.png"; 
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    CategoryModel.Image = uniqueFileName;
                }

                var checkData = await _myDbContext.Category.FirstOrDefaultAsync(x => x.CategoryName.ToLower().Equals(CategoryModel.CategoryName.ToLower()));
                if (checkData != null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin đã tồn tại trong hệ thống");
                }

                CategoryModel.CreatedDate = DateTime.Now;

                var dataMap = AutoMapperUtils.AutoMap<CategoryCreateModel, Data.DataModel.Category>(CategoryModel);
                _myDbContext.Category.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    _logger.LogInformation("Thêm mới danh mục thành công", CategoryModel);
                    return new ResponseObject<CategoryCreateModel>(CategoryModel, "Thêm mới danh mục thành công", Code.Success);
                }
                else
                {
                    _logger.LogError("Thêm mới danh mục thất bại", CategoryModel);
                    return new ResponseError(Code.ServerError, "Thêm mới danh mục thất bại");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{Message.CreateError} - {ex.Message}");
            }
        }

        public async Task<Response> UpdateCategory(CategoryCreateModel CategoryModel)
        {
            try
            {
                var validation = new ValidationCategoryModel();
                var result = await validation.ValidateAsync(CategoryModel);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                var data = await _myDbContext.Category.FirstOrDefaultAsync(x => x.CategoryId.Equals(CategoryModel.CategoryId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Danh mục không tồn tại");
                }

                data.CategoryId = CategoryModel.CategoryId;
                data.CategoryName = CategoryModel.CategoryName;
                data.Status = CategoryModel.Status;
                data.UpdatedDate = DateTime.Now;


                if (CategoryModel.Image != null)
                {
                    /// Convert the base64 string back to bytes
                    byte[] imageBytes = Convert.FromBase64String(CategoryModel.Image);

                    // Save the file to a location or process it as needed.
                    // For example, you can save it to the "wwwroot/images" folder:
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    data.Image = uniqueFileName;
                }

                _myDbContext.Category.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<CategoryCreateModel>(CategoryModel, $"{Message.UpdateSuccess}", Code.Success);
                }

                return new ResponseError(Code.ServerError, $"{Message.UpdateError}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> DeleteCategory(Guid? CategoryId)
        {
            try
            {
                if (CategoryId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường CategoryId không được để trống!");
                }

                var data = await _myDbContext.Category.FirstOrDefaultAsync(x => x.CategoryId.Equals(CategoryId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Danh mục không tồn tại trong hệ thống!");
                }

                data.Status = false;

                _myDbContext.Category.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    var proData = await _myDbContext.Product.Where(p => p.CategoryId == data.CategoryId).ToListAsync();
                    if(proData != null)
                    {
                        proData.ForEach(p => p.Status = false);
                        _myDbContext.Product.UpdateRange(proData);
                        await _myDbContext.SaveChangesAsync();
                        return new ResponseObject<Guid?>(CategoryId, $"Xóa danh mục thành công : {CategoryId}", Code.Success);
                    }
                }
                return new ResponseError(Code.ServerError, "Xóa danh mục thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> SortBy(string sort)
        {
            try
            {
                var data = await _myDbContext.Category.ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("sortid_asc") =>  data.OrderBy(x => x.CategoryId).ToList(),
                    var t when t.Equals("sortid_desc") => data.OrderByDescending(x => x.CategoryId).ToList(),
                    var t when t.Equals("sortname_asc") => data.OrderBy(x => x.CategoryName).ToList(),
                    var t when t.Equals("sortname_desc") => data.OrderByDescending(x => x.CategoryName).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Category, CategoryCreateModel>(data);
                return new ResponseObject<List<CategoryCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
    }
}