using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.AddressAccount
{
    public interface IAddressAccountHandler
    {
        /// <summary>
        /// lấy ra danh sách địa chỉ của người dùng 
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        Task<Response> GetAllAddressAccount(PageModel model);


        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// thêm mới địa chỉ người dùng 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> InsertAddressAccount(AddressAccountModel model);

        /// <summary>
        /// cập nhật địa chỉ người dùng 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> UpdateAddressAccount(AddressAccountModel model);
        
        /// <summary>
        /// xóa địa chỉ người dùng 
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Response> DeleteAddressAccount(Guid? addressId);

        /// <summary>
        /// lấy ra danh sách địa chỉ theo Id account
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Response> GetAddressAccountById(Guid? accountId);

        /// <summary>
        /// lấy ra danh sách địa chỉ theo accountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Response> GetAddressByAccountId(Guid? accountId);
    }
}
