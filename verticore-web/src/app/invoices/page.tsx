"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { apiDownload, apiRequest, getAuthToken } from "@/lib/api";
import type { Invoice } from "@/types";

const statusMap: Record<number, string> = {
  0: "Draft",
  1: "Sent",
  2: "Paid",
  3: "Overdue",
};

export default function InvoicesPage() {
  const router = useRouter();
  const [invoices, setInvoices] = useState<Invoice[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [busyInvoiceId, setBusyInvoiceId] = useState("");

  useEffect(() => {
    const token = getAuthToken();
    if (!token) {
      router.push("/login");
      return;
    }

    apiRequest<Invoice[]>("/api/Invoice")
      .then((result) => setInvoices(result))
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [router]);

  async function updateStatus(invoice: Invoice, status: number) {
    setError("");
    setBusyInvoiceId(invoice.id);
    try {
      await apiRequest<string>(`/api/Invoice/${invoice.id}/status`, {
        method: "PUT",
        body: JSON.stringify(status),
      });
      setInvoices((current) => current.map((item) => item.id === invoice.id ? { ...item, status } : item));
    } catch (err) {
      setError(err instanceof Error ? err.message : "Status update failed");
    } finally {
      setBusyInvoiceId("");
    }
  }

  async function downloadPdf(invoice: Invoice) {
    setError("");
    setBusyInvoiceId(invoice.id);
    try {
      const blob = await apiDownload(`/api/Invoice/${invoice.id}/pdf`);
      const url = URL.createObjectURL(blob);
      const anchor = document.createElement("a");
      anchor.href = url;
      anchor.download = `${invoice.invoiceNumber}.pdf`;
      anchor.click();
      URL.revokeObjectURL(url);
    } catch (err) {
      setError(err instanceof Error ? err.message : "PDF download failed");
    } finally {
      setBusyInvoiceId("");
    }
  }

  if (loading) return <div className="page-section"><p>Loading invoices...</p></div>;

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">Billing</p>
          <h1>Invoices</h1>
        </div>
        <Link className="primary-button" href="/invoices/new">New invoice <span aria-hidden="true">+</span></Link>
      </div>

      {error ? <div className="error-box">{error}</div> : null}

      <div className="panel">
        <table className="data-table">
          <thead>
            <tr>
              <th>Invoice</th>
              <th>Client</th>
              <th>Status</th>
              <th>Amount</th>
              <th>Due</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {invoices.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ textAlign: "center", padding: "28px 12px", color: "var(--muted)" }}>
                  No invoices yet. Create one to start tracking billing.
                </td>
              </tr>
            ) : null}
            {invoices.map((invoice) => (
              <tr key={invoice.id}>
                <td>{invoice.invoiceNumber}</td>
                <td>{invoice.clientName}</td>
                <td>
                  <select
                    aria-label={`Status for ${invoice.invoiceNumber}`}
                    value={invoice.status}
                    disabled={busyInvoiceId === invoice.id}
                    onChange={(event) => updateStatus(invoice, Number(event.target.value))}
                  >
                    {Object.entries(statusMap).map(([value, label]) => <option value={value} key={value}>{label}</option>)}
                  </select>
                </td>
                <td>PKR {invoice.totalAmount.toLocaleString()}</td>
                <td>{new Date(invoice.dueDate).toLocaleDateString()}</td>
                <td><button type="button" onClick={() => downloadPdf(invoice)} disabled={busyInvoiceId === invoice.id}>PDF</button></td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
