import { useEffect, useState } from "react";
import {
  getCategories,
  getBrands,
  getGenders,
  getSizes,
  getColors,
} from "../api/lookupsApi";
import { createProduct } from "../api/productsApi";
import "../styles/products.css";

export default function ProductAdminPanel({ onProductCreated }) {
  const role = localStorage.getItem("role");
  const isProductManager =
    role === "admin" || role === "advanced" || role === "simple";

  const [lookupsLoaded, setLookupsLoaded] = useState(false);
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [genders, setGenders] = useState([]);
  const [sizes, setSizes] = useState([]);
  const [colors, setColors] = useState([]);

  const [form, setForm] = useState({
    name: "",
    description: "",
    price: "",
    quantity: "",
    categoryId: "",
    brandId: "",
    genderId: "",
    sizeId: "",
    colorId: "",
    discountPercent: "",
  });

  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    async function loadLookups() {
      try {
        const [
          categoriesData,
          brandsData,
          gendersData,
          sizesData,
          colorsData,
        ] = await Promise.all([
          getCategories(),
          getBrands(),
          getGenders(),
          getSizes(),
          getColors(),
        ]);

        setCategories(categoriesData);
        setBrands(brandsData);
        setGenders(gendersData);
        setSizes(sizesData);
        setColors(colorsData);
        setLookupsLoaded(true);
      } catch (err) {
        console.error(err);
        setError("Failed to load lookup data.");
      }
    }

    loadLookups();
  }, []);

  if (!isProductManager) return null;

  function handleChange(field, value) {
    setForm((f) => ({
      ...f,
      [field]: value,
    }));
  }

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");

    if (!form.name || !form.price || !form.quantity) {
      setError("Name, price and quantity are required.");
      return;
    }

    if (
      !form.categoryId ||
      !form.brandId ||
      !form.genderId ||
      !form.sizeId ||
      !form.colorId
    ) {
      setError("Please choose category, brand, gender, size and color.");
      return;
    }

    const productPayload = {
      name: form.name,
      description: form.description,
      price: Number(form.price),
      discountPercent: form.discountPercent
        ? Number(form.discountPercent)
        : 0,
      quantity: Number(form.quantity),
      categoryId: Number(form.categoryId),
      brandId: Number(form.brandId),
      genderId: Number(form.genderId),
      sizeId: Number(form.sizeId),
      colorId: Number(form.colorId),
    };

    try {
      setSaving(true);
      const created = await createProduct(productPayload);

      setForm({
        name: "",
        description: "",
        price: "",
        quantity: "",
        categoryId: "",
        brandId: "",
        genderId: "",
        sizeId: "",
        colorId: "",
        discountPercent: "",
      });

      if (onProductCreated) onProductCreated(created);
    } catch (err) {
      console.error(err);
      setError("Failed to create product.");
    } finally {
      setSaving(false);
    }
  }

  return (
    <section className="products-admin">
      <h2 className="section-title">Product management</h2>
      <p className="section-subtitle">
        Create new products (available for admin, advanced and simple users).
      </p>

      {!lookupsLoaded && <p>Loading dropdowns...</p>}
      {error && <p className="form-error">{error}</p>}

      <form className="product-form" onSubmit={handleSubmit}>
        <div className="form-row">
          <div className="form-field">
            <label>Name</label>
            <input
              type="text"
              value={form.name}
              onChange={(e) => handleChange("name", e.target.value)}
              required
            />
          </div>

          <div className="form-field">
            <label>Price (€)</label>
            <input
              type="number"
              step="0.01"
              value={form.price}
              onChange={(e) => handleChange("price", e.target.value)}
              required
            />
          </div>

          <div className="form-field">
            <label>Quantity (stock)</label>
            <input
              type="number"
              value={form.quantity}
              onChange={(e) => handleChange("quantity", e.target.value)}
              required
            />
          </div>

          <div className="form-field">
            <label>Discount %</label>
            <input
              type="number"
              min="0"
              max="100"
              value={form.discountPercent}
              onChange={(e) =>
                handleChange("discountPercent", e.target.value)
              }
            />
          </div>
        </div>

        <div className="form-row">
          <div className="form-field">
            <label>Category</label>
            <select
              value={form.categoryId}
              onChange={(e) => handleChange("categoryId", e.target.value)}
            >
              <option value="">Select category</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label>Brand</label>
            <select
              value={form.brandId}
              onChange={(e) => handleChange("brandId", e.target.value)}
            >
              <option value="">Select brand</option>
              {brands.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label>Gender</label>
            <select
              value={form.genderId}
              onChange={(e) => handleChange("genderId", e.target.value)}
            >
              <option value="">Select gender</option>
              {genders.map((g) => (
                <option key={g.id} value={g.id}>
                  {g.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label>Size</label>
            <select
              value={form.sizeId}
              onChange={(e) => handleChange("sizeId", e.target.value)}
            >
              <option value="">Select size</option>
              {sizes.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          </div>

          <div className="form-field">
            <label>Color</label>
            <select
              value={form.colorId}
              onChange={(e) => handleChange("colorId", e.target.value)}
            >
              <option value="">Select color</option>
              {colors.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="form-row">
          <div className="form-field full-width">
            <label>Description</label>
            <textarea
              rows="2"
              value={form.description}
              onChange={(e) =>
                handleChange("description", e.target.value)
              }
            />
          </div>
        </div>

        <div className="form-actions">
          <button
            type="submit"
            className="btn-primary"
            disabled={saving || !lookupsLoaded}
          >
            {saving ? "Saving..." : "Create product"}
          </button>
        </div>
      </form>
    </section>
  );
}
