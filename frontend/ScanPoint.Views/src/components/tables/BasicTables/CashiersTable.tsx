import { useEffect, useState, useRef } from "react";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../../ui/table";
import api from "../../../api/axiosInstance";

interface Cashier {
  id: string;
  username: string;
  email: string;
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
  const [showForm, setShowForm] = useState(false);
  const [editCashierId, setEditCashierId] = useState<string | null>(null);
  const [formData, setFormData] = useState<CashierFormData>(emptyForm);
  const [errors, setErrors] = useState<FormErrors>({});
  const [loading, setLoading] = useState(false);

  // ✅ FIX autofill: ref mbi formën — e reset-ojmë DOM-in direkt
  const formRef = useRef<HTMLFormElement>(null);

  useEffect(() => {
    fetchCashiers();
  }, []);

  const fetchCashiers = async () => {
    try {
      const res = await api.get("/api/Cashier");
      const mappedData = res.data.map((c: any) => ({
        id: c.id || c.Id,
        username: c.username || c.Username,
        email: c.email || c.Email,
      }));
      setCashiers(mappedData);
    } catch (err) {
      console.error("Gabim gjatë marrjes së të dhënave", err);
    }
  };

  // ✅ Hap formën për SHTIM — reset i plotë
  const handleOpenAdd = () => {
    setEditCashierId(null);
    setFormData(emptyForm);
    setErrors({});
    setShowForm(true);
    // Reset DOM për të bllokuar autofill-in e browserit
    setTimeout(() => formRef.current?.reset(), 0);
  };

  // ✅ Hap formën për EDIT — mbush me të dhënat aktuale, fjalëkalim bosh
  const handleOpenEdit = (c: Cashier) => {
    setEditCashierId(c.id);
    setFormData({ username: c.username, email: c.email, password: "" });
    setErrors({});
    setShowForm(true);
    setTimeout(() => formRef.current?.reset(), 0);
  };

  const handleClose = () => {
    setShowForm(false);
    setEditCashierId(null);
    setFormData(emptyForm);
    setErrors({});
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

    // ✅ Fjalëkalimi kërkohet gjithmonë për krijim
    if (!editCashierId) {
      if (!formData.password.trim())
        errs.password = "Fjalëkalimi është i detyrueshëm.";
      else if (formData.password.length < 6)
        errs.password = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.";
    } else {
      // Në edit, nëse e shkruan duhet të jetë valid
      if (formData.password && formData.password.length < 6)
        errs.password = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.";
    }

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!validateForm()) return;

    setLoading(true);
    setErrors({});

    try {
      const payload: any = {
        username: formData.username.trim(),
        email: formData.email.trim(),
      };
      if (formData.password.trim()) payload.password = formData.password;

      if (editCashierId) {
        await api.put(`/api/Cashier/${editCashierId}`, payload);
      } else {
        payload.password = formData.password;
        await api.post("/api/Cashier", payload);
      }

      handleClose();
      await fetchCashiers();
    } catch (err: any) {
      // ✅ Mesazhi i saktë nga serveri (p.sh. email duplikat)
      setErrors({
        server:
          err.response?.data?.message ||
          err.response?.data ||
          "Kërkesa dështoi. Provoni përsëri.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-6">
        <h2 className="text-xl font-bold dark:text-white">Menaxhimi i Cashierëve</h2>
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition"
          onClick={showForm ? handleClose : handleOpenAdd}
        >
          {showForm ? "Mbyll" : "+ Shto Cashier"}
        </button>
      </div>

      {showForm && (
        <div className="p-6 border rounded-xl mb-8 bg-white dark:bg-gray-900 shadow-lg max-w-md">
          {/* ✅ autoComplete="off" në formë + autoComplete="new-password" në fushën e passwordit */}
          <form
            ref={formRef}
            onSubmit={handleSubmit}
            autoComplete="off"
            className="flex flex-col gap-4"
          >
            <h3 className="text-base font-bold dark:text-white">
              {editCashierId ? "Edito Cashierin" : "Shto Cashier të Ri"}
            </h3>

            {/* Username */}
            <div>
              <label className="text-sm font-semibold dark:text-gray-300">
                Username <span className="text-red-500">*</span>
              </label>
              <input
                type="text"
                name="cashier-username"
                autoComplete="off"
                value={formData.username}
                onChange={(e) => {
                  setFormData({ ...formData, username: e.target.value });
                  if (errors.username) setErrors((p) => ({ ...p, username: undefined }));
                }}
                className="w-full border p-2 rounded mt-1 dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {errors.username && (
                <p className="text-xs text-red-500 mt-1">{errors.username}</p>
              )}
            </div>

            {/* Email */}
            <div>
              <label className="text-sm font-semibold dark:text-gray-300">
                Email <span className="text-red-500">*</span>
              </label>
              <input
                type="email"
                name="cashier-email"
                autoComplete="off"
                value={formData.email}
                onChange={(e) => {
                  setFormData({ ...formData, email: e.target.value });
                  if (errors.email) setErrors((p) => ({ ...p, email: undefined }));
                }}
                className="w-full border p-2 rounded mt-1 dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {errors.email && (
                <p className="text-xs text-red-500 mt-1">{errors.email}</p>
              )}
            </div>

            {/* Fjalëkalimi */}
            <div>
              <label className="text-sm font-semibold dark:text-gray-300">
                Fjalëkalimi{" "}
                {!editCashierId
                  ? <span className="text-red-500">*</span>
                  : <span className="text-gray-400 font-normal text-xs">(lë bosh për të mos ndryshuar)</span>
                }
              </label>
              <input
                type="password"
                name="cashier-password"
                // ✅ new-password e bën browserin të mos sugjerojë fjalëkalim të ruajtur
                autoComplete="new-password"
                value={formData.password}
                onChange={(e) => {
                  setFormData({ ...formData, password: e.target.value });
                  if (errors.password) setErrors((p) => ({ ...p, password: undefined }));
                }}
                className="w-full border p-2 rounded mt-1 dark:bg-gray-800 dark:border-gray-700 dark:text-white"
              />
              {errors.password && (
                <p className="text-xs text-red-500 mt-1">{errors.password}</p>
              )}
            </div>

            {/* Gabimi nga serveri */}
            {errors.server && (
              <div className="px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600 dark:bg-red-900/20 dark:border-red-800">
                {errors.server}
              </div>
            )}

            <div className="flex gap-2 pt-1">
              <button
                type="button"
                onClick={handleClose}
                className="flex-1 py-2 rounded font-bold border border-gray-300 text-gray-600 hover:bg-gray-50 dark:text-gray-300 dark:border-gray-700 dark:hover:bg-gray-800 transition"
              >
                Anulo
              </button>
              <button
                type="submit"
                disabled={loading}
                className="flex-1 py-2 rounded font-bold text-white bg-green-600 hover:bg-green-700 disabled:opacity-60 transition"
              >
                {loading
                  ? "Duke ruajtur..."
                  : editCashierId
                  ? "Përditëso"
                  : "Ruaj Cashierin"}
              </button>
            </div>
          </form>
        </div>
      )}

      <div className="border rounded-lg overflow-hidden bg-white dark:bg-gray-900 shadow-sm">
        <Table>
          <TableHeader>
            <TableRow className="bg-gray-50 dark:bg-gray-800">
              <TableCell isHeader>Username</TableCell>
              <TableCell isHeader>Email</TableCell>
              <TableCell isHeader>Veprime</TableCell>
            </TableRow>
          </TableHeader>
          <TableBody>
            {cashiers.length === 0 ? (
              <TableRow>
                <TableCell colSpan={3} className="text-center py-6 text-gray-400">
                  Nuk u gjet asnjë kasier.
                </TableCell>
              </TableRow>
            ) : (
              cashiers.map((c) => (
                <TableRow key={c.id}>
                  <TableCell>{c.username}</TableCell>
                  <TableCell>{c.email}</TableCell>
                  <TableCell className="flex gap-3">
                    {/* ✅ handleOpenEdit — hap me të dhëna aktuale, jo me state të vjetër */}
                    <button
                      className="text-yellow-600 font-bold hover:underline"
                      onClick={() => handleOpenEdit(c)}
                    >
                      Edit
                    </button>
                    <button
                      className="text-red-600 font-bold hover:underline"
                      onClick={async () => {
                        if (window.confirm(`A jeni i sigurt për fshirjen e ${c.username}?`)) {
                          await api.delete(`/api/Cashier/${c.id}`);
                          fetchCashiers();
                        }
                      }}
                    >
                      Delete
                    </button>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}