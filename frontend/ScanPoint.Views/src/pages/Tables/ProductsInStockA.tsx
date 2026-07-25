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

interface FormErrors {
  name?: string;
  barcode?: string;
  price?: string;
  unit?: string;
  quantity?: string;
  expiryDate?: string;
  shopId?: string;
  server?: string;
}

interface ProductFormState {
  name: string;
  barcode: string;
  price: string;
  unit: string;
  quantity: string;
  expiryDate: string;
  category: string;
  shopId: string;
}

const UNITS = ["cope", "liter", "kg", "pako"] as const;

const emptyFormState = (defaultShopId = ""): ProductFormState => ({
  name: "",
  barcode: "",
  price: "",
  unit: "cope",
  quantity: "",
  expiryDate: "",
  category: "0",
  shopId: defaultShopId,
});

// ✅ FIX data: nuk përdorim toISOString() — kjo e kthen në UTC dhe humb një ditë
function toLocalDateString(dateStr: string): string {
  const d = new Date(dateStr);
  const yyyy = d.getFullYear();
  const mm = String(d.getMonth() + 1).padStart(2, "0");
  const dd = String(d.getDate()).padStart(2, "0");
  return `${yyyy}-${mm}-${dd}`;
}

function todayString(): string {
  return toLocalDateString(new Date().toISOString());
}

function getExpiryStatus(expiryDate: string): "expired" | "week" | "month" | "ok" {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const expiry = new Date(expiryDate);
  expiry.setHours(0, 0, 0, 0);
  const diffDays = Math.floor((expiry.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
  if (diffDays < 0) return "expired";
  if (diffDays <= 7) return "week";
  if (diffDays <= 30) return "month";
  return "ok";
}

function getExpiryRowClass(status: ReturnType<typeof getExpiryStatus>): string {
  if (status === "expired") return "bg-red-50 hover:bg-red-100/70";
  if (status === "week")    return "bg-red-50/60 hover:bg-red-100/50";
  if (status === "month")   return "bg-orange-50 hover:bg-orange-100/50";
  return "hover:bg-gray-50/50";
}

function getExpiryBadgeClass(status: ReturnType<typeof getExpiryStatus>): string {
  if (status === "expired") return "text-red-700 font-semibold";
  if (status === "week")    return "text-red-600 font-semibold";
  if (status === "month")   return "text-orange-600 font-semibold";
  return "text-gray-700";
}

export default function AdminProductsTable() {
  const [products, setProducts] = useState<Product[]>([]);
  const [shops, setShops] = useState<Shop[]>([]);
  const [selectedShop, setSelectedShop] = useState<string>("all");
  const [searchQuery, setSearchQuery] = useState("");
  const [loading, setLoading] = useState(true);

  const [showForm, setShowForm] = useState(false);
  const [editingProduct, setEditingProduct] = useState<Product | null>(null);
  // ✅ FIX: controlled form state — eliminon autofill dhe fields të ngatërruara
  const [formState, setFormState] = useState<ProductFormState>(emptyFormState());
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [submitLoading, setSubmitLoading] = useState(false);

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

  useEffect(() => {
    fetchShops();
    fetchProducts();
  }, []);

  const filteredProducts = products
    .filter((p) => p.quantity > 0)
    .filter((p) =>
      searchQuery
        ? p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          p.barcode.toLowerCase().includes(searchQuery.toLowerCase())
        : true
    )
    .filter((p) => selectedShop === "all" || p.shopId === selectedShop);

  const deleteProduct = async (id: string) => {
    if (!window.confirm("A jeni të sigurt që dëshironi ta fshini këtë produkt?")) return;
    try {
      await api.delete(`/api/Products/${id}`);
      setProducts((prev) => prev.filter((p) => p.id !== id));
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi.");
    }
  };

  // ✅ FIX: openForm mbush state me të dhënat AKTUALE të produktit
  const openForm = (product?: Product) => {
    setFormErrors({});
    if (product) {
      setEditingProduct(product);
      setFormState({
        name: product.name,
        barcode: product.barcode,
        price: String(product.price),
        unit: product.unit,
        quantity: String(product.quantity),
        // ✅ FIX data: toLocalDateString jo toISOString
        expiryDate: toLocalDateString(product.expiryDate),
        category: String(product.category),
        shopId: product.shopId ?? shops[0]?.id ?? "",
      });
    } else {
      setEditingProduct(null);
      setFormState(emptyFormState(shops[0]?.id ?? ""));
    }
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditingProduct(null);
    setFormState(emptyFormState());
    setFormErrors({});
  };

  const setField = (field: keyof ProductFormState, value: string) => {
    setFormState((prev) => ({ ...prev, [field]: value }));
    // Pastro error-in e fushës kur përdoruesi shkruan
    if (formErrors[field as keyof FormErrors]) {
      setFormErrors((prev) => ({ ...prev, [field]: undefined }));
    }
  };

  const validateForm = (): FormErrors => {
    const errors: FormErrors = {};
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (!formState.name.trim())
      errors.name = "Emri është i detyrueshëm.";

    // ✅ Barkodi: 8–14 shifra numerike (EAN-8/EAN-13)
    if (!formState.barcode.trim())
      errors.barcode = "Barkodi është i detyrueshëm.";
    else if (!/^\d{8,14}$/.test(formState.barcode.trim()))
      errors.barcode = "Barkodi duhet të ketë 8–14 shifra numerike.";

    const price = Number(formState.price);
    if (!formState.price || isNaN(price) || price <= 0)
      errors.price = "Çmimi duhet të jetë më i madh se 0.";

    if (!formState.unit)
      errors.unit = "Njësia është e detyrueshme.";

    const quantity = Number(formState.quantity);
    if (!formState.quantity || !Number.isInteger(quantity) || quantity < 1)
      errors.quantity = "Sasia duhet të jetë të paktën 1.";

    if (!formState.expiryDate) {
      errors.expiryDate = "Data e skadencës është e detyrueshme.";
    } else {
      const expiry = new Date(formState.expiryDate);
      expiry.setHours(0, 0, 0, 0);
      if (expiry < today)
        errors.expiryDate = "Data e skadencës nuk mund të jetë në të shkuarën.";
    }

    if (role === "admin" && !formState.shopId)
      errors.shopId = "Dyqani është i detyrueshëm.";

    return errors;
  };

  const submitForm = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const errors = validateForm();
    if (Object.keys(errors).length > 0) {
      setFormErrors(errors);
      return;
    }
    setFormErrors({});
    setSubmitLoading(true);

    const productData: any = {
      name: formState.name.trim(),
      barcode: formState.barcode.trim(),
      price: Number(formState.price),
      unit: formState.unit,
      quantity: Number(formState.quantity),
      expiryDate: formState.expiryDate,
      category: Number(formState.category),
    };

    if (role === "admin") productData.shopId = formState.shopId;

    try {
      if (editingProduct) {
        await api.put(`/api/Products/${editingProduct.id}`, productData);
      } else {
        await api.post("/api/Products", productData);
      }
      await fetchProducts();
      closeForm();
    } catch (err: any) {
      // ✅ Mesazhi i saktë nga serveri shfaqet në formë
      setFormErrors({
        server:
          err.response?.data?.message ||
          err.response?.data ||
          "Veprimi dështoi. Provoni përsëri.",
      });
    } finally {
      setSubmitLoading(false);
    }
  };

  if (loading)
    return <div className="p-6 text-center text-gray-500">Duke u ngarkuar...</div>;

  const showShopColumn = role === "admin";

  return (
    <div className="p-4">
      <div className="mb-6">
        <h1 className="text-2xl font-bold text-gray-800 dark:text-white">Menaxhimi i Produkteve</h1>
        <p className="text-sm text-gray-500 italic font-medium">Lista e produkteve aktive në stok</p>
      </div>

      {/* Filter + Search + Add */}
      <div className="mb-4 flex flex-wrap items-center gap-4">
        {role === "admin" && (
          <div className="flex items-center gap-2">
            <label className="text-sm font-semibold dark:text-gray-300">Dyqani:</label>
            <select
              value={selectedShop}
              onChange={(e) => setSelectedShop(e.target.value)}
              className="border p-2 rounded-lg bg-white dark:bg-gray-800 shadow-sm outline-none focus:ring-2 focus:ring-blue-500"
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
          className="border p-2 rounded-lg flex-1 min-w-[200px] shadow-sm outline-none focus:ring-2 focus:ring-blue-500"
        />
        {role !== "cashier" && (
          <button
            className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition font-medium shadow-md"
            onClick={() => openForm()}
          >
            + Shto Produkt
          </button>
        )}
      </div>

      {/* Tabela */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="bg-gray-50 border-b border-gray-100 dark:border-white/[0.05]">
              <TableRow>
                <TableCell isHeader>Emri</TableCell>
                <TableCell isHeader>Barkodi</TableCell>
                <TableCell isHeader>Çmimi</TableCell>
                <TableCell isHeader>Njësia</TableCell>
                <TableCell isHeader>Sasia</TableCell>
                <TableCell isHeader>Skadenca</TableCell>
                <TableCell isHeader>Kategoria</TableCell>
                {showShopColumn && <TableCell isHeader>Dyqani</TableCell>}
                {role !== "cashier" && <TableCell isHeader className="text-right">Veprimet</TableCell>}
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
              {filteredProducts.length > 0 ? (
                filteredProducts.map((product) => {
                  const expiryStatus = getExpiryStatus(product.expiryDate);
                  return (
                    <TableRow key={product.id} className={`transition-colors ${getExpiryRowClass(expiryStatus)}`}>
                      <TableCell className="font-medium text-gray-900">{product.name}</TableCell>
                      <TableCell className="text-gray-500 text-sm">{product.barcode}</TableCell>
                      <TableCell>{product.price.toFixed(2)} €</TableCell>
                      <TableCell>{product.unit}</TableCell>
                      <TableCell>
                        <span className="px-2 py-1 bg-green-100 text-green-700 rounded-md text-xs font-bold ring-1 ring-green-200">
                          {product.quantity}
                        </span>
                      </TableCell>
                      <TableCell className={`text-sm ${getExpiryBadgeClass(expiryStatus)}`}>
                        {new Date(product.expiryDate).toLocaleDateString()}
                        {expiryStatus === "expired" && <span className="ml-1 text-xs text-red-500">(Skaduar)</span>}
                        {expiryStatus === "week"    && <span className="ml-1 text-xs text-red-500">(Skadon së shpejti)</span>}
                        {expiryStatus === "month"   && <span className="ml-1 text-xs text-orange-500">(Skadon këtë muaj)</span>}
                      </TableCell>
                      <TableCell className="text-sm text-gray-600">
                        {["Pije","Ushqim","Higjien","Tjera"][product.category] ?? "—"}
                      </TableCell>
                      {showShopColumn && (
                        <TableCell className="text-sm">
                          {shops.find((s) => s.id === product.shopId)?.name ?? "—"}
                        </TableCell>
                      )}
                      {role !== "cashier" && (
                        <TableCell className="text-right">
                          <div className="flex justify-end gap-2">
                            <button
                              className="px-3 py-1 bg-blue-50 text-blue-600 border border-blue-100 rounded-md text-sm hover:bg-blue-600 hover:text-white transition"
                              onClick={() => openForm(product)}
                            >
                              Edit
                            </button>
                            <button
                              className="px-3 py-1 bg-red-50 text-red-600 border border-red-100 rounded-md text-sm hover:bg-red-600 hover:text-white transition"
                              onClick={() => deleteProduct(product.id!)}
                            >
                              Delete
                            </button>
                          </div>
                        </TableCell>
                      )}
                    </TableRow>
                  );
                })
              ) : (
                <TableRow>
                  <TableCell colSpan={showShopColumn ? 9 : 8} className="text-center py-10 text-gray-400 italic">
                    Nuk u gjet asnjë produkt në stok.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </div>
      </div>

      {/* Modal Form */}
      {showForm && (
        <div className="fixed inset-0 bg-black/40 backdrop-blur-sm flex items-center justify-center z-50 p-4">
          <div className="bg-white dark:bg-gray-900 p-6 rounded-2xl w-full max-w-[520px] shadow-2xl">
            <h2 className="text-2xl font-bold mb-6 text-gray-800 dark:text-white border-b pb-2">
              {editingProduct ? "Përditëso Produktin" : "Shto Produkt të Ri"}
            </h2>

            <form onSubmit={submitForm} autoComplete="off" noValidate className="space-y-4">
              <div className="grid grid-cols-2 gap-4">

                {/* Emri */}
                <div className="col-span-2">
                  <label className="text-xs font-bold uppercase text-gray-500">Emri <span className="text-red-500">*</span></label>
                  <input
                    type="text"
                    value={formState.name}
                    onChange={(e) => setField("name", e.target.value)}
                    placeholder="p.sh. Coca Cola 0.5L"
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-800 dark:text-white ${formErrors.name ? "border-red-400" : ""}`}
                  />
                  {formErrors.name && <p className="text-red-500 text-xs mt-1">{formErrors.name}</p>}
                </div>

                {/* Barkodi */}
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Barkodi <span className="text-red-500">*</span></label>
                  <input
                    type="text"
                    value={formState.barcode}
                    onChange={(e) => setField("barcode", e.target.value.replace(/\D/g, ""))}
                    placeholder="8–14 shifra"
                    maxLength={14}
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-800 dark:text-white ${formErrors.barcode ? "border-red-400" : ""}`}
                  />
                  {formErrors.barcode && <p className="text-red-500 text-xs mt-1">{formErrors.barcode}</p>}
                </div>

                {/* Çmimi */}
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Çmimi (€) <span className="text-red-500">*</span></label>
                  <input
                    type="number"
                    value={formState.price}
                    onChange={(e) => setField("price", e.target.value)}
                    step="0.01"
                    min="0.01"
                    onKeyDown={(e) => ["e","E","+","-"].includes(e.key) && e.preventDefault()}
                    placeholder="0.00"
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-800 dark:text-white ${formErrors.price ? "border-red-400" : ""}`}
                  />
                  {formErrors.price && <p className="text-red-500 text-xs mt-1">{formErrors.price}</p>}
                </div>

                {/* Njësia — dropdown */}
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Njësia <span className="text-red-500">*</span></label>
                  <select
                    value={formState.unit}
                    onChange={(e) => setField("unit", e.target.value)}
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-800 dark:text-white ${formErrors.unit ? "border-red-400" : ""}`}
                  >
                    {UNITS.map((u) => (
                      <option key={u} value={u}>{u}</option>
                    ))}
                  </select>
                  {formErrors.unit && <p className="text-red-500 text-xs mt-1">{formErrors.unit}</p>}
                </div>

                {/* Sasia */}
                <div>
                  <label className="text-xs font-bold uppercase text-gray-500">Sasia <span className="text-red-500">*</span></label>
                  <input
                    type="number"
                    value={formState.quantity}
                    onChange={(e) => setField("quantity", e.target.value)}
                    min="1"
                    step="1"
                    onKeyDown={(e) => ["e","E","+","-","."].includes(e.key) && e.preventDefault()}
                    placeholder="1"
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 font-bold dark:bg-gray-800 dark:text-white ${formErrors.quantity ? "border-red-400" : ""}`}
                  />
                  {formErrors.quantity && <p className="text-red-500 text-xs mt-1">{formErrors.quantity}</p>}
                </div>

                {/* Data e Skadencës */}
                <div className="col-span-2">
                  <label className="text-xs font-bold uppercase text-gray-500">Data e Skadencës <span className="text-red-500">*</span></label>
                  <input
                    type="date"
                    value={formState.expiryDate}
                    onChange={(e) => setField("expiryDate", e.target.value)}
                    min={todayString()}
                    className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 dark:bg-gray-800 dark:text-white ${formErrors.expiryDate ? "border-red-400" : ""}`}
                  />
                  {formErrors.expiryDate && <p className="text-red-500 text-xs mt-1">{formErrors.expiryDate}</p>}
                </div>

                {/* Kategoria */}
                <div className={role === "admin" ? "" : "col-span-2"}>
                  <label className="text-xs font-bold uppercase text-gray-500">Kategoria</label>
                  <select
                    value={formState.category}
                    onChange={(e) => setField("category", e.target.value)}
                    className="w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-800 dark:text-white"
                  >
                    <option value="0">Pije</option>
                    <option value="1">Ushqim</option>
                    <option value="2">Higjien</option>
                    <option value="3">Tjera</option>
                  </select>
                </div>

                {/* Dyqani — vetëm Admin */}
                {role === "admin" && (
                  <div>
                    <label className="text-xs font-bold uppercase text-gray-500">Dyqani <span className="text-red-500">*</span></label>
                    <select
                      value={formState.shopId}
                      onChange={(e) => setField("shopId", e.target.value)}
                      className={`w-full border p-2 rounded-lg mt-1 outline-none focus:ring-2 focus:ring-blue-500 bg-white dark:bg-gray-800 dark:text-white ${formErrors.shopId ? "border-red-400" : ""}`}
                    >
                      {shops.map((shop) => (
                        <option key={shop.id} value={shop.id}>{shop.name}</option>
                      ))}
                    </select>
                    {formErrors.shopId && <p className="text-red-500 text-xs mt-1">{formErrors.shopId}</p>}
                  </div>
                )}
              </div>

              {/* ✅ Mesazhi nga serveri */}
              {formErrors.server && (
                <div className="px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600 dark:bg-red-900/20 dark:border-red-800">
                  {formErrors.server}
                </div>
              )}

              <div className="flex justify-end gap-3 pt-4 border-t mt-2">
                <button
                  type="button"
                  onClick={closeForm}
                  className="px-5 py-2 text-gray-500 font-medium hover:text-gray-700 transition"
                >
                  Anulo
                </button>
                <button
                  type="submit"
                  disabled={submitLoading}
                  className="px-6 py-2 bg-blue-600 text-white rounded-xl font-bold hover:bg-blue-700 shadow-lg transition-all active:scale-95 disabled:opacity-60"
                >
                  {submitLoading
                    ? "Duke ruajtur..."
                    : editingProduct
                    ? "Përditëso"
                    : "Shto Produkt"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}