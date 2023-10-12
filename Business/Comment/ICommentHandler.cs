using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Comment
{
    public interface ICommentHandler
    {
        /// <summary>
        /// lấy ra danh sách comment 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> GetAllComment(PageModel model);

        /// <summary>
        /// sắp xếp 
        /// </summary>
        /// <param name="sort"></param>
        /// <returns></returns>
        Task<Response> SortBy(string sort);

        /// <summary>
        /// lấy comment theo id
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        Task<Response> GetCommentById(Guid? commentId);

        /// <summary>
        /// lấy comment theo productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<Response> GetCommentByProductId(Guid? productId);

        /// <summary>
        /// Thêm mới comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> InsertComment(CommentModel model);

        /// <summary>
        /// cập nhật comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Response> UpdateComment(CommentModel model);

        /// <summary>
        /// xóa comment 
        /// </summary>
        /// <param name="commentId"></param>
        /// <returns></returns>
        Task<Response> DeleteComment(Guid? commentId);
    }
}
