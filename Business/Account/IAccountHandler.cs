using Shared;

namespace Business.Account
{
    public interface IAccountHandler
    {
        /// <summary>
        /// lấy ra danh sách tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> GetAllAccount(PageModel model);


        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// đăng ký tài khoản người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> SingUpAccount(AccountCreateModel model);

        /// <summary>
        /// đăng nhập 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> Login(AccountModel model);

        /// <summary>
        /// cập nhật tài khoản người dùng
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> UpdateAccount(AccountCreateModel model);

        /// <summary>
        /// xóa tài khoản người dùng
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Response> DeleteAccount(Guid? accountId);

        /// <summary>
        ///  lấy account theo Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        Task<Response> GetAccountById(Guid? accountId);

        /// <summary>
        /// dưới app sẽ decode Token ra truyền email lên để lấy thông tin người dùng
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Response> GetAccountByEmail(string email);

        /// <summary>
        /// luồng quên mật khẩu lấy mã  code
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<Response> GetAccessCode(string email);

        /// <summary>
        /// check passcode
        /// </summary>
        /// <param name="passcode"></param>
        /// <returns></returns>
        Task<Response> CheckPassCode(Guid? accountId ,string passcode);

        /// <summary>
        /// check passcode thay đổi mật khẩu
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<Response> ChangePassword(Guid? userId, string password);


        Task<Response> UpdateAccountImage(string id, string imageBase64Str);
    }
}