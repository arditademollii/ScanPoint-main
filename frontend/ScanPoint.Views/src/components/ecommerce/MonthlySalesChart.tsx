import Chart from "react-apexcharts";
import { ApexOptions } from "apexcharts";
import { useState, useEffect } from "react";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

export default function CombinedDailyCharts() {
  const { user, isAuthenticated } = useAuth();

  const [totalSeries, setTotalSeries] = useState<{ name: string; data: number[] }[]>([]);
  const [shopSeries, setShopSeries] = useState<{ name: string; data: number[] }[]>([]);
  const [categories, setCategories] = useState<string[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      if (!isAuthenticated || !user?.accessToken) return;

      try {
        const today = new Date();
        const year = today.getFullYear();
        const month = today.getMonth(); // 0-based
        const daysInMonth = new Date(year, month + 1, 0).getDate();

        const dayLabels: string[] = [];
        const dailyTotals: number[] = [];
        const shopTotals: Record<string, number[]> = {};

        for (let day = 1; day <= daysInMonth; day++) {
          const date = new Date(year, month, day);
          const start = `${year}-${String(month + 1).padStart(2, "0")}-${String(day).padStart(2, "0")}`;
          const end = start;

          const res = await api.get(
            `/api/Invoice/summary?start=${start}&end=${end}`,
            { headers: { Authorization: `Bearer ${user.accessToken}` } }
          );

          const summary = res.data as {
            shopName: string;
            totalAmount: number;
          }[];

          dayLabels.push(`${day}`);

          // mbledhim totalin e krejt shopave për këtë ditë
          const totalAmountAllShops = summary.reduce(
            (sum, s) => sum + (s.totalAmount || 0),
            0
          );
          dailyTotals.push(totalAmountAllShops);

          // ruajmë vlerat sipas shop-it
          for (const s of summary) {
            if (!shopTotals[s.shopName]) {
              shopTotals[s.shopName] = Array(daysInMonth).fill(0);
            }
            shopTotals[s.shopName][day - 1] = s.totalAmount || 0;
          }
        }

        setCategories(dayLabels);
        setTotalSeries([{ name: "Total Revenue (All Shops)", data: dailyTotals }]);
        setShopSeries(
          Object.keys(shopTotals).map((shopName) => ({
            name: shopName,
            data: shopTotals[shopName],
          }))
        );
      } catch (err) {
        console.error("Error fetching daily sales:", err);
      }
    };

    fetchData();
  }, [isAuthenticated, user]);

  const totalOptions: ApexOptions = {
    chart: { type: "bar", toolbar: { show: false } },
    plotOptions: { bar: { horizontal: false, columnWidth: "50%" } },
    dataLabels: { enabled: false },
    xaxis: { categories, title: { text: "Day of Month" } },
    yaxis: { title: { text: "Revenue (€)" } },
    legend: { position: "top", horizontalAlign: "left" },
    tooltip: { y: { formatter: (val: number) => `${val.toFixed(2)} €` } },
  };

  const shopOptions: ApexOptions = {
    chart: { type: "bar", stacked: false, toolbar: { show: false } },
    plotOptions: { bar: { horizontal: false, columnWidth: "50%" } },
    dataLabels: { enabled: false },
    xaxis: { categories, title: { text: "Day of Month" } },
    yaxis: { title: { text: "Revenue (€)" } },
    legend: { position: "top", horizontalAlign: "left" },
    tooltip: { y: { formatter: (val: number) => `${val.toFixed(2)} €` } },
  };

  return (
    <div className="space-y-8">
      {/* Chart 1: Total Revenue All Shops */}
      <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white px-5 pt-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90 mb-4">
          Daily Revenue (All Shops)
        </h3>
        <Chart options={totalOptions} series={totalSeries} type="bar" height={300} />
      </div>

      {/* Chart 2: Revenue by Shop */}
      <div className="overflow-hidden rounded-2xl border border-gray-200 bg-white px-5 pt-5 dark:border-gray-800 dark:bg-white/[0.03]">
        <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90 mb-4">
          Daily Revenue by Shop
        </h3>
        <Chart options={shopOptions} series={shopSeries} type="bar" height={400} />
      </div>
    </div>
  );
}
