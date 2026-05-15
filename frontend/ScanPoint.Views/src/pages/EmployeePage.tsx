import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ID_Employee mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
interface Employee {
  ID_Employee: number;
  emriEmployee: string;
  mbiemriEmployee: string;
}

interface FormData {
  emriEmployee: string;
  mbiemriEmployee: string;
}

const emptyForm: FormData = { emriEmployee: "", mbiemriEmployee: "" };

export default function EmployeePage() {
  const [Employeet, setEmployeet] = useState<Employee[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  const fetchEmployeet = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Employee");
      setEmployeet(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchEmployeet();
  }, []);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (s: Employee) => {
    setEditId(s.ID_Employee);
    setFormData({ emriEmployee: s.emriEmployee, mbiemriEmployee: s.mbiemriEmployee });
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriEmployee.trim() || !formData.mbiemriEmployee.trim()) {
      alert("Të gjitha fushat janë të detyrueshme!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Employee/${editId}`, formData);
      } else {
        await api.post("/api/Employee", formData);
      }
      setShowForm(false);
      fetchEmployeet();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt që dëshiron të fshish këtë employee?")) return;
    try {
      await api.delete(`/api/Employee/${id}`);
      fetchEmployeet();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
      <h1>🏫 Menaxhimi i Employeeve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Empployee të Re
      </button>

      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Employee" : "➕ Shto Employee të Re"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Employee:</label>
              <input
                type="text"
                value={formData.emriEmployee}
                onChange={(e) =>
                  setFormData({ ...formData, emriEmployee: e.target.value })
                }
                placeholder="p.sh. Employee 'Eqrem Çabej'"
                style={styles.input}
                required
              />
            </div>
            <div style={styles.formGroup}>
              <label>mbiemriEmployee:</label>
              <input
                type="text"
                value={formData.mbiemriEmployee}
                onChange={(e) =>
                  setFormData({ ...formData, mbiemriEmployee: e.target.value })
                }
                placeholder="p.sh. Prishtinë"
                style={styles.input}
                required
              />
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
        <table style={styles.table}>
          <thead>
            <tr style={styles.tableHeader}>
              <th style={styles.th}>ID</th>
              <th style={styles.th}>Emri i Shkollës</th>
              <th style={styles.th}>mbiemriEmployee</th>
              <th style={styles.th}>Veprimet</th>
            </tr>
          </thead>
          <tbody>
            {Employeet.length === 0 ? (
              <tr>
                <td colSpan={4} style={{ textAlign: "center", padding: "20px" }}>
                  Nuk ka Employee të regjistruara
                </td>
              </tr>
            ) : (
              Employeet.map((s) => (
                <tr key={s.ID_Employee} style={styles.tableRow}>
                  <td style={styles.td}>{s.ID_Employee}</td>
                  <td style={styles.td}>{s.emriEmployee}</td>
                  <td style={styles.td}>{s.mbiemriEmployee}</td>
                  <td style={styles.td}>
                    <button onClick={() => handleOpenEdit(s)} style={styles.btnEdit}>
                      ✏️ Edito
                    </button>
                    <button
                      onClick={() => handleDelete(s.ID_Employee)}
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
      )}
    </div>
  );
}

const styles: { [key: string]: React.CSSProperties } = {
  btnAdd: { backgroundColor: "#2ecc71", color: "white", padding: "10px 20px", border: "none", borderRadius: "6px", cursor: "pointer", marginBottom: "20px", fontSize: "16px" },
  formBox: { backgroundColor: "#f0f4f8", border: "1px solid #ccc", borderRadius: "8px", padding: "20px", marginBottom: "20px" },
  formGroup: { marginBottom: "12px" },
  input: { display: "block", width: "100%", padding: "8px", marginTop: "4px", border: "1px solid #ccc", borderRadius: "4px", fontSize: "14px" },
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
