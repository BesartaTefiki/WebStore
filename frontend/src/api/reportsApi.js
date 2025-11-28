import httpClient from "./httpClient";

// yyyy-mm-dd
export async function getDailyReport(date) {
  const response = await httpClient.get("/reports/daily", {
    params: { date }, 
  });
  return response.data;
}


export async function getMonthlyReport(year, month) {
  const response = await httpClient.get("/reports/monthly", {
    params: { year, month },
  });
  return response.data;
}

export async function getRangeReport(from, to) {
  const response = await httpClient.get("/reports", {
    params: { from, to },
  });
  return response.data;
}

export async function getTopProducts(from, to, topN = 5) {
  const response = await httpClient.get("/reports/top-products", {
    params: { from, to, topN },
  });
  return response.data;
}
