using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles();
app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    var message = "Некорректные данные";
    if (request.Path == "/api/user/Sale")
    {
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
                    message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                }
            }
        }
        catch { }
        await response.WriteAsJsonAsync(new { text = message });
    }
    else if (request.Path == "/api/user/Settle")
    {
        try
        {
            var settleRequest = await request.ReadFromJsonAsync<SettleRequest>();
            if (settleRequest != null)
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
                    message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                }
            }
        }
        catch(Exception e)
        {
            message = e.Message;
        }
        await response.WriteAsJsonAsync(new { text = message });

    }
    else if(request.Path == "/api/user/TerminalStatus")
    {
        try
        {
            using (var httpClient = new HttpClient())
            {
                string url = "https://test.spinpos.net/spin/v2/Common/TerminalStatus" + request.QueryString;
                using HttpResponseMessage jsonMessage = await httpClient.GetAsync(url);
                var jsonResponse = await jsonMessage.Content.ReadAsStringAsync();
                message = jsonResponse;
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }
        await response.WriteAsJsonAsync(new { text = message });
    }
    else
    {
        response.ContentType = "text/html; charset=utf-8";
        await response.SendFileAsync("wwwroot/index.html");
    }
});

app.Run();

public record SaleRequest(string Amount, string TipAmount, string PaymentType, string ReferenceId, string PrintReceipt, string GetReceipt, string InvoiceNumber, string Tpn, string AuthKey);
public record SettleRequest(string ReferenceId, bool GetReceipt, string SettlementType, string Tpn, string AuthKey, int SPInProxyTimeout);
public record TerminalStatusRequest(string Tpn, int AuthKey);