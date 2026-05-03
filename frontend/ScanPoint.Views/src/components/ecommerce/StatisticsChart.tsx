import Chart from "react-apexcharts";
import { ApexOptions } from "apexcharts";
import { useEffect, useState } from "react";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

export default function CashierPerformanceChart() {
  const { user, isAuthenticated } = useAuth();

  const [series, setSeries] = useState<{ name: string; data: number[] }[]>([]);
  const [categories, setCategories] = useState<string[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      if (!isAuthenticated || !user?.accessToken) return;

      try {
        const today = new Date();

        // 👉 Marrim 7 ditët e fundit
        const days: Date[] = [];
        for (let i = 6; i >= 0; i--) {
          const d = new Date();
          d.setDate(today.getDate() - i);
          days.push(d);
        }

        const dayLabels: string[] = [];
        const cashierMap: Record<string, number[]> = {};

        for (let i = 0; i < days.length; i++) {
          const date = days[i];

          const start = `${date.toISOString().split("T")[0]}T00:00:00`;
          const end = `${date.toISOString().split("T")[0]}T23:59:59`;

          const res = await api.get(
            `/api/Invoice/cashier-performance?start=${start}&end=${end}`,
            {
              headers: {
                Authorization: `Bearer ${user.accessToken}`,
              },
            }
          );

          const data = res.data as {
            cashierName: string;
            totalRevenue: number;
          }[];

          // 👉 Emrat e ditëve të javës
          const dayName = date.toLocaleDateString("en-US", {
            weekday: "short",
          });

          dayLabels.push(dayName);

          data.forEach((c) => {
            if (!cashierMap[c.cashierName]) {
              cashierMap[c.cashierName] = Array(7).fill(0);
            }

            cashierMap[c.cashierName][i] = c.totalRevenue || 0;
          });
        }

        setCategories(dayLabels);

        const chartSeries = Object.keys(cashierMap).map((name) => ({
          name,
          data: cashierMap[name],
        }));

        setSeries(chartSeries);
      } catch (err) {
        console.error("Error fetching cashier performance:", err);
      }
    };

    fetchData();
  }, [isAuthenticated, user]);

  const options: ApexOptions = {
    chart: {
      type: "bar",
      toolbar: { show: false },
    },
    plotOptions: {
      bar: {
        horizontal: true,
        barHeight: "80%", // 🔥 më të trasha (rrite nga 60% → 80%)
      },
    },
    dataLabels: {
      enabled: false,
    },
    xaxis: {
      categories: categories,
      title: {
        text: "Revenue (€)",
      },
    },
    yaxis: {
      title: {
        text: "Days (Last 7 Days)",
      },
    },
    legend: {
      position: "top",
      horizontalAlign: "left",
    },
    tooltip: {
      y: {
        formatter: (val: number) => `${val.toFixed(2)} €`,
      },
    },
  };

  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03]">
      <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90 mb-4">
        Cashier Performance (Last 7 Days)
      </h3>

      <Chart options={options} series={series} type="bar" height={450} />
    </div>
  );
}