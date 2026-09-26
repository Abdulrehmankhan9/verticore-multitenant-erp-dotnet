"use client";

import { useEffect, useState, useSyncExternalStore, type FormEvent } from "react";
import { useRouter } from "next/navigation";
import { apiRequest, getAuthToken, getUserRole, subscribeToAuthChanges } from "@/lib/api";
import type { UserRecord, WorkTask } from "@/types";
import styles from "./page.module.css";

const statuses = ["Open", "In progress", "Completed"];

export default function TasksPage() {
  const router = useRouter();
  const role = useSyncExternalStore(subscribeToAuthChanges, () => getUserRole() ?? "", () => "");
  const canAssign = role === "TenantAdmin" || role === "Manager";
  const [tasks, setTasks] = useState<WorkTask[]>([]);
  const [staff, setStaff] = useState<UserRecord[]>([]);
  const [form, setForm] = useState({ title: "", description: "", assignedUserId: "", dueDate: "" });
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [busyTaskId, setBusyTaskId] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    if (!getAuthToken()) {
      router.replace("/login");
      return;
    }

    Promise.all([
      apiRequest<WorkTask[]>("/api/Task"),
      canAssign ? apiRequest<UserRecord[]>("/api/User") : Promise.resolve([]),
    ])
      .then(([loadedTasks, users]) => {
        setTasks(loadedTasks);
        const activeStaff = users.filter((user) => user.isActive && user.role === 3);
        setStaff(activeStaff);
        if (activeStaff[0]) setForm((current) => ({ ...current, assignedUserId: activeStaff[0].id }));
      })
      .catch((loadError) => setError(loadError instanceof Error ? loadError.message : "Unable to load tasks"))
      .finally(() => setLoading(false));
  }, [canAssign, router]);

  async function createTask(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setError("");
    try {
      const created = await apiRequest<WorkTask>("/api/Task", {
        method: "POST",
        body: JSON.stringify({ ...form, dueDate: form.dueDate ? new Date(`${form.dueDate}T12:00:00`).toISOString() : null }),
      });
      setTasks((current) => [created, ...current]);
      setForm((current) => ({ ...current, title: "", description: "", dueDate: "" }));
    } catch (createError) {
      setError(createError instanceof Error ? createError.message : "Unable to assign task");
    } finally {
      setSaving(false);
    }
  }

  async function updateStatus(task: WorkTask, status: number) {
    setBusyTaskId(task.id);
    setError("");
    try {
      await apiRequest<string>(`/api/Task/${task.id}/status`, {
        method: "PUT",
        body: JSON.stringify({ status }),
      });
      setTasks((current) => current.map((item) => item.id === task.id ? { ...item, status } : item));
    } catch (updateError) {
      setError(updateError instanceof Error ? updateError.message : "Unable to update task");
    } finally {
      setBusyTaskId("");
    }
  }

  if (loading) return <div className="page-section"><p>Loading tasks...</p></div>;

  return (
    <div className="page-section">
      <div className="page-header">
        <div><p className="eyebrow">Team workflow</p><h1>{canAssign ? "Tasks" : "My tasks"}</h1></div>
      </div>

      {error ? <div className="error-box" role="alert">{error}</div> : null}

      {canAssign ? (
        <form className={`card-form ${styles.assignForm}`} onSubmit={createTask}>
          <h3>Assign a task</h3>
          {staff.length === 0 ? <p className={styles.emptyMessage}>Invite an active staff member before assigning tasks.</p> : (
            <>
              <div className={styles.formGrid}>
                <label>Task title<input value={form.title} onChange={(event) => setForm({ ...form, title: event.target.value })} maxLength={160} required /></label>
                <label>Assign to<select value={form.assignedUserId} onChange={(event) => setForm({ ...form, assignedUserId: event.target.value })} required>{staff.map((user) => <option key={user.id} value={user.id}>{user.fullName}</option>)}</select></label>
                <label>Due date <span className={styles.optional}>Optional</span><input type="date" min={new Date().toISOString().slice(0, 10)} value={form.dueDate} onChange={(event) => setForm({ ...form, dueDate: event.target.value })} /></label>
                <label className={styles.descriptionField}>Description <span className={styles.optional}>Optional</span><textarea rows={2} value={form.description} onChange={(event) => setForm({ ...form, description: event.target.value })} maxLength={2000} /></label>
              </div>
              <div className={styles.formActions}><button className="primary-button" type="submit" disabled={saving}>{saving ? "Assigning..." : "Assign task"}</button></div>
            </>
          )}
        </form>
      ) : null}

      <div className={`panel ${styles.taskPanel}`}>
        <h3>{canAssign ? "Workspace tasks" : "Assigned to me"}</h3>
        {tasks.length === 0 ? <p className={styles.emptyMessage}>No tasks yet.</p> : (
          <div className={styles.taskList}>
            {tasks.map((task) => (
              <article className={styles.taskRow} key={task.id}>
                <div className={styles.taskCopy}>
                  <h4>{task.title}</h4>
                  {task.description ? <p>{task.description}</p> : null}
                  <span>{canAssign ? `Assigned to ${task.assignedUserName}` : task.assignedUserName}{task.dueDate ? ` · Due ${new Date(task.dueDate).toLocaleDateString()}` : " · No due date"}</span>
                </div>
                <select aria-label={`Status for ${task.title}`} value={task.status} disabled={busyTaskId === task.id} onChange={(event) => updateStatus(task, Number(event.target.value))}>
                  {statuses.map((status, index) => <option key={status} value={index}>{status}</option>)}
                </select>
              </article>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}