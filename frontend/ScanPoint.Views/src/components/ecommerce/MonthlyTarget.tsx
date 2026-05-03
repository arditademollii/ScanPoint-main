import Chart from "react-apexcharts";
import { ApexOptions } from "apexcharts";
import { useState, useEffect } from "react";
import Dropdown from "../ui/dropdown/Dropdown";
import { DropdownItem } from "../ui/dropdown/DropdownItem";
import { MoreDotIcon } from "../../icons";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

/* ===================== TYPES ===================== */
interface PeakHourDto {
  hour: number;
  totalInvoices: number;
  totalRevenue: number;
}

/* ===================== COMPONENT ===================== */
export default function DashboardAnalytics() {
  const { user, isAuthenticated } = useAuth();

  /* ===================== BEST SELLER ===================== */
  const [bestSellerSeries, setBestSellerSeries] = useState<number[]>([0]);
  const [bestSeller, setBestSeller] = useState<any>(null);
  const [isBestOpen, setIsBestOpen] = useState(false);

  /* ===================== PEAK HOURS ===================== */
  const [peakSeries, setPeakSeries] = useState<any[]>([]);
  const [categories, setCategories] = useState<string[]>([]);
  const [peakHour, setPeakHour] = useState<PeakHourDto | null>(null);
  const [isPeakOpen, setIsPeakOpen] = useState(false);

  /* ===================== BEST SELLER FETCH ===================== */
  useEffect(() => {
    const fetchBestSeller = async () => {
      if (!isAuthenticated || !user?.accessToken) return;

      try {
        const today = new Date();
        const start = new Date(today);
        start.setDate(today.getDate() - 7);

        const res = await api.get(
          `/api/invoice/best-sellers?start=${start.toISOString()}&end=${today.toISOString()}`,
          {
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          }
        );

        const data = res.data;

        if (data.length > 0) {
          const topProduct = data[0];
          setBestSeller(topProduct);

          const totalQty = data.reduce(
            (sum: number, p: any) => sum + p.totalQuantitySold,
            0
          );

          const percent =
            totalQty > 0
              ? (topProduct.totalQuantitySold / totalQty) * 100
              : 0;

          setBestSellerSeries([parseFloat(percent.toFixed(2))]);
        }
      } catch (err) {
        console.error("Error fetching best sellers:", err);
      }
    };

    fetchBestSeller();
  }, [isAuthenticated, user]);

  /* ===================== PEAK HOURS FETCH ===================== */
  useEffect(() => {
    const fetchPeakHours = async () => {
      if (!isAuthenticated || !user?.accessToken) return;

      try {
        const today = new Date();
        const start = new Date(today);
        start.setDate(today.getDate() - 7);

        const res = await api.get(
          `/api/Invoice/peak-hours?start=${start.toISOString()}&end=${today.toISOString()}`,
          {
            headers: {
              Authorization: `Bearer ${user.accessToken}`,
            },
          }
        );

        const data: PeakHourDto[] = res.data;

        const sorted = [...data].sort((a, b) => a.hour - b.hour);

        const labels = sorted.map(
          (x) =>
            `${x.hour.toString().padStart(2, "0")}:00 - ${(x.hour + 1)
              .toString()
              .padStart(2, "0")}:00`
        );

        setCategories(labels);

        setPeakSeries([
          {
            name: "Invoices",
            data: sorted.map((x) => x.totalInvoices),
          },
          {
            name: "Revenue (€)",
            data: sorted.map((x) => x.totalRevenue),
          },
        ]);

        const top = sorted.reduce((max, item) =>
          item.totalInvoices > max.totalInvoices ? item : max
        );

        setPeakHour(top);
      } catch (err) {
        console.error("Error fetching peak hours:", err);
      }
    };

    fetchPeakHours();
  }, [isAuthenticated, user]);

  /* ===================== BEST SELLER OPTIONS ===================== */
  const bestSellerOptions: ApexOptions = {
    colors: ["#465FFF"],
    chart: {
      fontFamily: "Outfit, sans-serif",
      type: "radialBar",
      height: 330,
      sparkline: { enabled: true },
    },
    plotOptions: {
      radialBar: {
        startAngle: -85,
        endAngle: 85,
        hollow: { size: "80%" },
        track: {
          background: "#E4E7EC",
          strokeWidth: "100%",
          margin: 5,
        },
        dataLabels: {
          name: { show: false },
          value: {
            fontSize: "36px",
            fontWeight: "600",
            offsetY: -40,
            color: "#1D2939",
            formatter: (val) => `${val}%`,
          },
        },
      },
    },
    fill: { type: "solid", colors: ["#465FFF"] },
    stroke: { lineCap: "round" },
    labels: ["Progress"],
  };

  /* ===================== PEAK HOURS OPTIONS ===================== */
  const peakOptions: ApexOptions = {
    chart: {
      type: "bar",
      toolbar: { show: false },
      fontFamily: "Outfit, sans-serif",
    },
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: "65%",
        borderRadius: 6,
      },
    },
    dataLabels: { enabled: false },
    stroke: { width: 2 },
    xaxis: { categories },
    yaxis: [
      { title: { text: "Invoices" } },
      { opposite: true, title: { text: "Revenue (€)" } },
    ],
    legend: { position: "top" },
    tooltip: {
      shared: true,
      intersect: false,
    },
  };

  return (
    <div className="space-y-6">

      {/* ================= BEST SELLER ================= */}
      <div className="rounded-2xl border border-gray-200 bg-gray-100 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="px-5 pt-5 bg-white shadow-default rounded-2xl pb-11 dark:bg-gray-900 sm:px-6 sm:pt-6">
          <div className="flex justify-between">
            <div>
              <h3 className="text-lg font-semibold text-gray-800 dark:text-white/90">
                Weekly Best Seller
              </h3>
              <p className="mt-1 text-gray-500 text-theme-sm dark:text-gray-400">
                Produkti më i shitur këtë javë
              </p>
            </div>

            <button onClick={() => setIsBestOpen(!isBestOpen)}>
              <MoreDotIcon className="text-gray-400 size-6" />
            </button>
          </div>

          <Chart
            options={bestSellerOptions}
            series={bestSellerSeries}
            type="radialBar"
            height={330}
          />

          {bestSeller && (
            <>
              <span className="block text-center mt-2 text-sm text-gray-500">
                {bestSeller.productName} ({bestSeller.totalQuantitySold} units)
              </span>

              <p className="text-center mt-4 text-sm text-gray-500">
                Totali: {bestSeller.totalRevenue.toFixed(2)} €
              </p>
            </>
          )}
        </div>
      </div>

      {/* ================= PEAK HOURS ================= */}
      <div className="rounded-2xl border border-gray-200 bg-gray-100 dark:border-gray-800 dark:bg-white/[0.03]">
        <div className="px-5 pt-5 bg-white shadow-default rounded-2xl pb-11 dark:bg-gray-900 sm:px-6 sm:pt-6">
          <div className="flex justify-between">
            <div>
              <h3 className="text-lg font-semibold">
                Peak Hours Analytics
              </h3>
              <p className="text-gray-500 text-sm">
                Orët më të ngarkuara të shitjeve
              </p>
            </div>

            <button onClick={() => setIsPeakOpen(!isPeakOpen)}>
              <MoreDotIcon className="text-gray-400 size-6" />
            </button>
          </div>

          <div className="mt-6">
            <Chart
              options={peakOptions}
              series={peakSeries}
              type="bar"
              height={350}
            />
          </div>

          {peakHour && (
            <div className="text-center mt-6">
              <p className="font-semibold">
                Peak:{" "}
                {peakHour.hour.toString().padStart(2, "0")}:00 -{" "}
                {(peakHour.hour + 1).toString().padStart(2, "0")}:00
              </p>
              <p className="text-sm text-gray-500">
                {peakHour.totalInvoices} invoices •{" "}
                {peakHour.totalRevenue.toFixed(2)} €
              </p>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}