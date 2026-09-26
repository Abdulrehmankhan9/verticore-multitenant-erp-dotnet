"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, getAuthToken } from "@/lib/api";
import type { Client } from "@/types";
import styles from "./page.module.css";

export default function ClientsPage() {
  const router = useRouter();
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ fullName: "", email: "", phone: "", address: "" });

  const loadClients = async () => {
    const token = getAuthToken();
    if (!token) {
      router.push("/login");
      return;
    }

    try {
      const result = await apiRequest<Client[]>("/api/Client");
      setClients(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unable to load clients");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!getAuthToken()) {
      router.push("/login");
      return;
    }

    apiRequest<Client[]>("/api/Client")
      .then((result) => setClients(result))
      .catch((err) => setError(err instanceof Error ? err.message : "Unable to load clients"))
      .finally(() => setLoading(false));
  }, [router]);

  async function handleCreate(e: React.FormEvent) {
    e.preventDefault();
    try {
      await apiRequest<Client>("/api/Client", {
        method: "POST",
        body: JSON.stringify(form),
      });
      setForm({ fullName: "", email: "", phone: "", address: "" });
      await loadClients();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Create failed");
    }
  }

  if (loading) return <div className="page-section"><p>Loading clients...</p></div>;

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">Directory</p>
          <h1>Clients</h1>
        </div>
      </div>

      <form onSubmit={handleCreate} className={`card-form ${styles.clientForm}`}>
        <h3>Add client</h3>
        <div className={`form-grid ${styles.clientFields}`}>
          <input value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} placeholder="Full name" required />
          <input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} placeholder="Email" required />
          <input value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} placeholder="Phone" required />
          <input value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} placeholder="Address" required />
        </div>
        <div className={styles.clientActions}>
          <button type="submit" className="primary-button">Create client</button>
        </div>
      </form>

      {error ? <div className="error-box">{error}</div> : null}

      <div className="panel">
        <table className="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Address</th>
            </tr>
          </thead>
          <tbody>
            {clients.map((client) => (
              <tr key={client.id}>
                <td>{client.fullName}</td>
                <td>{client.email}</td>
                <td>{client.phone}</td>
                <td>{client.address}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
