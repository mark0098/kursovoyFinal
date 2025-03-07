using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


var builder = WebApplication.CreateBuilder(args);

// Добавляем поддержку Razor Pages
builder.Services.AddRazorPages();

// Добавляем поддержку сессий
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Включаем поддержку сессий
app.UseSession();

// Настройка маршрутизации
app.MapRazorPages();

app.Run();