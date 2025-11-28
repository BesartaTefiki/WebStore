// src/api/productsApi.js
import httpClient from "./httpClient";

export async function getProducts() {
  const response = await httpClient.get("/products");
  const data = response.data;

  console.log("GET /products response data:", data);

  // ✅ CASE 1: backend returns plain array: [ { ... }, { ... } ]
  if (Array.isArray(data)) {
    return data;
  }

  // ✅ CASE 2: backend returns wrapper like { items: [...] }
  if (data && Array.isArray(data.items)) {
    return data.items;
  }

  // ✅ CASE 3: backend returns wrapper like { products: [...] }
  if (data && Array.isArray(data.products)) {
    return data.products;
  }

  // If we reach here, it’s not in a shape we expect
  console.error("getProducts() did not return a usable array shape:", data);
  return [];
}

export async function searchProducts(filters) {
  const params = new URLSearchParams();

  if (filters.categoryId) params.append("categoryId", filters.categoryId);
  if (filters.brandId) params.append("brandId", filters.brandId);
  if (filters.genderId) params.append("genderId", filters.genderId);
  if (filters.sizeId) params.append("sizeId", filters.sizeId);
  if (filters.colorId) params.append("colorId", filters.colorId);
  if (filters.priceMin) params.append("priceMin", filters.priceMin);
  if (filters.priceMax) params.append("priceMax", filters.priceMax);
  if (filters.inStock) params.append("inStock", "true");

  const response = await httpClient.get(
    `/products/search?${params.toString()}`
  );
  return response.data;
}

export async function createProduct(product) {
  const response = await httpClient.post("/products", product);
  return response.data;
}

export async function updateProduct(id, product) {
  await httpClient.put(`/products/${id}`, product);
}

export async function deleteProduct(id) {
  await httpClient.delete(`/products/${id}`);
}

export async function setProductDiscount(id, discountPercent) {
  await httpClient.put(`/products/${id}/discount`, { discountPercent });
}

export async function getProductQuantity(id) {
  const response = await httpClient.get(`/products/${id}/quantity`);
  return response.data;
}
