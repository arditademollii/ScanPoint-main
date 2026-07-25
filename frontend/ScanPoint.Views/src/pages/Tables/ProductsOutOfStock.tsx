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
  const role = user?.role ?? null;

  // Fetch shops
  const fetchShops = async () => {
    try {
      const res = await api.get<Shop[]>("/api/Shops/my-shops");
      setShops(res.data);
    } catch (err) {
      console.error("Error fetching shops:", err);
    }
  };

  // Fetch products
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
  .filter((p) => p.quantity === 0) // vetëm produktet out of stock
  .filter((p) =>
    searchQuery
      ? p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
        p.barcode.toLowerCase().includes(searchQuery.toLowerCase())
      : true
  )
  .filter((p) => (selectedShop === "all" ? true : p.shopId === selectedShop));

  const deleteProduct = async (id: string) => {
    try {
      await api.delete(`/api/Products/${id}`);
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err) {
      console.error("Error deleting product:", err);
    }
  };

  const openForm = (product?: Product) => {
    if (product) setEditingProduct(product);
    else setEditingProduct(null);
    setShowForm(true);
  };

  const closeForm = () => {
    setEditingProduct(null);
    setShowForm(false);
  };

  const submitForm = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
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
      shopId: role === "manager" ? (user as any)?.shopId ?? "" : (formData.get("shopId") as string),
      id: editingProduct?.id,
    };

    try {
      if (editingProduct) {
        await api.put(`/api/Products/${editingProduct.id}`, productData);
      } else {
        await api.post("/api/Products", productData);
      }
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

  if (loading) return <div>Loading...</div>;

  const showShopColumn = role === "admin";

  return (
    <div className="p-4">
      {/* Filter, Search + Add */}
      <div className="mb-4 flex flex-wrap items-center gap-4">
        {role === "admin" && (
          <>
            <label>Filter by Shop:</label>
            <select
              value={selectedShop}
              onChange={(e) => setSelectedShop(e.target.value)}
              className="border p-1 rounded"
            >
              <option value="all">All</option>
              {shops.map((shop) => (
                <option key={shop.id} value={shop.id}>
                  {shop.name}
                </option>
              ))}
            </select>
          </>
        )}
        <input
          type="text"
          placeholder="Search by Name or Barcode..."
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          className="border p-1 rounded flex-1 min-w-[200px]"
        />
        {role !== "cashier" && (
          <button
            className="px-3 py-1 bg-green-500 text-white rounded"
            onClick={() => openForm()}
          >
            Add Product
          </button>
        )}
      </div>

      {/* Products Table */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell isHeader>Name</TableCell>
                <TableCell isHeader>Barcode</TableCell>
                <TableCell isHeader>Price</TableCell>
                <TableCell isHeader>Unit</TableCell>
                <TableCell isHeader>Quantity</TableCell>
                <TableCell isHeader>Expiry</TableCell>
                <TableCell isHeader>Category</TableCell>
                {showShopColumn && <TableCell isHeader>Shop</TableCell>}
                {role !== "cashier" && <TableCell isHeader>Actions</TableCell>}
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {filteredProducts.map((product) => (
                <TableRow key={product.id}>
                  <TableCell>{product.name}</TableCell>
                  <TableCell>{product.barcode}</TableCell>
                  <TableCell>{product.price.toFixed(2)}</TableCell>
                  <TableCell>{product.unit}</TableCell>
                  <TableCell>{product.quantity}</TableCell>
                  <TableCell>{new Date(product.expiryDate).toLocaleDateString()}</TableCell>
                  <TableCell>
                    {product.category === 0
                      ? "Pije"
                      : product.category === 1
                      ? "Ushqim"
                      : product.category === 2
                      ? "Higjien"
                      : "Tjera"}
                  </TableCell>
                  {showShopColumn && (
                    <TableCell>{shops.find((s) => s.id === product.shopId)?.name}</TableCell>
                  )}
                  {role !== "cashier" && (
                    <TableCell className="flex gap-2">
                      <button
                        className="px-2 py-1 bg-blue-500 text-white rounded"
                        onClick={() => openForm(product)}
                      >
                        Edit
                      </button>
                      <button
                        className="px-2 py-1 bg-red-500 text-white rounded"
                        onClick={() => deleteProduct(product.id!)}
                      >
                        Delete
                      </button>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>

      {/* Add/Edit Form Modal */}
      {showForm && (
        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50">
          <div className="bg-white p-6 rounded-lg w-[500px]">
            <h2 className="text-xl mb-4">{editingProduct ? "Edit Product" : "Add Product"}</h2>
            <form onSubmit={submitForm} className="space-y-3">
              <input
                type="text"
                name="name"
                placeholder="Name"
                defaultValue={editingProduct?.name || ""}
                required
                className="w-full border p-2 rounded"
              />
              <input
                type="text"
                name="barcode"
                placeholder="Barcode"
                defaultValue={editingProduct?.barcode || ""}
                required
                className="w-full border p-2 rounded"
              />
              <input
                type="number"
                name="price"
                placeholder="Price"
                defaultValue={editingProduct?.price || 0.0}
                step="0.01"
                min="0"
                required
                className="w-full border p-2 rounded"
              />
              <input
                type="text"
                name="unit"
                placeholder="Unit"
                defaultValue={editingProduct?.unit || ""}
                required
                className="w-full border p-2 rounded"
              />
              <input
                type="number"
                name="quantity"
                placeholder="Quantity"
                defaultValue={editingProduct?.quantity || 0}
                required
                className="w-full border p-2 rounded"
              />
              <input
                type="date"
                name="expiryDate"
                placeholder="Expiry Date"
                defaultValue={
                  editingProduct
                    ? new Date(editingProduct.expiryDate).toISOString().split("T")[0]
                    : ""
                }
                required
                className="w-full border p-2 rounded"
              />
              <select
                name="category"
                defaultValue={editingProduct?.category || 0}
                className="w-full border p-2 rounded"
              >
                <option value={0}>Pije</option>
                <option value={1}>Ushqim</option>
                <option value={2}>Higjien</option>
                <option value={3}>Tjera</option>
              </select>
              {role === "admin" && (
                <select
                  name="shopId"
                  defaultValue={editingProduct?.shopId || shops[0]?.id}
                  className="w-full border p-2 rounded"
                >
                  {shops.map((shop) => (
                    <option key={shop.id} value={shop.id}>
                      {shop.name}
                    </option>
                  ))}
                </select>
              )}
              <div className="flex justify-end gap-2 mt-2">
                <button
                  type="button"
                  onClick={closeForm}
                  className="px-3 py-1 bg-gray-300 rounded"
                >
                  Cancel
                </button>
                <button type="submit" className="px-3 py-1 bg-blue-500 text-white rounded">
                  {editingProduct ? "Update" : "Add"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
