"use client";

import { FormEvent, useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { apiRequest, getAuthToken } from "@/lib/api";
import type { Client, Invoice } from "@/types";
import styles from "./page.module.css";

type InvoiceItemDraft = {
  id: string;
  description: string;
  quantity: string;
  unitPrice: string;
};

function getTomorrowDate() {
  const date = new Date();
  date.setDate(date.getDate() + 1);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  return `${year}-${month}-${day}`;
}

export default function NewInvoicePage() {
  const router = useRouter();
  const [clients, setClients] = useState<Client[]>([]);
  const [form, setForm] = useState(() => ({ clientId: "", dueDate: getTomorrowDate(), notes: "" }));
  const [minimumDueDate] = useState(getTomorrowDate);
  const [items, setItems] = useState<InvoiceItemDraft[]>([
    { id: "line-1", description: "", quantity: "1", unitPrice: "" },
  ]);
  const [loadingClients, setLoadingClients] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    let cancelled = false;
    const token = getAuthToken();

    if (!token) {
      router.replace("/login");
      return;
    }

    async function loadClients() {
      try {
        const result = await apiRequest<Client[]>("/api/Client");
        if (!cancelled) {
          setClients(result);
          setForm((current) => ({ ...current, clientId: result[0]?.id ?? "" }));
        }
      } catch (loadError) {
        if (!cancelled) {
          setError(loadError instanceof Error ? loadError.message : "Unable to load clients");
        }
      } finally {
        if (!cancelled) setLoadingClients(false);
      }
    }

    loadClients();
    return () => {
      cancelled = true;
    };
  }, [router]);

  function updateItem(id: string, field: keyof Omit<InvoiceItemDraft, "id">, value: string) {
    setItems((current) => current.map((item) => item.id === id ? { ...item, [field]: value } : item));
  }

  function addItem() {
    setItems((current) => [
      ...current,
      { id: crypto.randomUUID(), description: "", quantity: "1", unitPrice: "" },
    ]);
  }

  function removeItem(id: string) {
    setItems((current) => current.length > 1 ? current.filter((item) => item.id !== id) : current);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError("");

    try {
      const dueDate = new Date(`${form.dueDate}T12:00:00`).toISOString();
      await apiRequest<Invoice>("/api/Invoice", {
        method: "POST",
        body: JSON.stringify({
          clientId: form.clientId,
          dueDate,
          notes: form.notes,
          items: items.map((item) => ({
            description: item.description.trim(),
            quantity: Number(item.quantity),
            unitPrice: Number(item.unitPrice),
          })),
        }),
      });
      router.push("/invoices");
    } catch (submitError) {
      setError(submitError instanceof Error ? submitError.message : "Unable to create invoice");
    } finally {
      setSaving(false);
    }
  }

  const total = items.reduce((sum, item) => {
    return sum + (Number(item.quantity) || 0) * (Number(item.unitPrice) || 0);
  }, 0);

  if (loadingClients) return <div className="page-section"><p>Loading clients...</p></div>;

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">Billing / New invoice</p>
          <h1>Create invoice</h1>
        </div>
        <Link className={styles.backLink} href="/invoices">Cancel</Link>
      </div>

      {error ? <div className="error-box" role="alert">{error}</div> : null}

      {clients.length === 0 ? (
        <div className={styles.noClients}>
          <div>
            <strong>Add a client first</strong>
            <p>Invoices need a client to be assigned before they can be created.</p>
          </div>
          <Link className="primary-button" href="/clients">Go to clients</Link>
        </div>
      ) : (
        <form className={`panel ${styles.form}`} onSubmit={handleSubmit}>
          <div className={styles.formSection}>
            <div className={styles.sectionHeading}>
              <span>01</span>
              <div><h2>Invoice details</h2><p>Choose who this invoice is for and when it is due.</p></div>
            </div>
            <div className={styles.detailsGrid}>
              <label>
                Client
                <select
                  value={form.clientId}
                  onChange={(event) => setForm({ ...form, clientId: event.target.value })}
                  required
                >
                  {clients.map((client) => <option key={client.id} value={client.id}>{client.fullName}</option>)}
                </select>
              </label>
              <label>
                Due date
                <input
                  type="date"
                  min={minimumDueDate}
                  value={form.dueDate}
                  onChange={(event) => setForm({ ...form, dueDate: event.target.value })}
                  required
                />
              </label>
              <label className={styles.notesField}>
                Notes <span className={styles.optional}>Optional</span>
                <textarea
                  value={form.notes}
                  onChange={(event) => setForm({ ...form, notes: event.target.value })}
                  rows={3}
                  placeholder="Add payment terms or a short note"
                />
              </label>
            </div>
          </div>

          <div className={styles.formSection}>
            <div className={styles.sectionHeading}>
              <span>02</span>
              <div><h2>Line items</h2><p>Add the work or products included in this invoice.</p></div>
            </div>
            <div className={styles.itemList}>
              {items.map((item, index) => (
                <div className={styles.itemRow} key={item.id}>
                  <label className={styles.descriptionField}>
                    Description
                    <input
                      value={item.description}
                      onChange={(event) => updateItem(item.id, "description", event.target.value)}
                      placeholder={`Item ${index + 1}`}
                      required
                    />
                  </label>
                  <label>
                    Quantity
                    <input
                      type="number"
                      min="1"
                      step="1"
                      value={item.quantity}
                      onChange={(event) => updateItem(item.id, "quantity", event.target.value)}
                      required
                    />
                  </label>
                  <label>
                    Unit price
                    <input
                      type="number"
                      min="0.01"
                      step="0.01"
                      value={item.unitPrice}
                      onChange={(event) => updateItem(item.id, "unitPrice", event.target.value)}
                      placeholder="0.00"
                      required
                    />
                  </label>
                  <button
                    className={styles.removeItem}
                    type="button"
                    onClick={() => removeItem(item.id)}
                    disabled={items.length === 1}
                    aria-label={`Remove item ${index + 1}`}
                    title="Remove item"
                  >
                    ×
                  </button>
                </div>
              ))}
            </div>
            <button className={styles.addItem} type="button" onClick={addItem}>+ Add line item</button>
          </div>

          <div className={styles.formFooter}>
            <div className={styles.total}><span>Invoice total</span><strong>${total.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 })}</strong></div>
            <button className="primary-button" type="submit" disabled={saving || clients.length === 0}>
              {saving ? "Creating..." : "Create invoice"}
            </button>
          </div>
        </form>
      )}
    </div>
  );
}