import { useEffect, useState } from "react";
import {
  getProducts,
  searchProducts,
  getProductQuantity,
  updateProduct,
  deleteProduct,
} from "../api/productsApi";
import {
  getCategories,
  getBrands,
  getGenders,
  getSizes,
  getColors,
} from "../api/lookupsApi";
import QuickOrder from "./QuickOrder";
import "../styles/products.css";
import productImagesByName, { defaultProductImage } from "../productImages";

export default function ProductList() {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [genders, setGenders] = useState([]);
  const [sizes, setSizes] = useState([]);
  const [colors, setColors] = useState([]);

  const [filters, setFilters] = useState({
    categoryId: "",
    brandId: "",
    genderId: "",
    sizeId: "",
    colorId: "",
    priceMin: "",
    priceMax: "",
    inStock: false,
  });

  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({
    price: "",
    quantity: "",
    discountPercent: "",
  });

  const rawRole = localStorage.getItem("role");
  const role = rawRole ? rawRole.toLowerCase() : "";
  const hasClientId = !!localStorage.getItem("clientId");
  const isProductManager =
    (role === "admin" || role === "advanced" || role === "simple") &&
    !hasClientId;

  const isClient = hasClientId;

  // real-time stock
  async function syncAllProductStocks(productsList) {
    if (!productsList || !productsList.length) {
      setProducts(productsList || []);
      return;
    }

    try {
      const updated = await Promise.all(
        productsList.map(async (p) => {
          const originalQty = p.quantity;
          try {
            const dto = await getProductQuantity(p.id);
            return {
              ...p,
              initialQuantity: originalQty,
              quantity: dto.currentQuantity,
            };
          } catch (err) {
            console.error("Failed to load quantity for product", p.id, err);
            return { ...p, initialQuantity: originalQty };
          }
        })
      );

      setProducts(updated);
    } catch (err) {
      console.error("Failed to sync product stocks", err);
      setProducts(productsList);
    }
  }

  // initial load
  useEffect(() => {
    async function loadInitial() {
      try {
        setLoading(true);
        setError("");

        const [
          productsData,
          categoriesData,
          brandsData,
          gendersData,
          sizesData,
          colorsData,
        ] = await Promise.all([
          getProducts(),
          getCategories(),
          getBrands(),
          getGenders(),
          getSizes(),
          getColors(),
        ]);

        await syncAllProductStocks(productsData);

        setCategories(categoriesData);
        setBrands(brandsData);
        setGenders(gendersData);
        setSizes(sizesData);
        setColors(colorsData);
      } catch (err) {
        console.error(err);
        setError("Failed to load products");
      } finally {
        setLoading(false);
      }
    }

    loadInitial();
  }, []);

  async function handleSearch(e) {
    e.preventDefault();

    try {
      setLoading(true);
      setError("");

      const data = await searchProducts(filters);
      await syncAllProductStocks(data);
    } catch (err) {
      console.error(err);
      setError("Failed to load products");
    } finally {
      setLoading(false);
    }
  }

  function handleReset() {
    setFilters({
      categoryId: "",
      brandId: "",
      genderId: "",
      sizeId: "",
      colorId: "",
      priceMin: "",
      priceMax: "",
      inStock: false,
    });

    (async () => {
      try {
        setLoading(true);
        const data = await getProducts();
        await syncAllProductStocks(data);
      } catch (err) {
        console.error(err);
        setError("Failed to load products");
      } finally {
        setLoading(false);
      }
    })();
  }

  async function refreshProductStock(productId) {
    try {
      const quantityInfo = await getProductQuantity(productId);
      const currentQty = quantityInfo.currentQuantity;

      setProducts((prev) =>
        prev.map((p) =>
          p.id === productId ? { ...p, quantity: currentQty } : p
        )
      );
    } catch (err) {
      console.error("Failed to refresh product quantity", err);
    }
  }

  function startEdit(product) {
    setEditingId(product.id);

    const initialQty =
      product.initialQuantity !== undefined
        ? product.initialQuantity
        : product.quantity;

    setEditForm({
      price: product.price.toString(),
      quantity: initialQty != null ? initialQty.toString() : "",
      discountPercent:
        product.discountPercent != null
          ? product.discountPercent.toString()
          : "",
    });
  }

  function cancelEdit() {
    setEditingId(null);
  }

  async function handleSaveEdit(e, product) {
    e.preventDefault();
    setError("");

    if (!editForm.price || !editForm.quantity) {
      setError("Price and quantity are required.");
      return;
    }

    const payload = {
      id: product.id,
      name: product.name,
      description: product.description,
      price: Number(editForm.price),
      discountPercent: editForm.discountPercent
        ? Number(editForm.discountPercent)
        : 0,
      quantity: Number(editForm.quantity),
      categoryId: product.categoryId,
      brandId: product.brandId,
      genderId: product.genderId,
    };

    try {
      await updateProduct(product.id, payload);

      setProducts((prev) =>
        prev.map((p) =>
          p.id === product.id
            ? {
                ...p,
                price: payload.price,
                discountPercent: payload.discountPercent,
                initialQuantity: payload.quantity,
              }
            : p
        )
      );

      await refreshProductStock(product.id);

      setEditingId(null);
    } catch (err) {
      console.error(err);
      setError("Failed to update product.");
    }
  }

  async function handleDelete(productId) {
    const confirmDelete = window.confirm(
      `Are you sure you want to delete product #${productId}?`
    );
    if (!confirmDelete) return;

    setError("");

    try {
      await deleteProduct(productId);
      setProducts((prev) => prev.filter((p) => p.id !== productId));
    } catch (err) {
      console.error(err);
      setError("Failed to delete product.");
    }
  }

  if (loading && !products.length) return <p>Loading products...</p>;
  if (error) return <p style={{ color: "red" }}>{error}</p>;

  return (
    <div className="products-page">

      <form className="filters-bar" onSubmit={handleSearch}>
        <div className="filters-grid">
          
          <div className="filter-field">
            <label>Category</label>
            <select
              className="filter-input"
              value={filters.categoryId}
              onChange={(e) =>
                setFilters((f) => ({ ...f, categoryId: e.target.value }))
              }
            >
              <option value="">All</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>

 {/* Brand */}
          <div className="filter-field">
            <label>Brand</label>
            <select
              className="filter-input"
              value={filters.brandId}
              onChange={(e) =>
                setFilters((f) => ({ ...f, brandId: e.target.value }))
              }
            >
              <option value="">All brands</option>
              {brands.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </div>


          <div className="filter-field">
            <label>Gender</label>
            <select
              className="filter-input"
              value={filters.genderId}
              onChange={(e) =>
                setFilters((f) => ({ ...f, genderId: e.target.value }))
              }
            >
              <option value="">Any</option>
              {genders.map((g) => (
                <option key={g.id} value={g.id}>
                  {g.name}
                </option>
              ))}
            </select>
          </div>

    {/* Size */}
          <div className="filter-field">
            <label>Size</label>
            <select
              className="filter-input"
              value={filters.sizeId}
              onChange={(e) =>
                setFilters((f) => ({ ...f, sizeId: e.target.value }))
              }
            >
              <option value="">Any size</option>
              {sizes.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name}
                </option>
              ))}
            </select>
          </div>

{/* Color */}
          <div className="filter-field">
            <label>Color</label>
            <select
              className="filter-input"
              value={filters.colorId}
              onChange={(e) =>
                setFilters((f) => ({ ...f, colorId: e.target.value }))
              }
            >
              <option value="">Any color</option>
              {colors.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>

   {/* Price range */}
          <div className="filter-field price-range">
            <label>Price range (€)</label>
            <div style={{ display: "flex", gap: "0.35rem" }}>
              <input
                type="number"
                className="filter-input"
                placeholder="Min"
                value={filters.priceMin}
                onChange={(e) =>
                  setFilters((f) => ({ ...f, priceMin: e.target.value }))
                }
              />
              <span className="price-separator">-</span>
              <input
                type="number"
                className="filter-input"
                placeholder="Max"
                value={filters.priceMax}
                onChange={(e) =>
                  setFilters((f) => ({ ...f, priceMax: e.target.value }))
                }
              />
            </div>
          </div>

{/* In stock */}
          <div className="filter-field filter-checkbox">
            <label className="checkbox-label">
              <input
                type="checkbox"
                checked={filters.inStock}
                onChange={(e) =>
                  setFilters((f) => ({
                    ...f,
                    inStock: e.target.checked,
                  }))
                }
              />
              In stock only
            </label>
          </div>
        </div>

        <div className="filters-actions">
          <button type="submit" className="btn-primary">
            Search
          </button>
          <button type="button" className="btn-secondary" onClick={handleReset}>
            Reset
          </button>
        </div>
      </form>

      {!loading && !products.length && <p>No products found.</p>}

      <div className="products-grid">
        {products.map((p) => {
          const isEditing = editingId === p.id;
          const displayedQty =
            p.quantity !== undefined ? p.quantity : p.initialQuantity;

          const outOfStock = displayedQty === 0;

          const imgSrc = productImagesByName[p.name] || defaultProductImage;

          return (
            <article
              key={p.id}
              className={`product-card ${
                outOfStock ? "out-of-stock-card" : ""
              }`}
            >
              {p.discountPercent ? (
                <span className="discount-badge">-{p.discountPercent}%</span>
              ) : null}

              <div className="product-image-wrapper">
                <img src={imgSrc} alt={p.name} className="product-image" />
              </div>

              <h3 className="product-title">{p.name}</h3>

              {p.description && (
                <p className="product-description">{p.description}</p>
              )}

              <div className="product-price-row">
                <span className="product-price">
                  {p.price.toFixed(2)}
                  <span className="currency">€</span>
                </span>
              </div>

              <div className="product-tags">
                {p.brand && <span className="tag">{p.brand.name}</span>}
                {p.category && <span className="tag">{p.category.name}</span>}

                {Array.isArray(p.sizes) &&
                  p.sizes.map((s) => (
                    <span key={`size-${p.id}-${s.id}`} className="tag">
                      {s.name}
                    </span>
                  ))}

                {Array.isArray(p.colors) &&
                  p.colors.map((c) => (
                    <span key={`color-${p.id}-${c.id}`} className="tag">
                      {c.name}
                    </span>
                  ))}

                {p.gender && <span className="tag">{p.gender.name}</span>}
              </div>

              <p className="product-stock">
                Stock <strong>{displayedQty}</strong>
                {p.initialQuantity !== undefined && (
                  <span className="product-initial-stock">
                    {" "}
                    (initial: {p.initialQuantity})
                  </span>
                )}
              </p>

              {isProductManager && !isEditing && (
                <div className="product-actions">
                  <button
                    type="button"
                    className="btn-secondary"
                    onClick={() => startEdit(p)}
                  >
                    Edit
                  </button>

                  <button
                    type="button"
                    className="btn-danger"
                    onClick={() => handleDelete(p.id)}
                    style={{ marginLeft: "0.5rem" }}
                  >
                    Delete
                  </button>
                </div>
              )}

              {isProductManager && isEditing && (
                <form
                  className="product-edit-form"
                  onSubmit={(e) => handleSaveEdit(e, p)}
                >
                  <div className="form-row">
                    <div className="form-field">
                      <label>Price (€)</label>
                      <input
                        type="number"
                        step="0.01"
                        value={editForm.price}
                        onChange={(e) =>
                          setEditForm((f) => ({
                            ...f,
                            price: e.target.value,
                          }))
                        }
                      />
                    </div>

                    <div className="form-field">
                      <label>Initial quantity</label>
                      <input
                        type="number"
                        value={editForm.quantity}
                        onChange={(e) =>
                          setEditForm((f) => ({
                            ...f,
                            quantity: e.target.value,
                          }))
                        }
                      />
                    </div>

                    <div className="form-field">
                      <label>Discount %</label>
                      <input
                        type="number"
                        min="0"
                        max="100"
                        value={editForm.discountPercent}
                        onChange={(e) =>
                          setEditForm((f) => ({
                            ...f,
                            discountPercent: e.target.value,
                          }))
                        }
                      />
                    </div>
                  </div>

                  <div className="form-actions">
                    <button type="submit" className="btn-primary">
                      Save
                    </button>
                    <button
                      type="button"
                      className="btn-secondary"
                      onClick={cancelEdit}
                      style={{ marginLeft: "0.5rem" }}
                    >
                      Cancel
                    </button>
                  </div>
                </form>
              )}

              {isClient && (
                <QuickOrder
                  product={p}
                  onOrderPlaced={() => refreshProductStock(p.id)}
                />
              )}
            </article>
          );
        })}
      </div>
    </div>
  );
}
