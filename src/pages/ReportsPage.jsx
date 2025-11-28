import { useState } from "react";
import {
  getDailyReport,
  getMonthlyReport,
  getRangeReport,
  getTopProducts,
} from "../api/reportsApi";
import "../styles/reports.css";

export default function ReportsPage() {
 
  const [reportType, setReportType] = useState("daily");
  const [date, setDate] = useState("");
  const [monthYear, setMonthYear] = useState(""); // yyyy-mm 
  const [from, setFrom] = useState("");
  const [to, setTo] = useState("");
  const [topN, setTopN] = useState(5);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [report, setReport] = useState(null);
  const [topProducts, setTopProducts] = useState([]);

  const role = localStorage.getItem("role");
  const notAllowed = role !== "admin" && role !== "advanced";

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setReport(null);
    setTopProducts([]);
    setLoading(true);

    try {
      if (reportType === "daily") {
        if (!date) throw new Error("Please choose a date.");
        const data = await getDailyReport(date);
        setReport(data);
      } else if (reportType === "monthly") {
        if (!monthYear) throw new Error("Please choose a month.");
        const [yearStr, monthStr] = monthYear.split("-");
        const year = parseInt(yearStr, 10);
        const month = parseInt(monthStr, 10);
        const data = await getMonthlyReport(year, month);
        setReport(data);
      } else if (reportType === "range") {
        if (!from || !to) throw new Error("Please choose a date range.");
        const data = await getRangeReport(from, to);
        setReport(data);
      } else if (reportType === "top") {
        if (!from || !to) throw new Error("Please choose a date range.");
        const top = await getTopProducts(from, to, topN);
        setTopProducts(top);
      }
    } catch (err) {
      console.error(err);
      setError(err.message || "Failed to load report.");
    } finally {
      setLoading(false);
    }
  }

  if (notAllowed) {
    return (
      <div className="page reports-page">
        <h1 className="page-title">Reports</h1>
        <p className="reports-not-allowed">
          You must be <strong>admin</strong> or <strong>advanced</strong> to view
          reports.
        </p>
      </div>
    );
  }

  return (
    <div className="page reports-page">
      <h1 className="page-title">Reports</h1>
      <p className="page-subtitle">
        View earnings and top-selling products generated from your orders.
      </p>

      <form className="reports-form" onSubmit={handleSubmit}>
        <div className="reports-type-row">
          <label>Report type</label>
          <select
            value={reportType}
            onChange={(e) => {
              setReportType(e.target.value);
              setReport(null);
              setTopProducts([]);
              setError("");
            }}
          >
            <option value="daily">Daily earnings</option>
            <option value="monthly">Monthly earnings</option>
            <option value="range">Range earnings</option>
            <option value="top">Top-selling products</option>
          </select>
        </div>

        <div className="reports-inputs">
          {reportType === "daily" && (
            <div className="field">
              <label>Date</label>
              <input
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
              />
            </div>
          )}

          {reportType === "monthly" && (
            <div className="field">
              <label>Month</label>
              <input
                type="month"
                value={monthYear}
                onChange={(e) => setMonthYear(e.target.value)}
              />
            </div>
          )}

          {(reportType === "range" || reportType === "top") && (
            <>
              <div className="field">
                <label>From</label>
                <input
                  type="date"
                  value={from}
                  onChange={(e) => setFrom(e.target.value)}
                />
              </div>
              <div className="field">
                <label>To</label>
                <input
                  type="date"
                  value={to}
                  onChange={(e) => setTo(e.target.value)}
                />
              </div>
            </>
          )}

          {reportType === "top" && (
            <div className="field small">
              <label>Top N</label>
              <input
                type="number"
                min={1}
                max={20}
                value={topN}
                onChange={(e) => setTopN(Number(e.target.value))}
              />
            </div>
          )}

          <button className="btn-primary" type="submit" disabled={loading}>
            {loading ? "Loading..." : "Generate"}
          </button>
        </div>

        {error && <p className="reports-error">{error}</p>}
      </form>

      {report && (
        <div className="reports-result">
          <h2>Earnings</h2>
          <p>
            <strong>From:</strong>{" "}
            {new Date(report.fromDate).toLocaleString()}
          </p>
          <p>
            <strong>To:</strong> {new Date(report.toDate).toLocaleString()}
          </p>
          <p className="reports-total">
            Total earnings:{" "}
            <span>{report.totalEarnings.toFixed(2)} €</span>
          </p>
          {report.topProductId && (
            <p className="reports-top-product">
              Top product id: <strong>{report.topProductId}</strong>
            </p>
          )}
        </div>
      )}

      {topProducts.length > 0 && (
        <div className="reports-result">
          <h2>Top-selling products</h2>
          <table className="reports-table">
            <thead>
              <tr>
                <th>#</th>
                <th>Product</th>
                <th>Quantity sold</th>
                <th>Total earnings (€)</th>
              </tr>
            </thead>
            <tbody>
              {topProducts.map((p, idx) => (
                <tr key={p.productId}>
                  <td>{idx + 1}</td>
                  <td>{p.productName}</td>
                  <td>{p.quantitySold}</td>
                  <td>{p.totalEarnings.toFixed(2)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
