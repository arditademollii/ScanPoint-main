
import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

interface Mjeku {
  ID: number;
  emriMjekut: string;
  paga: number;
  dataPunesimit: string;
  eshteSpecialist: boolean;
  ID_Spitali: number;
  emriSpitalit: string;
}

interface Spitali {
  ID_Spitali: number;
  emriSpitalit: string;
}

interface FormData {
  emriMjekut: string;
  paga: string;
  dataPunesimit: string;
  eshteSpecialist: boolean;
  ID_Spitali: number;
}

const emptyForm: FormData = {
  emriMjekut: "",
  paga: "",
  dataPunesimit: "",
  eshteSpecialist: false,
  ID_Spitali: 0,
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

export default function MjekuPage() {
  const [Mjekut, setMjekut] = useState<Mjeku[]>([]);
  const [spitalet, setSpitalet] = useState<Spitali[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);
  const [formData, setFormData] = useState<FormData>(emptyForm);
  const [submitLoading, setSubmitLoading] = useState(false);
  const [formError, setFormError] = useState<string | null>(null);

  // ==========================
  // FILTRIMI
  // ==========================

  const [filterSpitaliId, setFilterSpitaliId] = useState<number>(0);

  // ✅ SHEMBULL TJETËR:
  // nëse filtrojmë sipas emrit të mjekut:
  // const [filterEmriMjekut, setFilterEmriMjekut] =
  // useState<string>("");

  // ✅ SHEMBULL TJETËR:
  // nëse filtrojmë sipas specialistit:
  // const [filterSpecialist, setFilterSpecialist] =
  // useState<boolean | null>(null);

  const fetchSpitalet = async (): Promise<void> => {
    try {
      const res = await api.get("/api/Spitali");
      setSpitalet(res.data);
    } catch {}
  };

  // ==========================
  // FETCH MJEKËT
  // ==========================

  const fetchMjekut = async () => {
    setLoading(true);
    setError(null);
    try {
      const url =
        filterSpitaliId > 0
          ? `/api/Mjeku/bySpitali/${filterSpitaliId}`
          : "/api/Mjeku";

      // ✅ SHEMBULL TJETËR:
      // const url =
      //   filterEmriMjekut.trim() !== ""
      //     ? `/api/Mjeku/byName/${filterEmriMjekut}`
      //     : "/api/Mjeku";

      // ✅ SHEMBULL TJETËR:
      // const url =
      //   filterSpecialist !== null
      //     ? `/api/Mjeku/bySpecialist/${filterSpecialist}`
      //     : "/api/Mjeku";

      const res = await api.get(url);
      setMjekut(res.data);
    } catch (err: any) {
      setError(err.response?.data?.message || "Ngarkimi dështoi");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSpitalet();
  }, []);

  useEffect(() => {
    fetchMjekut();
  }, [filterSpitaliId]);

  // ✅ SHEMBULL TJETËR:
  // useEffect(() => {
  //   fetchMjekut();
  // }, [filterEmriMjekut]);

  // ✅ SHEMBULL TJETËR:
  // useEffect(() => {
  //   fetchMjekut();
  // }, [filterSpecialist]);

  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setFormError(null);
    setShowForm(true);
  };

  const handleOpenEdit = (n: Mjeku) => {
    setEditId(n.ID);
    setFormData({
      emriMjekut: n.emriMjekut,
      paga: Number(n.paga).toFixed(2),
      dataPunesimit: toInputDate(n.dataPunesimit),
      eshteSpecialist: n.eshteSpecialist,
      ID_Spitali: n.ID_Spitali,
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

    if (!formData.emriMjekut.trim()) {
      setFormError("Emri i mjekut është i detyrueshëm.");
      return;
    }

    // ✅ VALIDIM MANUAL për input type="text"
    // Lejon numra me presje dhjetore: 850, 850.5, 850.50
    const pagaRegex = /^\d+(\.\d{1,2})?$/;
    if (
      formData.paga === "" ||
      !pagaRegex.test(formData.paga.trim()) ||
      parseFloat(formData.paga) <= 0
    ) {
      setFormError("Paga duhet të jetë një numër pozitiv (p.sh. 850 ose 850.50).");
      return;
    }

    if (!formData.dataPunesimit.trim()) {
      setFormError("Data e punësimit është e detyrueshme.");
      return;
    }

    if (formData.ID_Spitali === 0) {
      setFormError("Duhet të zgjidhni një spital.");
      return;
    }

    const payload = {
      ...formData,
      // ✅ Konverto në number vetëm këtu, para dërgimit
      paga: parseFloat(formData.paga),
    };

    setSubmitLoading(true);
    try {
      if (editId !== null) {
        await api.put(`/api/Mjeku/${editId}`, payload);
      } else {
        await api.post("/api/Mjeku", payload);
      }

      setShowForm(false);
      setFormData(emptyForm);
      fetchMjekut();
    } catch (err: any) {
      setFormError(err.response?.data?.message || "Operacioni dështoi");
    } finally {
      setSubmitLoading(false);
    }
  };

  const handleDelete = async (id: number) => {
    if (!confirm("A je i sigurt?")) return;
    try {
      await api.delete(`/api/Mjeku/${id}`);
      fetchMjekut();
    } catch (err: any) {
      alert(err.response?.data?.message || "Fshirja dështoi");
    }
  };

  return (
    <div
      style={{
        padding: "20px",
        maxWidth: "1000px",
        margin: "0 auto",
      }}
    >
      <h1>👨‍⚕️ Menaxhimi i Mjekëve</h1>

      {/* ==========================
          BUTTON ADD
      ========================== */}

      <button
        onClick={handleOpenAdd}
        style={styles.btnAdd}
      >
        + Shto Mjek të Ri
      </button>

      {/* ==========================
          FILTRI
      ========================== */}

      <div style={styles.filterBox}>
        <label
          style={{
            marginRight: "10px",
            fontWeight: "bold",
          }}
        >
          🔍 Filtro sipas spitalit:
        </label>

        <select
          value={filterSpitaliId}
          onChange={(e) =>
            setFilterSpitaliId(Number(e.target.value))
          }
          style={styles.select}
        >
          <option value={0}>
            — Të gjitha spitalet —
          </option>

          {spitalet.map((s) => (
            <option
              key={s.ID_Spitali}
              value={s.ID_Spitali}
            >
              {s.emriSpitalit}
            </option>
          ))}
        </select>

        {/* ✅ BUTON PËR PASTRIM */}
        {filterSpitaliId > 0 && (
          <button
            onClick={() => setFilterSpitaliId(0)}
            style={styles.btnClearFilter}
          >
            ✕ Pastro filtrin
          </button>
        )}
      </div>

      {/* ==========================
          FORMA
      ========================== */}

      {showForm && (
        <div style={styles.formBox}>
          <h3>
            {editId
              ? "✏️ Edito Mjekun"
              : "➕ Shto Mjek të Ri"}
          </h3>

          {formError && (
            <p style={styles.formErrorMsg}>
              ⚠️ {formError}
            </p>
          )}

          <form onSubmit={handleSubmit}>
            {/* ==========================
                EMRI
            ========================== */}

            <div style={styles.formGroup}>
              <label>Emri i Mjekut:</label>
              <input
                type="text"
                value={formData.emriMjekut}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    emriMjekut: e.target.value,
                  })
                }
                placeholder="p.sh. Dr. Artan Krasniqi"
                style={styles.input}
              />
            </div>

            {/* ==========================
                PAGA — type="text" për të shmangur
                bug-un e browser me type="number"
                që fshin vlerën kur fillon shtypja
            ========================== */}

            <div style={styles.formGroup}>
              <label>Paga (€):</label>
              <input
                type="text"
                inputMode="decimal"
                value={formData.paga}
                onChange={(e) => {
                  // ✅ Lejo vetëm shifra dhe një pikë dhjetore
                  const val = e.target.value;
                  if (val === "" || /^\d*\.?\d{0,2}$/.test(val)) {
                    setFormData({ ...formData, paga: val });
                  }
                }}
                placeholder="p.sh. 850.50"
                style={styles.input}
              />
            </div>

            {/* ==========================
                DATA
            ========================== */}

            <div style={styles.formGroup}>
              <label>Data e Punësimit:</label>
              <input
                type="date"
                value={formData.dataPunesimit}
                max={
                  new Date()
                    .toISOString()
                    .split("T")[0]
                }
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    dataPunesimit: e.target.value,
                  })
                }
                style={styles.input}
              />
            </div>

            {/* ==========================
                CHECKBOX
            ========================== */}

            <div style={styles.formGroup}>
              <label
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: "8px",
                  cursor: "pointer",
                }}
              >
                <input
                  type="checkbox"
                  checked={formData.eshteSpecialist}
                  onChange={(e) =>
                    setFormData({
                      ...formData,
                      eshteSpecialist: e.target.checked,
                    })
                  }
                />
                Është Specialist
              </label>
            </div>

            {/* ==========================
                SPITALI
            ========================== */}

            <div style={styles.formGroup}>
              <label>Spitali:</label>
              <select
                value={formData.ID_Spitali}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    ID_Spitali: Number(e.target.value),
                  })
                }
                style={{
                  ...styles.select,
                  border:
                    formData.ID_Spitali === 0
                      ? "1px solid #e74c3c"
                      : "1px solid #ccc",
                }}
              >
                <option value={0}>
                  — Zgjedh Spitalin —
                </option>

                {spitalet.map((s) => (
                  <option
                    key={s.ID_Spitali}
                    value={s.ID_Spitali}
                  >
                    {s.emriSpitalit}
                  </option>
                ))}
              </select>
            </div>

            {/* ==========================
                BUTTONS
            ========================== */}

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
                {submitLoading
                  ? "Duke ruajtur..."
                  : "💾 Ruaj"}
              </button>

              <button
                type="button"
                onClick={handleCancel}
                style={styles.btnCancel}
              >
                ✕ Anulo
              </button>
            </div>
          </form>
        </div>
      )}

      {/* ==========================
          LOADING + ERROR
      ========================== */}

      {loading && <p>⏳ Duke ngarkuar...</p>}

      {error && (
        <p style={{ color: "red" }}>
          ❌ {error}
        </p>
      )}

      {/* ==========================
          TABELA
      ========================== */}

      {!loading && !error && (
        <>
          <p style={{ color: "#666" }}>
            {filterSpitaliId > 0 ? (
              <>
                Gjenden{" "}
                <strong>{Mjekut.length}</strong>
                {" "}mjekë në spitalin e zgjedhur
              </>
            ) : (
              <>
                Gjithsej:{" "}
                <strong>{Mjekut.length}</strong>
                {" "}mjekë
              </>
            )}
          </p>

          <table style={styles.table}>
            <thead>
              <tr style={styles.tableHeader}>
                <th style={styles.th}>ID</th>
                <th style={styles.th}>
                  Emri i Mjekut
                </th>
                <th style={styles.th}>Paga</th>
                <th style={styles.th}>
                  Data e Punësimit
                </th>
                <th style={styles.th}>
                  Specialist
                </th>
                <th style={styles.th}>
                  Spitali
                </th>
                <th style={styles.th}>
                  Veprime
                </th>
              </tr>
            </thead>

            <tbody>
              {Mjekut.length === 0 ? (
                <tr>
                  <td
                    colSpan={7}
                    style={{
                      textAlign: "center",
                      padding: "20px",
                    }}
                  >
                    Nuk ka mjekë të regjistruar
                  </td>
                </tr>
              ) : (
                Mjekut.map((n) => (
                  <tr
                    key={n.ID}
                    style={styles.tableRow}
                  >
                    <td style={styles.td}>
                      {n.ID}
                    </td>

                    <td style={styles.td}>
                      {n.emriMjekut}
                    </td>

                    {/* ✅ SHFAQ DECIMAL */}
                    <td style={styles.td}>
                      {Number(n.paga).toFixed(2)} €
                    </td>

                    <td style={styles.td}>
                      {formatDate(n.dataPunesimit)}
                    </td>

                    <td style={styles.td}>
                      {n.eshteSpecialist ? "Po" : "Jo"}
                    </td>

                    <td style={styles.td}>
                      {n.emriSpitalit}
                    </td>

                    <td style={styles.td}>
                      <button
                        onClick={() => handleOpenEdit(n)}
                        style={styles.btnEdit}
                      >
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

// ==========================
// STYLES
// ==========================

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
  btnClearFilter: { backgroundColor: "#e74c3c", color: "white", padding: "6px 12px", border: "none", borderRadius: "4px", cursor: "pointer", marginLeft: "10px" },
  formErrorMsg: { color: "#e74c3c", backgroundColor: "#fdecea", border: "1px solid #e74c3c", borderRadius: "4px", padding: "8px 12px" },
};
