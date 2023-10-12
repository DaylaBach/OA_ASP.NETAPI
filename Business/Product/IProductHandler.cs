using Shared;

namespace Business.Product
{
    public interface IProductHandler
    {
        /// <summary>
        /// Lấy danh sách Product
        /// </summary>
        /// <returns></returns>
        Task<Response> getAllProduct(PageModel model);



        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// Lấy Product theo Id
        /// </summary>
        /// <returns></returns>
        Task<Response> getProductById(Guid? productId);

        /// <summary>
        /// Lọc Product
        /// </summary>
        /// <returns></returns>
        Task<Response> searchProduct(PageModel model, string? name, int? quantity, float? priceMin, float? priceMax, string? address, bool status, Guid? categoryId);


        /// <summary>
        /// thêm mới sản phẩm 
        /// </summary>
        /// <param name="ProductModel"></param>
        /// <returns></returns>

        Task<Response> CreateProduct(ProductCreateModel ProductModel);
        /// <summary>
        /// cập nhật sản phẩm
        /// </summary>
        /// <param name="ProductModel"></param>
        /// <returns></returns>
        Task<Response> UpdateProduct(ProductCreateModel ProductModel);

        /// <summary>
        /// cập nhật sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        Task<Response> UpdateProductQuantity(Guid? productId, int? quantity);


        /// <summary>
        /// xóa sản phẩm
        /// </summary>
        /// <param name="ProductId"></param>
        /// <returns></returns>
        Task<Response> DeleteProduct(Guid? ProductId);

        /// <summary>
        /// lấy sản phẩm theo danh muc
        /// </summary>
        /// <param name="CateId"></param>
        /// <returns></returns>
        Task<Response> GetProductByCate(Guid? cateId);

        Task<Response> getAllProductForUser(PageModel model);

        Task<Response> GetNewProducts(PageModel model);

        Task<Response> GetProductOrderByQuantityDesc(PageModel model);
    }
}
