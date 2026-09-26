"use client";

import { useEffect, useState, useSyncExternalStore } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, getAuthToken, getUserRole, subscribeToAuthChanges } from "@/lib/api";
import type { Client } from "@/types";
import styles from "./page.module.css";

export default function ClientsPage() {
  const router = useRouter();
  const role = useSyncExternalStore(subscribeToAuthChanges, () => getUserRole() ?? "", () => "");
  const canManageClients = role === "TenantAdmin" || role === "Manager";
  const [clients, setClients] = useState<Client[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [form, setForm] = useState({ fullName: "", email: "", phone: "", address: "" });
  const [editingClient, setEditingClient] = useState<Client | null>(null);

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
      await apiRequest<Client | string>(editingClient ? `/api/Client/${editingClient.id}` : "/api/Client", {
        method: editingClient ? "PUT" : "POST",
        body: JSON.stringify({ ...form, isActive: editingClient?.isActive ?? true }),
      });
      setForm({ fullName: "", email: "", phone: "", address: "" });
      setEditingClient(null);
      await loadClients();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Create failed");
    }
  }

  function beginEdit(client: Client) {
    setEditingClient(client);
    setForm({ fullName: client.fullName, email: client.email, phone: client.phone, address: client.address });
    window.scrollTo({ top: 0, behavior: "smooth" });
  }

  async function deleteClient(client: Client) {
    if (!window.confirm(`Delete ${client.fullName}? This action cannot be undone.`)) return;
    setError("");
    try {
      await apiRequest<string>(`/api/Client/${client.id}`, { method: "DELETE" });
      if (editingClient?.id === client.id) {
        setEditingClient(null);
        setForm({ fullName: "", email: "", phone: "", address: "" });
      }
      await loadClients();
    } catch (err) {
      setError(err instanceof Error ? err.message : "Delete failed");
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

      {canManageClients ? <form onSubmit={handleCreate} className={`card-form ${styles.clientForm}`}>
        <h3>{editingClient ? "Edit client" : "Add client"}</h3>
        <div className={`form-grid ${styles.clientFields}`}>
          <input value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} placeholder="Full name" required />
          <input value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} placeholder="Email" required />
          <input value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} placeholder="Phone" required />
          <input value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} placeholder="Address" required />
        </div>
        <div className={styles.clientActions}>
          <button type="submit" className="primary-button">{editingClient ? "Save changes" : "Create client"}</button>
          {editingClient ? (
            <button type="button" className={styles.cancelEdit} onClick={() => { setEditingClient(null); setForm({ fullName: "", email: "", phone: "", address: "" }); }}>Cancel</button>
          ) : null}
        </div>
      </form> : null}

      {error ? <div className="error-box">{error}</div> : null}

      <div className="panel">
        <table className="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>Phone</th>
              <th>Address</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {clients.map((client) => (
              <tr key={client.id}>
                <td>{client.fullName}</td>
                <td>{client.email}</td>
                <td>{client.phone}</td>
                <td>{client.address}</td>
                <td className={styles.rowActions}>
                  {canManageClients ? <>
                    <button type="button" onClick={() => beginEdit(client)}>Edit</button>
                    <button type="button" className={styles.deleteAction} onClick={() => deleteClient(client)}>Delete</button>
                  </> : <span>View only</span>}
                </td>
              </tr>
            ))}
            {clients.length === 0 ? <tr><td colSpan={5} className={styles.emptyRow}>No clients yet. Add your first client above.</td></tr> : null}
          </tbody>
        </table>
      </div>
    </div>
  );
}
