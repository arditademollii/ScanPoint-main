import { useEffect, useState } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../ui/table";
import axios from "axios";

interface Product {
  id?: string;
  name: string;
  barcode: string;
  price: number;
  unit: string;
  quantity: number;
  expiryDate: string;
  category: number;
  shopId: string;
}

interface Shop {
  id: string;
  name: string;
}

export default function AdminProductsTable() {
  const [products, setProducts] = useState<Product[]>([]);
  const [shops, setShops] = useState<Shop[]>([]);
  const [selectedShop, setSelectedShop] = useState<string | "all">("all");
  const [searchQuery, setSearchQuery] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);

  const token = localStorage.getItem("accessToken");
  const role = localStorage.getItem("role");
  const shopIdClaim = localStorage.getItem("shopId");

  const fetchShops = async () => {
    try {
      const res = await axios.get<Shop[]>("http://import.meta.env.VITE_API_URL/api/Shops/my-shops", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setShops(res.data);
    } catch (err) {
      console.error("Error fetching shops:", err);
    }
  };

  const fetchProducts = async () => {
    try {
      const res = await axios.get<Product[]>("http://import.meta.env.VITE_API_URL/api/Products", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProducts(res.data);
    } catch (err) {
      console.error("Error fetching products:", err);
    } finally {
      setLoading(false);
    }
  };

  const filteredProducts = products
    .filter((p) => p.quantity === 0)
    .filter((p) =>
      searchQuery
        ? p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          p.barcode.toLowerCase().includes(searchQuery.toLowerCase())
        : true
    )
    .filter((p) => (selectedShop === "all" ? true : p.shopId === selectedShop));

  const deleteProduct = async (id: string) => {
    if (!window.confirm("A jeni të sigurt që dëshironi të fshini këtë produkt?")) return;
    try {
      await axios.delete(`http://import.meta.env.VITE_API_URL/api/Products/${id}`, {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      console.error("Error deleting product:", err);
    }
  };

  const openForm = (product: Product) => {
    setEditingProduct(product);
    setShowForm(true);
  };

  const closeForm = () => {
    setEditingProduct(null);
    setShowForm(false);
  };

  const submitForm = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!editingProduct) return;

    const form = e.currentTarget;
    const formData = new FormData(form);

    const productData: Product = {
      name: formData.get("name") as string,
      barcode: formData.get("barcode") as string,
      price: Number(formData.get("price")),
      unit: formData.get("unit") as string,
      quantity: Number(formData.get("quantity")),
      expiryDate: formData.get("expiryDate") as string,
      category: Number(formData.get("category")),
      shopId: role === "Manager" ? shopIdClaim! : (formData.get("shopId") as string),
      id: editingProduct.id,
    };

    try {
      await axios.put(`http://import.meta.env.VITE_API_URL/api/Products/${editingProduct.id}`, productData, {
        headers: { Authorization: `Bearer ${token}` },
      });
      fetchProducts();
      closeForm();
    } catch (err) {
      console.error("Error submitting product:", err);
    }
  };

  useEffect(() => {
    fetchShops();
    fetchProducts();
  }, []);

  if (loading) return <div className="p-6 text-center">Loading products...</div>;

  const showShopColumn = role === "Admin";

  return (
    <div className="p-4">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-red-600">Produktet jashtë gjendjes (Out of Stock)</h1>
        <p className="text-gray-500">Më poshtë janë produktet me sasi 0.</p>
      </div>

      {/* Filter & Search */}
      <div className="mb-4 flex flex-wrap items-center gap-4">
        {role === "Admin" && (
          <div className="flex items-center gap-2">
            <label className="text-sm font-medium">Shop:</label>
            <select
              value={selectedShop}
              onChange={(e) => setSelectedShop(e.target.value)}
              className="border p-1.5 rounded bg-white dark:bg-gray-800"
            >
              <option value="all">Të gjitha</option>
              {shops.map((shop) => (
                <option key={shop.id} value={shop.id}>
                  {shop.name}
                </option>
              ))}
            </select>
          </div>
        )}
        <input
          type="text"
          placeholder="Kërko me emër ose barkod..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="border p-1.5 rounded flex-1 min-w-[200px] dark:bg-gray-800"
        />
      </div>

      {/* Products Table */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05] bg-gray-50 dark:bg-gray-900">
              <TableRow>
                <TableCell isHeader>Emri</TableCell>
                <TableCell isHeader>Barkodi</TableCell>
                <TableCell isHeader>Çmimi</TableCell>
                <TableCell isHeader>Njësia</TableCell>
                <TableCell isHeader>Sasia</TableCell>
                <TableCell isHeader>Skadenca</TableCell>
                <TableCell isHeader>Kategoria</TableCell>
                {showShopColumn && <TableCell isHeader>Dyqani</TableCell>}
                {role !== "Cashier" && <TableCell isHeader className="text-right">Veprime</TableCell>}
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {filteredProducts.length > 0 ? (
                filteredProducts.map((product) => (
                  <TableRow key={product.id}>
                    <TableCell className="font-medium">{product.name}</TableCell>
                    <TableCell className="text-gray-500">{product.barcode}</TableCell>
                    <TableCell>{product.price.toFixed(2)} €</TableCell>
                    <TableCell>{product.unit}</TableCell>
                    <TableCell>
                      <span className="px-2 py-0.5 bg-red-100 text-red-700 rounded-full text-xs font-bold">
                        {product.quantity}
                      </span>
                    </TableCell>
                    <TableCell>{new Date(product.expiryDate).toLocaleDateString()}</TableCell>
                    <TableCell>
                      {product.category === 0 ? "Pije" :
                       product.category === 1 ? "Ushqim" :
                       product.category === 2 ? "Higjien" : "Tjera"}
                    </TableCell>
                    {showShopColumn && (
                      <TableCell>{shops.find((s) => s.id === product.shopId)?.name || "—"}</TableCell>
                    )}
                    {role !== "Cashier" && (
                      <TableCell className="text-right flex justify-end gap-2">
                        <button
                          className="px-3 py-1 bg-blue-500 text-white rounded text-sm hover:bg-blue-600 transition"
                          onClick={() => openForm(product)}
                        >
                          Edit
                        </button>
                        <button
                          className="px-3 py-1 bg-red-500 text-white rounded text-sm hover:bg-red-600 transition"
                          onClick={() => deleteProduct(product.id!)}
                        >
                          Delete
                        </button>
                      </TableCell>
                    )}
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={showShopColumn ? 9 : 8} className="text-center py-10 text-gray-400 italic">
                    Nuk u gjet asnjë produkt jashtë gjendjes.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </div>

      {/* Edit Form Modal */}
      {showForm && editingProduct && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white dark:bg-gray-900 p-6 rounded-2xl w-full max-w-[500px] shadow-2xl">
            <h2 className="text-2xl font-semibold mb-6 text-gray-800 dark:text-white">
              Përditëso Produktin
            </h2>
            <form onSubmit={submitForm} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2">
                  <label className="block text-sm font-medium mb-1">Emri</label>
                  <input type="text" name="name" defaultValue={editingProduct.name} required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Barkodi</label>
                  <input type="text" name="barcode" defaultValue={editingProduct.barcode} required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Çmimi</label>
                  <input type="number" name="price" defaultValue={editingProduct.price} step="0.01" min="0" required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Njësia</label>
                  <input type="text" name="unit" defaultValue={editingProduct.unit} required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Sasia</label>
                  <input type="number" name="quantity" defaultValue={editingProduct.quantity} required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div className="col-span-2">
                  <label className="block text-sm font-medium mb-1">Data e Skadencës</label>
                  <input type="date" name="expiryDate" defaultValue={new Date(editingProduct.expiryDate).toISOString().split("T")[0]} required className="w-full border p-2 rounded-lg dark:bg-gray-800" />
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1">Kategoria</label>
                  <select name="category" defaultValue={editingProduct.category} className="w-full border p-2 rounded-lg dark:bg-gray-800">
                    <option value={0}>Pije</option>
                    <option value={1}>Ushqim</option>
                    <option value={2}>Higjien</option>
                    <option value={3}>Tjera</option>
                  </select>
                </div>
                {role === "Admin" && (
                  <div>
                    <label className="block text-sm font-medium mb-1">Dyqani</label>
                    <select name="shopId" defaultValue={editingProduct.shopId} className="w-full border p-2 rounded-lg dark:bg-gray-800">
                      {shops.map((shop) => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))}
                    </select>
                  </div>
                )}
              </div>
              <div className="flex justify-end gap-3 mt-6">
                <button type="button" onClick={closeForm} className="px-5 py-2 bg-gray-200 dark:bg-gray-700 rounded-lg font-medium transition hover:bg-gray-300">
                  Anulo
                </button>
                <button type="submit" className="px-5 py-2 bg-blue-600 text-white rounded-lg font-medium transition hover:bg-blue-700">
                  Ruaj
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}