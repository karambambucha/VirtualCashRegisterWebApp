const productsList = document.getElementById("products-list");
const cartList = document.getElementById("cart-list");
const totalPriceElement = document.getElementById("total-price");
const clearCartButton = document.getElementById("clear-cart");

let products = [];

fetch("products.json")
  .then((response) => response.json())
  .then((data) => {
    products = data;
    renderProducts();
  });
function generateGUID() {
  var guid = "";
  for (var i = 0; i < 32; i++) {
    guid += Math.floor(Math.random() * 16).toString(16);
  }
  return guid;
}

function renderProducts() {
  productsList.innerHTML = "";
  products.forEach((product) => {
    const li = document.createElement("li");
    li.textContent = `${product.name} - ${product.price} руб.`;
    li.onclick = () => addToCart(product);
    productsList.appendChild(li);
  });
}

function addToCart(product) {
  const li = document.createElement("li");
  li.textContent = `${product.name} - ${product.price} руб.`;
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
document
  .getElementById("send-sale-request")
  .addEventListener("click", sendSaleRequest);
async function sendSaleRequest() {
  if (
    document.getElementById("cart-list").getElementsByTagName("li").length > 0
  ) {
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
      PrintReceipt:
        receiptPrintSelect.options[receiptPrintSelect.selectedIndex].value,
      GetReceipt: receiptSelect.options[receiptSelect.selectedIndex].value,
      InvoiceNumber: "10",
      Tpn: document.getElementById("tpn-input").value,
      Authkey: document.getElementById("auth-key-input").value,
    };
    var requestBody = JSON.stringify(obj, null, 4);
    document.getElementById("sale-request").innerText = requestBody;
    const response = await fetch("/api/user/Sale", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: requestBody,
    });
    const message = await response.json();
    document.getElementById("sale-response").innerText = message.text;
    const json = JSON.parse(message.text);
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

function openMainTab(evt, checkName) {
  var i, tabcontent, tablinks;

  tabcontent = document.getElementsByClassName("main-tabcontent");
  for (i = 0; i < tabcontent.length; i++) {
    tabcontent[i].style.display = "none";
  }
  tablinks = document.getElementsByClassName("main-tablinks");
  for (i = 0; i < tablinks.length; i++) {
    tablinks[i].className = tablinks[i].className.replace(" active", "");
  }

  document.getElementById(checkName).style.display = "block";
  evt.currentTarget.className += " active";
}

document.getElementById("defaultOpenMainTab").click();

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
  const response = await fetch("/api/user/Settle", {
    method: "POST",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: requestBody,
  });
  const message = await response.json();
  document.getElementById("settle-response").innerText = message.text;
  const json = JSON.parse(message.text);
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
    `api/user/TerminalStatus?request.tpn=${Tpn}&request.authkey=${Authkey}`
  );
  const message = await response.json();
  const json = JSON.parse(message.text);
  if (json.TerminalStatus == "Online")
    statusText.innerHTML = "Терминал подключен!";
  else if (json.TerminalStatus == "Offline")
    statusText.innerHTML = "Терминал не подключен";
  else
    statusText.innerHTML = "Терминал не найден!";
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
  const response = await fetch("/api/user/StatusList", {
    method: "POST",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: requestBody,
  });
  const message = await response.json();
  const json = JSON.parse(message.text);
  document.getElementById("status-list-response").innerText = message.text;
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
  var obj = {
    PaymentType: "Credit",
    MerchantNumber: 1,
    PrintReceipt: "Both",
    GetReceipt: "Both",
    ReferenceId: document.getElementById("status-reference-id").value,
    CaptureSignature: true,
    GetExtendedData: true,
    Tpn: document.getElementById("tpn-input").value,
    AuthKey: document.getElementById("auth-key-input").value,
    SPInProxyTimeout: document.getElementById("status-timeout").value,
  };
  var requestBody = JSON.stringify(obj, null, 4);
  document.getElementById("status-request").innerText = requestBody;
  const response = await fetch("/api/user/Status", {
    method: "POST",
    headers: { Accept: "application/json", "Content-Type": "application/json" },
    body: requestBody,
  });
  const message = await response.json();
  const json = JSON.parse(message.text);
  document.getElementById("status-response").innerText = message.text;
  if (json.hasOwnProperty("Receipts")) {
    receipts = json.Receipts;
    if (receipts.hasOwnProperty("Customer"))
      document.getElementById("status-receipt-customer").innerHTML =
        json.Receipts.Customer;
    if (receipts.hasOwnProperty("Merchant"))
      document.getElementById("status-receipt-merchant").innerHTML =
        json.Receipts.Merchant;
  }
  document.getElementById("status-response-text").innerText =
    deserializeJsonSaleResponse(json);
}
