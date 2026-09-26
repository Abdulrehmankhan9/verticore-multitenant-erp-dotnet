import { redirect } from "next/navigation";

export default async function LegacySetPasswordPage({
  searchParams,
}: {
  searchParams: Promise<{ token?: string | string[] }>;
}) {
  const { token } = await searchParams;
  const invitationToken = Array.isArray(token) ? token[0] : token;
  const destination = invitationToken
    ? `/set-password?token=${encodeURIComponent(invitationToken)}`
    : "/set-password";

  redirect(destination);
}