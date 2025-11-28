import httpClient from "./httpClient";

// items = [{ productId, quantity }]
export async function createOrder(items) {
  const response = await httpClient.post("/orders", {
    items,
  });
  return response.data;
}

// (admin + advanced only)
export async function getOrders() {
  const response = await httpClient.get("/orders");
  return response.data;
}

// (admin + advanced only)
export async function updateOrderStatus(orderId, status) {
  await httpClient.put(`/orders/${orderId}/status`, { status });
}
