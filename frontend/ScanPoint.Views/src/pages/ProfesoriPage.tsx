import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ID_Universiteti mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
// ===========================

interface Profesori {
  ID: number;          // Backend kthen "ID" — toCamel e ruan si "ID" (2+ shkronja të mëdha)
  emriProfesorit: string;
  lenda: string;
  ID_Universiteti: number;
  emriUniversitetit: string;
}

interface Universiteti {
  ID_Universiteti: number;
  emriUniversitetit: string;
  shteti: string;
}

interface FormData {
  emriProfesorit: string;
  lenda: string;
  ID_Universiteti: number;
}

const emptyForm: FormData = { emriProfesorit: "", lenda: "", ID_Universiteti: 0 };

export default function ProfesoriPage() {
  // ===========================
  // STATE
  // ===========================
  const [Profesorit, setProfesorit] = useState<Profesori[]>([]);
  const [Universitetit, setUniversitetit] = useState<Universiteti[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  // 0 = të gjithë nxënësit
  const [filterUniversitetiId, setFilterUniversitetiId] = useState<number>(0);

  // ===========================
  // NGARKIM I TË DHËNAVE
  // ===========================
  const fetchUniversitetit = async () => {
    try {
      const res = await api.get("/api/Universiteti");
      setUniversitetit(res.data);
    } catch {
      // Nuk e stopojmë faqen nëse dështon Universitetit
    }
  };

  const fetchProfesorit = async () => {
    setLoading(true);
    setError(null);
    try {
      const url =
        filterUniversitetiId > 0
          ? `/api/Profesori/byUniversiteti/${filterUniversitetiId}`
          : "/api/Profesori";
      const res = await api.get(url);
      setProfesorit(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchUniversitetit();
  }, []);

  useEffect(() => {
    fetchProfesorit();
  }, [filterUniversitetiId]);

  // ===========================
  // HAP FORMA
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Profesori) => {
    setEditId(n.ID);
    setFormData({
      emriProfesorit: n.emriProfesorit,
      lenda: n.lenda,
      ID_Universiteti: n.ID_Universiteti,
    });
    setShowForm(true);
  };

  // ===========================
  // DORËZIMI I FORMËS
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriProfesorit.trim() || !formData.lenda.trim()) {
      alert("Emri dhe lenda janë të detyrueshme!");
      return;
    }
    if (formData.ID_Universiteti === 0) {
      alert("Duhet të zgjidhni një Universitet!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Profesori/${editId}`, formData);
      } else {
        await api.post("/api/Profesori", formData);
      }
      setShowForm(false);
      fetchProfesorit();
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
      await api.delete(`/api/Profesori/${id}`);
      fetchProfesorit();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  // ===========================
  // UI
  // ===========================
  return (
    <div style={{ padding: "20px", maxWidth: "1000px", margin: "0 auto" }}>
      <h1>👨‍🎓 Menaxhimi i Profesoreve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto profesore të Ri
      </button>

      {/* FILTRI */}
      <div style={styles.filterBox}>
        <label style={{ marginRight: "10px", fontWeight: "bold" }}>
          🔍 Filtro sipas uni-te:
        </label>
        <select
          value={filterUniversitetiId}
          onChange={(e) => setFilterUniversitetiId(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjitha Universitetit —</option>
          {Universitetit.map((s) => (
            <option key={s.ID_Universiteti} value={s.ID_Universiteti}>
              {s.emriUniversitetit} ({s.shteti})
            </option>
          ))}
        </select>
      </div>

      {/* FORMA */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Nxënësin" : "➕ Shto Nxënës të Ri"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i profesorit:</label>
              <input
                type="text"
                value={formData.emriProfesorit}
                onChange={(e) =>
                  setFormData({ ...formData, emriProfesorit: e.target.value })
                }
                placeholder="p.sh. Artan Krasniqi"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>lenda:</label>
              <input
                type="text"
                value={formData.lenda}
                onChange={(e) =>
                  setFormData({ ...formData, lenda: e.target.value })
                }
                placeholder="p.sh. 10A"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Universiteti:</label>
              <select
                value={formData.ID_Universiteti}
                onChange={(e) =>
                  setFormData({ ...formData, ID_Universiteti: Number(e.target.value) })
                }
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Uni-n —</option>
                {Universitetit.map((s) => (
                  <option key={s.ID_Universiteti} value={s.ID_Universiteti}>
                    {s.emriUniversitetit} — {s.shteti}
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
            Gjithsej: <strong>{Profesorit.length}</strong> profesore
          </p>
          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>Emri i Nxënësit</th>
                <th style={styles.th}>lenda</th>
                <th style={styles.th}>Universiteti</th>
                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>
            <tbody>
              {Profesorit.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: "center", padding: "20px" }}>
                    Nuk ka profesore të regjistruar
                  </td>
                </tr>
              ) : (
                Profesorit.map((n) => (
                  <tr key={n.ID} style={styles.tableRow}>
                    <td style={styles.td}>{n.ID}</td>
                    <td style={styles.td}>{n.emriProfesorit}</td>
                    <td style={styles.td}>{n.lenda}</td>
                    <td style={styles.td}>{n.emriUniversitetit}</td>
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
