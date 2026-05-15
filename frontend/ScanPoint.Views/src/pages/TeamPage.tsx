import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ID_Team mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
interface Team {
  ID_Team232470351: number;
  emriTeam232470351: string;
  
}

interface FormData {
  emriTeam232470351: string;
  
}

const emptyForm: FormData = { emriTeam232470351: ""};

export default function TeamPage() {
  const [Teamt, setTeamt] = useState<Team[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  const fetchTeamt = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Team");
      setTeamt(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTeamt();
  }, []);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (s: Team) => {
    setEditId(s.ID_Team232470351);
    setFormData({ emriTeam232470351: s.emriTeam232470351});
    setShowForm(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriTeam232470351.trim()) {
      alert("Të gjitha fushat janë të detyrueshme!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Team/${editId}`, formData);
      } else {
        await api.post("/api/Team", formData);
      }
      setShowForm(false);
      fetchTeamt();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt që dëshiron të fshish këtë Teamë?")) return;
    try {
      await api.delete(`/api/Team/${id}`);
      fetchTeamt();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
      <h1>🏫 Menaxhimi i Teamve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Team të Re
      </button>

      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Teamën" : "➕ Shto Teamë të Re"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Teamës:</label>
              <input
                type="text"
                value={formData.emriTeam232470351}
                onChange={(e) =>
                  setFormData({ ...formData, emriTeam232470351: e.target.value })
                }
                placeholder="p.sh. Team 'Eqrem Çabej'"
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
              <th style={styles.th}>Emri i Teamës</th>
             
              <th style={styles.th}>Veprimet</th>
            </tr>
          </thead>
          <tbody>
            {Teamt.length === 0 ? (
              <tr>
                <td colSpan={4} style={{ textAlign: "center", padding: "20px" }}>
                  Nuk ka Team të regjistruara
                </td>
              </tr>
            ) : (
              Teamt.map((s) => (
                <tr key={s.ID_Team232470351} style={styles.tableRow}>
                  <td style={styles.td}>{s.ID_Team232470351}</td>
                  <td style={styles.td}>{s.emriTeam232470351}</td>
                
                  <td style={styles.td}>
                    <button onClick={() => handleOpenEdit(s)} style={styles.btnEdit}>
                      ✏️ Edito
                    </button>
                    <button
                      onClick={() => handleDelete(s.ID_Team232470351)}
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
