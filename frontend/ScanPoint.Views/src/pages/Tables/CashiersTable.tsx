import { useEffect, useState, useRef } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../components/ui/table";
import api from "../../api/axiosInstance";

interface Cashier {
  id: string;
  username: string;
  email: string;
  isDeleted: boolean;
  deletedAt?: string;
}

interface CashierFormData {
  username: string;
  email: string;
  password: string;
}

interface FormErrors {
  username?: string;
  email?: string;
  password?: string;
  server?: string;
}

const emptyForm: CashierFormData = {
  username: "",
  email: "",
  password: "",
};

const validateEmail = (email: string) =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

export default function CashiersTable() {
  const [cashiers, setCashiers] = useState<Cashier[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editCashierId, setEditCashierId] = useState<string | null>(null);
  const [formData, setFormData] = useState<CashierFormData>(emptyForm);
  const [formErrors, setFormErrors] = useState<FormErrors>({});
  const [submitLoading, setSubmitLoading] = useState(false);

  const [showDeleted, setShowDeleted] = useState(false);
  const [restoreError, setRestoreError] = useState<string | null>(null);

  // ✅ FIX autofill: ref për reset DOM
  const formRef = useRef<HTMLFormElement>(null);

  const fetchAll = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Cashier/all");
      setCashiers(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { fetchAll(); }, []);

  // ✅ Hap formën për SHTIM
  const handleOpenAdd = () => {
    setEditCashierId(null);
    setFormData(emptyForm);
    setFormErrors({});
    setShowForm(true);
    setTimeout(() => formRef.current?.reset(), 0);
  };

  // ✅ Hap formën për EDIT — password gjithmonë bosh
  const handleOpenEdit = (c: Cashier) => {
    setEditCashierId(c.id);
    setFormData({ username: c.username, email: c.email, password: "" });
    setFormErrors({});
    setShowForm(true);
    setTimeout(() => formRef.current?.reset(), 0);
  };

  const handleClose = () => {
    setShowForm(false);
    setEditCashierId(null);
    setFormData(emptyForm);
    setFormErrors({});
  };

  const validateForm = (): boolean => {
    const errs: FormErrors = {};

    if (!formData.username.trim())
      errs.username = "Username-i është i detyrueshëm.";
    else if (formData.username.trim().length < 3)
      errs.username = "Username-i duhet të ketë të paktën 3 karaktere.";

    if (!formData.email.trim())
      errs.email = "Email-i është i detyrueshëm.";
    else if (!validateEmail(formData.email))
      errs.email = "Email-i nuk është i vlefshëm.";

    if (!editCashierId) {
      if (!formData.password.trim())
        errs.password = "Fjalëkalimi është i detyrueshëm.";
      else if (formData.password.length < 6)
        errs.password = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.";
    } else {
      if (formData.password && formData.password.length < 6)
        errs.password = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.";
    }

    setFormErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    setSubmitLoading(true);
    setFormErrors({});

    try {
      const payload: any = {
        username: formData.username.trim(),
        email: formData.email.trim(),
      };
      if (formData.password.trim()) payload.password = formData.password;

      if (editCashierId) {
        await api.put(`/api/Cashier/${editCashierId}`, payload);
      } else {
        await api.post("/api/Cashier", payload);
      }

      handleClose();
      await fetchAll();
    } catch (err: any) {
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

  const handleDelete = async (id: string) => {
    if (!window.confirm("A jeni i sigurt që dëshironi ta fshini këtë cashier?")) return;
    try {
      await api.delete(`/api/Cashier/${id}`);
      await fetchAll();
    } catch (err: any) {
      setError(err.response?.data?.message || "Fshirja dështoi.");
    }
  };

  const handleRestore = async (id: string) => {
    setRestoreError(null);
    try {
      await api.post(`/api/Cashier/${id}/restore`);
      await fetchAll();
    } catch (err: any) {
      const msg = err.response?.data;
      setRestoreError(
        typeof msg === "string" ? msg : msg?.message || "Rikthimi dështoi."
      );
    }
  };

  const activeCashiers = cashiers.filter((c) => !c.isDeleted);
  const deletedCashiers = cashiers.filter((c) => c.isDeleted);

  if (loading)
    return <div className="p-6 text-center italic text-gray-500">Duke ngarkuar...</div>;

  return (
    <div className="p-4 space-y-6">
      {/* HEADER */}
      <div className="flex justify-between items-center bg-white dark:bg-gray-900 p-4 rounded-xl border shadow-sm">
        <button
          className="bg-blue-600 hover:bg-blue-700 text-white px-5 py-2 rounded-lg font-bold transition-all active:scale-95"
          onClick={showForm ? handleClose : handleOpenAdd}
        >
          {showForm ? "✕ Mbyll Formën" : "＋ Shto Cashier"}
        </button>

        <div className="flex gap-4 items-center">
          <span className="text-sm text-gray-400">
            Aktivë: <b>{activeCashiers.length}</b>
          </span>
          {deletedCashiers.length > 0 && (
            <button
              onClick={() => setShowDeleted(!showDeleted)}
              className="text-sm bg-gray-100 hover:bg-gray-200 text-blue-700 px-4 py-2 rounded-lg font-bold transition-colors"
            >
              {showDeleted
                ? "👁️ Fshih Arkivën"
                : `🗑️ Shiko Arkivën (${deletedCashiers.length})`}
            </button>
          )}
        </div>
      </div>

      {restoreError && (
        <div className="p-3 bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg">
          ⚠️ {restoreError}
        </div>
      )}

      {error && (
        <div className="p-3 bg-amber-50 text-amber-700 rounded-lg border border-amber-200 italic">
          {error}
        </div>
      )}

      {/* FORM */}
      {showForm && (
        <div className="p-6 border rounded-xl bg-gray-50 dark:bg-gray-900 shadow-inner">
          <h3 className="text-lg font-bold mb-4 text-gray-700 dark:text-white">
            {editCashierId ? "📝 Ndrysho të dhënat" : "👤 Regjistro Cashier të ri"}
          </h3>
          <form
            ref={formRef}
            onSubmit={handleSubmit}
            autoComplete="off"
            className="grid grid-cols-1 md:grid-cols-2 gap-4"
          >
            {/* Username */}
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                Username <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                name="cashier-username"
                autoComplete="off"
                placeholder="p.sh. john_doe"
                value={formData.username}
                onChange={(e) => {
                  setFormData({ ...formData, username: e.target.value });
                  if (formErrors.username) setFormErrors(p => ({ ...p, username: undefined }));
                }}
                className="border p-2.5 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {formErrors.username && (
                <p className="text-xs text-red-500">{formErrors.username}</p>
              )}
            </div>

            {/* Email */}
            <div className="flex flex-col gap-1">
              <label className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                Email <span className="text-red-500">*</span>
              </label>
              <input
                type="email"
                name="cashier-email"
                autoComplete="off"
                placeholder="p.sh. john@email.com"
                value={formData.email}
                onChange={(e) => {
                  setFormData({ ...formData, email: e.target.value });
                  if (formErrors.email) setFormErrors(p => ({ ...p, email: undefined }));
                }}
                className="border p-2.5 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {formErrors.email && (
                <p className="text-xs text-red-500">{formErrors.email}</p>
              )}
            </div>

            {/* Password */}
            <div className="flex flex-col gap-1 md:col-span-2">
              <label className="text-xs font-semibold text-gray-500 uppercase tracking-wide">
                Fjalëkalimi{" "}
                {!editCashierId
                  ? <span className="text-red-500">*</span>
                  : <span className="text-gray-400 font-normal normal-case">(lë bosh për të mos ndryshuar)</span>
                }
              </label>
              <input
                type="password"
                name="cashier-password"
                autoComplete="new-password"
                placeholder={editCashierId ? "Lë bosh për të mos ndryshuar" : "Minimum 6 karaktere"}
                value={formData.password}
                onChange={(e) => {
                  setFormData({ ...formData, password: e.target.value });
                  if (formErrors.password) setFormErrors(p => ({ ...p, password: undefined }));
                }}
                className="border p-2.5 rounded-lg focus:ring-2 focus:ring-blue-500 outline-none bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {formErrors.password && (
                <p className="text-xs text-red-500">{formErrors.password}</p>
              )}
            </div>

            {/* Server error */}
            {formErrors.server && (
              <div className="md:col-span-2 px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600 dark:bg-red-900/20 dark:border-red-800">
                {formErrors.server}
              </div>
            )}

            <div className="flex gap-3 md:col-span-2 mt-2">
              <button
                type="submit"
                disabled={submitLoading}
                className="bg-green-600 text-white px-8 py-2.5 rounded-lg font-bold hover:bg-green-700 shadow-md transition-all disabled:opacity-60"
              >
                {submitLoading
                  ? "Duke ruajtur..."
                  : editCashierId
                  ? "Ruaj Ndryshimet"
                  : "Krijo Llogarinë"}
              </button>
              <button
                type="button"
                onClick={handleClose}
                className="bg-gray-300 text-gray-700 px-8 py-2.5 rounded-lg font-bold hover:bg-gray-400 transition-all"
              >
                Anulo
              </button>
            </div>
          </form>
        </div>
      )}

      {/* ACTIVE TABLE */}
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:bg-gray-900 shadow-sm">
        <Table>
          <TableHeader className="bg-gray-50 dark:bg-gray-800 border-b">
            <TableRow>
              <TableCell isHeader className="py-4">Username</TableCell>
              <TableCell isHeader>Email</TableCell>
              <TableCell isHeader className="text-right px-6">Veprime</TableCell>
            </TableRow>
          </TableHeader>
          <TableBody>
            {activeCashiers.length > 0 ? (
              activeCashiers.map((c) => (
                <TableRow
                  key={c.id}
                  className="hover:bg-gray-50/50 dark:hover:bg-gray-800/50 transition-colors border-b last:border-0"
                >
                  <TableCell className="font-semibold py-4">{c.username}</TableCell>
                  <TableCell className="text-gray-600">{c.email}</TableCell>
                  <TableCell className="text-right px-6 space-x-2">
                    {/* ✅ handleOpenEdit — jo inline */}
                    <button
                      onClick={() => handleOpenEdit(c)}
                      className="text-yellow-600 hover:bg-yellow-50 px-3 py-1.5 rounded-md border border-yellow-200 font-medium text-sm transition-all"
                    >
                      Ndrysho
                    </button>
                    <button
                      onClick={() => handleDelete(c.id)}
                      className="text-red-600 hover:bg-red-50 px-3 py-1.5 rounded-md border border-red-200 font-medium text-sm transition-all"
                    >
                      Fshi
                    </button>
                  </TableCell>
                </TableRow>
              ))
            ) : (
              <TableRow>
                <TableCell colSpan={3} className="text-center py-12 text-gray-400 italic">
                  Asnjë cashier aktiv në sistem.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>

      {/* ARKIVA */}
      {showDeleted && deletedCashiers.length > 0 && (
        <div className="mt-12 border-t-2 border-dashed border-gray-200 pt-8">
          <h4 className="text-red-500 font-black mb-6 flex items-center gap-2 text-lg uppercase tracking-wider">
            🗑️ Arkiva e Cashierëve të Fshirë
          </h4>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {deletedCashiers.map((c) => (
              <div
                key={c.id}
                className="flex flex-col p-5 border border-red-100 rounded-2xl bg-red-50/30 hover:bg-red-50 transition-all shadow-sm"
              >
                <div className="flex justify-between items-start mb-2">
                  <div>
                    <p className="font-bold text-gray-800 text-lg line-through decoration-red-400">
                      {c.username}
                    </p>
                    <p className="text-xs text-gray-500 italic">{c.email}</p>
                  </div>
                  <button
                    onClick={() => handleRestore(c.id)}
                    className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-xs font-bold shadow-md transition-all active:scale-95"
                  >
                    RIKTHE
                  </button>
                </div>
                {c.deletedAt && (
                  <div className="mt-2 pt-2 border-t border-red-100/50">
                    <p className="text-[10px] uppercase tracking-widest font-bold text-red-400">
                      Fshirë më: {new Date(c.deletedAt).toLocaleDateString("sq-AL")}
                    </p>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}