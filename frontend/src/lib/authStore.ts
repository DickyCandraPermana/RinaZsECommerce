import { create } from "zustand";

interface UserPayload {
  id: string;
  name: string;
  email: string;
  role: "Admin" | "Customer";
  profileId: string;
}

interface AuthStore {
  token: string | null;
  user: UserPayload | null;
  isLoading: boolean;
  login: (token: string) => void;
  logout: () => void;
  initialize: () => void;
}

function decodeJwt(token: string): UserPayload | null {
  try {
    const parts = token.split(".");
    if (parts.length !== 3) return null;
    const payload = JSON.parse(atob(parts[1]));
    return {
      id: payload.sub || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
      name: payload.name || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] || "User",
      email: payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || "",
      role: (payload.role || payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || "Customer") as "Admin" | "Customer",
      profileId: payload.profileId || "",
    };
  } catch {
    return null;
  }
}

export const useAuthStore = create<AuthStore>((set) => ({
  token: null,
  user: null,
  isLoading: true,

  initialize: () => {
    if (typeof window === "undefined") return;
    const token = localStorage.getItem("rinazs_token");
    if (token) {
      const user = decodeJwt(token);
      if (user) {
        set({ token, user, isLoading: false });
        return;
      }
      localStorage.removeItem("rinazs_token");
    }
    set({ token: null, user: null, isLoading: false });
  },

  login: (token: string) => {
    if (typeof window !== "undefined") {
      localStorage.setItem("rinazs_token", token);
    }
    const user = decodeJwt(token);
    set({ token, user, isLoading: false });
  },

  logout: () => {
    if (typeof window !== "undefined") {
      localStorage.removeItem("rinazs_token");
    }
    set({ token: null, user: null, isLoading: false });
  },
}));
