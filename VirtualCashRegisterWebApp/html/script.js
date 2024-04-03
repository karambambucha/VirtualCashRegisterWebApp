function generateGUID() {
  return "10000000-1000-4000-8000-100000000000".replace(/[018]/g, (c) =>
    (
      +c ^
      (crypto.getRandomValues(new Uint8Array(1))[0] & (15 >> (+c / 4)))
    ).toString(16)
  );
}

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

