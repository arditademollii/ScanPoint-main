import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import api from "../../api/axiosInstance";

interface Manager {
  id: string;
  username: string;
  email: string;
  role: string;
  shopId: string;
  shopName: string;
  isDeleted: boolean;
  deletedAt?: string;
  shopIsDeleted: boolean;
}

interface Shop {
  id: string;
  name: string;
}

interface ManagerForm {
  username: string;
  email: string;
  password: string;
  shopId: string;
}

export default function ManagerManagement() {
  const navigate = useNavigate();

  const [managers, setManagers] = useState<Manager[]>([]);
  const [shops, setShops] = useState<Shop[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showDeleted, setShowDeleted] = useState(false);
  const [restoreError, setRestoreError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null);

  const [formData, setFormData] = useState<ManagerForm>({
    username: "",
    email: "",
    password: "",
    shopId: "",
  });

  const fetchAll = async () => {
    try {
      const [shopsRes, managersRes] = await Promise.all([
        api.get("/api/Shops/my-shops"),
        api.get("/api/Manager/all"),
      ]);

      setShops(shopsRes.data);

      setManagers(managersRes.data);
    } catch {
      setError("Failed to load data.");
    }
  };

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      await fetchAll();
      setLoading(false);
    };
    load();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleCreateOrUpdate = async () => {
    setFormError(null);

    if (!formData.shopId) {
      setFormError("Ju lutem zgjedhni një shop.");
      return;
    }
    if (!formData.username.trim()) {
      setFormError("Username është i detyrueshëm.");
      return;
    }
    if (!formData.email.trim()) {
      setFormError("Email është i detyrueshëm.");
      return;
    }
    if (!editId && !formData.password.trim()) {
      setFormError("Fjalëkalimi është i detyrueshëm.");
      return;
    }
    try {
      if (editId) {
        const payload = {
          ...formData,
          password: formData.password || undefined, // mos dërgo password bosh
        };

        await api.put(`/api/Manager/${editId}`, payload);
      } else {
        await api.post("/api/Manager", formData);
      }

      setShowForm(false);
      setEditId(null);

      setFormData({
        username: "",
        email: "",
        password: "",
        shopId: shops.length === 1 ? shops[0].id : "",
      });

      await fetchAll();
    } catch (err: any) {
      const msg =
        err.response?.data?.message ||
        err.response?.data?.Message ||
        (typeof err.response?.data === "string"
          ? err.response.data
          : "Ndodhi një gabim.");

      if (msg.toLowerCase().includes("email")) {
        setFormError("Ekziston një user me këtë email!");
      } else if (msg.toLowerCase().includes("username")) {
        setFormError("Ekziston një user me këtë username!");
      } else {
        setFormError(msg);
      }
    }
  };

  const handleEdit = (manager: Manager) => {
    setEditId(manager.id);

    setFormData({
      username: manager.username,
      email: manager.email,
      password: "",
      shopId: manager.shopId,
    });

    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("A jeni i sigurt?")) return;

    try {
      await api.delete(`/api/Manager/${id}`);
      await fetchAll();
    } catch {
      alert("Failed to delete manager.");
    }
  };

  const handleRestore = async (id: string) => {
    setRestoreError(null);

    try {
      await api.post(`/api/Manager/${id}/restore`);
      await fetchAll();
    } catch (err: any) {
      const msg =
        typeof err.response?.data === "string"
          ? err.response.data
          : err.response?.data?.message || "Rikthimi dështoi.";

      setRestoreError(msg);
    }
  };

  const activeManagers = managers.filter((m) => !m.isDeleted);
  const deletedManagers = managers.filter((m) => m.isDeleted);

  if (loading) return <div>Loading...</div>;

  if (error)
    return (
      <div>
        <p className="text-red-600">{error}</p>
        <button onClick={() => navigate("/login")}>Go to Login</button>
      </div>
    );

  return (
    <div className="space-y-6">
      {/* HEADER */}
      <div className="flex justify-between items-center">
        <button
          onClick={() => {
            setShowForm(!showForm);
            setEditId(null);
            setFormError(null);

            setFormData({
              username: "",
              email: "",
              password: "",
              shopId: shops.length === 1 ? shops[0].id : "",
            });
          }}
          className="px-4 py-2 bg-green-600 text-white rounded"
        >
          {showForm ? "Mbyll Formën" : "Shto Manager"}
        </button>

        {deletedManagers.length > 0 && (
          <span
            onClick={() => setShowDeleted(!showDeleted)}
            className="text-sm text-blue-600 hover:underline cursor-pointer font-medium"
          >
            {showDeleted
              ? "Fshih menaxherët e fshirë"
              : `Shiko menaxherët e fshirë (${deletedManagers.length})`}
          </span>
        )}
      </div>

      {/* RESTORE ERROR */}
      {restoreError && (
        <div className="p-3 bg-red-100 text-red-700 text-sm rounded border border-red-200">
          ⚠️ {restoreError}
        </div>
      )}

      {/* FORM */}
      {showForm && (
        <div className="p-4 border rounded bg-gray-50 max-w-sm shadow-sm">
          <h3 className="mb-3 font-semibold text-gray-700">
            {editId ? "Ndrysho Manager" : "Krijo Manager"}
          </h3>

          {formError && (
            <div className="mb-2 p-2 bg-red-100 text-red-600 text-sm rounded">
              {formError}
            </div>
          )}

          <div className="space-y-3">
            <input
              name="username"
              placeholder="Username"
              value={formData.username}
              onChange={handleInputChange}
              className="border p-2 w-full rounded"
            />

            <input
              type="email"
              name="email"
              placeholder="Email"
              value={formData.email}
              onChange={handleInputChange}
              className="border p-2 w-full rounded"
            />

            <input
              type="password"
              name="password"
              placeholder={
                editId
                  ? "Fjalëkalimi i ri (lëre bosh)"
                  : "Fjalëkalimi"
              }
              value={formData.password}
              onChange={handleInputChange}
              className="border p-2 w-full rounded"
            />

            <select
              name="shopId"
              value={formData.shopId}
              onChange={handleInputChange}
              className="border p-2 w-full rounded"
            >
              <option value="">Zgjedh Shop</option>
              {shops.map((shop) => (
                <option key={shop.id} value={shop.id}>
                  {shop.name}
                </option>
              ))}
            </select>

            <div className="flex gap-2">
              <button
                onClick={handleCreateOrUpdate}
                className="bg-blue-600 text-white px-4 py-2 rounded flex-1"
              >
                {editId ? "Update" : "Create"}
              </button>

              <button
                onClick={() => {
                  setShowForm(false);
                  setEditId(null);
                }}
                className="bg-gray-400 text-white px-4 py-2 rounded"
              >
                Anulo
              </button>
            </div>
          </div>
        </div>
      )}

      {/* ACTIVE TABLE */}
      <div className="border rounded-xl overflow-hidden bg-white shadow-sm">
        <table className="min-w-full">
          <thead className="bg-gray-50 border-b">
            <tr>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">User</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Email</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Shop</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Veprime</th>
            </tr>
          </thead>

          <tbody className="divide-y">
            {activeManagers.map((m) => (
              <tr key={m.id} className="hover:bg-gray-50">
                <td className="p-3 text-sm">{m.username}</td>
                <td className="p-3 text-sm">{m.email}</td>
                <td className="p-3 text-sm">{m.shopName}</td>

                <td className="p-3 flex gap-2">
                  <button
                    onClick={() => handleEdit(m)}
                    className="bg-yellow-500 text-white px-2 py-1 rounded text-xs"
                  >
                    Edit
                  </button>

                  <button
                    onClick={() => handleDelete(m.id)}
                    className="bg-red-600 text-white px-2 py-1 rounded text-xs"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}

            {activeManagers.length === 0 && (
              <tr>
                <td
                  colSpan={4}
                  className="text-center p-6 text-gray-400 text-sm italic"
                >
                  Asnjë manager aktiv.
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {/* DELETED */}
      {showDeleted && deletedManagers.length > 0 && (
        <div className="mt-8 border-t pt-6">
          <h4 className="mb-4 text-red-600 font-bold">
            🗑️ Menaxherët e fshirë
          </h4>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {deletedManagers.map((m) => (
              <div
                key={m.id}
                className="flex justify-between items-center p-4 border rounded-lg bg-red-50 border-red-100"
              >
                <div>
                  <p className="font-medium text-gray-700 line-through">
                    {m.username}
                  </p>
                  <p className="text-xs text-gray-500">{m.email}</p>
                  <p className="text-[10px] text-red-400 mt-1">
                    Dyqani: {m.shopName}
                    {m.deletedAt &&
                      ` • Fshirë: ${new Date(m.deletedAt).toLocaleDateString()}`}
                  </p>
                </div>

                <button
                  onClick={() => handleRestore(m.id)}
                  className="bg-green-600 hover:bg-green-700 text-white px-3 py-1 rounded text-xs"
                >
                  Rikthe
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}