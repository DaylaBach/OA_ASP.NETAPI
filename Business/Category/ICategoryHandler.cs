using Microsoft.AspNetCore.Http;
using Shared;

namespace Business.Category
{
    public interface ICategoryHandler
    {
        /// <summary>
        /// Lấy danh sách Category
        /// </summary>
        /// /// <param name="modell"></param>
        /// <returns></returns>
        Task<Response> getAllCategory(PageModel model);

        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// Lấy Category theo Id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        Task<Response> getCategoryById(Guid? categoryId);

        /// <summary>
        /// Lọc Category
        /// </summary>
        /// <param name="model"></param>
        /// <param name="name"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<Response> searchCategory(PageModel model, string name, bool status);


        /// <summary>
        /// thêm mới danh mục 
        /// </summary>
        /// <param name="CategoryModel"></param>
        /// <returns></returns>
        Task<Response> CreateCategory(CategoryCreateModel CategoryModel);


        /// <summary>
        /// cập nhật danh mục
        /// </summary>
        /// <param name="CategoryModel"></param>
        /// <returns></returns>
        Task<Response> UpdateCategory(CategoryCreateModel CategoryModel);


        /// <summary>
        /// xóa danh mục
        /// </summary>
        /// <param name="CategoryId"></param>
        /// <returns></returns>
        Task<Response> DeleteCategory(Guid? CategoryId);
    }
}