import { useState, useEffect } from "react";
import axios from "axios";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../ui/table";

// 🔹 Interface i produktit për POS
interface PosProduct {
  id: string;
  name: string;
  barcode: string;
  price: number;
  quantity: number;
}

export default function PosTable() {
  const [search, setSearch] = useState("");
  const [products, setProducts] = useState<PosProduct[]>([]);
  const [amountPaid, setAmountPaid] = useState<number>(0);
  const [userRole, setUserRole] = useState<string | null>(null);
  const [shopId, setShopId] = useState<string | null>(null);

  // 🔹 Totali dhe change
  const total = products.reduce((acc, p) => acc + (p.price || 0) * (p.quantity || 0), 0);
  const change = amountPaid - total;

  // 🔹 Axios defaults me token nga localStorage
  useEffect(() => {
    const token =
      localStorage.getItem("accessToken") || localStorage.getItem("token");
    if (!token) {
      alert("No token found. Please login.");
      window.location.href = "/login";
      return;
    }

    axios.defaults.baseURL = "http://import.meta.env.VITE_API_URL";
    axios.defaults.headers.common["Authorization"] = `Bearer ${token}`;
  }, []);

  // 🔹 Merr user info
  useEffect(() => {
    const fetchUser = async () => {
      try {
        const res = await axios.get("/api/User/me");
        // Kontrollo strukturen e të dhënave
        setUserRole(res.data.role || res.data.data?.role);
        setShopId(res.data.ShopId || res.data.data?.ShopId || null);
      } catch (err: any) {
        console.error("Cannot fetch user info:", err.response?.data || err);
        if (err.response?.status === 401 || err.response?.status === 403) {
          alert("Session expired. Please login again.");
          localStorage.removeItem("accessToken");
          localStorage.removeItem("token");
          window.location.href = "/login";
        } else {
          alert("Error fetching user info");
        }
      }
    };
    fetchUser();
  }, []);

  // 🔹 Search/Add product by barcode
  const handleSearch = async () => {
    if (!search) return;
    try {
      const res = await axios.get(`/api/pos/products/${search}`);
      const productData = res.data;

      if (!productData || !productData.id) {
        alert("Product not found");
        return;
      }

      const existing = products.find((p) => p.id === productData.id);
      if (existing) {
        setProducts(
          products.map((p) =>
            p.id === productData.id
              ? { ...p, quantity: (p.quantity || 0) + 1, price: p.price || 0 }
              : p
          )
        );
      } else {
        setProducts([
          ...products,
          { ...productData, quantity: 1, price: productData.price || 0 },
        ]);
      }

      setSearch("");
    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 401 || err.response?.status === 403) {
        alert("Session expired. Please login again.");
        localStorage.removeItem("accessToken");
        localStorage.removeItem("token");
        window.location.href = "/login";
      } else {
        alert(err.response?.data || "Product not found");
      }
    }
  };

  // 🔹 Inline edit quantity
  const handleQuantityChange = (id: string, value: number) => {
    if (value <= 0) return;
    setProducts(products.map((p) => (p.id === id ? { ...p, quantity: value } : p)));
  };

  // 🔹 Remove product
  const handleRemoveProduct = (id: string) => {
    setProducts(products.filter((p) => p.id !== id));
  };

  // 🔹 Checkout
  const handleCheckout = async () => {
    if (!userRole) {
      alert("User not authorized");
      return;
    }
    if (products.length === 0) {
      alert("No products added");
      return;
    }
    try {
      const res = await axios.post("/api/pos/checkout", {
        items: products.map((p) => ({ barcode: p.barcode, quantity: p.quantity })),
        amountPaid,
      });

      alert(`Change to return: $${res.data.change.toFixed(2)}`);
      setProducts([]);
      setAmountPaid(0);
    } catch (err: any) {
      console.error(err);
      if (err.response?.status === 401 || err.response?.status === 403) {
        alert("Session expired. Please login again.");
        localStorage.removeItem("accessToken");
        localStorage.removeItem("token");
        window.location.href = "/login";
      } else {
        alert(err.response?.data || "Checkout failed");
      }
    }
  };

  return (
    <div className="flex gap-6">
      {/* LEFT: Table + Search */}
      <div className="flex-1">
        <div className="flex gap-2 mb-4">
          <input
            type="text"
            placeholder="Scan barcode or enter product name"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && handleSearch()}
            className="border p-2 rounded flex-1"
          />
          <button onClick={handleSearch} className="bg-blue-500 text-white px-4 rounded">
            Adddddd
          </button>
        </div>

        <div className="overflow-x-auto rounded-xl border border-gray-200 bg-white p-2">
          <Table>
            <TableHeader className="border-b border-gray-100">
              <TableRow>
                <TableCell isHeader>Barcode</TableCell>
                <TableCell isHeader>Name</TableCell>
                <TableCell isHeader>Price</TableCell>
                <TableCell isHeader>Quantity</TableCell>
                <TableCell isHeader>SubTotal</TableCell>
                <TableCell isHeader>Action</TableCell>
              </TableRow>
            </TableHeader>

            <TableBody>
              {products.map((p) => (
                <TableRow key={p.id}>
                  <TableCell>{p.barcode}</TableCell>
                  <TableCell>{p.name}</TableCell>
                  <TableCell>${(p.price || 0).toFixed(2)}</TableCell>
                  <TableCell>
                    <div
                      className="cursor-pointer"
                      onDoubleClick={() => {
                        const newQty = prompt("Enter new quantity:", p.quantity.toString());
                        if (newQty) handleQuantityChange(p.id, parseInt(newQty));
                      }}
                    >
                      {p.quantity}
                    </div>
                  </TableCell>
                  <TableCell>${((p.price || 0) * (p.quantity || 0)).toFixed(2)}</TableCell>
                  <TableCell>
                    <button onClick={() => handleRemoveProduct(p.id)} className="text-red-500">
                      Remove
                    </button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>

      {/* RIGHT: Totals + Amount Paid */}
      <div className="w-64 flex flex-col gap-4 p-4 border rounded bg-white">
        <div className="text-2xl font-bold">Total: ${total.toFixed(2)}</div>
        <div>
          <label className="block mb-1">Amount Paid</label>
          <input
            type="number"
            value={amountPaid}
            onChange={(e) => setAmountPaid(parseFloat(e.target.value))}
            className="border p-2 rounded w-full"
          />
        </div>
        <div className="text-xl font-semibold">
          Change: ${change >= 0 ? change.toFixed(2) : "0.00"}
        </div>
        <button onClick={handleCheckout} className="bg-green-500 text-white py-2 rounded mt-4">
          Checkout
        </button>
      </div>
    </div>
  );
}
