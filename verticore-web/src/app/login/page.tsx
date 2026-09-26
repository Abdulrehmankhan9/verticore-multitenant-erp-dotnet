"use client";

import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, setAuthToken } from "@/lib/api";
import { PasswordInput } from "@/app/components/PasswordInput";
import type { AuthResponse } from "@/types";

export default function LoginPage() {
  const router = useRouter();
  const [form, setForm] = useState({ email: "", password: "" });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const response = await apiRequest<AuthResponse>("/api/Auth/login", {
        method: "POST",
        body: JSON.stringify(form),
      });

      setAuthToken(response.token);
      router.push("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Login failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-card">
        <p className="eyebrow">Welcome</p>
        <h1>VertiCore Login</h1>
        <form onSubmit={handleSubmit} className="stack-form">
          <label>
            Email
            <input
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              placeholder="admin@company.com"
              required
            />
          </label>

          <label>
            Password
            <PasswordInput
              value={form.password}
              onChange={(e) => setForm({ ...form, password: e.target.value })}
              placeholder="••••••••"
              autoComplete="current-password"
              required
            />
          </label>

          <a className="auth-inline-link" href="/forgot-password">Forgot password?</a>

          {error ? <div className="error-box">{error}</div> : null}

          <button type="submit" disabled={loading} className="primary-button">
            {loading ? "Logging in..." : "Login"}
          </button>
        </form>

        <p className="auth-footer">
          New tenant? <a href="/register">Create account</a>
        </p>
      </div>
    </div>
  );
}
