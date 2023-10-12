using Business.AddressAccount;
using Business.Category;
using Business.Payment;
using Business.User;
using Data;
using Data.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Comment
{
    public class CommentHandler : ICommentHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<CommentHandler> _logger;

        public CommentHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<CommentHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> DeleteComment(Guid? commentId)
        {
            try
            {
                if(commentId ==  null)
                {
                    return new ResponseError(Code.BadRequest, $"Thông tin trường commentId Không được để trống!");
                }

                var data = await _myDbContext.Comment.FirstOrDefaultAsync(x => x.CommentId == commentId);
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Dữ liệu trống!");
                }

                _myDbContext.Comment.Remove(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return new ResponseObject<Guid?>(commentId, $"{Message.DeleteSuccess}", Code.Success);
                }
                return new ResponseError(Code.ServerError, $"{Message.DeleteError}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAllComment(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Comment.Include(x => x.Account).Include(x => x.Product).ToListAsync();
                if (model.PageSize.HasValue && model.PageNumber.HasValue)
                {
                    if (model.PageSize <= 0)
                    {
                        model.PageSize = 0;
                    }

                    int excludeRows = (model.PageNumber.Value - 1) * (model.PageSize.Value);
                    if (excludeRows <= 0)
                    {
                        excludeRows = 0;
                    }
                    data = data.Skip(excludeRows).Take(model.PageSize.Value).ToList();
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Comment, CommentModel>(data);
                return new ResponseObject<List<CommentModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetCommentById(Guid? commentId)
        {
            try
            {
                if (commentId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường commentId không đươc để trống!");
                }

                var data = await _myDbContext.Comment.Include(x => x.Account).Include(x => x.Product).FirstOrDefaultAsync(x => x.CommentId == commentId);
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Comment, CommentModel>(data);
                return new ResponseObject<CommentModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetCommentByProductId(Guid? productId)
        {
            try
            {
                if (productId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường productId không đươc để trống!");
                }

                var data = await _myDbContext.Comment.Include(x => x.Account).Include(x => x.Product).Where(x => x.ProductId == productId).ToListAsync();
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Comment, CommentModel>(data);
                return new ResponseObject<List<CommentModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> InsertComment(CommentModel model)
        {
            try
            {
                var validation = new ValidationCommentModel();
                var result = await validation.ValidateAsync(model);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                model.CreatedDate = DateTime.Now;

                var dataMap = AutoMapperUtils.AutoMap<CommentModel, Data.DataModel.Comment>(model);
                _myDbContext.Comment.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    return new ResponseObject<CommentModel>(model, $"{Message.CreateSuccess}",Code.Success);
                }
                return new ResponseError(Code.ServerError, $"{Message.CreateError}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> SortBy(string sort)
        {
            try
            {
                var data = await _myDbContext.Comment.Include(x => x.Account).Include(x => x.Product).ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("question_asc") => data.OrderBy(x => x.Question).ToList(),
                    var t when t.Equals("question_desc") => data.OrderByDescending(x => x.Question).ToList(),
                    var t when t.Equals("create_asc") => data.OrderBy(x => x.CreatedDate).ToList(),
                    var t when t.Equals("create_desc") => data.OrderByDescending(x => x.CreatedDate).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Comment, CommentModel>(data);
                return new ResponseObject<List<CommentModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdateComment(CommentModel model)
        {
            try
            {
                var validation = new ValidationCommentModel();
                var result = await validation.ValidateAsync(model);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                model.UpdatedDate = DateTime.Now;

                var dataMap = AutoMapperUtils.AutoMap<CommentModel, Data.DataModel.Comment>(model);
                _myDbContext.Comment.Update(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if(rs > 0)
                {
                    _logger.LogInformation($"{Message.UpdateSuccess}");
                    return new ResponseObject<CommentModel>(model, $"{Message.UpdateSuccess}",Code.Success);
                }
                _logger.LogError($"{Message.UpdateError}");
                return new ResponseError(Code.ServerError, $"{Message.UpdateError}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }
    }
}
