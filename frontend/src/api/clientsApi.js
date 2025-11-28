import httpClient from "./httpClient";


export async function getClients() {
  const response = await httpClient.get("/clients");
  return response.data;
}
