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

const statusColors: Record<number, string> = {
  0: "#374151",
  1: "#1e40af",
  2: "#065f46",
  3: "#7f1d1d",
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
      setInvoices((current) =>
        current.map((item) =>
          item.id === invoice.id ? { ...item, status } : item
        )
      );
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

  if (loading)
    return (
      <div className="page-section">
        <p>Loading invoices...</p>
      </div>
    );

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">Billing</p>
          <h1>Invoices</h1>
        </div>
        <Link className="primary-button" href="/invoices/new">
          New invoice <span aria-hidden="true">+</span>
        </Link>
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
                <td
                  colSpan={6}
                  style={{
                    textAlign: "center",
                    padding: "28px 12px",
                    color: "var(--muted)",
                  }}
                >
                  No invoices yet. Create one to start tracking billing.
                </td>
              </tr>
            ) : null}
            {invoices.map((invoice) => (
              <tr key={invoice.id}>
                <td>{invoice.invoiceNumber}</td>
                <td>{invoice.clientName}</td>

                {/* Status — badge + hidden select */}
                <td>
                  <div
                    style={{
                      position: "relative",
                      display: "inline-block",
                    }}
                  >
                    {/* Visible badge */}
                    <div
                      style={{
                        backgroundColor:
                          statusColors[invoice.status] ?? "#374151",
                        color: "white",
                        borderRadius: "20px",
                        padding: "4px 12px",
                        fontSize: "12px",
                        fontWeight: "600",
                        display: "inline-flex",
                        alignItems: "center",
                        gap: "6px",
                        cursor:
                          busyInvoiceId === invoice.id
                            ? "not-allowed"
                            : "pointer",
                        opacity: busyInvoiceId === invoice.id ? 0.5 : 1,
                        userSelect: "none",
                      }}
                    >
                      <span
                        style={{
                          width: "6px",
                          height: "6px",
                          borderRadius: "50%",
                          backgroundColor: "white",
                          opacity: 0.8,
                          display: "inline-block",
                          flexShrink: 0,
                        }}
                      />
                      {statusMap[invoice.status]}
                      <span style={{ fontSize: "10px", opacity: 0.7 }}>
                        ▾
                      </span>
                    </div>

                    {/* Invisible select on top */}
                    <select
                      aria-label={`Status for ${invoice.invoiceNumber}`}
                      value={invoice.status}
                      disabled={busyInvoiceId === invoice.id}
                      onChange={(event) =>
                        updateStatus(invoice, Number(event.target.value))
                      }
                      style={{
                        position: "absolute",
                        top: 0,
                        left: 0,
                        width: "100%",
                        height: "100%",
                        opacity: 0,
                        cursor: "pointer",
                        border: "none",
                      }}
                    >
                      {Object.entries(statusMap).map(([value, label]) => (
                        <option value={value} key={value}>
                          {label}
                        </option>
                      ))}
                    </select>
                  </div>
                </td>

                <td>PKR {invoice.totalAmount.toLocaleString()}</td>
                <td>{new Date(invoice.dueDate).toLocaleDateString()}</td>

                {/* PDF button */}
                <td>
                  <button
                    type="button"
                    onClick={() => downloadPdf(invoice)}
                    disabled={busyInvoiceId === invoice.id}
                    style={{
                      backgroundColor: "transparent",
                      border: "1px solid var(--accent, #2dd4bf)",
                      color: "var(--accent, #2dd4bf)",
                      borderRadius: "6px",
                      padding: "4px 12px",
                      fontSize: "12px",
                      fontWeight: "600",
                      cursor:
                        busyInvoiceId === invoice.id
                          ? "not-allowed"
                          : "pointer",
                      opacity: busyInvoiceId === invoice.id ? 0.5 : 1,
                      transition: "opacity 0.2s",
                      display: "inline-flex",
                      alignItems: "center",
                      gap: "4px",
                    }}
                  >
                    {busyInvoiceId === invoice.id ? "..." : "↓ PDF"}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}