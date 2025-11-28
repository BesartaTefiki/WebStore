import httpClient from "./httpClient";

export async function getCategories() {
  const res = await httpClient.get("/categories");
  return res.data;
}

export async function getBrands() {
  const res = await httpClient.get("/brands");
  return res.data;
}

export async function getGenders() {
  const res = await httpClient.get("/genders");
  return res.data;
}

export async function getSizes() {
  const res = await httpClient.get("/sizes");
  return res.data;
}

export async function getColors() {
  const res = await httpClient.get("/colors");
  return res.data;
}
