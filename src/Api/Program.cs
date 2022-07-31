using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddControllers();
};

var app = builder.Build();
{
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.MapControllers();
    app.Run();
}
