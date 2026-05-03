import { InvoiceListDto } from "../components/InvoiceList";
import api from "./axiosInstance";

export interface InvoiceFilters {
  shopId?: string;
  cashierId?: string;
  startDate?: string;
  endDate?: string;
}

// 1. MERR FATURAT (INVOICES)
export const getInvoices = async (filters?: InvoiceFilters): Promise<InvoiceListDto[]> => {
  try {
    const params: any = {};
    if (filters?.shopId) params.shopId = filters.shopId;
    if (filters?.cashierId) params.cashierId = filters.cashierId;
    if (filters?.startDate) params.startDate = filters.startDate;
    if (filters?.endDate) params.endDate = filters.endDate;

    const res = await api.get("/api/Invoice", { params });
    return Array.isArray(res.data?.data) ? res.data.data : res.data ?? [];
  } catch (err: any) {
    console.error("Error fetching invoices:", err.response?.status, err.response?.data);
    return [];
  }
};

// 2. MERR PËRMBLEDHJEN (SUMMARY) - E sinkronizuar me Swagger (start/end)
export const getInvoiceSummary = async (filters: InvoiceFilters) => {
  try {
    const params: any = {};

    if (filters.startDate) {
      // Krijo datën nga stringu "YYYY-MM-DD"
      const dStart = new Date(filters.startDate);
      dStart.setHours(0, 0, 0, 0);
      // Përdorim toISOString() pasi Swagger kërkon date-time
      params.start = dStart.toISOString(); 
    }
    
    if (filters.endDate) {
      const dEnd = new Date(filters.endDate);
      dEnd.setHours(23, 59, 59, 999);
      params.end = dEnd.toISOString();
    }

    if (filters.shopId) params.shopId = filters.shopId;
    if (filters.cashierId) params.cashierId = filters.cashierId;

    const res = await api.get("/api/Invoice/summary", { params });
    
    // KONTROLLI I STRUKTURËS: Disa API e kthejnë direkt, disa te .data
    return res.data?.data || res.data || [];
  } catch (err: any) {
    console.error("API Error Details:", err.response?.data);
    return [];
  }
};

// 3. SHOPS & CASHIERS
export const getShops = async () => {
  try {
    const res = await api.get("/api/Shops/my-shops");
    return res.data?.data ?? res.data ?? [];
  } catch (err: any) {
    console.error("Error fetching shops:", err);
    return [];
  }
};

export const getCashiers = async () => {
  try {
    const res = await api.get("/api/Cashier");
    return res.data?.data ?? res.data ?? [];
  } catch (err: any) {
    console.error("Error fetching cashiers:", err);
    return [];
  }
};