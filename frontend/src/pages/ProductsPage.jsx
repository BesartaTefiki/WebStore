import { useEffect, useState } from "react";
import {
  getProducts,
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
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [genders, setGenders] = useState([]);
  const [sizes, setSizes] = useState([]);
  const [colors, setColors] = useState([]);

  const [editingId, setEditingId] = useState(null);
  const [editForm, setEditForm] = useState({
    name: "",
    description: "",
    price: "",
    quantity: "",
    discountPercent: "",
    categoryId: "",
    brandId: "",
    genderId: "",
    sizeIds: [], 
    colorIds: [], 
  });

  const rawRole = localStorage.getItem("role");
  const role = rawRole ? rawRole.toLowerCase() : "";
  const hasClientId = !!localStorage.getItem("clientId");

  const isProductManager =
    (role === "admin" || role === "advanced" || role === "simple") &&
    !hasClientId;


  const isClient = hasClientId;

  function handleEditChange(field, value) {
    setEditForm((f) => ({
      ...f,
      [field]: value,
    }));
  }

  function toggleIdInList(list, id) {
    if (list.includes(id)) return list.filter((x) => x !== id);
    return [...list, id];
  }

  function handleEditSizeToggle(e) {
    const value = e.target.value;
    setEditForm((f) => ({
      ...f,
      sizeIds: toggleIdInList(f.sizeIds, value),
    }));
  }

  function handleEditColorToggle(e) {
    const value = e.target.value;
    setEditForm((f) => ({
      ...f,
      colorIds: toggleIdInList(f.colorIds, value),
    }));
  }
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

        await syncStocks(productsData);

        setCategories(categoriesData);
        setBrands(brandsData);
        setGenders(gendersData);
        setSizes(sizesData);
        setColors(colorsData);
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

  function startEdit(product) {
    setEditingId(product.id);

    const initialQty =
      product.initialQuantity !== undefined
        ? product.initialQuantity
        : product.quantity;

    setEditForm({
      name: product.name,
      description: product.description || "",
      price: product.price.toString(),
      quantity: initialQty != null ? initialQty.toString() : "",
      discountPercent:
        product.discountPercent != null
          ? product.discountPercent.toString()
          : "",
      categoryId: product.categoryId ? String(product.categoryId) : "",
      brandId: product.brandId ? String(product.brandId) : "",
      genderId: product.genderId ? String(product.genderId) : "",
      sizeIds: Array.isArray(product.sizes)
        ? product.sizes.map((s) => String(s.id))
        : [],
      colorIds: Array.isArray(product.colors)
        ? product.colors.map((c) => String(c.id))
        : [],
    });
  }

  function cancelEdit() {
    setEditingId(null);
  }

  async function handleSaveEdit(e, product) {
    e.preventDefault();
    setError("");

    if (!editForm.name || !editForm.price || !editForm.quantity) {
      setError("Name, price and quantity are required.");
      return;
    }

    if (
      !editForm.categoryId ||
      !editForm.brandId ||
      !editForm.genderId ||
      editForm.sizeIds.length === 0 ||
      editForm.colorIds.length === 0
    ) {
      setError(
        "Please choose category, brand, gender, at least one size and at least one color."
      );
      return;
    }

    const selectedSizes = sizes.filter((s) =>
      editForm.sizeIds.includes(String(s.id))
    );
    const selectedColors = colors.filter((c) =>
      editForm.colorIds.includes(String(c.id))
    );

    const payload = {
      id: product.id,
      name: editForm.name,
      description: editForm.description,
      price: Number(editForm.price),
      discountPercent: editForm.discountPercent
        ? Number(editForm.discountPercent)
        : 0,
      quantity: Number(editForm.quantity),
      categoryId: Number(editForm.categoryId),
      brandId: Number(editForm.brandId),
      genderId: Number(editForm.genderId),
      sizes: selectedSizes.map((s) => ({
        id: s.id,
        name: s.name,
      })),
      colors: selectedColors.map((c) => ({
        id: c.id,
        name: c.name,
      })),
    };

    try {
      await updateProduct(product.id, payload);
      setProducts((prev) =>
        prev.map((p) =>
          p.id === product.id
            ? {
                ...p,
                ...payload,
                initialQuantity: payload.quantity,
              }
            : p
        )
      );

      await refreshProduct(product.id);
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
          const isEditing = editingId === p.id;

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
              {p.description && !isEditing && (
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
                Stock <strong>{p.quantity}</strong>{" "}
                {p.initialQuantity !== undefined && (
                  <span className="product-initial-stock">
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
                    <div className="form-field full-width">
                      <label>Name</label>
                      <input
                        type="text"
                        value={editForm.name}
                        onChange={(e) =>
                          handleEditChange("name", e.target.value)
                        }
                      />
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-field full-width">
                      <label>Description</label>
                      <textarea
                        rows="2"
                        value={editForm.description}
                        onChange={(e) =>
                          handleEditChange("description", e.target.value)
                        }
                      />
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-field">
                      <label>Price (€)</label>
                      <input
                        type="number"
                        step="0.01"
                        value={editForm.price}
                        onChange={(e) =>
                          handleEditChange("price", e.target.value)
                        }
                      />
                    </div>

                    <div className="form-field">
                      <label>Initial quantity</label>
                      <input
                        type="number"
                        value={editForm.quantity}
                        onChange={(e) =>
                          handleEditChange("quantity", e.target.value)
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
                          handleEditChange("discountPercent", e.target.value)
                        }
                      />
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-field">
                      <label>Category</label>
                      <select
                        value={editForm.categoryId}
                        onChange={(e) =>
                          handleEditChange("categoryId", e.target.value)
                        }
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
                        value={editForm.brandId}
                        onChange={(e) =>
                          handleEditChange("brandId", e.target.value)
                        }
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
                        value={editForm.genderId}
                        onChange={(e) =>
                          handleEditChange("genderId", e.target.value)
                        }
                      >
                        <option value="">Select gender</option>
                        {genders.map((g) => (
                          <option key={g.id} value={g.id}>
                            {g.name}
                          </option>
                        ))}
                      </select>
                    </div>
                  </div>

                  <div className="form-row">
                    <div className="form-field">
                      <label>Sizes</label>
                      <div className="multi-checkbox-group">
                        {sizes.map((s) => (
                          <label key={s.id} className="checkbox-inline">
                            <input
                              type="checkbox"
                              value={s.id}
                              checked={editForm.sizeIds.includes(
                                String(s.id)
                              )}
                              onChange={handleEditSizeToggle}
                            />
                            {s.name}
                          </label>
                        ))}
                      </div>
                    </div>

                    <div className="form-field">
                      <label>Colors</label>
                      <div className="multi-checkbox-group">
                        {colors.map((c) => (
                          <label key={c.id} className="checkbox-inline">
                            <input
                              type="checkbox"
                              value={c.id}
                              checked={editForm.colorIds.includes(
                                String(c.id)
                              )}
                              onChange={handleEditColorToggle}
                            />
                            {c.name}
                          </label>
                        ))}
                      </div>
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
              {isClient && !isEditing && (
                <QuickOrder
                  product={p}
                  onOrderPlaced={() => refreshProduct(p.id)}
                />
              )}
            </article>
          );
        })}
      </div>
    </div>
  );
}
