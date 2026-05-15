import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ID_Team232470351 mbetet me kapitale — toCamel nuk e prek (fillon me 2+ shkronja të mëdha)
// ===========================

interface Player {
  ID: number;          // Backend kthen "ID" — toCamel e ruan si "ID" (2+ shkronja të mëdha)
  emriPlayer232470351: string;
  number: string;
  ID_Team232470351: number;
  emriTeam232470351: string;
}

interface Team {
  ID_Team232470351: number;
  emriTeam232470351: string;
  qyteti: string;
}

interface FormData {
  emriPlayer232470351: string;
  number: string;
  ID_Team232470351: number;
}

const emptyForm: FormData = { emriPlayer232470351: "", number: "", ID_Team232470351: 0 };

export default function PlayerPage() {
  // ===========================
  // STATE
  // ===========================
  const [Playert, setPlayert] = useState<Player[]>([]);
  const [team232470351, setTeam232470351t] = useState<Team[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  // 0 = të gjithë Playerit
  const [filterTeam232470351Id, setFilterTeam232470351Id] = useState<number>(0);

  // ===========================
  // NGARKIM I TË DHËNAVE
  // ===========================
  const fetchShkollat = async () => {
    try {
      const res = await api.get("/api/Team");
      setTeam232470351t(res.data);
    } catch {
      // Nuk e stopojmë faqen nëse dështon shkollat
    }
  };

  const fetchPlayert = async () => {
    setLoading(true);
    setError(null);
    try {
      const url =
        filterTeam232470351Id > 0
          ? `/api/Player/byShkolla/${filterTeam232470351Id}`
          : "/api/Player";
      const res = await api.get(url);
      setPlayert(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchShkollat();
  }, []);

  useEffect(() => {
    fetchPlayert();
  }, [filterTeam232470351Id]);

  // ===========================
  // HAP FORMA
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Player) => {
    setEditId(n.ID);
    setFormData({
        emriPlayer232470351: n.emriPlayer232470351,
      number: n.number,
      ID_Team232470351: n.ID_Team232470351,
    });
    setShowForm(true);
  };

  // ===========================
  // DORËZIMI I FORMËS
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriPlayer232470351.trim() || !formData.number.trim()) {
      alert("Emri dhe Number janë të detyrueshme!");
      return;
    }
    if (formData.ID_Team232470351 === 0) {
      alert("Duhet të zgjidhni një shkollë!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Player/${editId}`, formData);
      } else {
        await api.post("/api/Player", formData);
      }
      setShowForm(false);
      fetchPlayert();
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
      await api.delete(`/api/Player/${id}`);
      fetchPlayert();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  // ===========================
  // UI
  // ===========================
  return (
    <div style={{ padding: "20px", maxWidth: "1000px", margin: "0 auto" }}>
      <h1>👨‍🎓 Menaxhimi i Players</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Player të Ri
      </button>

      {/* FILTRI */}
      <div style={styles.filterBox}>
        <label style={{ marginRight: "10px", fontWeight: "bold" }}>
          🔍 Filtro sipas shkollës:
        </label>
        <select
          value={filterTeam232470351Id}
          onChange={(e) => setFilterTeam232470351Id(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjitha team —</option>
          {team232470351.map((s) => (
            <option key={s.ID_Team232470351} value={s.ID_Team232470351}>
              {s.emriTeam232470351} 
            </option>
          ))}
        </select>
      </div>

      {/* FORMA */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Playerin" : "➕ Shto Player të Ri"}</h3>
          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Playerit:</label>
              <input
                type="text"
                value={formData.emriPlayer232470351}
                onChange={(e) =>
                  setFormData({ ...formData, emriPlayer232470351: e.target.value})
                }
                placeholder="p.sh. Artan Krasniqi"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Number:</label>
              <input
                type="text"
                value={formData.number}
                onChange={(e) =>
                  setFormData({ ...formData, number: e.target.value })
                }
                placeholder="p.sh. 10A"
                style={styles.input}
                required
              />
            </div>

            <div style={styles.formGroup}>
              <label>Team:</label>
              <select
                value={formData.ID_Team232470351}
                onChange={(e) =>
                  setFormData({ ...formData, ID_Team232470351: Number(e.target.value) })
                }
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Player —</option>
                {team232470351.map((s) => (
                  <option key={s.ID_Team232470351} value={s.ID_Team232470351}>
                    {s.emriTeam232470351} 
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
            Gjithsej: <strong>{Playert.length}</strong> Player
          </p>
          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>Emri i Playerit</th>
                <th style={styles.th}>Number</th>
                <th style={styles.th}>Team</th>
                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>
            <tbody>
              {Playert.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: "center", padding: "20px" }}>
                    Nuk ka Player të regjistruar
                  </td>
                </tr>
              ) : (
                Playert.map((n) => (
                  <tr key={n.ID} style={styles.tableRow}>
                    <td style={styles.td}>{n.ID}</td>
                    <td style={styles.td}>{n.emriPlayer232470351}</td>
                    <td style={styles.td}>{n.number}</td>
                    <td style={styles.td}>{n.emriTeam232470351}</td>
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
