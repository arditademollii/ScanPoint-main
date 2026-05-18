import { useEffect, useState } from "react";
import api from "../api/axiosInstance";


interface Shkolla {
  ID_Shkolla: number;
  emriShkolles: string;
  qyteti: string;
}

interface FormData {
  emriShkolles: string;
  qyteti: string;
}

const emptyForm: FormData = { emriShkolles: "", qyteti: "" };

export default function ShkollaPage() {
  const [shkollat, setShkollat] = useState<Shkolla[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  const fetchShkollat = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Shkolla");
      setShkollat(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchShkollat();
  }, []);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (s: Shkolla) => {
    setEditId(s.ID_Shkolla);
    setFormData({ emriShkolles: s.emriShkolles, qyteti: s.qyteti });
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriShkolles.trim() || !formData.qyteti.trim()) {
      alert("Të gjitha fushat janë të detyrueshme!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Shkolla/${editId}`, formData);
      } else {
        await api.post("/api/Shkolla", formData);
      }
      setShowForm(false);
      fetchShkollat();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt që dëshiron të fshish këtë shkollë?")) return;
    try {
      await api.delete(`/api/Shkolla/${id}`);
      fetchShkollat();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
      <h1>🏫 Menaxhimi i Shkollave</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Shkollë të Re
      </button>

      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Shkollën" : "➕ Shto Shkollë të Re"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Shkollës:</label>
              <input
                type="text"
                value={formData.emriShkolles}
                onChange={(e) =>
                  setFormData({ ...formData, emriShkolles: e.target.value })
                }
                placeholder="p.sh. Shkolla 'Eqrem Çabej'"
                style={styles.input}
                required
              />
            </div>
            <div style={styles.formGroup}>
              <label>Qyteti:</label>
              <input
                type="text"
                value={formData.qyteti}
                onChange={(e) =>
                  setFormData({ ...formData, qyteti: e.target.value })
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
              <th style={styles.th}>Qyteti</th>
              <th style={styles.th}>Veprimet</th>
            </tr>
          </thead>
          <tbody>
            {shkollat.length === 0 ? (
              <tr>
                <td colSpan={4} style={{ textAlign: "center", padding: "20px" }}>
                  Nuk ka shkolla të regjistruara
                </td>
              </tr>
            ) : (
              shkollat.map((s) => (
                <tr key={s.ID_Shkolla} style={styles.tableRow}>
                  <td style={styles.td}>{s.ID_Shkolla}</td>
                  <td style={styles.td}>{s.emriShkolles}</td>
                  <td style={styles.td}>{s.qyteti}</td>
                  <td style={styles.td}>
                    <button onClick={() => handleOpenEdit(s)} style={styles.btnEdit}>
                      ✏️ Edito
                    </button>
                    <button
                      onClick={() => handleDelete(s.ID_Shkolla)}
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
