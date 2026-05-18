
import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

interface Spitali {
  ID_Spitali: number;
  emriSpitalit: string;
  numriKateve: number;
  kaUrgjence: boolean;
  dataHapjes: string;
}

interface FormData {
  emriSpitalit: string;
  numriKateve: number | "";   
  kaUrgjence: boolean;
  dataHapjes: string;
}

const emptyForm: FormData = {
  emriSpitalit: "",
  numriKateve: "",           
  kaUrgjence: false,
  dataHapjes: "",
};


const formatDate = (iso: string): string => {
  if (!iso) return "";
  const d = new Date(iso);
  if (isNaN(d.getTime())) return iso;
  return d.toLocaleDateString("sq-AL");
};


const toInputDate = (iso: string): string => {
  if (!iso) return "";
  return iso.split("T")[0];
};

export default function SpitaliPage() {
  const [Spitalit, setSpitalit] = useState<Spitali[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null); // ✅ FIX 2: gabim inline, jo alert()

  const fetchSpitalit = async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await api.get("/api/Spitali");
      setSpitalit(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSpitalit();
  }, []);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setFormError(null);        
    setShowForm(true);
  };

  const handleOpenEdit = (s: Spitali) => {
    setEditId(s.ID_Spitali);
    setFormData({
      emriSpitalit: s.emriSpitalit,
      numriKateve: s.numriKateve,
      kaUrgjence: s.kaUrgjence,
      dataHapjes: toInputDate(s.dataHapjes),
    });
    setFormError(null);      
    setShowForm(true);
  };

  const handleCancel = () => {
    setShowForm(false);
    setFormError(null);       
    setFormData(emptyForm);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError(null);

   
    if (!formData.emriSpitalit.trim()) {
      setFormError("Emri i spitalit është i detyrueshëm.");
      return;
    }
    if (formData.numriKateve === "" || Number(formData.numriKateve) <= 0) {
      setFormError("Numri i kateve duhet të jetë më i madh se 0.");
      return;
    }
    if (!formData.dataHapjes.trim()) {
      setFormError("Data e hapjes është e detyrueshme.");
      return;
    }


    const payload = {
      ...formData,
      numriKateve: Number(formData.numriKateve),
    };

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Spitali/${editId}`, payload);
      } else {
        await api.post("/api/Spitali", payload);
      }
      setShowForm(false);
      setFormData(emptyForm);
      fetchSpitalit();
    } catch (err: any) {
      setFormError(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt që dëshiron të fshish këtë Spital?")) return;
    try {
      await api.delete(`/api/Spitali/${id}`);
      fetchSpitalit();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div style={{ padding: "20px", maxWidth: "900px", margin: "0 auto" }}>
      <h1>🏥 Menaxhimi i Spitaleve</h1>

      <button onClick={handleOpenAdd} style={styles.btnAdd}>
        + Shto Spital të Ri
      </button>

      {showForm && (
        <div style={styles.formBox}>
          <h3>{editId ? "✏️ Edito Spitalin" : "➕ Shto Spital të Ri"}</h3>

          {/* ✅ FIX 2: gabim inline mbi formë, jo alert() */}
          {formError && (
            <p style={styles.formErrorMsg}>⚠️ {formError}</p>
          )}

          <form onSubmit={handleSubmit}>
            <div style={styles.formGroup}>
              <label>Emri i Spitalit:</label>
              <input
                type="text"
                value={formData.emriSpitalit}
                onChange={(e) =>
                  setFormData({ ...formData, emriSpitalit: e.target.value })
                }
                placeholder="p.sh. Spitali 'Eqrem Çabej'"
                style={styles.input}
              />
            </div>

            <div style={styles.formGroup}>
              <label>Numri i kateve:</label>
              <input
                type="number"
                value={formData.numriKateve}
                min={1}                           
                onChange={(e) =>
                  setFormData({
                    ...formData,
                   
                    numriKateve: e.target.value === "" ? "" : Number(e.target.value),
                  })
                }
                placeholder="p.sh. 5"
                style={styles.input}
              />
            </div>

            <div style={styles.formGroup}>
              <label style={{ display: "flex", alignItems: "center", gap: "8px", cursor: "pointer" }}>
                <input
                  type="checkbox"
                  checked={formData.kaUrgjence}
                  onChange={(e) =>
                    setFormData({ ...formData, kaUrgjence: e.target.checked })
                  }
                />
                Ka Urgjencë
              </label>
            </div>

            <div style={styles.formGroup}>
              <label>Data e Hapjes:</label>
              <input
                type="date"
                value={formData.dataHapjes}
                max={new Date().toISOString().split("T")[0]} // ✅ FIX 7: ndalon data të ardhshme
                onChange={(e) =>
                  setFormData({ ...formData, dataHapjes: e.target.value })
                }
                style={styles.input}
              />
            </div>

            <div style={{ display: "flex", gap: "10px" }}>
              <button type="submit" disabled={submitLoading} style={styles.btnSave}>
                {submitLoading ? "Duke ruajtur..." : "💾 Ruaj"}
              </button>
              <button type="button" onClick={handleCancel} style={styles.btnCancel}>
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
              <th style={styles.th}>Emri i Spitalit</th>
              <th style={styles.th}>Numri i kateve</th>
              <th style={styles.th}>Ka Urgjencë</th>
              <th style={styles.th}>Data e Hapjes</th>
              <th style={styles.th}>Veprime</th>
            </tr>
          </thead>
          <tbody>
            {Spitalit.length === 0 ? (
              <tr>
                <td colSpan={6} style={{ textAlign: "center", padding: "20px" }}>
                  Nuk ka Spitale të regjistruara
                </td>
              </tr>
            ) : (
              Spitalit.map((s) => (
                <tr key={s.ID_Spitali} style={styles.tableRow}>
                  <td style={styles.td}>{s.ID_Spitali}</td>
                  <td style={styles.td}>{s.emriSpitalit}</td>
                  <td style={styles.td}>{s.numriKateve}</td>
                  <td style={styles.td}>{s.kaUrgjence ? "Po" : "Jo"}</td>
                  <td style={styles.td}>{formatDate(s.dataHapjes)}</td>
                  <td style={styles.td}>
                    <button onClick={() => handleOpenEdit(s)} style={styles.btnEdit}>
                      ✏️ Edito
                    </button>
                    <button onClick={() => handleDelete(s.ID_Spitali)} style={styles.btnDelete}>
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
  formErrorMsg: { backgroundColor: "#fdecea", border: "1px solid #e74c3c", color: "#c0392b", padding: "10px 14px", borderRadius: "4px", marginBottom: "12px" },
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
