"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useSyncExternalStore } from "react";
import { clearAuthToken, getUserRole } from "@/lib/api";

const navItems = [
  { href: "/dashboard", label: "Dashboard" },
  { href: "/clients", label: "Clients" },
  { href: "/tasks", label: "Tasks" },
  { href: "/invoices", label: "Invoices" },
  { href: "/users", label: "Users" },
  { href: "/audit", label: "Audit" },
];

function subscribeToAuth(callback: () => void) {
  window.addEventListener("storage", callback);
  window.addEventListener("verticore-auth-change", callback);
  return () => {
    window.removeEventListener("storage", callback);
    window.removeEventListener("verticore-auth-change", callback);
  };
}

function getAuthRoleSnapshot() {
  return getUserRole() ?? "";
}

function getServerAuthRoleSnapshot() {
  return "";
}

export function AppShell({ children }: { children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();
  const isPublicRoute = pathname === "/" || pathname === "/login" || pathname === "/register" || pathname === "/forgot-password" || pathname === "/reset-password" || pathname === "/set-password";
  const role = useSyncExternalStore(subscribeToAuth, getAuthRoleSnapshot, getServerAuthRoleSnapshot);
  const visibleNavItems = navItems.filter((item) => {
    if (item.href === "/dashboard" || item.href === "/clients" || item.href === "/tasks") return true;
    if (role === "TenantAdmin") return true;
    if (role === "Manager") return item.href === "/invoices";
    return false;
  });

  function handleLogout() {
    clearAuthToken();
    router.push("/login");
  }

  if (isPublicRoute) {
    return <>{children}</>;
  }

  return (
    <div className="app-shell">
      <aside className="sidebar">
        <div className="brand-block">
          <div className="brand-mark">V</div>
          <div>
            <p className="eyebrow">ERP</p>
            <h2>VertiCore</h2>
          </div>
        </div>

        <nav className="nav">
          {visibleNavItems.map((item) => (
            <Link
              key={item.href}
              href={item.href}
              className={pathname === item.href ? "nav-item active" : "nav-item"}
            >
              {item.label}
            </Link>
          ))}
        </nav>

        <button type="button" className="logout-button" onClick={handleLogout}>
          Logout
        </button>
      </aside>

      <main className="content-area">{children}</main>
    </div>
  );
}
