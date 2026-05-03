import {
  createContext,
  useContext,
  useState,
  useCallback,
  useEffect,
  ReactNode,
} from "react";
import api from "../api/axiosInstance";

interface AuthUser {
  accessToken: string;
  refreshToken: string;
  role: "admin" | "manager" | "cashier";
}

interface UserInfo {
  username: string;
  email: string;
  profileImagePath?: string;
}

interface AuthContextType {
  user: AuthUser | null;
  userInfo: UserInfo | null;
  login: (accessToken: string, refreshToken: string, role: string) => Promise<void>;
  logout: () => Promise<void>;
  updateTokens: (accessToken: string, refreshToken: string) => void;
  refreshUser: () => Promise<void>;
  isAuthenticated: boolean;
}

const SESSION_KEY = "sp_auth";

function readSession(): AuthUser | null {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function writeSession(user: AuthUser | null): void {
  if (user) {
    sessionStorage.setItem(SESSION_KEY, JSON.stringify(user));
  } else {
    sessionStorage.removeItem(SESSION_KEY);
  }
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<AuthUser | null>(readSession);
  const [userInfo, setUserInfo] = useState<UserInfo | null>(null);

  // Kur aplikacioni hapet / page refresh — ngarko userInfo nga sesioni aktiv
  useEffect(() => {
    const session = readSession();
    if (!session) return;
    api
      .get("/api/profile/me")
      .then((res) => setUserInfo(res.data))
      .catch(() => {});
  }, []);

  // LOGIN — ruan session dhe ngarko userInfo
  const login = useCallback(
    async (accessToken: string, refreshToken: string, role: string) => {
      const authUser: AuthUser = {
        accessToken,
        refreshToken,
        role: role.toLowerCase() as AuthUser["role"],
      };
      setUser(authUser);
      writeSession(authUser);

      try {
        const res = await api.get("/api/profile/me");
        setUserInfo(res.data);
      } catch {}
    },
    []
  );

  // LOGOUT
  // ✅ Thërret /api/auth/logout për të invaliduar refresh token në DB
  // ✅ Pastron session lokal pavarësisht nga përgjigja e serverit
  const logout = useCallback(async () => {
    const session = readSession();

    // Invalido token-in në server (fire-and-forget — nuk presim)
    if (session?.refreshToken) {
      api
        .post("/api/auth/logout", { refreshToken: session.refreshToken })
        .catch(() => {});
    }

    // Pastro state dhe session lokalisht
    setUser(null);
    setUserInfo(null);
    writeSession(null);
  }, []);

  const updateTokens = useCallback(
    (accessToken: string, refreshToken: string) => {
      setUser((prev) => {
        if (!prev) return null;
        const updated = { ...prev, accessToken, refreshToken };
        writeSession(updated);
        return updated;
      });
    },
    []
  );

  // REFRESH USER — rifresko userInfo nga serveri
  const refreshUser = useCallback(async () => {
    const session = readSession();
    if (!session) return;
    try {
      const res = await api.get("/api/profile/me");
      setUserInfo(res.data);
    } catch {}
  }, []);

  return (
    <AuthContext.Provider
      value={{
        user,
        userInfo,
        login,
        logout,
        updateTokens,
        refreshUser,
        isAuthenticated: !!user,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextType {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth duhet të përdoret brenda AuthProvider");
  return ctx;
}
