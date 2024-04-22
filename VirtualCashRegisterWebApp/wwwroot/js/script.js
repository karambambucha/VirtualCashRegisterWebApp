const productsList = document.getElementById("products-list");
const cartList = document.getElementById("cart-list");
const totalPriceElement = document.getElementById("total-price");
const clearCartButton = document.getElementById("clear-cart");

getProduct();

async function getProduct()
{
    const response = await fetch("/api/Products", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const products = await response.json();
        renderProducts(products);
    }
}
function renderProducts(products) {
  productsList.innerHTML = "";
  products.forEach((product) => {
      const li = document.createElement("li");
      li.textContent = `${product.name} - ${product.cost} руб.`;
      li.id = product.id;
      li.onclick = () => addToCart(product);
      productsList.appendChild(li);
  });
  function generateGUID() {
    var guid = "";
    for (var i = 0; i < 32; i++) {
      guid += Math.floor(Math.random() * 16).toString(16);
    }
    return guid;
  }
  function addToCart(product) {
    const li = document.createElement("li");
      li.textContent = `${product.name} - ${product.cost} руб.`;
      li.id = product.id;
      li.name = product.name;
      li.cost = product.cost;
    li.onclick = () => removeFromCart(li);
    cartList.appendChild(li);

    updateTotalPrice();
  }

  function removeFromCart(_li) {
    cartList.removeChild(_li);
    updateTotalPrice();
  }

  function getTotalPrice() {
    let totalPrice = 0;
    const cartItems = cartList.querySelectorAll("li");
    cartItems.forEach((item) => {
      const price = parseFloat(item.textContent.split(" - ")[1]);
      totalPrice += price;
    });
    return totalPrice.toFixed(2);
  }

  function updateTotalPrice() {
    totalPriceElement.textContent = getTotalPrice();
  }

  clearCartButton.onclick = () => {
    cartList.innerHTML = "";
    updateTotalPrice();
  };
    document.getElementById("send-sale-request").addEventListener("click", sendSaleRequest);
    function ReadCart(cart) {
        var products = [];
        for (let item of cart) {
            products.push({
                id: item.id,
                name: item.name,
                cost: item.cost
            })
        }
        return products;
    }
    async function sendSaleRequest() {
        var cart = document.getElementById("cart-list").getElementsByTagName("li");
        var products = ReadCart(cart);
    if (cart.length > 0) 
    {
      document.getElementById("sale-response").innerText = "";
      document.getElementById("sale-response-text").innerText = "";
      document.getElementById("receipt-customer").innerText = "";
      document.getElementById("receipt-merchant").innerText = "";
      var receiptSelect = document.getElementById("receipt-recieve");
      var receiptPrintSelect = document.getElementById("receipt-print");
        var paymentSelect = document.getElementById("payment-type");
      var obj = {
        Amount: document.getElementById("total-price").innerHTML,
        TipAmount: document.getElementById("custom-tip").value,
        PaymentType: paymentSelect.options[paymentSelect.selectedIndex].value,
        ReferenceId: generateGUID(),
        PrintReceipt: receiptPrintSelect.options[receiptPrintSelect.selectedIndex].value,
        GetReceipt: receiptSelect.options[receiptSelect.selectedIndex].value,
        InvoiceNumber: "10",
        Tpn: document.getElementById("tpn-input").value,
        Authkey: document.getElementById("auth-key-input").value,
        Products: products
      };
      var requestBody = JSON.stringify(obj, null, 4);
      document.getElementById("sale-request").innerText = requestBody;
      const response = await fetch("/api/Sale", {
        method: "POST",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: requestBody,
      });
      const message = await response.json();
      const json = JSON.parse(message);
        document.getElementById("sale-response").innerText = JSON.stringify(json, null, 4);
      if (json.hasOwnProperty("Receipts")) {
        receipts = json.Receipts;
        if (receipts.hasOwnProperty("Customer"))
          document.getElementById("receipt-customer").innerHTML =
            json.Receipts.Customer;
        if (receipts.hasOwnProperty("Merchant"))
          document.getElementById("receipt-merchant").innerHTML =
            json.Receipts.Merchant;
      }
      document.getElementById("sale-response-text").innerText =
        deserializeJsonSaleResponse(json);
    } else {
      alert("Корзина пуста!");
    }
  }
  function deserializeJsonSaleResponse(json) {
    let response = `Ответ: ${json.GeneralResponse.Message}
    Детальный ответ: ${json.GeneralResponse.DetailedMessage}
    Код результата ${json.GeneralResponse.ResultCode}, код статуса: ${json.GeneralResponse.StatusCode}\n\n`;
    if (json.GeneralResponse.ResultCode == 0) {
      if (json.hasOwnProperty("Amounts"))
        response += `Полная цена: ${json.Amounts.TotalAmount} руб., себестоимость: ${json.Amounts.Amount} руб., 
        чаевые: ${json.Amounts.TipAmount} руб., взнос: ${json.Amounts.FeeAmount} руб., налоги: ${json.Amounts.TaxAmount} руб.\n`;
      if (json.hasOwnProperty("ReferenceId"))
        response += `ID транзакции: ${json.ReferenceId}\n\n`;
      if (json.hasOwnProperty("Voided")) {
        response += "Отменено: ";
        response +=
          json.hasOwnProperty("Voided") === false ? "Да\n\n" : "Нет\n\n";
      }
      if (json.hasOwnProperty("PaymentType"))
        response += `Вид оплаты: ${json.PaymentType}\n`;
      if (json.hasOwnProperty("CardData"))
        response += `Платежная система: ${json.CardData.CardType}\nСпособ оплаты: ${json.CardData.EntryType}
        Номер карты: ${json.CardData.First4} **** **** ${json.CardData.Last4}\nБИН: ${json.CardData.BIN}
        Имя владельца: ${json.CardData.Name}`;
    }
    return response;
  }

  document
    .getElementById("send-settle-request")
    .addEventListener("click", sendSettleRequest);
  async function sendSettleRequest() {
    document.getElementById("settle-response").innerText = "";
    document.getElementById("settle-response-text").innerText = "";
    document.getElementById("settle-receipt").innerText = "";
    var receiptSettleSelect = document.getElementById("settle-is-receipt");
    var obj = {
      ReferenceId: "string",
      GetReceipt:
        receiptSettleSelect.options[receiptSettleSelect.selectedIndex].value ===
        "true",
      SPInProxyTimeout: document.getElementById("settle-timeout").value,
      SettlementType: "Close",
      Tpn: document.getElementById("tpn-input").value,
      Authkey: document.getElementById("auth-key-input").value,
    };
    var requestBody = JSON.stringify(obj, null, 4);
    document.getElementById("settle-request").innerText = requestBody;
    const response = await fetch("/api/Settle", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: requestBody,
    });
    const message = await response.json();
    const json = JSON.parse(message);
    document.getElementById("settle-response").innerText = JSON.stringify(json, null, 4);
    deserializeJsonSettleResponse(json);
  }

  function deserializeJsonSettleResponse(json) {
    let response = `ЗАКРЫТИЕ СМЕНЫ
  Ответ: ${json.GeneralResponse.Message}
  Детальный ответ: ${json.GeneralResponse.DetailedMessage}
  Код результата ${json.GeneralResponse.ResultCode}, код статуса: ${json.GeneralResponse.StatusCode}\n\n`;
    settleDetails = json.SettleDetails[0];
    response += `Приложение: ${settleDetails.Application}
  Сообщение: ${settleDetails.DetailedMessage}, статус хоста: ${settleDetails.HostStatus}\n`;
    transactionsReports = settleDetails.TransactionsReports;
    response += `Всего транзакций в смене: ${transactionsReports.TransactionsCount}
  Продано на ${transactionsReports.SaleAmount} руб., на возврат: ${transactionsReports.ReturnAmount} руб., отменено транзакции на ${transactionsReports.VoidAmount} руб.
  Всего: ${transactionsReports.TotalAmount} руб.`;
    var receiptSettleSelect = document.getElementById("settle-is-receipt");
    if (
      receiptSettleSelect.options[receiptSettleSelect.selectedIndex].value ===
      "true"
    )
    document.getElementById("settle-receipt").innerHTML = settleDetails.Receipt;
    document.getElementById("settle-response-text").innerText = response;
  }

  document
    .getElementById("check-connect")
    .addEventListener("click", sendTerminalStatusRequest);
  async function sendTerminalStatusRequest() {
    var statusText = document.getElementById("connect-status");
    statusText.innerHTML = "";
    var Tpn = document.getElementById("tpn-input").value;
    var Authkey = document.getElementById("auth-key-input").value;
    const response = await fetch(
      `api/TerminalStatus/tpn=${Tpn}&authkey=${Authkey}`
    );
    const message = await response.json();
    if (message.TerminalStatus == "Online")
      statusText.innerHTML = `Терминал ${Tpn} подключен!`;
    else if (message.TerminalStatus == "Offline")
      statusText.innerHTML = `Терминал  ${Tpn} не подключен`;
    else statusText.innerHTML = `Терминал  ${Tpn} не найден!`;
  }

  document
    .getElementById("send-status-list-request")
    .addEventListener("click", sendStatusListRequest);
  async function sendStatusListRequest() {
    document.getElementById("status-list-response").innerText = "";
    document.getElementById("status-list-response-text").innerText = "";
    var obj = {
      PaymentType: "Credit",
      TransactionFromIndex: document.getElementById("status-list-from").value,
      TransactionToIndex: document.getElementById("status-list-to").value,
      Tpn: document.getElementById("tpn-input").value,
      AuthKey: document.getElementById("auth-key-input").value,
    };
    var requestBody = JSON.stringify(obj, null, 4);
    document.getElementById("status-list-request").innerText = requestBody;
    const response = await fetch("/api/StatusList", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: requestBody,
    });
    const message = await response.json();
    const json = JSON.parse(message);
      document.getElementById("status-list-response").innerText = JSON.stringify(json, null, 4);
    deserializeJsonStatusListResponse(json);
  }

  function deserializeJsonStatusListResponse(json) {
    let response = `ПРОСМОТР ТРАНЗАКЦИИ НА СМЕНЕ
    Ответ: ${json.GeneralResponse.Message}
    Детальный ответ: ${json.GeneralResponse.DetailedMessage}
    Код результата ${json.GeneralResponse.ResultCode}, код статуса: ${json.GeneralResponse.StatusCode}\n\n`;
    if (json.hasOwnProperty("Transactions")) {
      response += `Всего получено транзакций: ${json.Transactions.length}\n\n`;
      var transactions = json.Transactions;
      transactions.forEach((jsonObject) => {
        response += `Номер транзакции в смене: ${jsonObject.TransactionNumber}`;
        response += "\n=======================\n\n";
        response += deserializeJsonSaleResponse(jsonObject);
        response += "\n=======================\n\n\n\n";
      });
    }
    document.getElementById("status-list-response-text").innerText = response;
  }

  document
    .getElementById("send-status-request")
    .addEventListener("click", sendStatusRequest);
  async function sendStatusRequest() {
    document.getElementById("status-response").innerText = "";
    document.getElementById("status-response-text").innerText = "";
    document.getElementById("status-receipt-customer").innerText = "";
    document.getElementById("status-receipt-merchant").innerText = "";
      var id = document.getElementById("status-reference-id").value;
      const jsonResponse = await fetch(
          `api/Status/id=${id}`
      );
      const json = await jsonResponse.json();
      if (json.message != "Запись не найдена") {
          var response = `ТРАНЗАКЦИЯ ${json.id}\n\n`;
          response += `Полная цена: ${json.totalAmount} руб., себестоимость: ${json.amount} руб., 
        чаевые: ${json.tipAmount} руб., взнос: ${json.feeAmount} руб., налоги: ${json.taxAmount} руб.\n`;
          response += `ID транзакции: ${json.referenceId}\n\n`;
          response += `Вид оплаты: ${json.paymentType}\n`;
          response += `Платежная система: ${json.cardType}\nСпособ оплаты: ${json.entryType}
            Номер карты: ${json.first4} **** **** ${json.last4}\nБИН: ${json.bin}
            Имя владельца: ${json.cardName}`;
          console.log(json.products);
          response += "\n\n====ПРОДУКТЫ====\n";
          var productsText = "";
          for (let item of json.products) {
              productsText += `${item.name} - ${item.cost} руб.\n`;
          }
          response += productsText;
          document.getElementById("status-response-text").innerText = response;
          document.getElementById("status-receipt-customer").innerHTML = json.customerReceipt;
          document.getElementById("status-receipt-merchant").innerHTML = json.merchantReceipt;
      }
      else
      {
          document.getElementById("status-response-text").innerText = `Запись с номером ${id} не найдена`;
      }
      document.getElementById("status-response").innerText = JSON.stringify(json, null, 4);
      
  }
}
