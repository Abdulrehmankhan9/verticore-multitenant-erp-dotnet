"use client";

import { FormEvent, Suspense, useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { apiRequest } from "@/lib/api";
import { PasswordInput } from "@/app/components/PasswordInput";

function SetPasswordForm() {
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
      setError("This invitation link is missing its token.");
      return;
    }
    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    setSaving(true);
    try {
      await apiRequest<string>("/api/User/set-password", {
        method: "POST",
        body: JSON.stringify({ token, password }),
      });
      setSuccess(true);
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : "Unable to set password");
    } finally {
      setSaving(false);
    }
  }

  return (
    <div className="auth-page">
      <section className="auth-card" aria-labelledby="set-password-heading">
        <p className="eyebrow">Team invitation</p>
        <h1 id="set-password-heading">Set your password</h1>
        {success ? (
          <div className="stack-form">
            <p>Your account is ready. Sign in to join your workspace.</p>
            <Link className="primary-button" href="/login">Go to login</Link>
          </div>
        ) : (
          <form className="stack-form" onSubmit={handleSubmit}>
            <label>
              Password
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
              {saving ? "Saving..." : "Set password"}
            </button>
          </form>
        )}
      </section>
    </div>
  );
}

export default function SetPasswordPage() {
  return (
    <Suspense fallback={<div className="auth-page"><p>Loading invitation...</p></div>}>
      <SetPasswordForm />
    </Suspense>
  );
}