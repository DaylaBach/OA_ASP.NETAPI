
using AspNetCoreRateLimit;
using AutoMapper;
using Business.Account;
using Business.Category;
using Business.Product;
using Business.AddressAccount;
using Business.Payment;
using Business.User;
using Data;
using EasyCaching.InMemory;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Business.Comment;
using Business.Order;
using Business.OrderDetail;

namespace eProject_Sem4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // +> lấy key trong tệp cấu hình appseting
            var key = Configuration["Jwt:key"];
            // mã hoá cái key đấy
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            //Add Authentication Bearer (thêm mới xác thực  Bearer )
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // kiểm tra cái Issuer ( default true , Issuer tổ chức phát hành )
                    ValidateIssuer = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    // kiểm tra cái Audience ( default true )
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    // cấu hỉnh thời gian hết hạn cho thằng Token
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    // chỉ ra cái key sử dụng trong Token khi nó được sinh ra
                    IssuerSigningKey = signingKey,
                    RequireSignedTokens = true,
                    ClockSkew = TimeSpan.Zero
                };
            });


            services.AddMvc();
            services.AddRouting();
            services.AddHttpClient();
            services.AddMemoryCache();

            // cấu hình sử dụng RateLimit
            services.Configure<IpRateLimitOptions>(options =>
            {
                options.EnableEndpointRateLimiting = true;
                options.StackBlockedRequests = false;
                options.HttpStatusCode = 429;
                options.RealIpHeader = "X-Real-IP";
                options.ClientIdHeader = "X-ClientId";
                options.GeneralRules = new List<RateLimitRule>
                {
                    #region Đặt lịch
                    new RateLimitRule
                    {
                        Endpoint = "post:/api/User/login",
                        Period = "1m",
                        Limit = 3
                    },
                    #endregion Đặt lịch
                };
                options.QuotaExceededResponse = new QuotaExceededResponse
                {
                    ContentType = "application/json",
                    Content = "{{ \"message\": \"Thao tác quá nhanh!\", \"details\": \"Chỉ được phép gọi API {0} lần/{1}. Vui lòng thử lại sau {2} giây.\" }}"
                };
            });

            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            services.AddInMemoryRateLimiting();
            services.AddEasyCaching(options =>
            {
                options.UseInMemory(config =>
                {
                    config.DBConfig = new InMemoryCachingOptions
                    {
                        // scan time, default value is 60s
                        ExpirationScanFrequency = 120,
                        EnableReadDeepClone = true,
                        EnableWriteDeepClone = true,
                    };
                    config.EnableLogging = false;
                    config.LockMs = 5000;
                    config.SleepMs = 300;
                }, "default");
            });

            // mở cổng cho bên frontend dùng
            services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });

            // cấu hình kết nối đến cơ sở dữ liệu
            services.AddDbContext<MyDB_Context>(option =>
            {
                option.UseSqlServer(Configuration.GetConnectionString("MyDB"));
            });

            //cấu hình swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API - V1", Version = "v1" });
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "My API - V2", Version = "v2" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            //đăng kí sử dụng AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // chỗ cấu hình hangfire
            //services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("MyDB")));
            //services.AddHangfireServer();

            // chỗ cấu hình handeler (Dependency Injection)
            services.AddScoped<IUserHandler , UserHandler>();
            services.AddScoped<IAccountHandler ,  AccountHandler>();
            services.AddScoped<ICategoryHandler , CategoryHandler>();
            services.AddScoped<IProductHandler , ProductHandler>();
            services.AddScoped<IAddressAccountHandler,  AddressAccountHandler>();
            services.AddScoped<IPaymentHandler, PaymentHandler>();
            services.AddScoped<ICommentHandler, CommentHandler>();
            services.AddScoped<IOrderHandler, OrderHandler>();
            services.AddScoped<IOrderDetailHandler, OrderDetailHandler>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime appLifetime)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "v2 Docs");
                });
            }
            else
            {
                app.UseHsts();
            }
            //app.UseMiddleware<ExceptionMiddleware>();

            // mở khoá http
            app.UseCors(x => x
               .AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader());

            // appLifetime.ApplicationStarted.Register(OnStarted);

            //app.UseSentryTracing();

            app.UseIpRateLimiting();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseHangfireDashboard()/*;*/
        }

        #region

        //private void OnStarted()
        //{
        //    SystemHandler systemHandler = new SystemHandler();
        //    ParametersRequestModel model = new ParametersRequestModel();
        //    // RecurringJob.AddOrUpdate("checkStatusMessage", () => systemHandler.GetAllStatusMessage(model), Cron.Daily(23,59), TimeZoneInfo.Local);
        //}

        #endregion
    }
}