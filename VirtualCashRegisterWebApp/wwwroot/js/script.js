const productsList = document.getElementById("products-list");
const cartList = document.getElementById("cart-list");
const totalPriceElement = document.getElementById("total-price");
const clearCartButton = document.getElementById("clear-cart");

getProduct();

async function getProduct() {
  const response = await fetch("/api/Products", {
    method: "GET",
    headers: { Accept: "application/json" },
  });
  if (response.ok === true) {
    const products = await response.json();
    products.sort((a, b) => a.name > b.name ? 1 : -1);
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
}
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
document
  .getElementById("send-sale-request")
  .addEventListener("click", sendSaleRequest);
function ReadCart(cart) {
  var products = [];
  for (let item of cart) {
    products.push({
      id: item.id,
      name: item.name,
      cost: item.cost,
    });
  }
  return products;
}
async function sendSaleRequest() {
  var cart = document.getElementById("cart-list").getElementsByTagName("li");
  var products = ReadCart(cart);
  if (cart.length > 0) {
    document.getElementById("sale-response").innerText = "";
    document.getElementById("sale-response-text").innerText = "";
    document.getElementById("receipt-customer").innerText = "";
    document.getElementById("receipt-merchant").innerText = "";
    var receiptSelect = document.getElementById("receipt-recieve");
    var receiptPrintSelect = document.getElementById("receipt-print");
    var paymentSelect = document.getElementById("payment-type");
    var obj = {
      Amount: parseFloat(document.getElementById("total-price").innerHTML),
      TipAmount: parseFloat(document.getElementById("custom-tip").value),
      PaymentType: paymentSelect.options[paymentSelect.selectedIndex].value,
      ReferenceId: generateGUID(),
      PrintReceipt:
        receiptPrintSelect.options[receiptPrintSelect.selectedIndex].value,
      GetReceipt: receiptSelect.options[receiptSelect.selectedIndex].value,
      Tpn: document.getElementById("tpn-input").value,
      Authkey: document.getElementById("auth-key-input").value,
      Products: products,
    };
    var requestBody = JSON.stringify(obj, null, 4);
    document.getElementById("sale-request").innerText = requestBody;
    await fetch("/api/Sale", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: requestBody,
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        }
      })
      .then((responseJson) => {
        document.getElementById("sale-response").innerText = JSON.stringify(
          responseJson,
          null,
          4
        );
        document.getElementById("sale-response-text").innerText =
          deserializeJsonSaleResponse(responseJson);
        if (responseJson.generalResponse.resultCode == "Ok") {
          document.getElementById("receipt-customer").innerHTML =
            responseJson.receipts.customer;
          document.getElementById("receipt-merchant").innerHTML =
            responseJson.receipts.merchant;
        }
      });
  } else {
    alert("Корзина пуста!");
  }
}
function deserializeJsonSaleResponse(json) {
  let response = `Ответ: ${json.generalResponse.message}
    Детальный ответ: ${json.generalResponse.detailedMessage}
    Код результата ${json.generalResponse.resultCode}, код статуса: ${json.generalResponse.statusCode}\n\n`;
  if (json.generalResponse.resultCode == "Ok") {
    if (json.hasOwnProperty("amounts"))
      response += `Полная цена: ${json.amounts.totalAmount} руб., себестоимость: ${json.amounts.amount} руб., 
        чаевые: ${json.amounts.tipAmount} руб., взнос: ${json.amounts.feeAmount} руб., налоги: ${json.amounts.taxAmount} руб.\n`;
    if (json.hasOwnProperty("referenceId"))
      response += `ID транзакции: ${json.referenceId}\n\n`;
    if (json.hasOwnProperty("voided")) {
      response += "Отменено: ";
      response +=
        json.hasOwnProperty("voided") === false ? "Да\n\n" : "Нет\n\n";
    }
    if (json.hasOwnProperty("paymentType"))
      response += `Вид оплаты: ${json.paymentType}\n`;
    if (json.hasOwnProperty("cardData"))
      response += `Платежная система: ${json.cardData.cardType}\nСпособ оплаты: ${json.cardData.entryType}
        Номер карты: ${json.cardData.first4} **** **** ${json.cardData.last4}\nБИН: ${json.cardData.bin}
        Имя владельца: ${json.cardData.name}`;
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
    SPInProxyTimeout: parseFloat(
      document.getElementById("settle-timeout").value
    ),

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
  document.getElementById("settle-response").innerText = JSON.stringify(
    message,
    null,
    4
  );
  deserializeJsonSettleResponse(message);
}

function deserializeJsonSettleResponse(json) {
  let response = `ЗАКРЫТИЕ СМЕНЫ
  Ответ: ${json.generalResponse.message}
  Детальный ответ: ${json.generalResponse.detailedMessage}
  Код результата ${json.generalResponse.resultCode}, код статуса: ${json.generalResponse.statusCode}\n\n`;
  if(json.generalResponse.detailedMessage == "Ok")
  {
    settleDetails = json.settleDetails[0];
    response += `Приложение: ${settleDetails.application}
    Сообщение: ${settleDetails.detailedMessage}, статус хоста: ${settleDetails.hostStatus}\n`;
    transactionsReports = settleDetails.transactionsReports;
    response += `Всего транзакций в смене: ${transactionsReports.transactionsCount}
    Продано на ${transactionsReports.saleAmount} руб., на возврат: ${transactionsReports.returnAmount} руб., отменено транзакции на ${transactionsReports.voidAmount} руб.
    Всего: ${transactionsReports.totalAmount} руб.`;
    var receiptSettleSelect = document.getElementById("settle-is-receipt");
      if (
          receiptSettleSelect.options[receiptSettleSelect.selectedIndex].value ===
          "true"
      )
        document.getElementById("settle-receipt").innerHTML = settleDetails.receipt;
  }
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
  if (message.terminalStatus == "Online")
    statusText.innerHTML = `Терминал ${Tpn} подключен!`;
  else if (message.terminalStatus == "Offline")
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
    TransactionFromIndex: parseFloat(
      document.getElementById("status-list-from").value
    ),
    TransactionToIndex: parseFloat(
      document.getElementById("status-list-to").value
    ),
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
  document.getElementById("status-list-response").innerText = JSON.stringify(
    message,
    null,
    4
  );
  deserializeJsonStatusListResponse(message);
}

function deserializeJsonStatusListResponse(json) {
  let response = `ПРОСМОТР ТРАНЗАКЦИИ НА СМЕНЕ
    Ответ: ${json.generalResponse.message}
    Детальный ответ: ${json.generalResponse.detailedMessage}
    Код результата ${json.generalResponse.resultCode}, код статуса: ${json.generalResponse.statusCode}\n\n`;
  if (json.hasOwnProperty("transactions")) {
    response += `Всего получено транзакций: ${json.transactions.length}\n\n`;
    var transactions = json.transactions;
    transactions.forEach((jsonObject) => {
      response += `Номер транзакции в смене: ${jsonObject.transactionNumber}`;
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
    const jsonResponse = await fetch(`api/SaleResponse/id=${id}`);
  const json = await jsonResponse.json();
  if (json != null) {
    var response = `ТРАНЗАКЦИЯ ${json.id}\n\n`;
    response += `Полная цена: ${json.totalAmount} руб., себестоимость: ${json.amount} руб., 
        чаевые: ${json.tipAmount} руб., взнос: ${json.feeAmount} руб., налоги: ${json.taxAmount} руб.\n`;
    response += `ID транзакции: ${json.referenceId}\n\n`;
    response += `Вид оплаты: ${json.paymentType}\n`;
    response += `Платежная система: ${json.cardType}\nСпособ оплаты: ${json.entryType}
            Номер карты: ${json.first4} **** **** ${json.last4}\nБИН: ${json.bin}
            Имя владельца: ${json.cardName}`;
    if (json.products.length > 0) {
      response += "\n\n====ПРОДУКТЫ====\n";
      var productsText = "";
      for (let item of json.products) {
        productsText += `${item.name} - ${item.cost} руб. Количество: ${item.productCount}\n`;
      }
      response += productsText;
    }
    document.getElementById("status-response-text").innerText = response;
    document.getElementById("status-receipt-customer").innerHTML =
      json.customerReceipt;
    document.getElementById("status-receipt-merchant").innerHTML =
      json.merchantReceipt;
  } else {
    document.getElementById(
      "status-response-text"
    ).innerText = `Запись с номером ${id} не найдена`;
  }
  document.getElementById("status-response").innerText = JSON.stringify(
    json,
    null,
    4
  );
}

document
  .getElementById("send-sale-request-simple")
  .addEventListener("click", sendSimpleSaleRequest);
async function sendSimpleSaleRequest() {
  if (parseFloat(document.getElementById("cost-simple").value) >= 0.01) {
    document.getElementById("sale-response-simple").innerText = "";
    document.getElementById("sale-response-text-simple").innerText = "";
    document.getElementById("receipt-customer-simple").innerText = "";
    document.getElementById("receipt-merchant-simple").innerText = "";

    var receiptSelect = document.getElementById("receipt-recieve-simple");
    var receiptPrintSelect = document.getElementById("receipt-print-simple");
    var paymentSelect = document.getElementById("payment-type-simple");
    var obj = {
      Amount: parseFloat(document.getElementById("cost-simple").value),
      TipAmount: parseFloat(document.getElementById("custom-tip-simple").value),
      PaymentType: paymentSelect.options[paymentSelect.selectedIndex].value,
      ReferenceId: document.getElementById("reference-id-simple").value,
      PrintReceipt:
        receiptPrintSelect.options[receiptPrintSelect.selectedIndex].value,
      GetReceipt: receiptSelect.options[receiptSelect.selectedIndex].value,
      Tpn: document.getElementById("tpn-input").value,
      Authkey: document.getElementById("auth-key-input").value,
    };
    var requestBody = JSON.stringify(obj, null, 4);
    document.getElementById("sale-request-simple").innerText = requestBody;
    await fetch("/api/SaleSimple", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: requestBody,
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        }
      })
      .then((responseJson) => {
        document.getElementById("sale-response-simple").innerText = JSON.stringify(responseJson, null, 4);
        document.getElementById("sale-response-text-simple").innerText =deserializeJsonSaleResponse(responseJson);
        document.getElementById("receipt-customer-simple").innerHTML = responseJson.receipts.customer;
        document.getElementById("receipt-merchant-simple").innerHTML = responseJson.receipts.merchant;
      });
  } else {
    alert("Стоимость должна быть больше 0.01!");
  }
}
document
  .getElementById("generate-guid-simple")
  .addEventListener("click", generateGUIDSimple);
function generateGUIDSimple() {
  document.getElementById("reference-id-simple").value = generateGUID();
}
