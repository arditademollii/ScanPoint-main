import { useEffect, useState, useCallback } from "react";
import InvoiceFilters, { Filters } from "../components/InvoiceFilters";
import InvoiceList from "../components/InvoiceList";
import { InvoiceListDto } from "../components/InvoiceList";
import { getInvoices, getShops, getCashiers } from "../api/invoiceApi";

const InvoicesPage = () => {
  const [allInvoices, setAllInvoices] = useState<InvoiceListDto[]>([]);
  const [invoices, setInvoices] = useState<InvoiceListDto[]>([]);
  const [shops, setShops] = useState<any[]>([]);
  const [cashiers, setCashiers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [sh, ca, inv] = await Promise.all([
        getShops(),
        getCashiers(),
        getInvoices()
      ]);
      
      setShops(sh);
      setCashiers(ca);
      setAllInvoices(inv);
      setInvoices(inv);
    } catch (error) {
      console.error("Error loading data:", error);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    loadData();
    const handleStorageChange = (e: StorageEvent) => {
      if (e.key === "accessToken") loadData();
    };
    window.addEventListener("storage", handleStorageChange);
    return () => window.removeEventListener("storage", handleStorageChange);
  }, [loadData]);

  const handleApplyFilters = (filters: Filters) => {
    let data = [...allInvoices];

    // 1. Filtri për Dyqanin
    if (filters.shopName) {
      data = data.filter((inv) => inv.shopName === filters.shopName);
    }

    // 2. Filtri për Arkëtarin
    if (filters.cashierName) {
      data = data.filter((inv) => inv.cashierName === filters.cashierName);
    }

    // 3. Filtri për Datat (Logjika e saktë për ISO Strings)
    if (filters.startDate || filters.endDate) {
      data = data.filter((inv) => {
        // Kthejmë datën e faturës në milisekonda
        const invTime = new Date(inv.createdAt).getTime();

        if (filters.startDate) {
          const start = new Date(filters.startDate);
          start.setHours(0, 0, 0, 0); // Fillimi i ditës
          if (invTime < start.getTime()) return false;
        }

        if (filters.endDate) {
          const end = new Date(filters.endDate);
          end.setHours(23, 59, 59, 999); // Fundi i ditës
          if (invTime > end.getTime()) return false;
        }

        return true;
      });
    }

    setInvoices(data);
  };

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      <header className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Invoices Management</h1>
        <p className="text-gray-500">View and filter your store invoices</p>
      </header>

      <InvoiceFilters
        shops={shops}
        cashiers={cashiers}
        onApplyFilters={handleApplyFilters}
      />

      <div className="mt-8">
        {loading ? (
          <div className="flex justify-center p-10">
            <div className="animate-spin rounded-full h-10 w-10 border-b-2 border-blue-600"></div>
          </div>
        ) : (
          <InvoiceList invoices={invoices} />
        )}
      </div>
    </div>
  );
};

export default InvoicesPage;