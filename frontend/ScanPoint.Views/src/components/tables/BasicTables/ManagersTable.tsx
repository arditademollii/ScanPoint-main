import { useState, useEffect } from "react";
import api from "../../../api/axiosInstance";

interface Manager {
  id: string;
  username: string;
  email: string;
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
  const [managers, setManagers] = useState<Manager[]>([]);
  const [shops, setShops] = useState<Shop[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showDeleted, setShowDeleted] = useState(false);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<string | null>(null);
  const [restoreError, setRestoreError] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null); // State për gabimet e formës

  const [formData, setFormData] = useState<ManagerForm>({
    username: "", email: "", password: "", shopId: "",
  });

  const fetchShops = async () => {
    try {
      const res = await api.get("/api/Shops/my-shops");
      setShops(res.data);
      if (res.data.length === 1)
        setFormData((prev) => ({ ...prev, shopId: res.data[0].id }));
    } catch {
      setError("Failed to fetch shops.");
    }
  };

  const fetchManagers = async () => {
    try {
      const res = await api.get("/api/Manager/all");
      setManagers(res.data);
    } catch {
      setError("Failed to fetch managers.");
    }
  };

  useEffect(() => {
    const load = async () => {
      setLoading(true);
      await fetchShops();
      await fetchManagers();
      setLoading(false);
    };
    load();
  }, []);

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormError(null); // Hiq gabimin kur përdoruesi ndryshon diçka
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleCreateOrUpdate = async () => {
    setFormError(null);

    // 1. Validimi i fushave bazë
    if (!formData.username || !formData.email || !formData.shopId || (!editId && !formData.password)) {
      setFormError("Ju lutem plotësoni të gjitha fushat.");
      return;
    }

    // 2. Validimi për email unik
    // Kontrollojmë nëse emaili ekziston te menaxherët tjerë (përveç atij që po editojmë)
    const emailExists = managers.some(m =>
      m.email.toLowerCase() === formData.email.toLowerCase() && m.id !== editId
    );

    if (emailExists) {
      setFormError("Ky email është i zënë nga një menaxher tjetër.");
      return;
    }

    try {
      if (editId) {
        await api.put(`/api/Manager/${editId}`, formData);
      } else {
        await api.post("/api/Manager", formData);
      }
      setShowForm(false);
      setEditId(null);
      setFormData({ username: "", email: "", password: "", shopId: "" });
      await fetchManagers();
    } catch (err: any) {
      const msg = err.response?.data?.Message || err.response?.data || "This menager already exists";
      setFormError(typeof msg === "string" ? msg : "Gabim gjatë ruajtjes.");
    }
  };

  const handleEdit = (manager: Manager) => {
    setFormError(null);
    setEditId(manager.id);
    setFormData({ username: manager.username, email: manager.email, password: "", shopId: manager.shopId });
    setShowForm(true);
  };

  const handleDelete = async (id: string) => {
    if (!window.confirm("Are you sure?")) return;
    try {
      await api.delete(`/api/Manager/${id}`);
      await fetchManagers();
    } catch {
      alert("Failed to delete manager.");
    }
  };

  const handleRestore = async (id: string) => {
    setRestoreError(null);
    try {
      await api.post(`/api/Manager/${id}/restore`);
      await fetchManagers();
    } catch (err: any) {
      const msg = err.response?.data || "Rikthimi dështoi.";
      setRestoreError(typeof msg === "string" ? msg : msg.message || "Rikthimi dështoi.");
    }
  };

  const activeManagers = managers.filter((m) => !m.isDeleted);
  const deletedManagers = managers.filter((m) => m.isDeleted);

  if (loading) return <div>Loading...</div>;
  if (error) return <p className="text-red-600">{error}</p>;

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <button
          type="button"
          onClick={() => {
            setShowForm(true);
            setEditId(null);
            setFormError(null);
            setFormData({ username: "", email: "", password: "", shopId: "" });
          }}
          className="px-4 py-2 bg-green-600 text-white rounded hover:bg-green-700 transition"
        >
          Add Manager
        </button>

        {deletedManagers.length > 0 && (
          <button
            type="button"
            onClick={() => setShowDeleted(!showDeleted)}
            className="text-sm text-brand-500 hover:text-brand-600 font-medium"
          >
            {showDeleted ? "Fshih të fshirat" : `Shfaq të fshirat (${deletedManagers.length})`}
          </button>
        )}
      </div>

      {restoreError && (
        <div className="p-3 rounded-lg bg-red-50 border border-red-200 text-red-700 text-sm">
          ⚠️ {restoreError}
        </div>
      )}

      {/* Forma me Mesazh Gabimi */}
      {showForm && (
        <div className="p-4 border rounded bg-gray-50 max-w-sm shadow-sm">
          <h3 className="mb-3 font-semibold text-gray-800">{editId ? "Edit Manager" : "Create Manager"}</h3>
         
          {formError && (
            <div className="mb-3 p-2 text-xs bg-red-100 border border-red-200 text-red-600 rounded">
              {formError}
            </div>
          )}

          <div className="space-y-2">
            <input name="username" placeholder="Username" value={formData.username} onChange={handleInputChange} className="border p-2 w-full rounded focus:ring-2 focus:ring-blue-500 outline-none" />
            <input name="email" placeholder="Email" value={formData.email} onChange={handleInputChange} className="border p-2 w-full rounded focus:ring-2 focus:ring-blue-500 outline-none" />
            <input type="password" name="password" placeholder={editId ? "New Password (leave blank to keep)" : "Password"} value={formData.password} onChange={handleInputChange} className="border p-2 w-full rounded focus:ring-2 focus:ring-blue-500 outline-none" />
           
            <select name="shopId" value={formData.shopId} onChange={handleInputChange} className="border p-2 w-full rounded outline-none focus:ring-2 focus:ring-blue-500">
              <option value="">Zgjedh Shop</option>
              {shops.map((shop) => (
                <option key={shop.id} value={shop.id}>{shop.name}</option>
              ))}
            </select>
          </div>

          <div className="flex gap-2 mt-4">
            <button type="button" onClick={handleCreateOrUpdate} className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700 flex-1">
              {editId ? "Update" : "Create"}
            </button>
            <button type="button" onClick={() => { setShowForm(false); setEditId(null); }} className="bg-gray-400 text-white px-4 py-2 rounded hover:bg-gray-500 flex-1">
              Cancel
            </button>
          </div>
        </div>
      )}

      {/* Tabela e managerëve aktivë */}
      <div className="overflow-x-auto border rounded-xl shadow-sm">
        <table className="min-w-full">
          <thead className="bg-gray-50 border-b">
            <tr>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">User</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Email</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Shop</th>
              <th className="p-3 text-left text-sm font-semibold text-gray-600">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100 bg-white">
            {activeManagers.map((m) => (
              <tr key={m.id} className="hover:bg-gray-50 transition">
                <td className="p-3 text-sm text-gray-700">{m.username}</td>
                <td className="p-3 text-sm text-gray-700">{m.email}</td>
                <td className="p-3 text-sm text-gray-700">{m.shopName}</td>
                <td className="p-3 flex gap-2">
                  <button type="button" onClick={() => handleEdit(m)} className="bg-yellow-500 text-white px-2 py-1 rounded text-xs hover:bg-yellow-600 transition">Edit</button>
                  <button type="button" onClick={() => handleDelete(m.id)} className="bg-red-600 text-white px-2 py-1 rounded text-xs hover:bg-red-700 transition">Delete</button>
                </td>
              </tr>
            ))}
            {activeManagers.length === 0 && (
              <tr><td colSpan={4} className="p-6 text-sm text-gray-400 text-center italic">Asnjë manager aktiv nuk u gjet.</td></tr>
            )}
          </tbody>
        </table>
      </div>

      {/* Seksioni i managerëve të fshirë */}
      {showDeleted && deletedManagers.length > 0 && (
        <div className="border-t pt-6 bg-gray-50/50 p-4 rounded-xl">
          <h4 className="mb-4 font-semibold text-gray-500 flex items-center gap-2 text-sm uppercase tracking-wider">
            <span className="w-2 h-2 rounded-full bg-red-400 inline-block animate-pulse"></span>
            Managerët e fshirë
          </h4>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
            {deletedManagers.map((m) => (
              <div key={m.id} className="flex items-center justify-between p-4 rounded-xl border border-red-100 bg-white shadow-sm">
                <div>
                  <p className="font-medium text-gray-700 line-through">{m.username}</p>
                  <p className="text-xs text-gray-400">{m.email}</p>
                  <p className="text-xs text-gray-400 mt-0.5">
                    Shop: <span className={m.shopIsDeleted ? "text-red-400 font-medium" : "text-gray-500"}>{m.shopName} {m.shopIsDeleted ? "(i fshirë)" : ""}</span>
                  </p>
                  {m.deletedAt && (
                    <p className="text-[10px] text-red-300 mt-1 uppercase">
                      Fshirë: {new Date(m.deletedAt).toLocaleDateString("sq-AL")}
                    </p>
                  )}
                </div>
                <button
                  onClick={() => handleRestore(m.id)}
                  disabled={m.shopIsDeleted}
                  title={m.shopIsDeleted ? `Rikthe fillimisht shopin '${m.shopName}'` : "Rikthe managerin"}
                  className={`px-3 py-1.5 text-xs font-medium rounded-lg border transition-all ${
                    m.shopIsDeleted
                      ? "bg-gray-100 text-gray-400 border-gray-200 cursor-not-allowed"
                      : "text-green-600 bg-green-50 border-green-200 hover:bg-green-600 hover:text-white"
                  }`}
                >
                  {m.shopIsDeleted ? "Shopi i fshirë" : "Rikthe"}
                </button>
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}