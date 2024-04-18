using Microsoft.EntityFrameworkCore;
using VirtualCashRegisterWebApp.Data;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.DependencyInjection;
using NuGet.Protocol;

var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=Sales;Trusted_Connection=True;";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/Products", async (ApplicationContext db) => {

    var d = await db.Products.ToListAsync();
    return d;
});

app.MapPost("/api/Sale", async (SaleRequest saleRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(
        System.Text.Json.JsonSerializer.Serialize(new
        {
            saleRequest.Amount,
            saleRequest.TipAmount,
            saleRequest.PaymentType,
            saleRequest.ReferenceId,
            saleRequest.PrintReceipt,
            saleRequest.GetReceipt,
            saleRequest.InvoiceNumber,
            saleRequest.Tpn,
            saleRequest.AuthKey,
        }),
        Encoding.UTF8,
        "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/Sale", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        return Results.Json(jsonResponse);
    }
});

app.MapPost("/api/Settle", async (SettleRequest settleRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(
        System.Text.Json.JsonSerializer.Serialize(new
        {
            settleRequest.ReferenceId,
            settleRequest.GetReceipt,
            settleRequest.SettlementType,
            settleRequest.Tpn,
            settleRequest.AuthKey,
            settleRequest.SPInProxyTimeout
        }),
        Encoding.UTF8,
        "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/Settle", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        return Results.Json(jsonResponse);
    }
});

app.MapGet("/api/TerminalStatus/tpn={Tpn}&authkey={AuthKey}", async (string Tpn, string AuthKey) =>
{
    using (var httpClient = new HttpClient())
    {
        string url = "https://test.spinpos.net/spin/v2/Common/TerminalStatus?" + $"request.tpn={Tpn}&request.authkey={AuthKey}";
        using HttpResponseMessage jsonMessage = await httpClient.GetAsync(url);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        return jsonResponse;
    }
});

app.MapPost("/api/StatusList", async(StatusListRequest statusListRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(
        System.Text.Json.JsonSerializer.Serialize(new
        {
            statusListRequest.PaymentType,
            statusListRequest.TransactionFromIndex,
            statusListRequest.TransactionToIndex,
            statusListRequest.Tpn,
            statusListRequest.AuthKey
        }),
        Encoding.UTF8,
        "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/StatusList", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        return Results.Json(jsonResponse);
    }
});

app.MapPost("/api/Status", async (StatusRequest statusRequest) =>
{
    using (var httpClient = new HttpClient())
    {
        using StringContent jsonContent = new(
        System.Text.Json.JsonSerializer.Serialize(new
        {
            statusRequest.ReferenceId,
            statusRequest.PaymentType,
            statusRequest.PrintReceipt,
            statusRequest.GetReceipt,
            statusRequest.MerchantNumber,
            statusRequest.CaptureSignature,
            statusRequest.GetExtendedData,
            statusRequest.Tpn,
            statusRequest.AuthKey,
            statusRequest.SPInProxyTimeout
        }),
        Encoding.UTF8,
        "application/json");
        using HttpResponseMessage jsonMessage = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/Status", jsonContent);
        var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
        return Results.Json(jsonResponse);
    }
});

app.Run();

public class SaleRequest()
{
    public string? Amount { get; set; }
    public string? TipAmount { get; set; }
    public string? PaymentType { get; set; }
    public string? ReferenceId { get; set; }
    public string? PrintReceipt { get; set; }
    public string? GetReceipt { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? Tpn { get; set; }
    public string? AuthKey { get; set; }
};

public class SettleRequest()
{
    public string? ReferenceId { get; set; }
    public bool GetReceipt { get; set; }
    public string? SettlementType { get; set; }
    public string? Tpn { get; set; }
    public string? AuthKey { get; set; }
    public int SPInProxyTimeout { get; set; }
};
public class StatusListRequest()
{
    public string? PaymentType { get; set; }
    public string? TransactionFromIndex { get; set; }
    public string? TransactionToIndex { get; set; }
    public string? Tpn { get; set; }
    public string? AuthKey { get; set; }
};
public class StatusRequest()
{
    public string? ReferenceId { get; set; }
    public string? PaymentType { get; set; }
    public string? PrintReceipt { get; set; }
    public string? GetReceipt { get; set; }
    public int MerchantNumber { get; set; }
    public bool CaptureSignature{ get; set; }
    public bool GetExtendedData { get; set; }
    public string? Tpn { get; set; }
    public string? AuthKey { get; set; }
    public int SPInProxyTimeout { get; set; }
};