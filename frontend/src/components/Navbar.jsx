import { NavLink, useNavigate } from "react-router-dom";
import "../styles/navbar.css";

export default function Navbar() {
  const navigate = useNavigate();

  const username = localStorage.getItem("username");
  const rawRole = localStorage.getItem("role");
  const role = rawRole ? rawRole.toLowerCase() : "";
  const hasClientId = !!localStorage.getItem("clientId");

  function handleLogout() {
    localStorage.removeItem("token");
    localStorage.removeItem("username");
    localStorage.removeItem("role");
    localStorage.removeItem("clientId");
    navigate("/login");
  }

  // permissions
  const canViewOrders =
    (role === "admin" || role === "advanced") && !hasClientId;
  const canViewReports =
    (role === "admin" || role === "advanced") && !hasClientId;
  const canManageUsers = role === "admin" && !hasClientId;

  return (
    <header className="navbar">
      <div className="navbar-inner">
        <div className="navbar-left">
          <NavLink to="/" className="navbar-logo">
            Besartas WebStore
          </NavLink>

          <nav className="navbar-links">
            <NavLink
              to="/"
              end
              className={({ isActive }) =>
                isActive ? "nav-link nav-link-active" : "nav-link"
              }
            >
              Home
            </NavLink>

            <NavLink
              to="/products"
              className={({ isActive }) =>
                isActive ? "nav-link nav-link-active" : "nav-link"
              }
            >
              Products
            </NavLink>

 {/* Orders: admin + advanced */}
            {canViewOrders && (
              <NavLink
                to="/orders"
                className={({ isActive }) =>
                  isActive ? "nav-link nav-link-active" : "nav-link"
                }
              >
                Orders
              </NavLink>
            )}

 {/* Reports: admin + advanced */}
            {canViewReports && (
              <NavLink
                to="/reports"
                className={({ isActive }) =>
                  isActive ? "nav-link nav-link-active" : "nav-link"
                }
              >
                Reports
              </NavLink>
            )}

            {canManageUsers && (
              <NavLink
                to="/users"
                className={({ isActive }) =>
                  isActive ? "nav-link nav-link-active" : "nav-link"
                }
              >
                Users
              </NavLink>
            )}
          </nav>
        </div>

        <div className="navbar-right">
          {username && (
            <span className="navbar-user">
              {username} <span className="navbar-role">({rawRole})</span>
            </span>
          )}

          {!username ? (
            <>
              <NavLink to="/login" className="nav-btn nav-btn-ghost">
                Login
              </NavLink>
              <NavLink to="/register" className="nav-btn nav-btn-primary">
                Register
              </NavLink>
            </>
          ) : (
            <button onClick={handleLogout} className="nav-btn nav-btn-ghost">
              Logout
            </button>
          )}
        </div>
      </div>
    </header>
  );
}
