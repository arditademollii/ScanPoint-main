import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from "axios";

const BASE_URL = import.meta.env.VITE_API_URL || "http://localhost:5055";
const SESSION_KEY = "sp_auth";

// --- HELPERS ---
function getSession(): { accessToken: string; refreshToken: string } | null {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    return raw ? JSON.parse(raw) : null;
  } catch {
    return null;
  }
}

function updateSession(accessToken: string, refreshToken: string): void {
  try {
    const raw = sessionStorage.getItem(SESSION_KEY);
    const current = raw ? JSON.parse(raw) : {};
    sessionStorage.setItem(
      SESSION_KEY,
      JSON.stringify({ ...current, accessToken, refreshToken })
    );
  } catch {}
}

function clearSession(): void {
  sessionStorage.removeItem(SESSION_KEY);
}

// --- CAMELCASE CONVERTER ---
// Konverton çdo response nga PascalCase (C#) në camelCase (TypeScript)
const toCamel = (obj: any): any => {
  if (Array.isArray(obj)) return obj.map(toCamel);
  if (obj !== null && typeof obj === "object") {
    return Object.keys(obj).reduce((acc, key) => {
      const camelKey = key.charAt(0).toLowerCase() + key.slice(1);
      acc[camelKey] = toCamel(obj[key]);
      return acc;
    }, {} as any);
  }
  return obj;
};

// --- AXIOS INSTANCE ---
const api: AxiosInstance = axios.create({
  baseURL: BASE_URL,
});

// --- REQUEST INTERCEPTOR ---
// Shton Authorization header dhe vendos Content-Type korrekt
api.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const session = getSession();
    if (session?.accessToken) {
      config.headers.Authorization = `Bearer ${session.accessToken}`;
    }

    // ✅ Mos vendos Content-Type për FormData — browser-i e vendos vetë me boundary
    if (!(config.data instanceof FormData)) {
      config.headers["Content-Type"] = "application/json";
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// --- RESPONSE INTERCEPTOR ---
// ✅ Refresh token automatik me queue për requeste paralele
let isRefreshing = false;
let failedQueue: Array<{
  resolve: (token: string) => void;
  reject: (err: unknown) => void;
}> = [];

function processQueue(error: unknown, token: string | null): void {
  failedQueue.forEach((p) => (error ? p.reject(error) : p.resolve(token!)));
  failedQueue = [];
}

api.interceptors.response.use(
  (response: AxiosResponse) => {
    if (response.data) {
      response.data = toCamel(response.data);
    }
    return response;
  },
  async (error) => {
    const originalRequest = error.config;

    // Mos u provo refresh nëse:
    // 1. Nuk është 401
    // 2. Tashmë u provua
    // 3. Është vetë endpoint-i i auth (shmang loop-in infinit)
    if (
      error.response?.status !== 401 ||
      originalRequest._retry ||
      originalRequest.url?.includes("/api/auth/")
    ) {
      return Promise.reject(error);
    }

    // Nëse tashmë po refreshohet — fut në queue
    if (isRefreshing) {
      return new Promise((resolve, reject) => {
        failedQueue.push({ resolve, reject });
      }).then((token) => {
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return api(originalRequest);
      });
    }

    originalRequest._retry = true;
    isRefreshing = true;

    const session = getSession();
    if (!session?.refreshToken) {
      clearSession();
      window.location.href = "/signin";
      return Promise.reject(error);
    }

    try {
      // ✅ Refresh token rotation — backend kthen refresh token të ri
      const res = await axios.post(`${BASE_URL}/api/auth/refresh`, {
        refreshToken: session.refreshToken,
      });

      const { accessToken, refreshToken } = res.data;
      updateSession(accessToken, refreshToken);
      processQueue(null, accessToken);

      originalRequest.headers.Authorization = `Bearer ${accessToken}`;
      return api(originalRequest);
    } catch (refreshError) {
      processQueue(refreshError, null);
      clearSession();
      window.location.href = "/signin";
      return Promise.reject(refreshError);
    } finally {
      isRefreshing = false;
    }
  }
);

export default api;