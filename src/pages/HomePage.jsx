import "../styles/layout.css";
import "../styles/products.css";
import ProductList from "../components/ProductList";

export default function HomePage() {
  return (
    <div className="home-page">
      <section className="hero">
        <h1>WebStore</h1>
        <p className="hero-subtitle">
  Welcome to Besarta’s clothing web store.
</p>
      </section>

      <section className="home-section">
        <h2 className="section-title">Products</h2>
        <ProductList />
      </section>
    </div>
  );
}
