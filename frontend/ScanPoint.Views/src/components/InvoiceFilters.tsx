import React, { useState, useEffect } from "react";

interface Shop {
  id: string;
  name: string;
}

interface Cashier {
  id: string;
  username: string;
}

export interface Filters {
  shopId?: string;
  cashierId?: string;
  shopName?: string;    // Për InvoicesPage
  cashierName?: string; // Për InvoicesPage
  startDate?: string;
  endDate?: string;
}

interface Props {
  shops: Shop[];
  cashiers: Cashier[];
  onApplyFilters: (filters: Filters) => void;
  initialFilters?: Filters; // Lejon SummaryPage të vendosë datat "Sot" që në fillim
}

const InvoiceFilters: React.FC<Props> = ({ shops, cashiers, onApplyFilters, initialFilters }) => {
  const [filters, setFilters] = useState<Filters>(initialFilters || {});

  // Përditësohet nëse initialFilters ndryshon nga jashtë
  useEffect(() => {
    if (initialFilters) setFilters(initialFilters);
  }, [initialFilters]);

  const updateFilters = (newFields: Partial<Filters>) => {
    setFilters((prev) => ({ ...prev, ...newFields }));
  };

  const handleApply = () => {
    onApplyFilters(filters);
  };

  const handleReset = () => {
    const empty = {};
    setFilters(empty);
    onApplyFilters(empty);
  };

  return (
    <div className="flex flex-wrap items-end gap-4 bg-white p-4 rounded-xl shadow-sm border border-gray-100">
      {/* SHOP */}
      <div className="flex flex-col">
        <label className="text-sm font-medium text-gray-700 mb-1">Shop</label>
        <select
          className="border rounded-lg p-2 w-48 focus:ring-2 focus:ring-blue-500 outline-none"
          value={filters.shopId || ""}
          onChange={(e) => {
            const selectedShop = shops.find(s => s.id === e.target.value);
            updateFilters({ 
              shopId: e.target.value || undefined,
              shopName: selectedShop?.name || undefined // Jep emrin për Frontend filter
            });
          }}
        >
          <option value="">All Shops</option>
          {shops.map((shop) => (
            <option key={shop.id} value={shop.id}>{shop.name}</option>
          ))}
        </select>
      </div>

      {/* CASHIER */}
      <div className="flex flex-col">
        <label className="text-sm font-medium text-gray-700 mb-1">Cashier</label>
        <select
          className="border rounded-lg p-2 w-48 focus:ring-2 focus:ring-blue-500 outline-none"
          value={filters.cashierId || ""}
          onChange={(e) => {
            const selectedCashier = cashiers.find(c => c.id === e.target.value);
            updateFilters({ 
              cashierId: e.target.value || undefined,
              cashierName: selectedCashier?.username || undefined // Jep username-in për Frontend filter
            });
          }}
        >
          <option value="">All Cashiers</option>
          {cashiers.map((c) => (
            <option key={c.id} value={c.id}>{c.username}</option>
          ))}
        </select>
      </div>

      {/* START DATE */}
      <div className="flex flex-col">
        <label className="text-sm font-medium text-gray-700 mb-1">Start Date</label>
        <input
          type="date"
          className="border rounded-lg p-2 focus:ring-2 focus:ring-blue-500 outline-none text-gray-700"
          value={filters.startDate || ""}
          onChange={(e) => updateFilters({ startDate: e.target.value || undefined })}
        />
      </div>

      {/* END DATE */}
      <div className="flex flex-col">
        <label className="text-sm font-medium text-gray-700 mb-1">End Date</label>
        <input
          type="date"
          className="border rounded-lg p-2 focus:ring-2 focus:ring-blue-500 outline-none text-gray-700"
          value={filters.endDate || ""}
          onChange={(e) => updateFilters({ endDate: e.target.value || undefined })}
        />
      </div>

      {/* ACTIONS */}
      <div className="flex gap-2">
        <button
          className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg font-medium transition-all active:scale-95"
          onClick={handleApply}
        >
          Apply
        </button>
        <button
          className="bg-gray-100 hover:bg-gray-200 text-gray-600 px-5 py-2 rounded-lg font-medium transition-all active:scale-95"
          onClick={handleReset}
        >
          Reset
        </button>
      </div>
    </div>
  );
};

export default InvoiceFilters;