using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using VirtualCashRegisterWebApp.Data;

var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=Sales;Trusted_Connection=True;";
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

app.MapPost("/api/Sale", async (SaleRequest saleRequest, ApplicationContext db) =>
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
        dynamic json = JsonConvert.DeserializeObject(jsonResponse);
        if (json["GeneralResponse"]["Message"] == "Approved")
        {
            var cart = saleRequest.Products;
            var products = new List<Product>();
            foreach (ProductBase product in cart)
            {
                var prod = (from p in db.Products
                            where p.Id == product.Id
                            select p).ToList();
                products.Add(prod[0]);
            }
            var saleResponse = new SaleResponse
            {
                ReferenceId = json["ReferenceId"],
                TotalAmount = json["Amounts"]["TotalAmount"],
                Amount = json["Amounts"]["Amount"],
                TipAmount = json["Amounts"]["TipAmount"],
                FeeAmount = json["Amounts"]["FeeAmount"],
                TaxAmount = json["Amounts"]["TaxAmount"],
                PaymentType = json["PaymentType"],
                AuthCode = json["AuthCode"],
                CardType = json["CardData"]["CardType"],
                EntryType = json["CardData"]["EntryType"],
                Last4 = json["CardData"]["Last4"],
                First4 = json["CardData"]["First4"],
                BIN = json["CardData"]["BIN"],
                CardName = json["CardData"]["Name"],
                CustomerReceipt = json["Receipts"]["Customer"],
                MerchantReceipt = json["Receipts"]["Merchant"],
                Products = products
            };
            db.SaleResponses.Add(saleResponse);
            db.SaveChanges();
        }
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

app.MapPost("/api/StatusList", async (StatusListRequest statusListRequest) =>
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

app.MapGet("/api/Status/id={id}", async (int id, ApplicationContext db) =>
{
    using (var httpClient = new HttpClient())
    {
        var saleResponse = await db.SaleResponses
            .Include(c => c.Products).Select(x => new
            {
                id = x.Id,
                referenceId = x.ReferenceId,
                amount = x.Amount,
                authCode = x.AuthCode,
                bin = x.BIN,
                cardName = x.CardName,
                cardType = x.CardType,
                customerReceipt = x.CustomerReceipt,
                entryType = x.EntryType,
                feeAmount = x.FeeAmount,
                first4 = x.First4,
                last4 = x.Last4,
                merchantReceipt = x.MerchantReceipt,
                paymentType = x.PaymentType,
                taxAmount = x.TaxAmount,
                tipAmount = x.TipAmount,
                totalAmount = x.TotalAmount,
                products = x.Products.Select(k => new {k.Name, k.Cost}).ToList()
            })
            .FirstOrDefaultAsync(u => u.id == id);
        if (saleResponse == null) return Results.NotFound(new { message = "Запись не найдена" });
        return Results.Json(saleResponse);
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
    public List<ProductBase>? Products { get; set; }
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
    public bool CaptureSignature { get; set; }
    public bool GetExtendedData { get; set; }
    public string? Tpn { get; set; }
    public string? AuthKey { get; set; }
    public int SPInProxyTimeout { get; set; }
};

public class ProductBase
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public double Cost { get; set; }
}