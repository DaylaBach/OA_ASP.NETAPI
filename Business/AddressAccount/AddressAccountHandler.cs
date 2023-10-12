using Business.Category;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using System.Reflection.Metadata.Ecma335;

namespace Business.AddressAccount
{
    public class AddressAccountHandler : IAddressAccountHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<AddressAccountHandler> _logger;

        public AddressAccountHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<AddressAccountHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> DeleteAddressAccount(Guid? addressId)
        {
            try
            {
                if (addressId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường addressId không được để trống!");
                }

                var data = await _myDbContext.AddressAccount.FirstOrDefaultAsync(x => x.AddressId == addressId);

                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Dữ liệu trống!");
                }

                _myDbContext.AddressAccount.Remove(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs == 0)
                {
                    return new ResponseError(Code.ServerError, "Xoá thất bại");
                }

                return new ResponseObject<Guid?>(addressId, "Xóa thành công", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAddressAccountById(Guid? addressId)
        {
            try
            {
                if (addressId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường addressId không được để trống");
                }

                var data = await _myDbContext.AddressAccount.Include(x => x.Account).FirstOrDefaultAsync(x => x.AddressId == addressId);
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Dữ liệu trống!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.AddressAccount, AddressAccountModel>(data);
                return new ResponseObject<AddressAccountModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAddressByAccountId(Guid? accountId)
        {
            try
            {
                if (accountId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường accountId không đươc để trống!");
                }

                var data = await _myDbContext.AddressAccount.Include(x => x.Account).Where(x => x.AccountId == accountId).ToListAsync();
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.AddressAccount, AddressAccountModel>(data);
                return new ResponseObject<List<AddressAccountModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAllAddressAccount(PageModel model)
      {
            try
            {
                var data = await _myDbContext.AddressAccount.Include(x => x.Account).ToListAsync();
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

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.AddressAccount, AddressAccountModel>(data);
                return new ResponseObject<List<AddressAccountModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> InsertAddressAccount(AddressAccountModel model)
        {
            try
            {
                if (model.AccountId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường AccountId không được để trống!");
                }

                model.CreatedDate = DateTime.Now;
                var dataMap = AutoMapperUtils.AutoMap<AddressAccountModel, Data.DataModel.AddressAccount>(model);
                _myDbContext.AddressAccount.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<AddressAccountModel>(model, $"{Message.IntegratedSuccess}", Code.Success);
                }

                return new ResponseError(Code.ServerError, $"{Message.IntegratedError}");
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
                var data = await _myDbContext.AddressAccount.Include(x => x.Account).ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("addressid_asc") => data.OrderBy(x => x.AddressId).ToList(),
                    var t when t.Equals("addressid_desc") => data.OrderByDescending(x => x.AddressId).ToList(),
                    var t when t.Equals("address_asc") => data.OrderBy(x => x.Address).ToList(),
                    var t when t.Equals("address_desc") => data.OrderByDescending(x => x.Address).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.AddressAccount, AddressAccountModel>(data);
                return new ResponseObject<List<AddressAccountModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdateAddressAccount(AddressAccountModel model)
        {
            try
            {
                var data = await _myDbContext.AddressAccount.FirstOrDefaultAsync(x => x.AddressId.Equals(model.AddressId));
                if(data == null)
                {
                    return new ResponseError(Code.BadRequest, $"{Message.DataNull}");
                }

                data.AddressId = model.AddressId;
                data.Address = model.Address;
                data.Image = model.Image;
                data.CreatedDate = model.CreatedDate;
                data.UpdatedDate = DateTime.Now;
                data.AccountId = model.AccountId;

                _myDbContext.AddressAccount.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return new ResponseObject<AddressAccountModel>(model, $"{Message.UpdateSuccess}", Code.Success);
                }

                return new ResponseError(Code.ServerError, $"{Message.UpdateError}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
    }
}