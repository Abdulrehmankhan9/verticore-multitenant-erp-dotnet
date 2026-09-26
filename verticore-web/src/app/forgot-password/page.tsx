"use client";

import { FormEvent, useState } from "react";
import Link from "next/link";
import { apiRequest } from "@/lib/api";

export default function ForgotPasswordPage() {
  const [email, setEmail] = useState("");
  const [message, setMessage] = useState("");
  const [error, setError] = useState("");
  const [sending, setSending] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSending(true);
    setMessage("");
    setError("");

    try {
      const response = await apiRequest<string>("/api/Auth/forgot-password", {
        method: "POST",
        body: JSON.stringify({ email }),
      });
      setMessage(response);
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : "Unable to request a reset link");
    } finally {
      setSending(false);
    }
  }

  return (
    <div className="auth-page">
      <section className="auth-card" aria-labelledby="forgot-password-heading">
        <p className="eyebrow">Account recovery</p>
        <h1 id="forgot-password-heading">Forgot password?</h1>
        <p className="auth-description">Enter your account email and we will send a password reset link if it matches an account.</p>
        {message ? (
          <div className="auth-success" role="status">{message}</div>
        ) : (
          <form className="stack-form" onSubmit={handleSubmit}>
            <label>
              Email
              <input
                type="email"
                autoComplete="email"
                value={email}
                onChange={(event) => setEmail(event.target.value)}
                placeholder="you@company.com"
                required
              />
            </label>
            {error ? <div className="error-box" role="alert">{error}</div> : null}
            <button className="primary-button" type="submit" disabled={sending}>
              {sending ? "Sending..." : "Send reset link"}
            </button>
          </form>
        )}
        <p className="auth-footer"><Link href="/login">Back to login</Link></p>
      </section>
    </div>
  );
}