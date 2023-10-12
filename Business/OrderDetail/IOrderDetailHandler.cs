using Business.Order;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.OrderDetail
{
    public interface IOrderDetailHandler
    {
        /// <summary>
        /// Lấy danh sách OrderDetail
        /// </summary>
        /// <returns></returns>
        Task<Response> getAllOrderDetail(PageModel model);

        /// <summary>
        /// Lấy OrderDetail theo Id
        /// </summary>
        /// <returns></returns>
        Task<Response> getOrderDetailByOrderId(Guid? OrderId);

        /// <summary>
        /// Lọc OrderDetail
        /// </summary>
        /// <returns></returns>
        Task<Response> searchOrderDetail(PageModel model);


        /// <summary>
        /// thêm mới OrderDetail
        /// </summary>
        /// <param name="OrderDetailModel"></param>
        /// <returns></returns>

        Task<Response> CreateOrderDetail(OrderDetailCreateModel OrderDetailModel);
        /// <summary>
        /// cập nhật OrderDetail
        /// </summary>
        /// <param name="OrderDetailModel"></param>
        /// <returns></returns>
        Task<Response> UpdateOrderDetail(OrderDetailCreateModel OrderDetailModel);


        /// <summary>
        /// xóa OrderDetail
        /// </summary>
        /// <param name="OrderDetailId"></param>
        /// <returns></returns>
        Task<Response> DeleteOrderDetail(Guid? OrderDetailId);


        /// <summary>
        /// xóa OrderDetail
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        Task<Response> DeleteOrderDetailByOrderId(Guid? OrderId);
    }
}
