"use client";

import { FormEvent, useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, getAuthToken } from "@/lib/api";
import type { UserRecord } from "@/types";
import styles from "./page.module.css";

export default function UsersPage() {
  const router = useRouter();
  const [users, setUsers] = useState<UserRecord[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [notice, setNotice] = useState("");
  const [inviting, setInviting] = useState(false);
  const [inviteForm, setInviteForm] = useState({ fullName: "", email: "", role: "3" });

  async function loadUsers() {
    const token = getAuthToken();
    if (!token) {
      router.push("/login");
      return;
    }

    try {
      const result = await apiRequest<UserRecord[]>("/api/User");
      setUsers(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load users");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    if (!getAuthToken()) {
      router.push("/login");
      return;
    }

    apiRequest<UserRecord[]>("/api/User")
      .then((result) => setUsers(result))
      .catch((err) => setError(err instanceof Error ? err.message : "Unable to load users"))
      .finally(() => setLoading(false));
  }, [router]);

  async function handleInvite(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setInviting(true);
    setError("");
    setNotice("");

    try {
      await apiRequest<string>("/api/User/invite", {
        method: "POST",
        body: JSON.stringify({
          fullName: inviteForm.fullName.trim(),
          email: inviteForm.email.trim(),
          role: Number(inviteForm.role),
        }),
      });
      setInviteForm({ fullName: "", email: "", role: "3" });
      setNotice("Invitation sent. The user can set a password from the email link.");
      await loadUsers();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to send invitation");
    } finally {
      setInviting(false);
    }
  }

  const roleNames: Record<number, string> = {
    0: "Super Admin",
    1: "Tenant Admin",
    2: "Manager",
    3: "Staff",
  };

  if (loading) return <div className="page-section"><p>Loading users...</p></div>;

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">People</p>
          <h1>Users</h1>
        </div>
      </div>

      {error ? <div className="error-box">{error}</div> : null}
      {notice ? <div className={styles.notice} role="status">{notice}</div> : null}

      <form className={`card-form ${styles.inviteForm}`} onSubmit={handleInvite}>
        <div className={styles.formHeading}>
          <div><h3>Invite a teammate</h3><p>They will receive a link to set their password.</p></div>
        </div>
        <div className={styles.formGrid}>
          <label>
            Full name
            <input
              value={inviteForm.fullName}
              onChange={(event) => setInviteForm({ ...inviteForm, fullName: event.target.value })}
              placeholder="Alex Morgan"
              required
            />
          </label>
          <label>
            Email
            <input
              type="email"
              value={inviteForm.email}
              onChange={(event) => setInviteForm({ ...inviteForm, email: event.target.value })}
              placeholder="alex@company.com"
              required
            />
          </label>
          <label>
            Role
            <select
              value={inviteForm.role}
              onChange={(event) => setInviteForm({ ...inviteForm, role: event.target.value })}
            >
              <option value="2">Manager</option>
              <option value="3">Staff</option>
            </select>
          </label>
        </div>
        <button type="submit" className="primary-button" disabled={inviting}>
          {inviting ? "Sending..." : "Send invitation"}
        </button>
      </form>

      <div className="panel">
        <table className="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Role</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {users.map((user) => (
              <tr key={user.id}>
                <td>{user.fullName}</td>
                <td>{user.email}</td>
                <td>{roleNames[user.role] ?? "Unknown"}</td>
                <td>{user.isActive ? "Active" : "Inactive"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
