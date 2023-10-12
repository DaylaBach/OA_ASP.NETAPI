using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Payment
{
    public interface IPaymentHandler
    {
        /// <summary>
        /// Lấy ra danh sách loại hình thanh toán 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> GetAllPayment(PageModel model);


        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// thêm mới loại hình thanh toán 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> InsertPayment(PaymentModel model);

        /// <summary>
        /// cập nhật loại hình thanh toán 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> UpdatePayment(PaymentModel model);

        /// <summary>
        /// xóa loại hình thanh toán 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        Task<Response> DeletePayment(Guid? paymentId);

        /// <summary>
        /// lấy chi tiết loại hình 
        /// </summary>
        /// <param name="paymentId"></param>
        /// <returns></returns>
        Task<Response> GetPaymentById(Guid? paymentId);
    }
}
