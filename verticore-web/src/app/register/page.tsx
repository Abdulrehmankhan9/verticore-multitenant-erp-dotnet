"use client";

import { FormEvent, useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, setAuthToken } from "@/lib/api";
import { PasswordInput } from "@/app/components/PasswordInput";
import type { AuthResponse } from "@/types";

export default function RegisterPage() {
  const router = useRouter();
  const [form, setForm] = useState({
    tenantName: "",
    fullName: "",
    email: "",
    password: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setLoading(true);
    setError("");

    try {
      const response = await apiRequest<AuthResponse>("/api/Auth/register", {
        method: "POST",
        body: JSON.stringify(form),
      });

      setAuthToken(response.token);
      router.push("/dashboard");
    } catch (err) {
      setError(err instanceof Error ? err.message : "Registration failed");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="auth-page">
      <div className="auth-card wide-card">
        <p className="eyebrow">Create tenant</p>
        <h1>Start your VertiCore workspace</h1>
        <form onSubmit={handleSubmit} className="stack-form">
          <label>
            Tenant Name
            <input
              value={form.tenantName}
              onChange={(e) => setForm({ ...form, tenantName: e.target.value })}
              placeholder="Northwind Business"
              required
            />
          </label>

          <label>
            Admin Full Name
            <input
              value={form.fullName}
              onChange={(e) => setForm({ ...form, fullName: e.target.value })}
              placeholder="Jane Smith"
              required
            />
          </label>

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
              placeholder="Minimum 8 characters"
              autoComplete="new-password"
              required
            />
          </label>

          {error ? <div className="error-box">{error}</div> : null}

          <button type="submit" disabled={loading} className="primary-button">
            {loading ? "Creating..." : "Create account"}
          </button>
        </form>

        <p className="auth-footer">
          Already have an account? <a href="/login">Login</a>
        </p>
      </div>
    </div>
  );
}
