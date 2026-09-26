"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { apiRequest, getAuthToken, getUserRole } from "@/lib/api";
import type { DashboardData, StaffDashboardData } from "@/types";

type DashboardView =
  | { role: "Staff"; data: StaffDashboardData }
  | { role: "Manager" | "TenantAdmin"; data: DashboardData };

export default function DashboardPage() {
  const router = useRouter();
  const [view, setView] = useState<DashboardView | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const token = getAuthToken();
    if (!token) {
      router.push("/login");
      return;
    }

    const role = getUserRole();
    const isStaff = role === "Staff";
    const dashboardRequest = isStaff
      ? apiRequest<StaffDashboardData>("/api/Dashboard/staff")
      : apiRequest<DashboardData>("/api/Dashboard");

    dashboardRequest
      .then((result) => {
        if (isStaff) {
          setView({ role: "Staff", data: result as StaffDashboardData });
        } else {
          setView({ role: role === "Manager" ? "Manager" : "TenantAdmin", data: result as DashboardData });
        }
      })
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, [router]);

  if (loading) return <div className="page-section"><p>Loading dashboard...</p></div>;
  if (error) return <div className="page-section"><p className="error-box">{error}</p></div>;
  if (!view) return null;

  if (view.role === "Staff") {
    const data = view.data;

    return (
      <div className="page-section">
        <div className="page-header">
          <div>
            <p className="eyebrow">My workspace</p>
            <h1>Staff dashboard</h1>
          </div>
          <Link className="primary-button" href="/clients">Open clients</Link>
        </div>

        <div className="stats-grid">
          <div className="stat-card"><span>Workspace clients</span><strong>{data.totalClientsCount}</strong></div>
          <div className="stat-card"><span>Active clients</span><strong>{data.activeClientsCount}</strong></div>
          <div className="stat-card"><span>My actions this week</span><strong>{data.myActionsThisWeek}</strong></div>
        </div>

        <div className="panel">
          <h3>My recent activity</h3>
          {data.recentActivity.length === 0 ? (
            <p className="dashboard-empty">Your client activity will appear here.</p>
          ) : (
            <table className="data-table">
              <thead><tr><th>Action</th><th>Details</th><th>Time</th></tr></thead>
              <tbody>
                {data.recentActivity.map((activity, index) => (
                  <tr key={`${activity.createdAt}-${index}`}>
                    <td>{activity.action} {activity.entityName}</td>
                    <td>{activity.details}</td>
                    <td>{new Date(activity.createdAt).toLocaleString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    );
  }

  const data = view.data;

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <p className="eyebrow">Overview</p>
          <h1>{view.role === "Manager" ? "Manager dashboard" : "Business dashboard"}</h1>
        </div>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <span>Total Revenue</span>
          <strong>PKR {data.totalRevenue.toLocaleString()}</strong>
        </div>
        <div className="stat-card">
          <span>Outstanding</span>
          <strong>PKR {data.outstandingAmount.toLocaleString()}</strong>
        </div>
        <div className="stat-card">
          <span>Overdue</span>
          <strong>{data.overdueInvoicesCount}</strong>
        </div>
        <div className="stat-card">
          <span>Active Clients</span>
          <strong>{data.activeClientsCount}</strong>
        </div>
      </div>

      <div className="panel">
        <h3>Top clients</h3>
        <table className="data-table">
          <thead>
            <tr>
              <th>Client</th>
              <th>Revenue</th>
            </tr>
          </thead>
          <tbody>
            {data.topClients.map((client) => (
              <tr key={client.clientName}>
                <td>{client.clientName}</td>
                <td>PKR {client.totalRevenue.toLocaleString()}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
