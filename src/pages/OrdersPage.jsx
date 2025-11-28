import { useEffect, useState } from "react";
import { getOrders, updateOrderStatus } from "../api/ordersApi";
import "../styles/orders.css";

export default function OrdersPage() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [statusDrafts, setStatusDrafts] = useState({}); // Order can be: "Pending" , "Confirmed" ,"Cancelled" 

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        setError("");

        const ordersData = await getOrders();
        setOrders(ordersData || []);

        const drafts = {};
        (ordersData || []).forEach((o) => {
          drafts[o.id] = o.status;
        });
        setStatusDrafts(drafts);
      } catch (err) {
        console.error(err);
        setError("Failed to load orders.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, []);

  async function handleUpdateStatus(orderId) {
    setError("");
    setSuccess("");

    const newStatus = statusDrafts[orderId];
    if (!newStatus) return;

    try {
      await updateOrderStatus(orderId, newStatus);

      setOrders((prev) =>
        prev.map((o) =>
          o.id === orderId ? { ...o, status: newStatus } : o
        )
      );

      setSuccess(`Order #${orderId} status updated to ${newStatus}.`);
    } catch (err) {
      console.error(err);
      setError("Failed to update order status.");
    }
  }

  if (loading) return <p style={{ padding: "2rem" }}>Loading orders...</p>;

  return (
    <div className="orders-page">
      <h1 className="page-title">Orders</h1>

      {error && <p className="order-error">{error}</p>}
      {success && <p className="order-success">{success}</p>}

      <section className="orders-list">
        {!orders.length && <p>No orders found.</p>}

        {orders.map((o) => (
          <article key={o.id} className="order-card">
            <header className="order-card-header">
              <div>
                <h3>Order #{o.id}</h3>
                <p className="order-meta">
                  <span>{new Date(o.createdAt).toLocaleString()}</span>
                  <span
                    className={`order-status order-status-${o.status.toLowerCase()}`}
                  >
                    {o.status}
                  </span>
                </p>

             
                <div className="order-status-edit">
                  <label>
                    Change status:
                    <select
                      value={statusDrafts[o.id] ?? o.status}
                      onChange={(e) =>
                        setStatusDrafts((prev) => ({
                          ...prev,
                          [o.id]: e.target.value,
                        }))
                      }
                    >
                      <option value="Pending">Pending</option>
                      <option value="Confirmed">Confirmed</option>
                      <option value="Cancelled">Cancelled</option>
                    </select>
                  </label>
                  <button
                    type="button"
                    className="btn-secondary"
                    onClick={() => handleUpdateStatus(o.id)}
                    style={{ marginLeft: "0.5rem" }}
                  >
                    Update
                  </button>
                </div>
              </div>

              <div className="order-client">
                <span className="order-client-label">Client</span>
                <span className="order-client-name">
                  {o.client?.fullName ?? "Unknown"}
                </span>
                <span className="order-client-email">{o.client?.email}</span>
              </div>
            </header>

            <ul className="order-items">
              {(o.items || []).map((item) => (
                <li key={item.id} className="order-item">
                  <span className="order-item-name">
                    {item.product?.name ?? `Product #${item.productId}`}
                  </span>
                  <span className="order-item-qty">Qty: {item.quantity}</span>
                  {item.product && (
                    <span className="order-item-price">
                      {item.product.price.toFixed(2)} €
                    </span>
                  )}
                </li>
              ))}
            </ul>
          </article>
        ))}
      </section>
    </div>
  );
}
