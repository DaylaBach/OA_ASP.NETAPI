using Microsoft.Extensions.Configuration;
using Business.Order;
using Data;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Data.DataModel;

namespace Business.OrderDetail
{
    public class OrderDetailHandler : IOrderDetailHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<OrderDetailHandler> _logger;

        public OrderDetailHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<OrderDetailHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> getAllOrderDetail(PageModel model)
        {
            try
            {
                var data = await _myDbContext.OrderDetail.ToListAsync();
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
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.OrderDetail, OrderDetailCreateModel>(data);
                return new ResponseObject<List<OrderDetailCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> getOrderDetailByOrderId(Guid? OrderId)
        {
            try
            {
                if (OrderId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường OrderId không được để trống!");
                }

                var data = await _myDbContext.OrderDetail.Include(x => x.Product).Where(x => x.OrderId.Equals(OrderId)).ToListAsync();
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin đơn hàng!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.OrderDetail, OrderDetailCreateModel>(data);
                return new ResponseObject<List<OrderDetailCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public Task<Response> searchOrderDetail(PageModel model)
        {
            throw new NotImplementedException();
        }
        public async Task<Response> CreateOrderDetail(OrderDetailCreateModel OrderDetailModel)
        {
            try
            {
                var dataMap = AutoMapperUtils.AutoMap<OrderDetailCreateModel, Data.DataModel.OrderDetail>(OrderDetailModel);
                _myDbContext.OrderDetail.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    _logger.LogInformation("Thêm mới chi tiết đơn hàng thành công", OrderDetailModel);
                    return new ResponseObject<OrderDetailCreateModel>(OrderDetailModel, "Thêm mới chi tiết đơn hàng thành công", Code.Success);
                }
                else 
                {
                    _logger.LogError("Thêm mới chi tiết đơn hàng thất bại", OrderDetailModel);
                    return new ResponseError(Code.ServerError, "Thêm mới chi tiết đơn hàng thất bại");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{Message.CreateError} - {ex.Message}");
            }
        }

        public async Task<Response> UpdateOrderDetail(OrderDetailCreateModel OrderDetailModel)
        {
            try
            {
                var data = await _myDbContext.OrderDetail.FirstOrDefaultAsync(x => x.Id.Equals(OrderDetailModel.Id));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "chi tiết đơn hàng không tồn tại");
                }
                else
                {
                    data.Id = OrderDetailModel.Id;
                    data.price = OrderDetailModel.price;
                    data.Quantity = OrderDetailModel.Quantity;
                    data.OrderId = OrderDetailModel.OrderId;
                    data.ProductId = OrderDetailModel.ProductId;

                    _myDbContext.OrderDetail.Update(data);
                    int rs = await _myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                        return new ResponseObject<OrderDetailCreateModel>(OrderDetailModel, $"{Message.UpdateSuccess}", Code.Success);
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

        public async Task<Response> DeleteOrderDetail(Guid? OrderDetailId)
        {
            try
            {
                if (OrderDetailId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường OrderDetailId không được để trống!");
                }

                var data = await _myDbContext.OrderDetail.FirstOrDefaultAsync(x => x.Id.Equals(OrderDetailId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "chi tiết đơn hàng không tồn tại trong hệ thống!");
                }
                _myDbContext.OrderDetail.Remove(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<Guid?>(OrderDetailId, $"Xóa chi tiết đơn hàng thành công : {OrderDetailId}", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Xóa chi tiết đơn hàng thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> DeleteOrderDetailByOrderId(Guid? OrderId)
        {
            try
            {
                if (OrderId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường OrderId không được để trống!");
                }

                var data = await _myDbContext.OrderDetail.FirstOrDefaultAsync(x => x.OrderId.Equals(OrderId));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "chi tiết đơn hàng không tồn tại trong hệ thống!");
                }
                _myDbContext.OrderDetail.Remove(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<Guid?>(OrderId, $"Xóa chi tiết đơn hàng thành công : {OrderId}", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Xóa chi tiết đơn hàng thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
    }
}
