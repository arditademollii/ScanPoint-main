import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ID_Employee mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
// ===========================

interface Contract {
  ID: number;          // Backend kthen "ID" — toCamel e ruan si "ID" (2+ shkronja të mëdha)
  title: string;
  description: string;
  ID_Employee: number;
  emriEmployee: string;
}

interface Employee {
  ID_Employee: number;
  emriEmployee: string;
  qyteti: string;
}

interface FormData {
  title: string;
  description: string;
  ID_Employee: number;
}

const emptyForm: FormData = { title: "", description: "", ID_Employee: 0 };

export default function ContractPage() {
  // ===========================
  // STATE
  // ===========================
  const [Contracts, setContractt] = useState<Contract[]>([]);
  const [Employeet, setEmployeet] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  // 0 = të gjithë nxënësit
  const [filterEmployeeId, setFilterEmployeeId] = useState<number>(0);

  // ===========================
  // NGARKIM I TË DHËNAVE
  // ===========================
  const fetchEmployeet = async () => {
    try {
      const res = await api.get("/api/Employee");
      setEmployeet(res.data);
    } catch {
      // Nuk e stopojmë faqen nëse dështon Employeet
    }
  };

  const fetchContractt = async () => {
    setLoading(true);
    setError(null);
    try {
      const url =
        filterEmployeeId > 0
          ? `/api/Contract/byEmployee/${filterEmployeeId}`
          : "/api/Contract";
      const res = await api.get(url);
      setContractt(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployeet();
  }, []);

  useEffect(() => {
    fetchContractt();
  }, [filterEmployeeId]);

  // ===========================
  // HAP FORMA
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Contract) => {
    setEditId(n.ID);
    setFormData({
      title: n.title,
      description: n.description,
      ID_Employee: n.ID_Employee,
    });
    setShowForm(true);
  };

  // ===========================
  // DORËZIMI I FORMËS
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.title.trim() || !formData.description.trim()) {
      alert("Emri dhe description janë të detyrueshme!");
      return;
    }
    if (formData.ID_Employee === 0) {
      alert("Duhet të zgjidhni një employee!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Contract/${editId}`, formData);
      } else {
        await api.post("/api/Contract", formData);
      }
      setShowForm(false);
      fetchContractt();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  // ===========================
  // FSHIRJA
  // ===========================
  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt?")) return;
    try {
      await api.delete(`/api/Contract/${id}`);
      fetchContractt();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  // ===========================
  // UI
  // ===========================
  return (
    <div style={{ padding: "20px", maxWidth: "1000px", margin: "0 auto" }}>
      <h1>👨‍🎓 Menaxhimi i Nxënësve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Contract të Ri
      </button>

      {/* FILTRI */}
      <div style={styles.filterBox}>
        <label style={{ marginRight: "10px", fontWeight: "bold" }}>
          🔍 Filtro sipas Employee:
        </label>
        <select
          value={filterEmployeeId}
          onChange={(e) => setFilterEmployeeId(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjitha Employeet —</option>
          {Employeet.map((s) => (
            <option key={s.ID_Employee} value={s.ID_Employee}>
              {s.emriEmployee} ({s.qyteti})
            </option>
          ))}
        </select>
      </div>

      {/* FORMA */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Nxënësin" : "➕ Shto Contract të Ri"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Contracts:</label>
              <input
                type="text"
                value={formData.title}
                onChange={(e) =>
                  setFormData({ ...formData, title: e.target.value })
                }
                placeholder="p.sh. Artan Krasniqi"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>description:</label>
              <input
                type="text"
                value={formData.description}
                onChange={(e) =>
                  setFormData({ ...formData, description: e.target.value })
                }
                placeholder="p.sh. 10A"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Employee:</label>
              <select
                value={formData.ID_Employee}
                onChange={(e) =>
                  setFormData({ ...formData, ID_Employee: Number(e.target.value) })
                }
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Shkollën —</option>
                {Employeet.map((s) => (
                  <option key={s.ID_Employee} value={s.ID_Employee}>
                    {s.emriEmployee} — {s.qyteti}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ display: "flex", gap: "10px" }}>
              <button type="submit" disabled={submitLoading} style={styles.btnSave}>
                {submitLoading ? "Duke ruajtur..." : "💾 Ruaj"}
              </button>
              <button
                type="button"
                onClick={() => setShowForm(false)}
                style={styles.btnCancel}
              >
                ✕ Anulo
              </button>
            </div>
          </form>
        </div>
      )}

      {loading && <p>⏳ Duke ngarkuar...</p>}
      {error && <p style={{ color: "red" }}>❌ {error}</p>}

      {!loading && !error && (
        <>
          <p style={{ color: "#666" }}>
            Gjithsej: <strong>{Contracts.length}</strong> Contract
          </p>
          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>Emri i Contracts</th>
                <th style={styles.th}>description</th>
                <th style={styles.th}>Employee</th>
                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>
            <tbody>
              {Contracts.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: "center", padding: "20px" }}>
                    Nuk ka Contract të regjistruar
                  </td>
                </tr>
              ) : (
                Contracts.map((n) => (
                  <tr key={n.ID} style={styles.tableRow}>
                    <td style={styles.td}>{n.ID}</td>
                    <td style={styles.td}>{n.title}</td>
                    <td style={styles.td}>{n.description}</td>
                    <td style={styles.td}>{n.emriEmployee}</td>
                    <td style={styles.td}>
                      <button onClick={() => handleOpenEdit(n)} style={styles.btnEdit}>
                        ✏️ Edito
                      </button>
                      <button
                        onClick={() => handleDelete(n.ID)}
                        style={styles.btnDelete}
                      >
                        🗑️ Fshi
                      </button>
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </>
      )}
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  btnAdd: { backgroundColor: "#2ecc71", color: "white", padding: "10px 20px", border: "none", borderRadius: "6px", cursor: "pointer", marginBottom: "16px", fontSize: "16px" },
  filterBox: { backgroundColor: "#eaf4fb", border: "1px solid #aed6f1", borderRadius: "6px", padding: "12px 16px", marginBottom: "20px", display: "flex", alignItems: "center" },
  formBox: { backgroundColor: "#f0f4f8", border: "1px solid #ccc", borderRadius: "8px", padding: "20px", marginBottom: "20px" },
  formGroup: { marginBottom: "12px" },
  input: { display: "block", width: "100%", padding: "8px", marginTop: "4px", border: "1px solid #ccc", borderRadius: "4px", fontSize: "14px" },
  select: { padding: "8px", border: "1px solid #ccc", borderRadius: "4px", fontSize: "14px", minWidth: "250px" },
  btnSave: { backgroundColor: "#3498db", color: "white", padding: "8px 20px", border: "none", borderRadius: "4px", cursor: "pointer" },
  btnCancel: { backgroundColor: "#95a5a6", color: "white", padding: "8px 20px", border: "none", borderRadius: "4px", cursor: "pointer" },
  table: { width: "100%", borderCollapse: "collapse", marginTop: "10px" },
  tableHeader: { backgroundColor: "#2c3e50", color: "white" },
  tableRow: { borderBottom: "1px solid #ddd" },
  th: { padding: "12px 15px", textAlign: "left" },
  td: { padding: "10px 15px" },
  btnEdit: { backgroundColor: "#f39c12", color: "white", padding: "5px 12px", border: "none", borderRadius: "4px", cursor: "pointer", marginRight: "8px" },
  btnDelete: { backgroundColor: "#e74c3c", color: "white", padding: "5px 12px", border: "none", borderRadius: "4px", cursor: "pointer" },
};
