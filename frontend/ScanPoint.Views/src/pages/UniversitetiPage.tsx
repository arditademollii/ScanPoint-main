import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ID_Universiteti mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
interface Universiteti {
  ID_Universiteti: number;
  emriUniversitetit: string;
  shteti: string;
}

interface FormData {
  emriUniversitetit: string;
  shteti: string;
}

const emptyForm: FormData = { emriUniversitetit: "", shteti: "" };

export default function UniversitetiPage() {
  const [Universitetit, setUniversitetit] = useState<Universiteti[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  const fetchUniversitetit = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Universiteti");
      setUniversitetit(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUniversitetit();
  }, []);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (s: Universiteti) => {
    setEditId(s.ID_Universiteti);
    setFormData({ emriUniversitetit: s.emriUniversitetit, shteti: s.shteti });
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriUniversitetit.trim() || !formData.shteti.trim()) {
      alert("Të gjitha fushat janë të detyrueshme!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Universiteti/${editId}`, formData);
      } else {
        await api.post("/api/Universiteti", formData);
      }
      setShowForm(false);
      fetchUniversitetit();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt që dëshiron të fshish këtë universitet?")) return;
    try {
      await api.delete(`/api/Universiteti/${id}`);
      fetchUniversitetit();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
      <h1>🏫 Menaxhimi i Universitetive</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto universitet të Re
      </button>

      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito univeristetin" : "➕ Shto univeristetin e Ri"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Shkollës:</label>
              <input
                type="text"
                value={formData.emriUniversitetit}
                onChange={(e) =>
                  setFormData({ ...formData, emriUniversitetit: e.target.value })
                }
                placeholder="p.sh. Universiteti 'Eqrem Çabej'"
                style={styles.input}
                required
              />
            </div>
            <div style={styles.formGroup}>
              <label>shteti:</label>
              <input
                type="text"
                value={formData.shteti}
                onChange={(e) =>
                  setFormData({ ...formData, shteti: e.target.value })
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
              <th style={styles.th}>shteti</th>
              <th style={styles.th}>Veprimet</th>
            </tr>
          </thead>
          <tbody>
            {Universitetit.length === 0 ? (
              <tr>
                <td colSpan={4} style={{ textAlign: "center", padding: "20px" }}>
                  Nuk ka Universiteti të regjistruara
                </td>
              </tr>
            ) : (
              Universitetit.map((s) => (
                <tr key={s.ID_Universiteti} style={styles.tableRow}>
                  <td style={styles.td}>{s.ID_Universiteti}</td>
                  <td style={styles.td}>{s.emriUniversitetit}</td>
                  <td style={styles.td}>{s.shteti}</td>
                  <td style={styles.td}>
                    <button onClick={() => handleOpenEdit(s)} style={styles.btnEdit}>
                      ✏️ Edito
                    </button>
                    <button
                      onClick={() => handleDelete(s.ID_Universiteti)}
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
