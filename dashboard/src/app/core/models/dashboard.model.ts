export interface DashboardStats {
  totalLogs: number;
  traceCount: number;
  debugCount: number;
  informationCount: number;
  warningCount: number;
  errorCount: number;
  criticalCount: number;
  logsByApplication: { [key: string]: number };
  logsByHour: { [key: string]: number };
  logsByDay: { [key: string]: number };
}

export interface DashboardSummary {
  totalApplications: number;
  activeApplications: number;
  logsLast24Hours: number;
  errorsLast24Hours: number;
  warningsLast24Hours: number;
}
