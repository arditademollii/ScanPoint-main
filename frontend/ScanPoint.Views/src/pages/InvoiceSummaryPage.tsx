import { useEffect, useState, useCallback, useRef } from "react";
import { getShops, getCashiers, getInvoiceSummary } from "../api/invoiceApi";
import InvoiceFilters, { Filters } from "../components/InvoiceFilters";

interface InvoiceSummaryDto {
  date: string;
  shopName: string;
  cashierName: string;
  totalAmount: number;
  invoiceCount: number;
}

const InvoiceSummaryPage = () => {
  const [summary, setSummary] = useState<InvoiceSummaryDto[]>([]);
  const [shops, setShops] = useState<any[]>([]);
  const [cashiers, setCashiers] = useState<any[]>([]);
  const [loading, setLoading] = useState(false);
  const [message, setMessage] = useState<string>("");

  const dataLoaded = useRef(false);

  // Funksion ndihmës për të marrë datën e sotme në formatin YYYY-MM-DD pa probleme Timezone
  const getLocalToday = () => {
    const now = new Date();
    const year = now.getFullYear();
    const month = String(now.getMonth() + 1).padStart(2, '0');
    const day = String(now.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  };

  const [activeFilters, setActiveFilters] = useState<Filters>({
    startDate: getLocalToday(),
    endDate: getLocalToday(),
  });

  const loadData = useCallback(async (filtersToUse: Filters) => {
    try {
      setLoading(true);
      setMessage("Loading summary...");

      // Ngarkojmë listat për dropdowns vetëm herën e parë
      if (!dataLoaded.current) {
        const [sh, ca] = await Promise.all([getShops(), getCashiers()]);
        setShops(sh);
        setCashiers(ca);
        dataLoaded.current = true;
      }

      const res = await getInvoiceSummary(filtersToUse);

      if (!res || res.length === 0) {
        setSummary([]);
        setMessage("No summary found for selected period.");
      } else {
        setSummary(res);
        setMessage(`Showing ${res.length} record(s).`);
      }
    } catch (err) {
      console.error("Error:", err);
      setMessage("Failed to load summary. Please check your connection.");
    } finally {
      setLoading(false);
    }
  }, []);

  // Ngarkimi fillestar
  useEffect(() => {
    loadData(activeFilters);
  }, [loadData]);

  const handleApplyFilters = (newFilters: Filters) => {
  // Sigurohemi që datat nuk janë boshe
  const filtersToApply = {
    ...newFilters,
    startDate: newFilters.startDate || getLocalToday(),
    endDate: newFilters.endDate || getLocalToday(),
  };
  
  setActiveFilters(filtersToApply);
  loadData(filtersToApply);
};

  return (
    <div className="p-6 bg-gray-50 min-h-screen">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Financial Summary</h1>
        <p className="text-gray-500">Overview of sales and invoices by date</p>
      </div>

      <InvoiceFilters 
        shops={shops} 
        cashiers={cashiers} 
        onApplyFilters={handleApplyFilters} 
      />

      <div className="mt-8">
        <div className={`mb-4 text-sm font-medium ${summary.length === 0 && !loading ? 'text-red-500' : 'text-gray-600'}`}>
          {message}
        </div>

        <div className="bg-white rounded-xl shadow-sm overflow-hidden border border-gray-100">
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-100">
                  <th className="px-6 py-4 text-sm font-semibold text-gray-700">Date</th>
                  <th className="px-6 py-4 text-sm font-semibold text-gray-700">Shop</th>
                  <th className="px-6 py-4 text-sm font-semibold text-gray-700">Cashier</th>
                  <th className="px-6 py-4 text-sm font-semibold text-gray-700 text-center">Invoices</th>
                  <th className="px-6 py-4 text-sm font-semibold text-gray-700 text-right">Total Amount</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {loading ? (
                  <tr>
                    <td colSpan={5} className="px-6 py-12 text-center">
                      <div className="flex justify-center items-center gap-2">
                        <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-blue-600"></div>
                        <span className="text-gray-500">Fetching records...</span>
                      </div>
                    </td>
                  </tr>
                ) : summary.length === 0 ? (
                  <tr>
                    <td colSpan={5} className="px-6 py-12 text-center text-gray-400 italic">
                      No data available for the selected filters.
                    </td>
                  </tr>
                ) : (
                  summary.map((item, idx) => (
                    <tr key={idx} className="hover:bg-blue-50/30 transition-colors">
                      <td className="px-6 py-4 text-sm text-gray-600">
                        {new Date(item.date).toLocaleDateString('sq-AL')}
                      </td>
                      <td className="px-6 py-4 text-sm font-medium text-gray-800">{item.shopName}</td>
                      <td className="px-6 py-4 text-sm text-gray-600">{item.cashierName}</td>
                      <td className="px-6 py-4 text-sm text-center">
                        <span className="bg-blue-50 text-blue-700 px-2.5 py-0.5 rounded-full text-xs font-bold border border-blue-100">
                          {item.invoiceCount}
                        </span>
                      </td>
                      <td className="px-6 py-4 text-sm font-bold text-blue-600 text-right">
                        € {item.totalAmount.toFixed(2)}
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
};

export default InvoiceSummaryPage;