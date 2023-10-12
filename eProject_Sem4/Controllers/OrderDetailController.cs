using Business.Order;
using Business.OrderDetail;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace eProject_Sem4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : Controller
    {
        private IOrderDetailHandler _orderDetailHandler;

        public OrderDetailController(IOrderDetailHandler orderDetailHandler)
        {
            _orderDetailHandler = orderDetailHandler;
        }

        [HttpGet]
        /*[Authorize]*/
        [Route("getall-orderdetail")]
        [ProducesResponseType(typeof(ResponseObject<List<OrderDetailCreateModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrder([FromQuery] PageModel model)
        {
            return Ok(await _orderDetailHandler.getAllOrderDetail(model));
        }

        [HttpGet]
        [Route("get-orderdetail-by-orderid")]
        [ProducesResponseType(typeof(ResponseObject<OrderDetailCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrderDetailByOrderId(Guid id)
        {
            return Ok(await _orderDetailHandler.getOrderDetailByOrderId(id));
        }

        [HttpPost]
        [Route("create-orderdetail")]
        [ProducesResponseType(typeof(ResponseObject<OrderDetailCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderDetailCreateModel model)
        {
            return Ok(await _orderDetailHandler.CreateOrderDetail(model));
        }

        [HttpPost]
        [Route("update-orderdetail")]
        [ProducesResponseType(typeof(ResponseObject<OrderDetailCreateModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateOrder([FromBody] OrderDetailCreateModel model)
        {
            return Ok(await _orderDetailHandler.UpdateOrderDetail(model));
        }

        [HttpGet]
        [Route("delete-orderdetail")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrder(Guid? Id)
        {
            return Ok(await _orderDetailHandler.DeleteOrderDetail(Id));
        }

        [HttpGet]
        [Route("delete-orderdetail-by-orderid")]
        [ProducesResponseType(typeof(ResponseObject<Guid>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteOrderDetailByOrderId(Guid? Id)
        {
            return Ok(await _orderDetailHandler.DeleteOrderDetailByOrderId(Id));
        }
    }
}
