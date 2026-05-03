import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ===========================

interface Nxenesi {
  id: number;
  emriNxenesit: string;
  klasa: string;
  ID_Shkolla: number;
  emriShkolles: string;
}

interface Shkolla {
  ID_Shkolla: number;
  emriShkolles: string;
  qyteti: string;
}

interface FormData {
  emriNxenesit: string;
  klasa: string;
  ID_Shkolla: number; // Çelësi i huaj — lidh nxënësin me shkollën
}

const emptyForm: FormData = { emriNxenesit: "", klasa: "", ID_Shkolla: 0 };

export default function NxenesiPage() {
  // ===========================
  // STATE
  // ===========================
  const [nxenesit, setNxenesit] = useState<Nxenesi[]>([]);
  const [shkollat, setShkollat] = useState<Shkolla[]>([]); // Për dropdown
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);

  // Filtri — shfaq nxënësit e një shkolle specifike
  // 0 = të gjithë nxënësit
  const [filterShkollaId, setFilterShkollaId] = useState<number>(0);

  // ===========================
  // NGARKIM I TË DHËNAVE
  // ===========================

  // Ngarko listën e shkollave — për dropdown
  const fetchShkollat = async () => {
    try {
      const res = await api.get("/api/Shkolla");
      setShkollat(res.data);
    } catch {
      // Nuk e stopojmë faqen nëse dështon shkollat
    }
  };

  // Ngarko nxënësit — me ose pa filtër
  const fetchNxenesit = async () => {
    setLoading(true);
    setError(null);
    try {
      let url = "/api/Nxenesi"; // Të gjithë nxënësit
      if (filterShkollaId > 0) {
        url = `/api/Nxenesi/byShkolla/${filterShkollaId}`; // Filtro sipas shkollës
      }
      const res = await api.get(url);
      setNxenesit(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  // Ngarko të dyja kur faqja hapet
  useEffect(() => {
    fetchShkollat();
  }, []);

  // Kur ndryshon filtri, ringarko nxënësit
  useEffect(() => {
    fetchNxenesit();
  }, [filterShkollaId]); // filterShkollaId është varësia

  // ===========================
  // HAP FORMA
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Nxenesi) => {
    setEditId(n.id);
    setFormData({
      emriNxenesit: n.emriNxenesit,
      klasa: n.klasa,
      ID_Shkolla: n.ID_Shkolla,
    });
    setShowForm(true);
  };

  // ===========================
  // DORËZIMI I FORMËS
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriNxenesit.trim() || !formData.klasa.trim()) {
      alert("Emri dhe klasa janë të detyrueshme!");
      return;
    }
    if (formData.ID_Shkolla === 0) {
      alert("Duhet të zgjidhni një shkollë!");
      return;
    }

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        // PUT — përditëso
        await api.put(`/api/Nxenesi/${editId}`, formData);
      } else {
        // POST — krijo
        await api.post("/api/Nxenesi", formData);
      }
      setShowForm(false);
      fetchNxenesit(); // Ringarko
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
      await api.delete(`/api/Nxenesi/${id}`);
      fetchNxenesit();
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

      {/* Butoni për shtim */}
      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Nxënës të Ri
      </button>

      {/* ===========================
          FILTRI SIPAS SHKOLLËS
          Dropdown me të gjitha shkollat
          =========================== */}
      <div style={styles.filterBox}>
        <label style={{ marginRight: "10px", fontWeight: "bold" }}>
          🔍 Filtro sipas shkollës:
        </label>
        <select
          value={filterShkollaId}
          onChange={(e) => setFilterShkollaId(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjitha shkollat —</option>
          {shkollat.map((s) => (
            <option key={s.ID_Shkolla} value={s.ID_Shkolla}>
              {s.emriShkolles} ({s.qyteti})
            </option>
          ))}
        </select>
      </div>

      {/* Forma e shtimit/editimit */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Nxënësin" : "➕ Shto Nxënës të Ri"}</h3>
          <form onSubmit={handleSubmit}>

            {/* Emri */}
            <div style={styles.formGroup}>
              <label>Emri i Nxënësit:</label>
              <input
                type="text"
                value={formData.emriNxenesit}
                onChange={(e) => setFormData({ ...formData, emriNxenesit: e.target.value })}
                placeholder="p.sh. Artan Krasniqi"
                style={styles.input}
                required
              />
            </div>

            {/* Klasa */}
            <div style={styles.formGroup}>
              <label>Klasa:</label>
              <input
                type="text"
                value={formData.klasa}
                onChange={(e) => setFormData({ ...formData, klasa: e.target.value })}
                placeholder="p.sh. 10A"
                style={styles.input}
                required
              />
            </div>

            {/* Dropdown për zgjedhjen e shkollës
                Gjatë krijimit, user zgjedh shkollën ekzistuese nga ky dropdown
                Gjatë editimit, forma mbushet me shkollën aktuale */}
            <div style={styles.formGroup}>
              <label>Shkolla:</label>
              <select
                value={formData.ID_Shkolla.toString()}
                onChange={(e) => setFormData({ ...formData, ID_Shkolla: Number(e.target.value) })}
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Shkollën —</option>
                {shkollat.map((s) => (
                  <option key={s.ID_Shkolla} value={s.ID_Shkolla}>
                    {s.emriShkolles} — {s.qyteti}
                  </option>
                ))}
              </select>
            </div>

            <div style={{ display: "flex", gap: "10px" }}>
              <button type="submit" disabled={submitLoading} style={styles.btnSave}>
                {submitLoading ? "Duke ruajtur..." : "💾 Ruaj"}
              </button>
              <button type="button" onClick={() => setShowForm(false)} style={styles.btnCancel}>
                ✕ Anulo
              </button>
            </div>
          </form>
        </div>
      )}

      {/* Mesazhet */}
      {loading && <p>⏳ Duke ngarkuar...</p>}
      {error && <p style={{ color: "red" }}>❌ {error}</p>}

      {/* Tabela e nxënësve */}
      {!loading && !error && (
        <>
          <p style={{ color: "#666" }}>
            Gjithsej: <strong>{nxenesit.length}</strong> nxënës
          </p>
          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>Emri i Nxënësit</th>
                <th style={styles.th}>Klasa</th>
                <th style={styles.th}>Shkolla</th>
                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>
            <tbody>
              {nxenesit.length === 0 ? (
                <tr>
                  <td colSpan={5} style={{ textAlign: "center", padding: "20px" }}>
                    Nuk ka nxënës të regjistruar
                  </td>
                </tr>
              ) : (
                nxenesit.map((n) => (
                  <tr key={n.id} style={styles.tableRow}>
                    <td style={styles.td}>{n.id}</td>
                    <td style={styles.td}>{n.emriNxenesit}</td>
                    <td style={styles.td}>{n.klasa}</td>
                    <td style={styles.td}>{n.emriShkolles}</td>
                    <td style={styles.td}>
                      <button onClick={() => handleOpenEdit(n)} style={styles.btnEdit}>
                        ✏️ Edito
                      </button>
                      <button onClick={() => handleDelete(n.id)} style={styles.btnDelete}>
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
  select: { padding: "8px", marginLeft: "0", border: "1px solid #ccc", borderRadius: "4px", fontSize: "14px", minWidth: "250px" },
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
