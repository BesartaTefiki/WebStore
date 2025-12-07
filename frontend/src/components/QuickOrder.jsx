import { useState } from "react";
import { createOrder } from "../api/ordersApi";
import "../styles/quickOrder.css";

export default function QuickOrder({ product }) {
  const [quantity, setQuantity] = useState(1);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  if (!product) return null;

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setSuccess("");

    const token = localStorage.getItem("token");
    const clientId = localStorage.getItem("clientId");


    if (!token || !clientId) {
      setError("You must be logged in as a client to place an order.");
      return;
    }

    if (quantity <= 0) {
      setError("Quantity must be at least 1.");
      return;
    }

    if (product.quantity <= 0) {
      setError("This product is out of stock.");
      return;
    }

    if (quantity > product.quantity) {
      setError(`Only ${product.quantity} item(s) left in stock.`);
      return;
    }

    try {
      setLoading(true);


      await createOrder([
        {
          productId: product.id,
          quantity: Number(quantity),
        },
      ]);

      setSuccess("Order placed successfully.");
      setQuantity(1);
    } catch (err) {
      console.error(err);

      let message = "Failed to place order.";
      if (err.response?.data?.message) {
        message = err.response.data.message;
      }

      setError(message);
    } finally {
      setLoading(false);
    }
  }

  return (
    <section className="quick-order">
      <h3 className="quick-order-title">Quick order</h3>
      <p className="quick-order-text">
        Place an order for <strong>{product.name}</strong> as the logged-in user.
      </p>

      <form className="quick-order-form" onSubmit={handleSubmit}>
        <div className="quick-order-row">
          <div className="quick-order-field quick-order-qty">
            <label>Quantity</label>
            <input
              type="number"
              min="1"
              value={quantity}
              onChange={(e) => setQuantity(Number(e.target.value))}
            />
          </div>
        </div>

        {error && <p className="quick-order-error">{error}</p>}
        {success && <p className="quick-order-success">{success}</p>}

        <button type="submit" className="btn-primary" disabled={loading}>
          {loading ? "Placing..." : "Place order"}
        </button>
      </form>
    </section>
  );
}
