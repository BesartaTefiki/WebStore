import { useEffect, useState } from "react";
import {
  getUsers,
  createUser,
  updateUserRole,
  deleteUser,
} from "../api/usersApi";
import "../styles/users.css";

export default function UsersPage() {
  const rawRole = localStorage.getItem("role");
  const role = rawRole ? rawRole.toLowerCase() : "";
  const hasClientId = !!localStorage.getItem("clientId");

  const isAdmin = role === "admin" && !hasClientId;

  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [savingId, setSavingId] = useState(null);

  const [newUser, setNewUser] = useState({
    username: "",
    password: "",
    role: "simple",
    type: "staff", 
  });
  const [creating, setCreating] = useState(false);

  useEffect(() => {
    if (!isAdmin) return;

    async function load() {
      try {
        setLoading(true);
        setError("");
        const data = await getUsers();
        setUsers(data);
      } catch (e) {
        console.error(e);
        setError("Failed to load users.");
      } finally {
        setLoading(false);
      }
    }

    load();
  }, [isAdmin]);

  function isClientUser(u) {
    return u.clientId !== null && u.clientId !== undefined;
  }

  const staffUsers = users.filter((u) => !isClientUser(u));
  const clientUsers = users.filter(isClientUser);

  function handleRoleChange(userId, newRole) {
    setUsers((prev) =>
      prev.map((u) =>
        u.id === userId ? { ...u, roleEdit: newRole } : u
      )
    );
  }

  async function handleSaveUser(user) {
    const newRole = (user.roleEdit || user.role || "").toLowerCase().trim();
    if (!newRole || newRole === user.role) return;

    try {
      setSavingId(user.id);
      setError("");

      const updated = await updateUserRole(user.id, newRole);

      setUsers((prev) =>
        prev.map((u) =>
          u.id === user.id
            ? {
                ...u,
                ...updated,
                roleEdit: undefined,
              }
            : u
        )
      );
    } catch (e) {
      console.error(e);
      setError("Failed to update role.");
    } finally {
      setSavingId(null);
    }
  }

  async function handleDelete(userId) {
    const ok = window.confirm(
      `Are you sure you want to delete user #${userId}?`
    );
    if (!ok) return;

    try {
      setSavingId(userId);
      setError("");
      await deleteUser(userId);
      setUsers((prev) => prev.filter((u) => u.id !== userId));
    } catch (e) {
      console.error(e);
      setError("Failed to delete user.");
    } finally {
      setSavingId(null);
    }
  }

  function handleNewChange(field, value) {
    setNewUser((prev) => ({
      ...prev,
      [field]: value,
    }));
  }

  async function handleCreateUser(e) {
    e.preventDefault();
    setError("");

    if (!newUser.username || !newUser.password) {
      setError("Username and password are required for new user.");
      return;
    }

    const isClient = newUser.type === "client";
    const roleToSend = isClient ? "simple" : newUser.role;

    try {
      setCreating(true);
      const created = await createUser({
        username: newUser.username,
        password: newUser.password,
        role: roleToSend,
        isClient,
      });

      setUsers((prev) => [...prev, created]);

      setNewUser({
        username: "",
        password: "",
        role: "simple",
        type: "staff",
      });
    } catch (e) {
      console.error(e);
      setError("Failed to create user.");
    } finally {
      setCreating(false);
    }
  }

  if (!isAdmin) {
    return (
      <div className="users-page">
        <h1 className="page-title">User management</h1>
        <p className="error">
          You are not allowed to manage users. Only admin staff accounts can
          access this page.
        </p>
      </div>
    );
  }

  if (loading) {
    return (
      <div className="users-page">
        <h1 className="page-title">User management</h1>
        <p className="loading">Loading users...</p>
      </div>
    );
  }

  function renderTable(title, list) {
    if (!list.length) {
      return (
        <section className="users-section">
          <h2 className="section-title">{title}</h2>
          <p>No users in this group.</p>
        </section>
      );
    }

    return (
      <section className="users-section">
        <h2 className="section-title">{title}</h2>
        <table className="users-table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Username</th>
              <th>Client ID</th>
              <th>Type</th>
              <th>Role</th>
              <th style={{ width: "220px" }}>Actions</th>
            </tr>
          </thead>
          <tbody>
            {list.map((u) => {
              const isClient = isClientUser(u);
              const effectiveRole = (u.roleEdit || u.role || "").toLowerCase();

              return (
                <tr key={u.id}>
                  <td>{u.id}</td>
                  <td>{u.username}</td>
                  <td>{u.clientId ?? "-"}</td>
                  <td>
                    {isClient ? (
                      <span className="user-type-badge user-type-client">
                        client
                      </span>
                    ) : (
                      <span className="user-type-badge user-type-staff">
                        staff
                      </span>
                    )}
                  </td>
                  <td>
                    <select
                      value={effectiveRole}
                      onChange={(e) =>
                        handleRoleChange(u.id, e.target.value)
                      }
                    >
                      <option value="admin">admin</option>
                      <option value="advanced">advanced</option>
                      <option value="simple">simple</option>
                    </select>
                  </td>
                  <td>
                    <button
                      type="button"
                      className="btn-primary"
                      disabled={savingId === u.id}
                      onClick={() => handleSaveUser(u)}
                    >
                      {savingId === u.id ? "Saving..." : "Save role"}
                    </button>

                    <button
                      type="button"
                      className="btn-danger"
                      style={{ marginLeft: "0.5rem" }}
                      disabled={savingId === u.id}
                      onClick={() => handleDelete(u.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </section>
    );
  }

  return (
    <div className="users-page">
      <h1 className="page-title">User management</h1>
      <p className="section-subtitle">
        Admin users can create accounts, change roles, and delete users.
      </p>

      {error && <p className="error">{error}</p>}

      <section className="users-section users-create">
        <h2 className="section-title">Create new user</h2>
        <form className="users-create-form" onSubmit={handleCreateUser}>
          <div className="users-create-grid">
            <div className="form-field">
              <label>Username</label>
              <input
                type="text"
                value={newUser.username}
                onChange={(e) =>
                  handleNewChange("username", e.target.value)
                }
              />
            </div>

            <div className="form-field">
              <label>Password</label>
              <input
                type="password"
                value={newUser.password}
                onChange={(e) =>
                  handleNewChange("password", e.target.value)
                }
              />
            </div>

            <div className="form-field">
              <label>Type</label>
              <select
                value={newUser.type}
                onChange={(e) =>
                  handleNewChange("type", e.target.value)
                }
              >
                <option value="staff">staff</option>
                <option value="client">client</option>
              </select>
            </div>

            <div className="form-field">
              <label>Role</label>
              <select
                value={newUser.type === "client" ? "simple" : newUser.role}
                onChange={(e) =>
                  handleNewChange("role", e.target.value)
                }
                disabled={newUser.type === "client"} 
              >
                <option value="admin">admin</option>
                <option value="advanced">advanced</option>
                <option value="simple">simple</option>
              </select>
              {newUser.type === "client" && (
                <small className="hint">
                  Client accounts are always created with role <b>simple</b>.
                </small>
              )}
            </div>
          </div>

          <button
            type="submit"
            className="btn-primary"
            disabled={creating}
          >
            {creating ? "Creating..." : "Create user"}
          </button>
        </form>
      </section>

      {renderTable("Staff users", staffUsers)}
      {renderTable("Client users", clientUsers)}
    </div>
  );
}
