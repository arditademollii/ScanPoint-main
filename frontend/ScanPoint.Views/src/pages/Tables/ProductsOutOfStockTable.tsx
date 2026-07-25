import { useEffect, useState } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../components/ui/table";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

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

  const { user, userInfo } = useAuth();
  const role = user?.role?.toLowerCase() ?? null;

  const fetchShops = async () => {
    try {
      const res = await api.get<Shop[]>("/api/Shops/my-shops");
      setShops(res.data);
    } catch (err) {
      console.error("Error fetching shops:", err);
    }
  };

  const fetchProducts = async () => {
    try {
      const res = await api.get<Product[]>("/api/Products");
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
    if (!window.confirm("A jeni të sigurt?")) return;
    try {
      await api.delete(`/api/Products/${id}`);
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
      shopId: role === "manager" ? (user as any)?.shopId : (formData.get("shopId") as string),
      id: editingProduct.id,
    };

    try {
      await api.put(`/api/Products/${editingProduct.id}`, productData);
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

  if (loading) return <div className="p-4 text-center text-gray-500">Duke ngarkuar...</div>;

  const showShopColumn = role === "admin";

  return (
    <div className="p-4">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-red-600">Produktet Out of Stock</h1>
        <p className="text-sm text-gray-500 font-medium italic">Lista e produkteve me sasi 0</p>
      </div>

      {/* Filter & Search */}
      <div className="mb-4 flex flex-wrap items-center gap-4">
        {role === "admin" && (
          <div className="flex items-center gap-2">
            <label className="text-sm font-semibold">Dyqani:</label>
            <select
              value={selectedShop}
              onChange={(e) => setSelectedShop(e.target.value)}
              className="border p-2 rounded-lg bg-white shadow-sm"
            >
              <option value="all">Të gjitha</option>
              {shops.map((shop) => (
                <option key={shop.id} value={shop.id}>{shop.name}</option>
              ))}
            </select>
          </div>
        )}
        <input
          type="text"
          placeholder="Kërko me emër ose barkod..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="border p-2 rounded-lg flex-1 min-w-[200px] shadow-sm"
        />
      </div>

      {/* Products Table */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
        <Table>
          <TableHeader className="bg-gray-50 border-b">
            <TableRow>
              <TableCell isHeader>Name</TableCell>
              <TableCell isHeader>Barcode</TableCell>
              <TableCell isHeader>Price</TableCell>
              <TableCell isHeader>Unit</TableCell>
              <TableCell isHeader>Quantity</TableCell>
              <TableCell isHeader>Expiry</TableCell>
              <TableCell isHeader>Category</TableCell>
              {showShopColumn && <TableCell isHeader>Shop</TableCell>}
              {role !== "cashier" && <TableCell isHeader className="text-right">Actions</TableCell>}
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-100">
            {filteredProducts.length > 0 ? (
              filteredProducts.map((product) => (
                <TableRow key={product.id} className="hover:bg-gray-50/50">
                  <TableCell className="font-medium">{product.name}</TableCell>
                  <TableCell className="text-gray-500 text-sm">{product.barcode}</TableCell>
                  <TableCell>{product.price.toFixed(2)} €</TableCell>
                  <TableCell>{product.unit}</TableCell>
                  <TableCell>
                    <span className="px-2 py-1 bg-red-100 text-red-700 rounded-md text-xs font-bold ring-1 ring-red-200">
                      {product.quantity}
                    </span>
                  </TableCell>
                  <TableCell className="text-sm">{new Date(product.expiryDate).toLocaleDateString()}</TableCell>
                  <TableCell className="text-sm text-gray-600">
                    {product.category === 0 ? "Pije" :
                     product.category === 1 ? "Ushqim" :
                     product.category === 2 ? "Higjien" : "Tjera"}
                  </TableCell>
                  {showShopColumn && (
                    <TableCell className="text-sm">{shops.find((s) => s.id === product.shopId)?.name || "—"}</TableCell>
                  )}
                  {role !== "cashier" && (
                    <TableCell className="text-right flex justify-end gap-2">
                      <button
                        className="px-3 py-1 bg-blue-50 text-blue-600 border border-blue-200 rounded-md text-sm hover:bg-blue-600 hover:text-white transition"
                        onClick={() => openForm(product)}
                      >
                        Edit
                      </button>
                      <button
                        className="px-3 py-1 bg-red-50 text-red-600 border border-red-200 rounded-md text-sm hover:bg-red-600 hover:text-white transition"
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
                <TableCell colSpan={showShopColumn ? 9 : 8} className="text-center py-12 text-gray-400 italic">
                  Nuk ka produkte jashtë gjendjes në këtë listë.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      {/* Edit Form Modal */}
      {showForm && editingProduct && (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white p-6 rounded-2xl w-full max-w-[500px] shadow-2xl animate-in fade-in zoom-in duration-200">
            <h2 className="text-2xl font-bold mb-6 text-gray-800">
              Përditëso Produktin
            </h2>
            <form onSubmit={submitForm} className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div className="col-span-2">
                  <label className="text-xs font-bold uppercase text-gray-500">Emri Produktit</label>
                  <input type="text" name="name" defaultValue={editingProduct.name} required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Barkodi</label>
                  <input type="text" name="barcode" defaultValue={editingProduct.barcode} required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Çmimi</label>
                  <input type="number" name="price" defaultValue={editingProduct.price} step="0.01" min="0" required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Njësia (copë, kg)</label>
                  <input type="text" name="unit" defaultValue={editingProduct.unit} required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Sasia</label>
                  <input type="number" name="quantity" defaultValue={editingProduct.quantity} required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div className="col-span-2">
                  <label className="text-xs font-bold uppercase text-gray-500">Data e Skadencës</label>
                  <input type="date" name="expiryDate" defaultValue={new Date(editingProduct.expiryDate).toISOString().split("T")[0]} required className="w-full border p-2 rounded-lg mt-1 outline-blue-500" />
                </div>
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Kategoria</label>
                  <select name="category" defaultValue={editingProduct.category} className="w-full border p-2 rounded-lg mt-1 outline-blue-500">
                    <option value={0}>Pije</option>
                    <option value={1}>Ushqim</option>
                    <option value={2}>Higjien</option>
                    <option value={3}>Tjera</option>
                  </select>
                </div>
                {role === "admin" && (
                  <div>
                    <label className="text-xs font-bold uppercase text-gray-500">Dyqani</label>
                    <select name="shopId" defaultValue={editingProduct.shopId} className="w-full border p-2 rounded-lg mt-1 outline-blue-500">
                      {shops.map((shop) => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))}
                    </select>
                  </div>
                )}
              </div>
              <div className="flex justify-end gap-3 pt-6 border-t mt-4">
                <button type="button" onClick={closeForm} className="px-5 py-2 text-gray-500 font-medium hover:text-gray-700">
                  Anulo
                </button>
                <button type="submit" className="px-6 py-2 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-lg transition-all">
                  Përditëso
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}