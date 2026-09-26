export type ApiResponse<T> = {
  success: boolean;
  message: string;
  data: T | null;
};

export type AuthResponse = {
  token: string;
  fullName: string;
  email: string;
  role: string;
};

export type Client = {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  address: string;
  isActive: boolean;
  createdAt: string;
};

export type Invoice = {
  id: string;
  invoiceNumber: string;
  clientName: string;
  status: number;
  totalAmount: number;
  dueDate: string;
  createdAt: string;
};

export type UserRecord = {
  id: string;
  fullName: string;
  email: string;
  role: number;
  isActive: boolean;
};

export type TopClient = {
  clientName: string;
  totalRevenue: number;
};

export type DashboardData = {
  totalRevenue: number;
  outstandingAmount: number;
  overdueInvoicesCount: number;
  activeClientsCount: number;
  topClients: TopClient[];
};

export type StaffDashboardData = {
  totalClientsCount: number;
  activeClientsCount: number;
  myActionsThisWeek: number;
  assignedTasksCount: number;
  openTasksCount: number;
  recentActivity: {
    action: string;
    entityName: string;
    details: string;
    createdAt: string;
  }[];
};

export type WorkTask = {
  id: string;
  assignedUserId: string;
  assignedUserName: string;
  title: string;
  description: string;
  dueDate: string | null;
  status: number;
  createdAt: string;
};

export type AuditLog = {
  id: string;
  userName: string;
  action: string;
  entityName: string;
  details: string;
  createdAt: string;
};
