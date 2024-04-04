function sendRequest() {
  if (getTotalPrice() != 0.0) {
    console.log(
      JSON.stringify({
        Amount: getTotalPrice(),
        TipAmount: document.getElementById("custom-tip").value,
        PaymentType: "Credit",
        ReferenceId: generateGUID(),
        PrintReceipt: "No",
        GetReceipt: "Both",
        InvoiceNumber: "10",
        Tpn: "Z11MAKSTEST",
        Authkey: "zbhRAW9N6x",
      })
    );
  }
}

const productsList = document.getElementById("products-list");
const cartList = document.getElementById("cart-list");
const totalPriceElement = document.getElementById("total-price");
const clearCartButton = document.getElementById("clear-cart");

let products = [];

// Загрузка данных из JSON
fetch("products.json")
  .then((response) => response.json())
  .then((data) => {
    products = data;
    renderProducts();
  });

// Функция для отображения товаров
function renderProducts() {
  productsList.innerHTML = "";
  products.forEach((product) => {
    const li = document.createElement("li");
    li.textContent = `${product.name} - ${product.price} руб.`;
    li.onclick = () => addToCart(product);
    productsList.appendChild(li);
  });
}

// Функция для добавления товара в корзину
function addToCart(product) {
  const li = document.createElement("li");
  li.textContent = `${product.name} - ${product.price} руб.`;
  li.onclick = () => removeFromCart(li);
  cartList.appendChild(li);

  updateTotalPrice();
}

// Функция для удаления товара из корзины
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
// Функция для обновления итоговой цены
function updateTotalPrice() {
  totalPriceElement.textContent = getTotalPrice();
}

// Очистка корзины
clearCartButton.onclick = () => {
  cartList.innerHTML = "";
  updateTotalPrice();
};
document.getElementById("sendRequest").addEventListener("click", send);
async function send() {
    document.getElementById("saleResponse").innerText = "";
    var receiptSelect = document.getElementById("receipt-recieve");
    const response = await fetch("/api/user", {
        method: "POST",
        headers: { "Accept": "application/json", "Content-Type": "application/json" },
        body: JSON.stringify({
            Amount: document.getElementById("total-price").innerHTML,
            TipAmount: document.getElementById("custom-tip").value,
            PaymentType: "Credit",
            PrintReceipt: "No",
            GetReceipt: receiptSelect.options[receiptSelect.selectedIndex].value,
            InvoiceNumber: "10",
            Tpn: document.getElementById("tpn-input").value,
            Authkey: document.getElementById("auth-key-input").value
        })
    });
    const message = await response.json();
    document.getElementById("saleResponse").innerText = message.text;
};