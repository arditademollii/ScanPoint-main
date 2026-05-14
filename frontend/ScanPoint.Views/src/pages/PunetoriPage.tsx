import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ID_Shkolla mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
// ===========================

interface Punetori {
  ID: number;          // Backend kthen "ID" — toCamel e ruan si "ID" (2+ shkronja të mëdha)
  emriPunetorit: string;
  mbiemriPunetorit: string;
  pozita: string;
  ID_Fabrika: number;
  emriFabrikes: string;
}

interface Fabrika {
  ID_Fabrika: number;
  emriFabrikes: string;
  lokacioni: string;
}

interface FormData {
  emriPunetorit: string;
  mbiemriPunetorit: string;
  pozita: string
  ID_Fabrika: number;
}

const emptyForm: FormData = { emriPunetorit: "", mbiemriPunetorit: "",pozita: "", ID_Fabrika: 0 };

export default function PunetoriPage() {
  // ===========================
  // STATE
  // ===========================
  const [punetoret, setPunetorit] = useState<Punetori[]>([]);
  const [fabrikat, setFabrikat] = useState<Fabrika[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  // 0 = të gjithë nxënësit
  const [filterFabrikaId, setFilterFabrikaId] = useState<number>(0);

  // ===========================
  // NGARKIM I TË DHËNAVE
  // ===========================
  const fetchFabrikat = async () => {
    try {
      const res = await api.get("/api/Fabrika");
      setFabrikat(res.data);
    } catch {
      // Nuk e stopojmë faqen nëse dështon shkollat
    }
  };

  const fetchPunetoret = async () => {
    setLoading(true);
    setError(null);
    try {
      const url =
        filterFabrikaId > 0
          ? `/api/Punetori/byFabrika/${filterFabrikaId}`
          : "/api/Punetori";
      const res = await api.get(url);
      setPunetorit(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchFabrikat();
  }, []);

  useEffect(() => {
    fetchPunetoret();
  }, [filterFabrikaId]);

  // ===========================
  // HAP FORMA
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Punetori) => {
    setEditId(n.ID);
    setFormData({
      emriPunetorit: n.emriPunetorit,
      mbiemriPunetorit: n.mbiemriPunetorit,
      pozita: n.pozita,
      ID_Fabrika: n.ID_Fabrika,
    });
    setShowForm(true);
  };

  // ===========================
  // DORËZIMI I FORMËS
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriPunetorit.trim() || !formData.mbiemriPunetorit.trim() || !formData.pozita.trim()) {
      alert("Emri, Mbiemri dhe Pozita janë të detyrueshme!");
      return;
    }
    if (formData.ID_Fabrika === 0) {
      alert("Duhet të zgjidhni një fabrike!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Punetori/${editId}`, formData);
      } else {
        await api.post("/api/Punetori", formData);
      }
      setShowForm(false);
      fetchPunetoret();
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
      await api.delete(`/api/Punetori/${id}`);
      fetchPunetoret();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  // ===========================
  // UI
  // ===========================
  return (
    <div style={{ padding: "20px", maxWidth: "1000px", margin: "0 auto" }}>
      <h1>👨‍🎓 Menaxhimi i Punetoreve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Punetore të Ri
      </button>

      {/* FILTRI */}
      <div style={styles.filterBox}>
        <label style={{ marginRight: "10px", fontWeight: "bold" }}>
          🔍 Filtro sipas fabrikes:
        </label>
        <select
          value={filterFabrikaId}
          onChange={(e) => setFilterFabrikaId(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjitha fabrikat —</option>
          {fabrikat.map((f) => (
            <option key={f.ID_Fabrika} value={f.ID_Fabrika}>
              {f.emriFabrikes} ({f.lokacioni})
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
              <label>Emri i Nxënësit:</label>
              <input
                type="text"
                value={formData.emriPunetorit}
                onChange={(e) =>
                  setFormData({ ...formData, emriPunetorit: e.target.value })
                }
                placeholder="p.sh. Artan Krasniqi"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Mbiemri:</label>
              <input
                type="text"
                value={formData.mbiemriPunetorit}
                onChange={(e) =>
                  setFormData({ ...formData, mbiemriPunetorit: e.target.value })
                }
                placeholder="p.sh. 10A"
                style={styles.input}
                required
              />
            </div>
            
            <div style={styles.formGroup}>
              <label>Pozita:</label>
              <input
                type="text"
                value={formData.pozita}
                onChange={(e) =>
                  setFormData({ ...formData, pozita: e.target.value })
                }
                placeholder="p.sh. IT"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Fabrika:</label>
              <select
                value={formData.ID_Fabrika}
                onChange={(e) =>
                  setFormData({ ...formData, ID_Fabrika: Number(e.target.value) })
                }
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Shkollën —</option>
                {fabrikat.map((s) => (
                  <option key={s.ID_Fabrika} value={s.ID_Fabrika}>
                    {s.emriFabrikes} — {s.lokacioni}
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
            Gjithsej: <strong>{punetoret.length}</strong> punetore
          </p>
          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>Emri i Punetorit</th>
                <th style={styles.th}>Mbiemri i punetorit</th>
                <th style={styles.th}>Pozita i punetorit</th>
                <th style={styles.th}>Fabrika</th>
                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>
            <tbody>
              {punetoret.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: "center", padding: "20px" }}>
                    Nuk ka Punetor të regjistruar
                  </td>
                </tr>
              ) : (
                punetoret.map((n) => (
                  <tr key={n.ID} style={styles.tableRow}>
                    <td style={styles.td}>{n.ID}</td>
                    <td style={styles.td}>{n.emriPunetorit}</td>
                    <td style={styles.td}>{n.mbiemriPunetorit}</td>
                    <td style={styles.td}>{n.pozita}</td>
                    <td style={styles.td}>{n.emriFabrikes}</td>
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
