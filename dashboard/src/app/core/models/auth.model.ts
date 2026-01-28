export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  name: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  name: string;
  expiresAt: string;
}

export interface User {
  id: string;
  email: string;
  name: string;
}
