import httpClient from "./httpClient";

export async function getUsers() {
  const res = await httpClient.get("/users");
  return res.data;
}

export async function createUser({ username, password, role, isClient }) {
  const res = await httpClient.post("/users", {
    username,
    password,
    role,
    isClient,
  });
  return res.data;
}

export async function updateUserRole(id, role) {
  const res = await httpClient.put(`/users/${id}/role`, { role });
  return res.data; 
}


export async function deleteUser(id) {
  await httpClient.delete(`/users/${id}`);
}
