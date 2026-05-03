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
      const res = await axios.get<Shop[]>("http://localhost:5055/api/Shops/my-shops", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setShops(res.data);
    } catch (err) {
      console.error(err);
    }
  };

  const fetchProducts = async () => {
    try {
      const res = await axios.get<Product[]>("http://localhost:5055/api/Products", {
        headers: { Authorization: `Bearer ${token}` },
      });
      setProducts(res.data);
    } finally {
      setLoading(false);
    }
  };

  // 🔥 EXPIRY LOGIC
  const getExpiryRowClass = (expiryDate: string) => {
    const today = new Date();
    const expiry = new Date(expiryDate);

    const diffTime = expiry.getTime() - today.getTime();
    const diffDays = diffTime / (1000 * 60 * 60 * 24);

    if (diffDays <= 7) {
      return "bg-red-100 dark:bg-red-900/20";
    }

    if (diffDays <= 30) {
      return "bg-orange-100 dark:bg-orange-900/20";
    }

    return "";
  };

  const filteredProducts = products
    .filter((p) => p.quantity > 0)
    .filter((p) =>
      searchQuery
        ? p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
          p.barcode.toLowerCase().includes(searchQuery.toLowerCase())
        : true
    )
    .filter((p) => (selectedShop === "all" ? true : p.shopId === selectedShop));

  const deleteProduct = async (id: string) => {
    await axios.delete(`http://localhost:5055/api/Products/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    setProducts((prev) => prev.filter((p) => p.id !== id));
  };

  const openForm = (product?: Product) => {
    setEditingProduct(product || null);
    setShowForm(true);
  };

  const closeForm = () => {
    setEditingProduct(null);
    setShowForm(false);
  };

  const submitForm = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    const formData = new FormData(e.currentTarget);

    const productData: Product = {
      id: editingProduct?.id,
      name: formData.get("name") as string,
      barcode: formData.get("barcode") as string,
      price: Number(formData.get("price")),
      unit: formData.get("unit") as string,
      quantity: Number(formData.get("quantity")),
      expiryDate: formData.get("expiryDate") as string,
      category: Number(formData.get("category")),
      shopId:
        role === "Manager"
          ? shopIdClaim!
          : (formData.get("shopId") as string),
    };

    if (editingProduct) {
      await axios.put(
        `http://localhost:5055/api/Products/${editingProduct.id}`,
        productData,
        { headers: { Authorization: `Bearer ${token}` } }
      );
    } else {
      await axios.post(
        `http://localhost:5055/api/Products`,
        productData,
        { headers: { Authorization: `Bearer ${token}` } }
      );
    }

    fetchProducts();
    closeForm();
  };

  useEffect(() => {
    fetchShops();
    fetchProducts();
  }, []);

  if (loading) return <div>Loading...</div>;

  const showShopColumn = role === "Admin";

  return (
    <div className="p-4">

      {/* TABLE */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-white/[0.05] dark:bg-white/[0.03]">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader>
              <TableRow>
                <TableCell isHeader>Name</TableCell>
                <TableCell isHeader>Barcode</TableCell>
                <TableCell isHeader>Price</TableCell>
                <TableCell isHeader>Unit</TableCell>
                <TableCell isHeader>Quantity</TableCell>
                <TableCell isHeader>Expiry</TableCell>
                <TableCell isHeader>Category</TableCell>
                {showShopColumn && <TableCell isHeader>Shop</TableCell>}
              </TableRow>
            </TableHeader>

            <TableBody>
              {filteredProducts.map((product) => (
                <TableRow
                  key={product.id}
                  className={getExpiryRowClass(product.expiryDate)}
                >
                  <TableCell>{product.name}</TableCell>
                  <TableCell>{product.barcode}</TableCell>
                  <TableCell>{product.price.toFixed(2)}</TableCell>
                  <TableCell>{product.unit}</TableCell>
                  <TableCell>{product.quantity}</TableCell>
                  <TableCell>
                    {new Date(product.expiryDate).toLocaleDateString()}
                  </TableCell>
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
                    <TableCell>
                      {shops.find((s) => s.id === product.shopId)?.name}
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      </div>
    </div>
  );
}