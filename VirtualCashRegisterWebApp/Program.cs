using Newtonsoft.Json;
using System.Text;
using System.Text.Json;

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

                    using HttpResponseMessage jsonResponse = await httpClient.PostAsync("https://test.spinpos.net/spin/v2/Payment/Sale", jsonContent);
                    var saleResponse = await jsonResponse.Content.ReadAsStringAsync();
                    var jObj = JsonConvert.DeserializeObject(saleResponse);
                    message = jObj.ToString();
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