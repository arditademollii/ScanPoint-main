import {
  ArrowUpIcon,
  BoxIconLine,
  GroupIcon,
} from "../../icons";
import Badge from "../ui/badge/Badge";
import { useEffect, useState } from "react";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

interface Shop {
  id: string;
  name: string;
}

export default function ShopAndInvoiceMetrics() {
  const [shopCount, setShopCount] = useState<number>(0);
  const [shops, setShops] = useState<Shop[]>([]);
  const [invoiceCounts, setInvoiceCounts] = useState<Record<string, number>>({});
  const { user, isAuthenticated } = useAuth();

  useEffect(() => {
    const fetchData = async () => {
      try {
        // 1. Merr shop-et
        const shopRes = await api.get("/api/Shops/my-shops", {
          headers: { Authorization: `Bearer ${user?.accessToken}` },
        });
        const shopList: Shop[] = shopRes.data;
        setShops(shopList);
        setShopCount(shopList.length);

        // 2. Për secilin shop merr summary për sot
        const today = new Date();
        const start = today.toISOString().split("T")[0]; // YYYY-MM-DD
        const end = start; // vetëm sot

        const counts: Record<string, number> = {};
        for (const shop of shopList) {
          const invRes = await api.get(
            `/api/Invoice/summary?start=${start}&end=${end}&shopId=${shop.id}`,
            { headers: { Authorization: `Bearer ${user?.accessToken}` } }
          );
          // summary kthen një array me total invoices dhe revenue
          // supozojmë që ka një fushë "invoiceCount"
          const summary = invRes.data;
          counts[shop.id] = summary.length > 0 ? summary[0].invoiceCount : 0;
        }
        setInvoiceCounts(counts);
      } catch (err) {
        console.error("Error fetching shops/invoices:", err);
      }
    };

    if (isAuthenticated && user?.accessToken) {
      fetchData();
    }
  }, [isAuthenticated, user]);

  return (
    <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 md:gap-6">
      {/* Total Shops metric */}
      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] md:p-6">
        <div className="flex items-center justify-center w-12 h-12 bg-gray-100 rounded-xl dark:bg-gray-800">
          <GroupIcon className="text-gray-800 size-6 dark:text-white/90" />
        </div>
        <div className="flex items-end justify-between mt-5">
          <div>
            <span className="text-sm text-gray-500 dark:text-gray-400">
              Total Shops
            </span>
            <h4 className="mt-2 font-bold text-gray-800 text-title-sm dark:text-white/90">
              {shopCount}
            </h4>
          </div>
          <Badge color="success">
            <ArrowUpIcon />
          </Badge>
        </div>
      </div>

      {/* Invoices Today per Shop */}
      {shops.map((shop) => (
        <div
          key={shop.id}
          className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] md:p-6"
        >
          <div className="flex items-center justify-center w-12 h-12 bg-gray-100 rounded-xl dark:bg-gray-800">
            <BoxIconLine className="text-gray-800 size-6 dark:text-white/90" />
          </div>
          <div className="flex items-end justify-between mt-5">
            <div>
              <span className="text-sm text-gray-500 dark:text-gray-400">
                {shop.name} – Invoices Today
              </span>
              <h4 className="mt-2 font-bold text-gray-800 text-title-sm dark:text-white/90">
                {invoiceCounts[shop.id] ?? 0}
              </h4>
            </div>
            <Badge color="success">
              <ArrowUpIcon />
            </Badge>
          </div>
        </div>
      ))}
    </div>
  );
}
