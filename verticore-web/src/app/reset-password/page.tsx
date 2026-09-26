"use client";

import { FormEvent, Suspense, useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { apiRequest } from "@/lib/api";
import { PasswordInput } from "@/app/components/PasswordInput";

function ResetPasswordForm() {
  const searchParams = useSearchParams();
  const token = searchParams.get("token") ?? "";
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [success, setSuccess] = useState(false);
  const [saving, setSaving] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError("");

    if (!token) {
      setError("This reset link is missing its token. Request another link.");
      return;
    }
    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setSaving(true);
    try {
      await apiRequest<string>("/api/Auth/reset-password", {
        method: "POST",
        body: JSON.stringify({ token, password }),
      });
      setSuccess(true);
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : "Unable to reset password");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="auth-page">
      <section className="auth-card" aria-labelledby="reset-password-heading">
        <p className="eyebrow">Account recovery</p>
        <h1 id="reset-password-heading">Reset your password</h1>
        {success ? (
          <div className="stack-form">
            <div className="auth-success" role="status">Your password has been updated.</div>
            <Link className="primary-button auth-button-link" href="/login">Go to login</Link>
          </div>
        ) : (
          <form className="stack-form" onSubmit={handleSubmit}>
            <label>
              New password
              <PasswordInput
                autoComplete="new-password"
                minLength={8}
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="At least 8 characters"
                required
              />
            </label>
            <label>
              Confirm password
              <PasswordInput
                autoComplete="new-password"
                minLength={8}
                value={confirmPassword}
                onChange={(event) => setConfirmPassword(event.target.value)}
                placeholder="Enter it again"
                required
              />
            </label>
            {error ? <div className="error-box" role="alert">{error}</div> : null}
            <button className="primary-button" type="submit" disabled={saving}>
              {saving ? "Saving..." : "Reset password"}
            </button>
          </form>
        )}
      </section>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense fallback={<div className="auth-page"><p>Loading reset link...</p></div>}>
      <ResetPasswordForm />
    </Suspense>
  );
}