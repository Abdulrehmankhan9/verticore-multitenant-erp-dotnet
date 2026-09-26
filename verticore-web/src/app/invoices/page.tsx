"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { apiRequest, getAuthToken } from "@/lib/api";
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
            </tr>
          </thead>
          <tbody>
            {invoices.length === 0 ? (
              <tr>
                <td colSpan={5} style={{ textAlign: "center", padding: "28px 12px", color: "var(--muted)" }}>
                  No invoices yet. Create one to start tracking billing.
                </td>
              </tr>
            ) : null}
            {invoices.map((invoice) => (
              <tr key={invoice.id}>
                <td>{invoice.invoiceNumber}</td>
                <td>{invoice.clientName}</td>
                <td>{statusMap[invoice.status] ?? "Unknown"}</td>
                <td>${invoice.totalAmount.toLocaleString()}</td>
                <td>{new Date(invoice.dueDate).toLocaleDateString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
