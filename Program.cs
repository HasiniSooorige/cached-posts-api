//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();


using CachedPostsApi.ExternalApi;
using CachedPostsApi.Repositories;
using CachedPostsApi.Services;
using CachedPostsApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<IPostService, PostService>();

builder.Services.AddHttpClient<IJsonPlaceholderClient, JsonPlaceholderClient>(
    client =>
    {
        client.BaseAddress = new Uri(
            "https://jsonplaceholder.typicode.com/");
        client.Timeout = TimeSpan.FromSeconds(10);
    });

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();