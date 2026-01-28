export interface Application {
  id: string;
  name: string;
  apiKey: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateApplicationRequest {
  name: string;
  description?: string;
}

export interface UpdateApplicationRequest {
  name?: string;
  description?: string;
  isActive?: boolean;
}
