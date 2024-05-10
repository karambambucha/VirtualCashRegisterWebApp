using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using VirtualCashRegisterWebApp.Data;
using VirtualCashRegisterWebApp.Enums;
using VirtualCashRegisterWebApp.TransactionContracts;

var builder = WebApplication.CreateBuilder();
string connection = builder.Configuration.GetConnectionString("DefaultConnection");
string testUrl = builder.Configuration.GetConnectionString("TestUrl");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));


var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/Products", async (ApplicationContext db) =>
await db.Products.Select(product => new
{
    product.Id,
    product.Name,
    product.Cost
}).ToListAsync());

app.MapPost("/api/Sale", async (SaleRequestContract saleRequest, ApplicationContext db) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(JsonSerializer.Serialize(saleRequest), Encoding.UTF8, "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync(testUrl + "Payment/Sale", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        var saleResponse = JsonSerializer.Deserialize<SaleResponseContract>(jsonResponse);
        if (saleResponse.GeneralResponse.ResultCode == ResultCode.Ok)
        {
            db.AddSaleResponse(saleResponse, saleRequest.Products);
        }
        return saleResponse;
    }
});

app.MapPost("/api/SaleSimple", async (SaleRequestContract saleRequest, ApplicationContext db) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(JsonSerializer.Serialize(saleRequest), Encoding.UTF8, "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync(testUrl + "Payment/Sale", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        var saleResponse = JsonSerializer.Deserialize<SaleResponseContract>(jsonResponse);
        if (saleResponse.GeneralResponse.ResultCode == ResultCode.Ok)
        {
            db.AddSaleResponse(saleResponse);
        }
        return saleResponse;
    }
});

app.MapPost("/api/Settle", async (SettleRequestContract settleRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(JsonSerializer.Serialize(settleRequest), Encoding.UTF8, "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync(testUrl + "Payment/Settle", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        var settleResponse = JsonSerializer.Deserialize<SettleResponseContract>(jsonResponse);
        return settleResponse;
    }
});

app.MapGet("/api/TerminalStatus/tpn={Tpn}&authkey={AuthKey}", async (string Tpn, string AuthKey) =>
{
    using (var httpClient = new HttpClient())
    {
        string url = testUrl + "/Common/TerminalStatus?" + $"request.tpn={Tpn}&request.authkey={AuthKey}";
        using HttpResponseMessage jsonMessage = await httpClient.GetAsync(url);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        var terminalStatusResponse = JsonSerializer.Deserialize<TerminalStatusResponseContract>(jsonResponse);
        return terminalStatusResponse;
    }
});

app.MapPost("/api/StatusList", async (StatusListRequestContract statusListRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(JsonSerializer.Serialize(statusListRequest), Encoding.UTF8, "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync(testUrl + "/Payment/StatusList", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        var statusListResponse = JsonSerializer.Deserialize<StatusListResponseContract>(jsonResponse);
        return statusListResponse;
    }
});

app.MapGet("/api/SaleResponse/id={id}", async (int id, ApplicationContext db) =>
{
    using (var httpClient = new HttpClient())
    {
        var saleResponse = await db.GetSaleResponse(id);
        return saleResponse;
    }
});

app.Run();