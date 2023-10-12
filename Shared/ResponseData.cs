using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public class Response
    {
        public Code Code { get; set; } = Code.Success;

        public string Message { get; set; } = "Thành công";

        public Response(Code code, string message)
        {
            Code = code;
            Message = message;
        }

        public Response(string message)
        {
            Message = message;
        }

        public Response()
        {
        }
    }

    public class ResponseErrorProcedure
    {
        public int? Code { get; set; }
        public string Message { get; set; }
        public ResponseErrorProcedure(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ResponseErrorProcedure(string message)
        {
            Message = message;
        }

        public ResponseErrorProcedure()
        {
        }
    }

    /// <summary>
    /// Trả về lỗi 
    /// </summary>

    public class ResponseError : Response
    {
        public IList<string> ErrorMessages { get; set; }

        public ResponseError(Code code, string message) : base(code, message)
        {
        }

        public ResponseError(Code code, string message, IList<string> errorMessages = null) : base(code, message)
        {
            ErrorMessages = errorMessages;
        }
    }

    /// <summary>
    /// Trả về đối tượng 
    /// </summary>
    public class ResponseObject<T> : Response
    {
        /// <summary>
        ///     Dữ liệu trả về
        /// </summary>
        public T Data { get; set; }

        public ResponseObject(T data)
        {
            Data = data;
        }

        public ResponseObject(T data, string message)
        {
            Data = data;
            Message = message;
        }

        public ResponseObject(T data, string message, Code code)
        {
            Code = code;
            Data = data;
            Message = message;
        }
    }

    public class ResponseErrorLogin : Response
    {
        public int CountError { get; set; }

        public ResponseErrorLogin(Code code, string message, int countError)
        {
            Code = code;
            Message = message;
            CountError = countError;
        }
    }
    public class PageModel
    {
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public class Regexs
    {
        // chuỗi regex chỉ cho phép nhập vào định dạng ngày , tháng , năm (dd/MM/yyyy)
        // VD : 05/12/2002
        public static string DateFormatRule = @"^\d{2}/\d{2}/\d{4}$";
    }

    public class Constant
    {
        public const string StringCharacter = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        public const string DateFormatString = "dd/MM/yyyy";
    }
    /// <summary>
    /// Message thông báo 
    /// </summary>
    public class Message
    {
        public static string GetDataErrorMessage = "Tải dữ liệu thất bại";
        public static string ErrorLogMessage = "An error occurred:";
        public static string GetDataSuccess = "Lấy dữ liệu thành công";
        public static string GetDataError = "Lấy dữ liệu thất bại";
        public static string CreateSuccess = "Thêm mới dữ liệu thành công ";
        public static string CreateError = "Thêm mới dữ liệu Thất bại";
        public static string UpdateSuccess = "Cập nhật dữ liệu thành công";
        public static string UpdateError = "Cập nhật dữ liệu Thất bại";
        public static string DeleteSuccess = "Xóa dữ liệu thành công";
        public static string DeleteError = "Xóa dữ liệu thất bại";
        public static string DataResponseError = "Dữ liệu trả về thất bại";
        public static string MEDONMessage = "Tích Hợp MEDON";
        public static string IntegratedSuccess = "Tích Hợp thành công";
        public static string IntegratedError = "Tích hợp dữ liệu không thành công";
        public static string SendMessageError = "Gửi thông báo thất bại";
        public static string SendMessageSuccess = "Gửi thông báo thành công";
        public static string DataNullOrEmpty = "Dữ liệu trả về bị null";
        public static string ServerErrorMessage = "Có lỗi trong quá trình xử lí";
        public static string SaveLogDBError = "Lưu Log DB thất bại";
        public static string SendSMSSuccess = "Gửi thành công";
        public static string SendSMSError = "Gửi thất bại";
        public static string DataNull = "Dữ liệu bị null";
        public static string LoginSuccess = "Đăng nhập thành công";
        public static string LoginError = "Đăng nhập thất bại";
        public static string InvalidData = "Dữ liệu không hợp lệ";
    }
    /// <summary>
    /// max loi
    /// </summary>
    public enum Code
    {
        Success = 200, // OK
        Created = 201, // xác nhận trạng thái đã hoạt động 
        BadRequest = 400, // lỗi dữ liệu do người dùng chuyền lên 
        Unauthorized = 401, // từ chối truy cập khi header ko chứa mã xác thực (Token) 
        Forbidden = 403, // từ chối người dùng truy cập vào API này 
        NotFound = 404, // không tìm thấy đường dẫn trên API 
        MethodNotAllowed = 405, // truy cập file không dược cho phép 
        Conflict = 409, // quá nhiều yêu cầu cho một file 
        ServerError = 500 // Server Error 
    }
}
