import { useState, useEffect } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../components/ui/table";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

interface PosProduct {
  id: string;
  name: string;
  barcode: string;
  price: number;
  quantity: number;
}

export default function PosTable() {
  const { user } = useAuth();
  const [search, setSearch] = useState("");
  const [products, setProducts] = useState<PosProduct[]>([]);
  const [amountPaid, setAmountPaid] = useState<number>(0);

  // 🔹 Key unik për çdo cashier
  const storageKey = user ? `pos_products_${user.id}` : "pos_products_guest";

  // 🔹 Ngarko produktet nga localStorage kur hapet komponenti
  useEffect(() => {
    const saved = localStorage.getItem(storageKey);
    if (saved) {
      setProducts(JSON.parse(saved));
    }
  }, [storageKey]);

  // 🔹 Ruaj produktet në localStorage sa herë që ndryshojnë
  useEffect(() => {
    localStorage.setItem(storageKey, JSON.stringify(products));
  }, [products, storageKey]);

  const total = products.reduce((acc, p) => acc + p.price * p.quantity, 0);
  const change = amountPaid - total;

  const handleSearch = async () => {
    if (!search.trim()) return;
    try {
      const res = await api.get(`/api/pos/products/${search}`);
      const productData = res.data;

      if (!productData?.id) { alert("Produkti nuk u gjet"); return; }

      setProducts((prev) => {
        const existing = prev.find((p) => p.id === productData.id);
        if (existing) {
          return prev.map((p) => p.id === productData.id ? { ...p, quantity: p.quantity + 1 } : p);
        }
        return [...prev, { ...productData, quantity: 1 }];
      });
      setSearch("");
    } catch (err: any) {
      alert(err.response?.data || "Produkti nuk u gjet");
    }
  };

  const handleQuantityChange = (id: string, value: number) => {
    if (value <= 0) {
      alert("Sasia duhet të jetë > 0");
      return;
    }
    setProducts((prev) => prev.map((p) => p.id === id ? { ...p, quantity: value } : p));
  };

  const handleRemove = (id: string) => setProducts((prev) => prev.filter((p) => p.id !== id));

  const handleCheckout = async () => {
    if (!user) { alert("Nuk jeni i kyçur"); return; }
    if (products.length === 0) { alert("Asnjë produkt nuk është shtuar"); return; }
    try {
      const res = await api.post("/api/pos/checkout", {
        items: products.map((p) => ({ barcode: p.barcode, quantity: p.quantity })),
        amountPaid,
      });
      alert(`Kusuri: €${res.data.change.toFixed(2)}`);
      setProducts([]);
      setAmountPaid(0);
      localStorage.removeItem(storageKey); // 🔹 pastro listën pas checkout
    } catch (err: any) {
      alert(err.response?.data || "Checkout dështoi");
    }
  };

  return (
    <div className="flex gap-6">
      <div className="flex-1">
        <div className="flex gap-2 mb-4">
          <input
            type="text"
            placeholder="Skano barkod "
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            className="border p-2 rounded flex-1"
          />
          <button onClick={handleSearch} className="bg-blue-500 text-white px-4 rounded">Shto</button>
        </div>

        <div className="overflow-x-auto rounded-xl border border-gray-200 bg-white p-2">
          <Table>
            <TableHeader className="border-b border-gray-100">
              <TableRow>
                <TableCell isHeader>Barkod</TableCell>
                <TableCell isHeader>Emri</TableCell>
                <TableCell isHeader>Çmimi</TableCell>
                <TableCell isHeader>Sasia</TableCell>
                <TableCell isHeader>Nëntotali</TableCell>
                <TableCell isHeader>Veprim</TableCell>
              </TableRow>
            </TableHeader>
            <TableBody>
              {products.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>{p.barcode}</TableCell>
                  <TableCell>{p.name}</TableCell>
                  <TableCell>€{p.price.toFixed(2)}</TableCell>
                  <TableCell>
                    <div
                      className="cursor-pointer"
                      onDoubleClick={() => {
                        const newQty = prompt("Sasia e re:", p.quantity.toString());
                        if (newQty) handleQuantityChange(p.id, parseInt(newQty));
                      }}
                    >
                      {p.quantity}
                    </div>
                  </TableCell>
                  <TableCell>€{(p.price * p.quantity).toFixed(2)}</TableCell>
                  <TableCell>
                    <button onClick={() => handleRemove(p.id)} className="text-red-500 hover:text-red-700">Hiq</button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>

      <div className="w-64 flex flex-col gap-4 p-4 border rounded bg-white">
        <div className="text-2xl font-bold">Totali: €{total.toFixed(2)}</div>
        <div>
          <label className="block mb-1 text-sm font-medium">Shuma e paguar</label>
          <input
            type="number"
            value={amountPaid}
            onChange={(e) => setAmountPaid(parseFloat(e.target.value) || 0)}
            className="border p-2 rounded w-full"
          />
        </div>
        <div className="text-xl font-semibold">
          Kusuri: €{change >= 0 ? change.toFixed(2) : "0.00"}
        </div>
        <button onClick={handleCheckout} className="bg-green-500 text-white py-2 rounded mt-4 hover:bg-green-600">
          Paguaj
        </button>
      </div>
    </div>
  );
}
