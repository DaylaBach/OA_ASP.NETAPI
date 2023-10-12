using Business.Product;
using Business.User;
using EasyCaching.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace eProject_Sem4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1", IgnoreApi = false)]
    public class UserController : ControllerBase
    {

        private IUserHandler _userHandler;
        private readonly IEasyCachingProviderFactory _cacheFactory;

        public UserController(IUserHandler userHandler, IEasyCachingProviderFactory cacheFactory)
        {
            _userHandler = userHandler;
            _cacheFactory = cacheFactory;
        }

        /// <summary>
        /// api đăng nhập
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("login")]
        [ProducesResponseType(typeof(ResponseObject<String>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login(UserModel userModel)
        {
            var cachekey = $"MODEL_{userModel.Email}_{userModel.Password}";
            var provider = _cacheFactory.GetCachingProvider("default");
            var cacheResult = await provider.GetAsync<Response>(cachekey);
            if(cacheResult != null && cacheResult.HasValue)
            {
                return Ok(cacheResult.Value);
            }
            var result = await _userHandler.Login(userModel);
            if(result.Code == Code.Success)
            {
                await provider.SetAsync(cachekey, result, TimeSpan.FromMinutes(10));
            }
            return Ok(result);
        }

        /// <summary>
        /// Lấy ra danh sách tài khoản 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getAll-user")]
        [ProducesResponseType(typeof(ResponseObject<List<UserCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUser()
        {
            return Ok(await _userHandler.GetAllUser());
        }

        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sortby-user")]
        [ProducesResponseType(typeof(ResponseObject<List<ProductCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SortByUser([FromQuery] string sort)
        {
            return Ok(await _userHandler.SortBy(sort));
        }

        /// <summary>
        /// create User
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("create-user")]
        [ProducesResponseType(typeof(ResponseObject<UserCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateUser(UserCreateModel userModel)
        {
            return Ok(await _userHandler.CreateUser(userModel));
        }

        /// <summary>
        /// lấy pass code
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("send-passcode")]
        [ProducesResponseType(typeof(ResponseObject<String>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendPassCode(string email)
        {
            return Ok(await _userHandler.GetAccessCode(email));
        }

        /// <summary>
        /// check pass code
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("check-passcode")]
        [ProducesResponseType(typeof(ResponseObject<String>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckPassCode(Guid? userId ,string passcode)
        {
            return Ok(await _userHandler.CheckPassCode(userId,passcode));
        }

        /// <summary>
        /// thay doi mat khau
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("change-password")]
        [ProducesResponseType(typeof(ResponseObject<String>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ChangePassword(Guid? userId, string password)
        {
            return Ok(await _userHandler.ChangePassword(userId, password));
        }

        /// <summary>
        /// lấy tên bởi token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("get-by-name-token")]
        [ProducesResponseType(typeof(ResponseObject<UserCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByNameToken(string email)
        {
            var cachekey = $"EMAIL_{email}";
            var provider = _cacheFactory.GetCachingProvider("default");
            var cacheResult = await provider.GetAsync<Response>(cachekey);
            if(cacheResult != null && cacheResult.HasValue)
            {
                return Ok(cacheResult.Value);
            }

            var result = await _userHandler.GetByNameToken(email);
            if(result.Code == Code.Success)
            {
                await provider.SetAsync(cachekey, result, TimeSpan.FromMinutes(10));
            }
            return Ok(result);
        }

        /// <summary>
        /// cập nhật user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("update-user")]
        [ProducesResponseType(typeof(ResponseObject<UserCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateUser(UserCreateModel model)
        {
            return Ok(await _userHandler.UpdateUser(model));
        }

        /// <summary>
        /// xóa tài khoản
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("delete-user")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(Guid? userId)
        {
            return Ok(await _userHandler.DeleteUser(userId));
        }

        [HttpGet]
        [Route("get-user-by-id")]
        [ProducesResponseType(typeof(ResponseObject<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(Guid? userId)
        {
            return Ok(await _userHandler.GetUserById(userId));
        }

        [HttpGet]
        [Route("get-user-image")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public IActionResult GetImage(string image)
        {
            Byte[] b = System.IO.File.ReadAllBytes(Directory.GetCurrentDirectory() + "\\wwwroot\\images\\" + image);
            return File(b, "image/jpeg");
        }
    }
}
