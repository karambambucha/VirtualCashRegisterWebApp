using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles();
app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    if (request.Path == "/api/user")
    {
        var message = "Некорректные данные";
        try
        {
            var saleRequest = await request.ReadFromJsonAsync<SaleRequest>();

            if (saleRequest != null)
            {
                using (var httpClient = new HttpClient())
                {
                    using StringContent jsonContent = new(
                    System.Text.Json.JsonSerializer.Serialize(new
                    {
                        Amount = saleRequest.Amount,
                        TipAmount = saleRequest.TipAmount,
                        PaymentType = saleRequest.PaymentType,
                        ReferenceId = Guid.NewGuid().ToString(),
                        PrintReceipt = saleRequest.PrintReceipt,
                        GetReceipt = saleRequest.GetReceipt,
                        InvoiceNumber = saleRequest.InvoiceNumber,
                        Tpn = saleRequest.Tpn,
                        AuthKey = saleRequest.AuthKey,
                    }),
                    Encoding.UTF8,
                    "application/json");

                    using HttpResponseMessage jsonMessage = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/Sale", jsonContent);
                    var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
                    SaleResponse saleResponse = JsonConvert.DeserializeObject<SaleResponse>(jsonResponse);
                    message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                }
            }
        }
        catch { }
        await response.WriteAsJsonAsync(new { text = message });
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("wwwroot/index.html");
    }
});

app.Run();

public record SaleRequest(string Amount, string TipAmount, string PaymentType, string PrintReceipt, string GetReceipt, string InvoiceNumber, string Tpn, string AuthKey);
public class SaleResponse
{
    public Amounts? Amounts;
    public GeneralResponse? GeneralResponse;
    public CardData? CardData;
    public string? PaymentType;
    public string? TransactionType;
    public string? AuthCode;
    public string? ReferenceId;
    public string? InvoiceNumber;
    public string? BatchNumber;
    public Receipts? Receipts;

};

public class Amounts
{
    public double TotalAmount;
    public double Amount;
    public double TipAmount;
    public double FeeAmount;
    public double TaxAmount;
    public override string ToString()
    {
        return $"Полная цена: {TotalAmount} руб., себестоимость: {Amount} руб., чаевые: {TipAmount}";
    }
}
public class GeneralResponse
{
    public string? ResultCode;
    public string? StatusCode;
    public string? Message;
    public string? DetailedMessage;
}

public class CardData
{
    public string? CardType;
    public string? EntryType;
    public string? Last4;
    public string? First4;
    public string? BIN;
    public string? Name;
}
public class Receipts
{
    public string? Customer;
    public string? Merchant;
}