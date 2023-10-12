using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using Data.DataModel;
using Business.User;
using FluentValidation;
using System.Text.RegularExpressions;
using Business.Category;

namespace Business.Account
{
    public class AccountHandler : IAccountHandler
    {
        private readonly MyDB_Context _myDbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountHandler> _logger;

        public AccountHandler(MyDB_Context myDbContext, IConfiguration config, ILogger<AccountHandler> logger)
        {
            _myDbContext = myDbContext;
            _config = config;
            _logger = logger;
        }

        public async Task<Response> ChangePassword(Guid? userId, string password)
        {
            try
            {
                if (userId == null)
                {
                    return new ResponseError(Code.BadRequest, "userId không được để trống");
                }
                if (String.IsNullOrEmpty(password))
                {
                    return new ResponseError(Code.BadRequest, "password không được để trống!");
                }
                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(userId));
                if (data == null)
                {
                    return new ResponseError(Code.ServerError, "Người dùng không tồn tai trong hệ thống");
                }

                data.Password = Utils.EncryptSha256(password);
                _myDbContext.Account.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<bool>(true, "Cập nhật mật khẩu thành công", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Cập nhật mật khẩu thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> CheckPassCode(Guid? accountId, string passcode)
        {
            try
            {
                if (accountId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường userId không được để trống!");
                }

                if (String.IsNullOrEmpty(passcode))
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường passcode không được để trống!");
                }

                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(accountId));

                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin tài khoản không tồn tại trong hệ thống!");
                }

                if (!data.TokenChangePassword.Contains(passcode))
                {
                    return new ResponseError(Code.BadRequest, "Mã truy cập không hợp lệ! Vui lòng thử lại");
                }

                return new ResponseObject<Guid?>(data.Id, "Mã truy cập hợp lệ", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> DeleteAccount(Guid? accountId)
        {
            try
            {
                if(accountId == null)
                {
                    return new ResponseError(Code.BadRequest, "Thoong tin trường accountId không được để trống!");
                }

                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(accountId));
                if(data == null)
                {
                    return new ResponseError(Code.BadRequest, "Tài khoản không tồn tại trong hệ thống!");
                }

                data.Sate = false;

                _myDbContext.Account.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    return new ResponseObject<Guid?>(accountId, $"Xóa tài khoản thnahf công : {accountId}", Code.Success);
                }
                return new ResponseError(Code.ServerError, "Xóa tài khoản thất bại");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAccessCode(string email)
        {
            try
            {
                if (String.IsNullOrEmpty(email))
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường email không được để trống");
                }
                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Email.Equals(email) || x.Phone.Equals(email));
                if(data == null)
                {
                    return new ResponseError(Code.ServerError, "Thông tin dữ liệu không tồn tại trong hệ thống");
                }

                Random r = new Random();
                string num = r.Next(0, 9999).ToString();
                data.TokenChangePassword = num;
                _myDbContext.Account.Update(data);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs == 0)
                {
                    return new ResponseError(Code.ServerError, "Cập nhật mã thất bại");
                }

                // thực hiện gửi mail
                string to = data.Email; //To address
                string from = "lebaotu05122002@gmail.com"; //From address
                var ms = data.UserName;
                var change_pass_word = data.TokenChangePassword;
                MailMessage message = new MailMessage(from, to);
                message.IsBodyHtml = true;
                message.Subject = "LEBAOTU - COMPANY";
                message.Body = bodyEmail(change_pass_word, ms);
                message.BodyEncoding = Encoding.UTF8;
                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp
                System.Net.NetworkCredential basicCredential1 = new
                System.Net.NetworkCredential(from, "ptezfclvjexuwbrk");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicCredential1;

                client.Send(message);
                return new ResponseObject<Guid?>(data.Id, "Gửi mã thành công",Code.Success);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public string bodyEmail(string token, string userFullName)
        {
            string strBody = string.Empty;
            strBody += "<html><head> </head>";
            strBody += "<body  style='width: 500px; border: solid 2px #888; padding: 20px; margin: auto;'>";
            strBody += Environment.NewLine;
            strBody += "<p style='text-align:center'><img alt='' src='https://www.codewithmukesh.com/wp-content/uploads/2020/05/codewithmukesh_logo_wordpress.png' style='height:89px; width:400px'/></p>";
            strBody += "<p>Xin Chào : " + userFullName + "</p>";
            strBody += "<p>Bạn vừa yêu cầu cấp mới mã truy cập với LEBAOTU - COMPANY.</p>";
            strBody += "<p><strong>Mã truy cập của bạn là:</strong></p>";
            strBody += "<div style = 'width: 500px; background-color: #FA4319;height: 60px; display: flex; margin-top:30px; border-radius: 10px;'>";
            strBody += "<span style = 'color: white; font-size: 30px;margin-left: 195px; margin-top:12px; letter-spacing: 15px;'>" + token + "</span></div>";
            strBody += "<hr style='border-top: 2px solid #bbb; margin-top: 10px'>";
            strBody += "<p><strong>KHÔNG CHIA SẺ</strong></p>";
            strBody += "<p>Email này chứa một mã bảo mật của LEBAOTU-COMPANY, vui lòng không chia sẻ email hoặc mã bảo mật này với người khác</p>";
            strBody += "<strong>CÂU HỎI KHÁC </strong>";
            strBody += "<p> Nếu bạn cần sửa đổi hoặc có câu hỏi về nội dung thông báo,vui lòng liên hệ LEBAOTU-COMPANY qua các kênh sau: Hotline: <b>0388334379</b> Hộp thư điện tử: <b>lebaotu05122002@gmail.com</b></p>";
            strBody += "</body></html >";
            return strBody;
        }

        public async Task<Response> GetAccountByEmail(string email)
        {
            try
            {
                if(String.IsNullOrEmpty(email))
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường email không được để trống");
                }

                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Email.Equals(email));
                if(data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin người dùng!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Account, AccountCreateModel>(data);
                return new ResponseObject<AccountCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAccountById(Guid? accountId)
        {
            try
            {
                if(accountId == null) 
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường accountId không được để trống!");
                }

                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(accountId));
                if(data == null)
                {
                    return new ResponseError(Code.ServerError, "Không tồn tại thông tin người dùng!");
                }

                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Account, AccountCreateModel>(data);
                return new ResponseObject<AccountCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> GetAllAccount(PageModel model)
        {
            try
            {
                var data = await _myDbContext.Account.Where(x => x.Sate == true).ToListAsync();
                if(model.PageSize.HasValue && model.PageNumber.HasValue)
                {
                    if(model.PageSize <= 0)
                    {
                        model.PageSize = 0;
                    }

                    int excludeRows = (model.PageNumber.Value - 1) * (model.PageSize.Value);
                    if(excludeRows <= 0)
                    {
                        excludeRows = 0;
                    }
                    data = data.Skip(excludeRows).Take(model.PageSize.Value).ToList();
                }
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Account, AccountCreateModel>(data);
                return new ResponseObject<List<AccountCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }


        public async Task<Response> Login(AccountModel model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.Email))
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường email không được để trống");
                }
                if (String.IsNullOrEmpty(model.Password))
                {
                    return new ResponseError(Code.BadRequest, "Thông tin trường passsword không được để trống");
                }
                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Email.Equals(model.Email) || x.Phone.Equals(model.Email));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Thông tin tài khoản không tồn tại trong hệ thống!");
                }
                if (data.Sate == false)
                {
                    return new ResponseError(Code.ServerError, "Tài khoản của bạn chưa được kích hoạt");
                }
                if (data.TimeLock > DateTime.Now)
                {
                    var timeLock = data.TimeLock - DateTime.Now;
                    data.IsLock = false;
                    _myDbContext.Account.Update(data);
                    await _myDbContext.SaveChangesAsync();
                    return new ResponseError(Code.BadRequest, $"Tài khoản của bạn vẫn đang trong thời gian khóa ! vui lòng thử lại sau {timeLock} phút");
                }

                // check soos laanf nhapaj sai
                if (data.CountError == 5)
                {
                    data.IsLock = true;
                    data.CountError = 0;
                    data.TimeLock = DateTime.Now.AddMinutes(5);
                    _myDbContext.Update(data);
                    await _myDbContext.SaveChangesAsync();
                    return new ResponseError(Code.ServerError, "Tài khoản của bạn đã bị khóa ! vui lòng thử lại sau 5 phút");
                }

                if (!data.Password.Equals(Utils.EncryptSha256(model.Password)))
                {
                    data.CountError++;
                    _myDbContext.Account.Update(data);
                    await _myDbContext.SaveChangesAsync();
                    var count = data.CountError;
                    return new ResponseErrorLogin(Code.ServerError, "Sai mật khẩu vui lòng thực hiện lại", count);
                }

                if (data.IsLock == false)
                {

                    data.CountError = 0;
                    data.IsLock = false;
                    _myDbContext.Account.Update(data);
                    await _myDbContext.SaveChangesAsync();

                    // lấy key trong file cấu hình ra
                    var key = _config["Jwt:Key"];

                    // mã hóa cái key lấy được
                    var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

                    // ký vào cái key đã mã hóa ( nghĩa là cái key nó sẽ là MK của cái Token sau khi nó được sinh ra )
                    var signingCredential = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

                    // customer JWT header
                    var header = new JwtHeader(signingCredential);

                    // tạo claims để cấu hình chứa thông tin của người dùng khi pass qua phần check TK
                    var claims = new List<Claim>
                        {
                        new Claim(ClaimTypes.Email,data.Email),
                        new Claim(ClaimTypes.MobilePhone,data.Phone),
                        };

                    // sét thời gian hết hạn cho Token
                    DateTime Expity = DateTime.UtcNow.AddMinutes(30);
                    int ts = (int)(Expity - new DateTime(1970, 1, 1)).TotalSeconds;

                    var token = new JwtSecurityToken(claims: claims, issuer: _config["Jwt:Issuer"], audience: _config["Jwt:Audience"], expires: Expity, signingCredentials: signingCredential);

                    // sinh ra chuỗi Token với các thông số ở trên
                    var lbtToken = new JwtSecurityTokenHandler().WriteToken(token);

                    return new ResponseObject<String>(lbtToken, "Đăng nhập thành công ", Code.Success);
                }
                else
                {
                    return new ResponseError(Code.ServerError, "Tài khoản của bạn bị khóa ! vui lòng liên hệ Admin để thực hiện mở tào khoản");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> SingUpAccount(AccountCreateModel model)
        {
            try
            {
                var validation = new ValidationAccountModel();
                var result = await validation.ValidateAsync(model);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }

                if (!string.IsNullOrEmpty(model.Birthday))
                {
                    if (!Regex.IsMatch(model.Birthday, Regexs.DateFormatRule))
                    {
                        return new ResponseError(Code.BadRequest, "Nhập sai dữ liệu ngày tháng năm (dd/MM/yyyy)");
                    }
                }

                var checkeEmail = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Email.Trim().Equals(model.Email));
                if(checkeEmail != null)
                {
                    return new ResponseError(Code.BadRequest, "Email đã tồn tại trong hệ thống");
                }

                var checkePhone = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Phone.Trim().Equals(model.Phone));
                if (checkeEmail != null)
                {
                    return new ResponseError(Code.BadRequest, "Phone đã tồn tại trong hệ thống");
                }

                if (model.Avatar != null)
                {
                    /// Convert the base64 string back to bytes
                    byte[] imageBytes = Convert.FromBase64String(model.Avatar);

                    // Save the file to a location or process it as needed.
                    // For example, you can save it to the "wwwroot/images" folder:
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    model.Avatar = uniqueFileName;
                }

                model.Password = Utils.EncryptSha256(model.Password);
                model.CreatedDate = DateTime.Now;
                model.TimeLock = DateTime.Now;

                var dataMap = AutoMapperUtils.AutoMap<AccountCreateModel, Data.DataModel.Account>(model);
                _myDbContext.Account.Add(dataMap);
                int rs = await _myDbContext.SaveChangesAsync();
                if (rs > 0)
                {
                    _logger.LogInformation("Đăng ký tài khoản thành công");
                    return new ResponseObject<AccountCreateModel>(model, "Đăng ký tài khoản thành công", Code.Success);
                }
                else
                {
                    _logger.LogError("Đăng ký tài khoản thất bại");
                    return new ResponseError(Code.ServerError, "Đăng ký tài khoản thất bại");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdateAccount(AccountCreateModel model)
        {
            try
            {
                var validation = new ValidationAccountModel();
                var result = await validation.ValidateAsync(model);
                if (!result.IsValid)
                {
                    var errorMessage = result.Errors.Select(x => x.ErrorMessage).ToList();
                    return new ResponseError(Code.ServerError, "Dữ liệu không hợp lệ!", errorMessage);
                }
                if (!string.IsNullOrEmpty(model.Birthday))
                {
                    if (!Regex.IsMatch(model.Birthday, Regexs.DateFormatRule))
                    {
                        return new ResponseError(Code.BadRequest, "Nhập sai dữ liệu ngày tháng năm (dd/MM/yyyy)");
                    }
                }
                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
                if(data == null)
                {
                    return new ResponseError(Code.BadRequest, "Tài khoản không tồn tại");
                }
                if (model.Password == data.Password)
                {
                    data.Id = model.Id;
                    data.UserName = model.UserName;
                    data.Address = model.Address;
                    data.Password = model.Password;
                    data.Email = model.Email;
                    data.Phone = model.Phone;
                    data.Avatar = model.Avatar;
                    data.CreatedDate = model.CreatedDate;
                    data.UpdatedDate = DateTime.Now;
                    data.Sate = model.Sate;
                    data.CountError = model.CountError;
                    data.TimeLock = model.TimeLock;
                    data.IsLock = model.IsLock;

                    if (model.Avatar != null)
                    {
                        /// Convert the base64 string back to bytes
                        byte[] imageBytes = Convert.FromBase64String(model.Avatar);

                        // Save the file to a location or process it as needed.
                        // For example, you can save it to the "wwwroot/images" folder:
                        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        System.IO.File.WriteAllBytes(filePath, imageBytes);
                        data.Avatar = uniqueFileName;
                    }

                    _myDbContext.Account.Update(data);
                    int rs = await _myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                        return new ResponseObject<AccountCreateModel>(model, $"{Message.UpdateSuccess}", Code.Success);
                    }
                }
                else
                {
                    // nhập password mới 
                    model.Password = Utils.EncryptSha256(model.Password);
                    data.Id = model.Id;
                    data.UserName = model.UserName;
                    data.Address = model.Address;
                    data.Password = model.Password;
                    data.Email = model.Email;
                    data.Phone = model.Phone;
                    data.Avatar = model.Avatar;
                    data.CreatedDate = model.CreatedDate;
                    data.UpdatedDate = DateTime.Now;
                    data.Sate = model.Sate;
                    data.CountError = model.CountError;
                    data.TimeLock = model.TimeLock;
                    data.IsLock = model.IsLock;

                    _myDbContext.Account.Update(data);
                    int rs = await _myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                        return new ResponseObject<AccountCreateModel>(model, $"{Message.UpdateSuccess}", Code.Success);
                    }
                }
                return new ResponseError(Code.ServerError, $"{Message.UpdateError}");
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
                var data = await _myDbContext.Account.ToListAsync();
                data = sort switch
                {
                    var t when t.Equals("accountid_asc") => data.OrderBy(x => x.Id).ToList(),
                    var t when t.Equals("accountid_desc") => data.OrderByDescending(x => x.Id).ToList(),
                    var t when t.Equals("username_asc") => data.OrderBy(x => x.UserName).ToList(),
                    var t when t.Equals("username_desc") => data.OrderByDescending(x => x.UserName).ToList(),
                    _ => data
                };

                _logger.LogInformation($"{Message.GetDataSuccess}");
                var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Account, AccountCreateModel>(data);
                return new ResponseObject<List<AccountCreateModel>>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Message.ErrorLogMessage);
                return new ResponseError(Code.ServerError, $"{ex.Message}");
            }
        }

        public async Task<Response> UpdateAccountImage(string id, string imageBase64Str)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return new ResponseError(Code.BadRequest, "Trường Id không được rỗng");
                }
                var data = await _myDbContext.Account.FirstOrDefaultAsync(x => x.Id.Equals(id));
                if (data == null)
                {
                    return new ResponseError(Code.BadRequest, "Tài khoản không tồn tại");
                }
                

                if (!string.IsNullOrEmpty(imageBase64Str))
                {
                        /// Convert the base64 string back to bytes
                    byte[] imageBytes = Convert.FromBase64String(imageBase64Str);

                        // Save the file to a location or process it as needed.
                        // For example, you can save it to the "wwwroot/images" folder:
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string uniqueFileName = Guid.NewGuid().ToString() + "_image.png";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    System.IO.File.WriteAllBytes(filePath, imageBytes);
                    data.Avatar = uniqueFileName;

                    _myDbContext.Account.Update(data);
                    int rs = await _myDbContext.SaveChangesAsync();
                    if (rs > 0)
                    {
                        var dataMap = AutoMapperUtils.AutoMap<Data.DataModel.Account, AccountCreateModel>(data);
                        return new ResponseObject<AccountCreateModel>(dataMap, $"{Message.GetDataSuccess}", Code.Success);
                    }
                }

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
