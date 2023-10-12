using Business.AddressAccount;
using Business.Category;
using Data;
using Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Payment
{
    public class PaymentHandler : IPaymentHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<PaymentHandler> _logger;

        public PaymentHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<PaymentHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> DeletePayment(Guid? paymentId)
        {
            try
            {
                if (paymentId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường paymentId không được để trống");
                }

                var data = await _myDbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId.Equals(paymentId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Dữ liệu trống!");
                }

                _myDbContext.Payments.Remove(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return new ResponseObject<Guid?>(paymentId, $"{Message.DeleteSuccess}", Code.Success);
                }
                return new ResponseError(Code.ServerError, $"{Message.DeleteError}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAllPayment(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Payments.ToListAsync();
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

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Payments, PaymentModel>(data);
                return new ResponseObject<List<PaymentModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetPaymentById(Guid? paymentId)
        {
            try
            {
                if(paymentId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường paymentId không được để trống");
                }

                var data = await _myDbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId.Equals(paymentId));
                if(data ==  null)
                {
                    return new ResponseError(Code.BadRequest, "Dữ liệu trống!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Payments, PaymentModel>(data);
                return new ResponseObject<PaymentModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> InsertPayment(PaymentModel model)
        {
            try
            {
                model.CreatedDate = DateTime.Now;
                var dataMap = AutoMapperUtils.AutoMap<PaymentModel, Payments>(model);
                _myDbContext.Payments.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return new ResponseObject<PaymentModel>(model, $"{Message.CreateSuccess}", Code.Success);
                }
                return new ResponseError(Code.ServerError, $"{Message.CreateError}");
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
                var data = await _myDbContext.Payments.ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("paymentid_asc") => data.OrderBy(x => x.PaymentId).ToList(),
                    var t when t.Equals("paymentid_desc") => data.OrderByDescending(x => x.PaymentId).ToList(),
                    var t when t.Equals("create_asc") => data.OrderBy(x => x.CreatedDate).ToList(),
                    var t when t.Equals("create_desc") => data.OrderByDescending(x => x.CreatedDate).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Payments, PaymentModel>(data);
                return new ResponseObject<List<PaymentModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdatePayment(PaymentModel model)
        {
            try
            {
                if(model.PaymentId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường PaymentId không được để trống!");
                }

                var data = await _myDbContext.Payments.FirstOrDefaultAsync(x => x.PaymentId == model.PaymentId);
                if(data == null)
                {
                    return new ResponseError(Code.ServerError, "Dữ liệu trống!");
                }

                data.PaymentId = model.PaymentId;
                data.Type = model.Type;
                data.Amount = model.Amount;
                data.Bank = model.Bank;
                data.Image = model.Image;
                data.CreatedDate = model.CreatedDate;
                data.UpdatedDate = DateTime.Now;

                _myDbContext.Payments.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    _logger.LogInformation("Cập nhật thành công!");
                    return new ResponseObject<PaymentModel>(model, $"{Message.UpdateSuccess}", Code.Success);
                }

                _logger.LogError($"Cập nhât thất bại {model}");
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
