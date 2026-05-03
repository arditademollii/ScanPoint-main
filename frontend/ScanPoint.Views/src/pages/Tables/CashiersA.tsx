import { useState, useEffect } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../components/ui/table";
import api from "../../api/axiosInstance";

interface Cashier {
  id: string;
  username: string;
  email: string;
  managerUsername: string;
  shopName: string;

  // soft delete fields
  isDeleted: boolean;
  deletedAt?: string;
  managerIsDeleted: boolean;
  shopIsDeleted: boolean;
}

interface Manager {
  id: string;
  username: string;
  shopId: string;
}

interface Shop {
  id: string;
  name: string;
}

export default function CashiersA() {
  const [cashiers, setCashiers] = useState<Cashier[]>([]);
  const [shops, setShops] = useState<Shop[]>([]);
  const [managers, setManagers] = useState<Manager[]>([]);
  const [filteredCashiers, setFilteredCashiers] = useState<Cashier[]>([]);
  const [selectedShop, setSelectedShop] = useState("all");
  const [selectedManager, setSelectedManager] = useState("all");
  const [showDeleted, setShowDeleted] = useState(false);
  const [restoreError, setRestoreError] = useState<string | null>(null);

  // ============================
  // FETCH ALL DATA
  // ============================
  const fetchAll = async () => {
    try {
      const [shopsRes, managersRes, cashiersRes] = await Promise.all([
        api.get("/api/Shops/my-shops"),
        api.get("/api/Manager"),
        api.get("/api/Cashier/all"), // përfshin edhe deleted
      ]);

      setShops(shopsRes.data);
      setManagers(managersRes.data);

      const all: Cashier[] = cashiersRes.data;
      setCashiers(all);
      setFilteredCashiers(all.filter((c) => !c.isDeleted));
    } catch (err) {
      console.error("Fetch failed", err);
    }
  };

  useEffect(() => {
    fetchAll();
  }, []);

  // ============================
  // FILTER
  // ============================
  const handleFilterSubmit = () => {
    let filtered = cashiers.filter((c) => !c.isDeleted);

    if (selectedShop !== "all") {
      filtered = filtered.filter((c) => c.shopName === selectedShop);
    }

    if (selectedManager !== "all") {
      filtered = filtered.filter((c) => c.managerUsername === selectedManager);
    }

    setFilteredCashiers(filtered);
  };

  // ============================
  // RESTORE
  // ============================
  const handleRestore = async (id: string) => {
    setRestoreError(null);

    try {
      await api.post(`/api/Cashier/${id}/restore`);
      await fetchAll();
    } catch (err: any) {
      const msg = err.response?.data || "Rikthimi dështoi.";
      setRestoreError(typeof msg === "string" ? msg : msg.message);
    }
  };

  // ============================
  // FILTER MANAGERS BASED ON SHOP
  // ============================
  const managersForSelectedShop =
    selectedShop === "all"
      ? managers
      : managers.filter(
          (m) =>
            cashiers.find((c) => c.managerUsername === m.username)?.shopName === selectedShop
        );

  const deletedCashiers = cashiers.filter((c) => c.isDeleted);

  return (
    <div>
      {/* ============================ */}
      {/* FILTER SECTION */}
      {/* ============================ */}
      <div className="flex gap-4 mb-4 flex-wrap">
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Shop</label>
          <select
            value={selectedShop}
            onChange={(e) => {
              setSelectedShop(e.target.value);
              setSelectedManager("all");
            }}
            className="border rounded px-2 py-1"
          >
            <option value="all">All</option>
            {shops.map((s) => (
              <option key={s.id} value={s.name}>
                {s.name}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700 mb-1">Manager</label>
          <select
            value={selectedManager}
            onChange={(e) => setSelectedManager(e.target.value)}
            className="border rounded px-2 py-1"
          >
            <option value="all">All</option>
            {managersForSelectedShop.map((m) => (
              <option key={m.id} value={m.username}>
                {m.username}
              </option>
            ))}
          </select>
        </div>

        <button
          onClick={handleFilterSubmit}
          className="px-4 py-1 bg-blue-500 text-white rounded self-end"
        >
          Filter
        </button>

        {deletedCashiers.length > 0 && (
          <button
            onClick={() => setShowDeleted(!showDeleted)}
            className="px-4 py-1 text-sm text-blue-500 font-medium self-end ml-auto"
          >
            {showDeleted
              ? "Fshih të fshirat"
              : `Shfaq të fshirat (${deletedCashiers.length})`}
          </button>
        )}
      </div>

      {/* ============================ */}
      {/* ERROR */}
      {/* ============================ */}
      {restoreError && (
        <div className="mb-4 p-3 rounded bg-red-100 text-red-700 text-sm">
          ⚠️ {restoreError}
        </div>
      )}

      {/* ============================ */}
      {/* ACTIVE CASHIERS TABLE */}
      {/* ============================ */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableCell isHeader>User</TableCell>
                <TableCell isHeader>Email</TableCell>
                <TableCell isHeader>Shop</TableCell>
                <TableCell isHeader>Manager</TableCell>
              </TableRow>
            </TableHeader>

            <TableBody>
              {filteredCashiers.map((c) => (
                <TableRow key={c.id}>
                  <TableCell>{c.username}</TableCell>
                  <TableCell>{c.email}</TableCell>
                  <TableCell>{c.shopName}</TableCell>
                  <TableCell>{c.managerUsername}</TableCell>
                </TableRow>
              ))}

              {filteredCashiers.length === 0 && (
                <TableRow>
                  <TableCell colSpan={4} className="text-center py-4">
                    Asnjë cashier aktiv.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </div>

      {/* ============================ */}
      {/* DELETED CASHIERS */}
      {/* ============================ */}
      {showDeleted && deletedCashiers.length > 0 && (
        <div className="mt-6 border-t pt-6">
          <h4 className="mb-4 font-semibold text-gray-500">
            Cashierët e fshirë
          </h4>

          <div className="space-y-3">
            {deletedCashiers.map((c) => {
              const blocked = c.managerIsDeleted || c.shopIsDeleted;

              return (
                <div
                  key={c.id}
                  className="flex items-center justify-between p-4 border rounded bg-red-50"
                >
                  <div>
                    <p className="line-through">{c.username}</p>
                    <p className="text-xs">{c.email}</p>
                    <p className="text-xs">
                      Shop: {c.shopName} | Manager: {c.managerUsername}
                    </p>

                    {c.deletedAt && (
                      <p className="text-xs text-red-400">
                        Fshirë: {new Date(c.deletedAt).toLocaleDateString("sq-AL")}
                      </p>
                    )}
                  </div>

                  <button
                    onClick={() => handleRestore(c.id)}
                    disabled={blocked}
                    className={`px-3 py-1 rounded ${
                      blocked
                        ? "bg-gray-200 text-gray-400"
                        : "bg-green-500 text-white"
                    }`}
                  >
                    {blocked ? "E bllokuar" : "Rikthe"}
                  </button>
                </div>
              );
            })}
          </div>
        </div>
      )}
    </div>
  );
}