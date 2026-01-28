export enum LogLevel {
  Trace = 0,
  Debug = 1,
  Information = 2,
  Warning = 3,
  Error = 4,
  Critical = 5
}

export interface LogEntry {
  id: string;
  applicationId: string;
  applicationName: string;
  level: LogLevel;
  message: string;
  exception?: string;
  stackTrace?: string;
  properties?: string;
  correlationId?: string;
  source?: string;
  timestamp: string;
}

export interface PagedResponse<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface LogQueryParams {
  applicationId?: string;
  minLevel?: LogLevel;
  maxLevel?: LogLevel;
  searchText?: string;
  correlationId?: string;
  from?: string;
  to?: string;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
}
