import type { ApiResponse } from "@/types";

export const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || "https://localhost:7153";

export async function apiRequest<T>(path: string, options: RequestInit = {}): Promise<T> {
  const publicEndpoints = new Set([
    "/api/Auth/login",
    "/api/Auth/register",
    "/api/Auth/forgot-password",
    "/api/Auth/reset-password",
    "/api/User/set-password",
  ]);
  const token = !publicEndpoints.has(path) && typeof window !== "undefined"
    ? localStorage.getItem("verticore_token")
    : null;

  const response = await fetch(`${API_BASE_URL}${path}`, {
    ...options,
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers || {}),
    },
  });

  const text = await response.text();
  const payload = text ? JSON.parse(text) : null;

  if (!response.ok) {
    throw new Error(payload?.message || "Request failed");
  }

  return (payload as ApiResponse<T>)?.data ?? (payload as T);
}

export function getAuthToken() {
  if (typeof window === "undefined") return null;
  return localStorage.getItem("verticore_token");
}

export function getUserRole() {
  const token = getAuthToken();
  if (!token) return null;

  try {
    const payload = token.split(".")[1];
    if (!payload) return null;

    const normalizedPayload = payload.replace(/-/g, "+").replace(/_/g, "/");
    const binaryPayload = atob(normalizedPayload.padEnd(Math.ceil(normalizedPayload.length / 4) * 4, "="));
    const payloadBytes = Uint8Array.from(binaryPayload, (character) => character.charCodeAt(0));
    const claims = JSON.parse(new TextDecoder().decode(payloadBytes)) as Record<string, string | string[]>;
    const role = claims.role ?? claims.roles ?? claims["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

    return Array.isArray(role) ? role[0] ?? null : role ?? null;
  } catch {
    return null;
  }
}

export function setAuthToken(token: string) {
  localStorage.setItem("verticore_token", token);
  window.dispatchEvent(new Event("verticore-auth-change"));
}

export function clearAuthToken() {
  localStorage.removeItem("verticore_token");
  window.dispatchEvent(new Event("verticore-auth-change"));
}
