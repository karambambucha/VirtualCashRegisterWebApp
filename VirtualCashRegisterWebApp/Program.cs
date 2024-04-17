using Microsoft.EntityFrameworkCore;
using VirtualCashRegisterWebApp.Data;
using Newtonsoft.Json;
using System.Text;
using static System.Net.WebRequestMethods;
using Newtonsoft.Json.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder();
string connection = "Server=(localdb)\\mssqllocaldb;Database=Sales;Trusted_Connection=True;";
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

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
        catch (Exception e)
        {
            message = e.Message;
        }
        await response.WriteAsJsonAsync(new { text = message });

    }
    else if (request.Path == "/api/user/TerminalStatus")
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
    else if (request.Path == "/api/user/StatusList")
    {
        try
        {
            var statusListRequest = await request.ReadFromJsonAsync<StatusListRequest>();
            if (statusListRequest != null)
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
                    message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                }
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }
        await response.WriteAsJsonAsync(new { text = message });
    }
    else if (request.Path == "/api/user/Status")
    {
        try
        {
            var statusRequest = await request.ReadFromJsonAsync<StatusRequest>();
            if (statusRequest != null)
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
                    message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                }
            }
        }
        catch (Exception e)
        {
            message = e.Message;
        }
        await response.WriteAsJsonAsync(new { text = message });
    }
    else if (request.Path == "/api/Products")
    {
        var product = new Product();
        product.Name = "Гавно";
        product.Cost = 55.01;
        var products = new List<Product>();
        products.Add(product);
        var jsonObject = JsonConvert.SerializeObject(products);

        await response.WriteAsJsonAsync(new { text = jsonObject });
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
public record StatusListRequest(string PaymentType, string TransactionFromIndex, string TransactionToIndex, string Tpn, string AuthKey);
public record StatusRequest(string ReferenceId, string PaymentType, string PrintReceipt, string GetReceipt, int MerchantNumber, bool CaptureSignature, bool GetExtendedData, string Tpn, string AuthKey, int SPInProxyTimeout);