using System.Collections.Specialized;
using System.Net;
using System.Text;

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

                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["Amount"] = saleRequest.Amount;
                    data["TipAmount"] = saleRequest.TipAmount;
                    data["PaymentType"] = saleRequest.PaymentType;
                    data["ReferenceId"] = saleRequest.ReferenceId;
                    data["PrintReceipt"] = saleRequest.PrintReceipt;
                    data["GetReceipt"] = saleRequest.GetReceipt;
                    data["InvoiceNumber"] = saleRequest.InvoiceNumber;
                    data["Tpn"] = saleRequest.Tpn;
                    data["Authkey"] = saleRequest.Authkey;

                    var saleResponse = wb.UploadValues("https://test.spinpos.net/spin/v2/Payment/Sale", "POST", data);
                    string responseInString = Encoding.UTF8.GetString(saleResponse);
                    Console.WriteLine(responseInString);
                    message = responseInString;
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

public record SaleRequest(string Amount, string TipAmount, string PaymentType, string ReferenceId, string PrintReceipt, string GetReceipt, string InvoiceNumber, string Tpn, string Authkey);