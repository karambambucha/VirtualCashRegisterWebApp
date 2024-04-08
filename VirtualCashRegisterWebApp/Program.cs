using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseStaticFiles();
app.Run(async (context) =>
{
    var response = context.Response;
    var request = context.Request;
    if (request.Path == "/api/user/Sale")
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
                        saleRequest.Amount,
                        saleRequest.TipAmount,
                        saleRequest.PaymentType,
                        ReferenceId = Guid.NewGuid().ToString(),
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
                    SaleResponse saleResponse = JsonConvert.DeserializeObject<SaleResponse>(jsonResponse);
                    //message = JsonConvert.DeserializeObject(jsonResponse).ToString();
                    message = saleResponse.ToString();
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
    public override string ToString()
    {
        string paymentType = "";
        switch (PaymentType)
        {
            default:
                paymentType = "ошибка";
                break;
            case "Credit":
                paymentType = "кредитная карта";
                break;
            case "Debit":
                paymentType = "дебетовая карта";
                break;
            case "EBT_Food":
                paymentType = "EBT Food";
                break;
            case "EBT_Cash":
                paymentType = "EBT Cash";
                break;
            case "Card":
                paymentType = "Card";
                break;
            case "Cash":
                paymentType = "наличные";
                break;
            case "Check":
                paymentType = "чек";
                break;
            case "Gift":
                paymentType = "подарочная карта";
                break;
        }
        return $"{GeneralResponse}\n\n{Amounts}\nВид оплаты: {paymentType}\n{CardData}\n\nID транзакции: {ReferenceId}\nНомер батча: {BatchNumber}\nЧеки:\n{Receipts}";
    }
}
public class Amounts
{
    public double TotalAmount;
    public double Amount;
    public double TipAmount;
    public double FeeAmount;
    public double TaxAmount;
    public override string ToString()
    {
        return $"Полная цена: {TotalAmount} руб., себестоимость: {Amount} руб., чаевые: {TipAmount} руб., налог: {TaxAmount} руб., сбор: {FeeAmount} руб.";
    }
}
public class GeneralResponse
{
    public string? ResultCode;
    public string? StatusCode;
    public string? Message;
    public string? DetailedMessage;
    public override string ToString()
    {
        return $"Сообщение: {Message}\nПодробнее: {DetailedMessage}\nКод результата: {ResultCode}, код статуса: {StatusCode}";
    }
}

public class CardData
{
    public string? CardType;
    public string? EntryType;
    public string? Last4;
    public string? First4;
    public string? BIN;
    public string? Name;
    public override string ToString()
    {
        return $"Данные карты:\nПлатежная система: {CardType}\nТип оплаты: {EntryType}\nПоследние 4 буквы: {Last4}, первые 4 бувы: {First4}\nБИН: {BIN}\nИмя владельца: {Name}";
    }
}
public class Receipts
{
    public string? Customer;
    public string? Merchant;
    public override string ToString()
    {
        var receiptString = new StringBuilder();
        if(Customer != null)
            receiptString.Append($"Чек покупателя: {Customer}");
        if (Merchant != null)
            receiptString.Append($"\n\nЧек продавца: {Merchant}");
        return receiptString.ToString();
    }
}