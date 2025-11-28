import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../api/authApi";
import "../styles/register.css";

export default function RegisterPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setSuccess("");
    setLoading(true);

    try {
      await register(username, password);
      setSuccess(
        "User registered successfully as a SIMPLE user. You can now login."
      );
      setTimeout(() => navigate("/login"), 1200);
    } catch (err) {
      console.error(err);
      setError("Registration failed.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="register-container">
      <h2>Create account</h2>
      <p className="register-note">
        New accounts are always created with the <strong>simple</strong> role.
      </p>

      <form onSubmit={handleSubmit} className="register-form">
        <div className="register-field">
          <label>Username</label>
          <input
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>

        <div className="register-field">
          <label>Password</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>

        {error && <p className="register-error">{error}</p>}
        {success && <p className="register-success">{success}</p>}

        <button type="submit" disabled={loading}>
          {loading ? "Creating..." : "Register"}
        </button>
      </form>
    </div>
  );
}
