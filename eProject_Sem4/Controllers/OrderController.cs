using Business.Account;
using Business.Order;
using Business.OrderDetail;
using Business.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace eProject_Sem4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private IOrderHandler _orderHandler;

        public OrderController(IOrderHandler orderHandler)
        {
            _orderHandler = orderHandler;
        }


        [HttpGet]
        [Route("getall-order")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrder([FromQuery] PageModel model)
        {
            return Ok(await _orderHandler.getAllOrder(model));
        }

        [HttpGet]
        /*[Authorize]*/
        [Route("getlist-state-order")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListStateOrder([FromQuery] int? state, Guid accountId)
        {
            return Ok(await _orderHandler.GetListStateOrder(state, accountId));
        }

        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("sortby-order")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SortByOrder([FromQuery] string sort)
        {
            return Ok(await _orderHandler.SortBy(sort));
        }


        [HttpGet]
        [Route("get-order-by-id")]
        [ProducesResponseType(typeof(ResponseObject<OrderCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderById(Guid id)
        {
            return Ok(await _orderHandler.getOrderById(id));
        }

        [HttpPost]
        [Route("create-order")]
        [ProducesResponseType(typeof(ResponseObject<OrderCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrder([FromBody]OrderCreateModel model)
        {
            return Ok(await _orderHandler.CreateOrder(model));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("update-order")]
        [ProducesResponseType(typeof(ResponseObject<OrderCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrder([FromBody]OrderCreateModel model)
        {
            return Ok(await _orderHandler.UpdateOrder(model));
        }

        [HttpPost]
        [Route("update-order-state")]
        [ProducesResponseType(typeof(ResponseObject<OrderCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrderState(Guid orderId, int state, string cancelationReason)
        {
            return Ok(await _orderHandler.UpdateOrderState(orderId, state, cancelationReason));
        }

        [HttpGet]
        [Route("delete-order")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrder(Guid? Id)
        {
            return Ok(await _orderHandler.DeleteOrder(Id));
        }
    }
}
