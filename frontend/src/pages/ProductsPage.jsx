import { useEffect, useState } from "react";
import { getProducts, getProductQuantity } from "../api/productsApi";
import QuickOrder from "../components/QuickOrder";
import productImagesByName, {
  defaultProductImage,
} from "../productImages";
import "../styles/products.css";


import ProductAdminPanel from "../components/ProductAdminPanel";

export default function ProductsPage() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // real-time stock for each product
  async function syncStocks(list) {
    const updated = await Promise.all(
      list.map(async (p) => {
        try {
          const dto = await getProductQuantity(p.id);
          return {
            ...p,
            initialQuantity: dto.initialQuantity,
            quantity: dto.currentQuantity,
          };
        } catch {
          console.warn("Failed to load real-time quantity for product", p.id);
          return {
            ...p,
            initialQuantity: p.quantity,
            quantity: p.quantity,
          };
        }
      })
    );
    setProducts(updated);
  }

  useEffect(() => {
    async function load() {
      try {
        setLoading(true);
        const data = await getProducts();
        await syncStocks(data);
      } catch (e) {
        console.error(e);
        setError("Failed to load products.");
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []); 


  async function handleProductCreated() {
    try {
      const data = await getProducts();
      await syncStocks(data);
    } catch (e) {
      console.error("Failed to reload products after create:", e);
    }
  }

  async function refreshProduct(id) {
    try {
      const dto = await getProductQuantity(id);
      setProducts((prev) =>
        prev.map((p) =>
          p.id === id
            ? {
                ...p,
                quantity: dto.currentQuantity,
                initialQuantity: dto.initialQuantity,
              }
            : p
        )
      );
    } catch (e) {
      console.warn("Failed to refresh product quantity:", e);
    }
  }

  if (loading) return <p className="loading">Loading...</p>;
  if (error) return <p className="error">{error}</p>;

  return (
    <div className="products-page">
      <h1 className="page-title">Products</h1>

     
      <ProductAdminPanel onProductCreated={handleProductCreated} />

      <div className="products-grid">
        {products.map((p) => {
          const outOfStock = p.quantity === 0;

          const imageSrc = productImagesByName[p.name] || defaultProductImage;

          return (
            <article
              key={p.id}
              className={`product-card ${outOfStock ? "out-of-stock-card" : ""}`}
            >
              {p.discountPercent ? (
                <span className="discount-badge">-{p.discountPercent}%</span>
              ) : null}

              <div className="product-image-wrapper">
                <img src={imageSrc} alt={p.name} className="product-image" />
              </div>

              <h3 className="product-title">{p.name}</h3>
              {p.description && (
                <p className="product-description">{p.description}</p>
              )}

              <div className="product-price-row">
                <span className="product-price">
                  {p.price.toFixed(2)} <span className="currency">€</span>
                </span>
              </div>

              <div className="product-tags">
                {p.brand && <span className="tag">{p.brand.name}</span>}
                {p.category && <span className="tag">{p.category.name}</span>}
                {p.size && <span className="tag">{p.size.name}</span>}
                {p.color && <span className="tag">{p.color.name}</span>}
                {p.gender && <span className="tag">{p.gender.name}</span>}
              </div>

              <p className="product-stock">
                Stock <strong>{p.quantity}</strong>{" "}
                {p.initialQuantity !== undefined && (
                  <span className="product-initial-stock">
                    (initial: {p.initialQuantity})
                  </span>
                )}
              </p>

              <QuickOrder
                product={p}
                onOrderPlaced={() => refreshProduct(p.id)}
              />
            </article>
          );
        })}
      </div>
    </div>
  );
}
