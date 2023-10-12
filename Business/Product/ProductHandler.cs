using Microsoft.Extensions.Configuration;
using Business.Category;
using Data;
using Data.DataModel;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace Business.Product
{
    public class ProductHandler : IProductHandler
    {

        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<ProductHandler> _logger;

        public ProductHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<ProductHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }
        public async Task<Response> getAllProduct(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Product.Include(c => c.Category).ToListAsync();
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
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> getProductById(Guid? productId)
        {
            try
            {
                if (productId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường productId không được để trống!");
                }

                var data = await _myDbContext.Product.Include(c => c.Category).FirstOrDefaultAsync(x => x.ProductId.Equals(productId));
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin sản phẩm!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<ProductCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> searchProduct(PageModel model, string? name, int? quantity, float? priceMin, float? priceMax, string? address, bool status, Guid? categoryId)
        {
            try
            {
                string sql = "SELECT * FROM products WHERE status = 1";
                if (name != null)
                {
                    sql += " AND lower(productName) LIKE N'%" + name.ToLower() + "%'";
                }
                if (quantity != null && quantity >= 0)
                {
                    sql += " AND quantity = " + quantity;
                }
                if (priceMin != null && priceMin >= 0)
                {
                    sql += " AND salePrice >= " + priceMin;
                }
                if (priceMax != null && priceMax >= 0)
                {
                    sql += " AND salePrice <= " + priceMax;
                }
                if (address != null)
                {
                    sql += " AND lower(address) LIKE '%" + address.ToLower() + "%'";
                }
                if (categoryId != null)
                {
                    sql += " AND categoryId = '" + categoryId + "'";
                }

                var data = await _myDbContext.Product.FromSqlRaw(sql).ToListAsync();
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tìm thấy sản phẩm!");
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

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
        public async Task<Response> CreateProduct(ProductCreateModel ProductModel)
        {
            try
            {
                var validation = new ValidationProductModel();
                var result = await validation.ValidateAsync(ProductModel);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                if (ProductModel.Image != null)
                {
                    /// Convert the base64 string back to bytes
                    byte[] imageBytes = Convert.FromBase64String(ProductModel.Image);

                    // Save the file to a location or process it as needed.
                    // For example, you can save it to the "wwwroot/images" folder:
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    ProductModel.Image = uniqueFileName;
                }

                var checkData = await _myDbContext.Product.FirstOrDefaultAsync(x => x.ProductName.ToLower().Equals(ProductModel.ProductName.ToLower()));
                if (checkData != null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin đã tồn tại trong hệ thống");
                }

                ProductModel.CreatedDate = DateTime.Now;

                var dataMap = AutoMapperUtils.AutoMap<ProductCreateModel, Data.DataModel.Product>(ProductModel);
                _myDbContext.Product.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    _logger.LogInformation("Thêm mới sản phẩm thành công", ProductModel);
                    return new ResponseObject<ProductCreateModel>(ProductModel, "Thêm mới sản phẩm thành công", Code.Success);
                }
                else
                {
                    _logger.LogError("Thêm mới sản phẩm thất bại", ProductModel);
                    return new ResponseError(Code.ServerError, "Thêm mới sản phẩm thất bại");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{Message.CreateError} - {ex.Message}");
            }
        }

        public async Task<Response> UpdateProduct(ProductCreateModel ProductModel)
        {
            try
            {
                var validation = new ValidationProductModel();
                var result = await validation.ValidateAsync(ProductModel);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                var data = await _myDbContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(ProductModel.ProductId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Sản phẩm không tồn tại");
                }
                else
                {
                    data.ProductId = ProductModel.ProductId;
                    data.ProductName = ProductModel.ProductName;
                    data.Address = ProductModel.Address;
                    data.Price = ProductModel.Price;
                    data.SalePrice = ProductModel.SalePrice;
                    data.Quantity = ProductModel.Quantity;
                    data.Status = ProductModel.Status;
                    data.Description = ProductModel.Description;
                    data.CategoryId = ProductModel.CategoryId;
                    data.UpdatedDate = DateTime.Now;

                    if (ProductModel.Image != null)
                    {
                        /// Convert the base64 string back to bytes
                        byte[] imageBytes = Convert.FromBase64String(ProductModel.Image);

                        // Save the file to a location or process it as needed.
                        // For example, you can save it to the "wwwroot/images" folder:
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        data.Image = uniqueFileName;
                    }

                    _myDbContext.Product.Update(data);
                    int rs = await _myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                        return new ResponseObject<ProductCreateModel>(ProductModel, $"{Message.UpdateSuccess}", Code.Success);
                    }

                    return new ResponseError(Code.ServerError, $"{Message.UpdateError}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> DeleteProduct(Guid? ProductId)
        {
            try
            {
                if (ProductId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường ProductId không được để trống!");
                }

                var data = await _myDbContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(ProductId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Sản phẩm không tồn tại trong hệ thống!");
                }
                data.Status = false;

                _myDbContext.Product.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<Guid?>(ProductId, $"Xóa sản phẩm thành công : {ProductId}", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Xóa sản phẩm thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetProductByCate(Guid? cateId)
        {
            try
            {
                if(cateId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường CateId không được để trống!");
                }

                var dataproduct = await _myDbContext.Product.Where(x => x.CategoryId == cateId).ToListAsync();
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(dataproduct);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdateProductQuantity(Guid? productId, int? quantity)
        {
            try
            {
                if (productId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường productId không được để trống!");
                }
                if (quantity == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường quantity không được để trống!");
                }

                var data = await _myDbContext.Product.FirstOrDefaultAsync(x => x.ProductId.Equals(productId));

                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin sản phẩm!");
                }
                else
                {
                    int? newQuantity = data.Quantity - quantity;

                    if (newQuantity < 0)
                    {
                        return new ResponseError(Code.BadRequest, "Không đủ sản phẩm trong kho! Còn " + quantity + " sản phẩm!");
                    }
                    else
                    {
                        data.Quantity = newQuantity;
                    }

                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<ProductCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
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
                var data = await _myDbContext.Product.Include(c => c.Category).ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("productid_asc") => data.OrderBy(x => x.ProductId).ToList(),
                    var t when t.Equals("productid_desc") => data.OrderByDescending(x => x.ProductId).ToList(),
                    var t when t.Equals("productname_asc") => data.OrderBy(x => x.ProductName).ToList(),
                    var t when t.Equals("productname_desc") => data.OrderByDescending(x => x.ProductName).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> getAllProductForUser(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Product.Include(c => c.Category).Where(p => p.Status == true).ToListAsync();
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
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetNewProducts(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Product.Include(c => c.Category).Where(p => p.Status == true).OrderByDescending(p => p.CreatedDate).ToListAsync();
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
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetProductOrderByQuantityDesc(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Product.Include(c => c.Category).Where(p => p.Status == true).OrderByDescending(p => p.Quantity).ToListAsync();
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
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Product, ProductCreateModel>(data);
                return new ResponseObject<List<ProductCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
    }
}
