import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ===========================
// TIPET
// ===========================

interface Satelite {
  ID: number;
  emriSatelitit: string;
  isDeleted: boolean;
  ID_Planet: number;
  emriPlanetit: string;
}

interface Planet {
  ID_Planet: number;
  emriPlanetit: string;
  type: string;
}

interface FormData {
  emriSatelitit: string;
  ID_Planet: number;
}

const emptyForm: FormData = {
  emriSatelitit: "",
  ID_Planet: 0,
};

export default function SatelitePage() {
  // ===========================
  // STATE
  // ===========================

  const [satelitet, setSatelitet] = useState<Satelite[]>([]);
  const [planetet, setPlanetet] = useState<Planet[]>([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);

  const [editId, setEditId] = useState<number | null>(null);

  const [formData, setFormData] = useState<FormData>(emptyForm);

  const [submitLoading, setSubmitLoading] = useState(false);

  // 0 = të gjithë satelitët
  const [filterPlanetId, setFilterPlanetId] = useState<number>(0);

  // ===========================
  // NGARKO PLANETËT
  // ===========================
  const fetchPlanetet = async () => {
    try {
      const res = await api.get("/api/Planentete");

      setPlanetet(res.data);
    } catch {
      // nuk e ndalim faqen
    }
  };

  // ===========================
  // NGARKO SATELITËT
  // ===========================
  const fetchSatelitet = async () => {
    setLoading(true);
    setError(null);

    try {
      const url =
        filterPlanetId > 0
          ? `/api/Satelitet/byPlanet/${filterPlanetId}`
          : "/api/Satelitet";

      const res = await api.get(url);

      setSatelitet(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlanetet();
  }, []);

  useEffect(() => {
    fetchSatelitet();
  }, [filterPlanetId]);

  // ===========================
  // HAP FORMËN
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  const handleOpenEdit = (s: Satelite) => {
    setEditId(s.ID);

    setFormData({
      emriSatelitit: s.emriSatelitit,

      ID_Planet: s.ID_Planet,
    });

    setShowForm(true);
  };

  // ===========================
  // SUBMIT
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!formData.emriSatelitit.trim()) {
      alert("Emri i satelitit është i detyrueshëm!");
      return;
    }

    if (formData.ID_Planet === 0) {
      alert("Duhet të zgjidhni një planet!");
      return;
    }

    setSubmitLoading(true);

    try {
      if (editId !== null) {
        await api.put(`/api/Satelitet/${editId}`, formData);
      } else {
        await api.post("/api/Satelitet", formData);
      }

      setShowForm(false);

      fetchSatelitet();
    } catch (err: any) {
      alert(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  // ===========================
  // DELETE
  // ===========================
  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt?")) return;

    try {
      await api.delete(`/api/Satelitet/${id}`);

      fetchSatelitet();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  // ===========================
  // UI
  // ===========================
  return (
    <div
      style={{
        padding: "20px",
        maxWidth: "1000px",
        margin: "0 auto",
      }}
    >
      <h1>🛰️ Menaxhimi i Satelitëve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Satelit të Ri
      </button>

      {/* FILTER */}
      <div style={styles.filterBox}>
        <label
          style={{
            marginRight: "10px",
            fontWeight: "bold",
          }}
        >
          🔍 Filtro sipas planetit:
        </label>

        <select
          value={filterPlanetId}
          onChange={(e) => setFilterPlanetId(Number(e.target.value))}
          style={styles.select}
        >
          <option value={0}>— Të gjithë planetët —</option>

          {planetet.map((p) => (
            <option key={p.ID_Planet} value={p.ID_Planet}>
              {p.emriPlanetit} ({p.type})
            </option>
          ))}
        </select>
      </div>

      {/* FORMA */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Satelitin" : "➕ Shto Satelit të Ri"}</h3>

          <form onSubmit={handleSubmit}>
            {/* EMRI */}
            <div style={styles.formGroup}>
              <label>Emri i Satelitit:</label>

              <input
                type="text"
                value={formData.emriSatelitit}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    emriSatelitit: e.target.value,
                  })
                }
                placeholder="p.sh. Europa"
                style={styles.input}
                required
              />
            </div>

            {/* PLANETI */}
            <div style={styles.formGroup}>
              <label>Planeti:</label>

              <select
                value={formData.ID_Planet}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    ID_Planet: Number(e.target.value),
                  })
                }
                style={styles.select}
                required
              >
                <option value={0}>— Zgjedh Planetin —</option>

                {planetet.map((p) => (
                  <option key={p.ID_Planet} value={p.ID_Planet}>
                    {p.emriPlanetit}
                    {" — "}
                    {p.type}
                  </option>
                ))}
              </select>
            </div>

            {/* BUTTONS */}
            <div
              style={{
                display: "flex",
                gap: "10px",
              }}
            >
              <button
                type="submit"
                disabled={submitLoading}
                style={styles.btnSave}
              >
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

      {/* LOADING */}
      {loading && <p>⏳ Duke ngarkuar...</p>}

      {/* ERROR */}
      {error && <p style={{ color: "red" }}>❌ {error}</p>}

      {/* TABLE */}
      {!loading && !error && (
        <>
          <p style={{ color: "#666" }}>
            Gjithsej: <strong>{satelitet.length}</strong> satelitë
          </p>

          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>

                <th style={styles.th}>Emri i Satelitit</th>

                <th style={styles.th}>Planeti</th>

                <th style={styles.th}>Veprimet</th>
              </tr>
            </thead>

            <tbody>
              {satelitet.length === 0 ? (
                <tr>
                  <td
                    colSpan={4}
                    style={{
                      textAlign: "center",
                      padding: "20px",
                    }}
                  >
                    Nuk ka satelitë të regjistruar
                  </td>
                </tr>
              ) : (
                satelitet.map((s) => (
                  <tr key={s.ID} style={styles.tableRow}>
                    <td style={styles.td}>{s.ID}</td>

                    <td style={styles.td}>{s.emriSatelitit}</td>

                    <td style={styles.td}>{s.emriPlanetit}</td>

                    <td style={styles.td}>
                      <button
                        onClick={() => handleOpenEdit(s)}
                        style={styles.btnEdit}
                      >
                        ✏️ Edito
                      </button>

                      <button
                        onClick={() => handleDelete(s.ID)}
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