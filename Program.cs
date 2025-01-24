using System.Text;
using me.admin.api.Data;
using me.admin.api.Data.Repositories;
using me.admin.api.DTOs;
using me.admin.api.Models;
using me.admin.api.Services;
using me.admin.api.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();
builder.Services.AddTransient<AuthService, AuthService>();
builder.Services.AddSingleton<AppDbContext, AppDbContext>();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                    Repositories                                                    */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddSingleton<UserRepository, UserRepository>();
builder.Services.AddSingleton<OutletRepository, OutletRepository>();
builder.Services.AddSingleton<DailyRecordRepository, DailyRecordRepository>();
builder.Services.AddSingleton<MenuRepository, MenuRepository>();
builder.Services.AddSingleton<OrderRepository, OrderRepository>();
builder.Services.AddSingleton<OrderItemRepository, OrderItemRepository>();
builder.Services.AddSingleton<ImportanceRepository, ImportanceRepository>();
builder.Services.AddSingleton<InvoiceRepository, InvoiceRepository>();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                      Services                                                      */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddSingleton<UserService, UserService>();
builder.Services.AddSingleton<OutletService, OutletService>();
builder.Services.AddSingleton<DailyRecordService, DailyRecordService>();
builder.Services.AddSingleton<MenuService, MenuService>();
builder.Services.AddSingleton<OrderService, OrderService>();
builder.Services.AddSingleton<OrderItemService, OrderItemService>();
builder.Services.AddSingleton<ImportanceService, ImportanceService>();
builder.Services.AddSingleton<InvoiceService, InvoiceService>();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                       Options                                                      */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddOptions<AuthorizationSetting>()
	.Bind(builder.Configuration.GetSection("Authorization"))
	.ValidateDataAnnotations().ValidateOnStart();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                      Jwt                                                           */
/* ------------------------------------------------------------------------------------------------------------------ */
var jwtSecret = builder.Configuration.GetValue<string>("Authorization:JwtSecret");
if (string.IsNullOrEmpty(jwtSecret))
{
	Log.Fatal("JwtSecret is not set in the configuration file");
	throw new Exception("JwtSecret is not set in the configuration file");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		var sign = Encoding.UTF8.GetBytes(jwtSecret);
		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(sign),
			ClockSkew = TimeSpan.FromSeconds(5)
		};
	});

builder.Services.AddEndpointsApiExplorer();

//----------------------------------------------------------------------------------------------------------------------
// @ Swagger
//----------------------------------------------------------------------------------------------------------------------
builder.Services.AddSwaggerGen(opt =>
{
	opt.SwaggerDoc("v1", new OpenApiInfo { Title = "ME Client Api", Version = "v1" });
	opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		In = ParameterLocation.Header,
		Description = "Please enter token",
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		BearerFormat = "JWT",
		Scheme = "bearer"
	});
	opt.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                      Controllers                                                     */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddControllers();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                      HttpClient                                                      */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                      AutoMapper                                                      */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddAutoMapper(cfg =>
{
	cfg.CreateMap<User, UserInfoForTokenDto>();
});

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                        Cors                                                        */
/* ------------------------------------------------------------------------------------------------------------------ */
builder.Services.AddCors(options =>
{
	var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new string[] {};

	options.AddPolicy("customOrigins",
		corsBuilder =>
			corsBuilder.WithOrigins(allowedOrigins)
				.AllowCredentials()
				.AllowAnyHeader()
				.AllowAnyMethod()
	);
});

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("Database"));

var app = builder.Build();

/* ------------------------------------------------------------------------------------------------------------------ */
/*                                                    Sync Database                                                   */
/* ------------------------------------------------------------------------------------------------------------------ */
var dbSynchronizeSchema = new DbSynchronizeSchema(app.Services.GetRequiredService<AppDbContext>());
dbSynchronizeSchema.Synchronize();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("customOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();