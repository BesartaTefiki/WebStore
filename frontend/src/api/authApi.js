import httpClient from "./httpClient";

export async function register(username, password) {
  const res = await httpClient.post("/auth/register", {
    username,  
    password,   
  });
  return res.data;
}

export async function login(username, password) {
  const res = await httpClient.post("/auth/login", {
    username,
    password,
  });
  return res.data;
}
