using Business.Account;
using Business.Category;
using Data.DataModel;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace eProject_Sem4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1", IgnoreApi = false)]
    public class AccountController : ControllerBase
    {
        private IAccountHandler _accountHandler;
        private readonly IEasyCachingProviderFactory _cacheFactory;

        public AccountController(IAccountHandler accountHandler, IEasyCachingProviderFactory cacheFactory)
        {
            _accountHandler = accountHandler;
            _cacheFactory = cacheFactory;
        }

        /// <summary>
        ///  lấy ra danh scahs tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getall-account")]
        [ProducesResponseType(typeof(ResponseObject<List<AccountCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAccount([FromQuery] PageModel model)
        {
            return Ok(await _accountHandler.GetAllAccount(model));
        }

        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sortby-account")]
        [ProducesResponseType(typeof(ResponseObject<List<AccountCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SortByAccount([FromQuery] string sort)
        {
            return Ok(await _accountHandler.SortBy(sort));
        }

        /// <summary>
        ///  đăng ký tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("signup-account")]
        [ProducesResponseType(typeof(ResponseObject<AccountCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SignUpAccount([FromBody] AccountCreateModel model)
        {
            return Ok(await _accountHandler.SingUpAccount(model));
        }

        /// <summary>
        /// đăng nhập
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResponseObject<AccountModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(AccountModel model)
        {
            return Ok(await _accountHandler.Login(model));
        }

        /// <summary>
        /// cập nhật tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("update-account")]
        [ProducesResponseType(typeof(ResponseObject<AccountCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAccount(AccountCreateModel model)
        {
            return Ok(await _accountHandler.UpdateAccount(model));
        }

        /// <summary>
        /// xóa tài khoản người dùng
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("delete-account")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteAccount(Guid? accountId)
        {
            return Ok(await _accountHandler.DeleteAccount(accountId));
        }

        /// <summary>
        /// lấy tài khoản theo Id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-account-by-id")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccountById(Guid? accountId)
        {
            return Ok(await _accountHandler.GetAccountById(accountId));
        }

        /// <summary>
        /// lấy thông tin người dùng theo email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-account-by-email")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccountByEmail(string email)
        {
            //var cahekey = $"EMAIL_{email}";
            //var provider = _cacheFactory.GetCachingProvider("default");
            //var cacheResult = await provider.GetAsync<Response>(cahekey);
            //if (cacheResult != null && cacheResult.HasValue)
            //{
            //    return Ok(cacheResult.Value);
            //}
            //var result = 
            //if (result.Code == Code.Success)
            //{
            //    await provider.SetAsync(cahekey, result, TimeSpan.FromMinutes(10));
            //}
            return Ok(await _accountHandler.GetAccountByEmail(email));
        }

        /// <summary>
        /// lấy pascode
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-access-code")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccessCode(string email)
        {
            return Ok(await _accountHandler.GetAccessCode(email));
        }

        /// <summary>
        /// check pascode
        /// </summary>
        /// <param name="passcode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check-pass-code")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckPassCode(Guid? accountId, string passcode)
        {
            return Ok(await _accountHandler.CheckPassCode(accountId, passcode));
        }

        /// <summary>
        /// thay đổi mật khẩu
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("change-password")]
        [ProducesResponseType(typeof(ResponseObject<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(Guid? userId, string password)
        {
            return Ok(await _accountHandler.ChangePassword(userId, password));
        }

        [HttpGet]
        [Route("get-account-image")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public IActionResult GetImage(string image)
        {
            Byte[] b = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\" + image);
            return File(b, "image/jpeg");
        }

        [HttpPost]
        [Route("update-account-image")]
        [ProducesResponseType(typeof(ResponseObject<AccountCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateAccountImage(string id, string imageBase64Str)
        {
            return Ok(await _accountHandler.UpdateAccountImage(id, imageBase64Str));
        }
    }
}