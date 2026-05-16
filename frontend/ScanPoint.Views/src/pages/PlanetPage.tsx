import { useEffect, useState } from "react";
import api from "../api/axiosInstance";

// ID_Planet mbetet me kapitale
interface Planet {
  ID_Planet: number;
  emriPlanetit: string;
  type: string;
  isDeleted: boolean;
}

interface FormData {
  emriPlanetit: string;
  type: string;
}

const emptyForm: FormData = {
  emriPlanetit: "",
  type: "",
};

export default function PlanetPage() {
  const [planetet, setPlanetet] = useState<Planet[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId] = useState<number | null>(null);

  const [formData, setFormData] =
    useState<FormData>(emptyForm);

  const [submitLoading, setSubmitLoading] =
    useState(false);

  // ===========================
  // GET ALL
  // ===========================
  const fetchPlanetet = async () => {
    setLoading(true);
    setError(null);

    try {
      const res =
        await api.get("/api/Planentete");

      setPlanetet(res.data);
    } catch (err: any) {
      setError(
        err.response?.data?.message ||
          "Ngarkimi dështoi"
      );
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchPlanetet();
  }, []);

  // ===========================
  // OPEN ADD FORM
  // ===========================
  const handleOpenAdd = () => {
    setEditId(null);
    setFormData(emptyForm);
    setShowForm(true);
  };

  // ===========================
  // OPEN EDIT FORM
  // ===========================
  const handleOpenEdit = (p: Planet) => {
    setEditId(p.ID_Planet);

    setFormData({
      emriPlanetit: p.emriPlanetit,
      type: p.type,
    });

    setShowForm(true);
  };

  // ===========================
  // SUBMIT FORM
  // ===========================
  const handleSubmit = async (
    e: React.FormEvent
  ) => {
    e.preventDefault();

    if (
      !formData.emriPlanetit.trim() ||
      !formData.type.trim()
    ) {
      alert(
        "Të gjitha fushat janë të detyrueshme!"
      );
      return;
    }

    setSubmitLoading(true);

    try {
      if (editId !== null) {
        await api.put(
          `/api/Planentete/${editId}`,
          formData
        );
      } else {
        await api.post(
          "/api/Planentete",
          formData
        );
      }

      setShowForm(false);

      fetchPlanetet();
    } catch (err: any) {
      alert(
        err.response?.data?.message ||
          "Operacioni dështoi"
      );
    } finally {
      setSubmitLoading(false);
    }
  };

  // ===========================
  // DELETE
  // ===========================
  const handleDelete = async (
    id: number
  ) => {
    if (
      !confirm(
        "A je i sigurt që dëshiron të fshish këtë planet?"
      )
    )
      return;

    try {
      await api.delete(
        `/api/Planentete/${id}`
      );

      fetchPlanetet();
    } catch (err: any) {
      alert(
        err.response?.data?.message ||
          "Fshirja dështoi"
      );
    }
  };

  return (
    <div
      style={{
        padding: "20px",
        maxWidth: "900px",
        margin: "0 auto",
      }}
    >
      <h1>🪐 Menaxhimi i Planetëve</h1>

      <button
        onClick={handleOpenAdd}
        style={styles.btnAdd}
      >
        + Shto Planet të Ri
      </button>

      {/* FORM */}
      {showForm && (
        <div style={styles.formBox}>
          <h3>
            {editId
              ? "✏️ Edito Planetin"
              : "➕ Shto Planet të Ri"}
          </h3>

          <form onSubmit={handleSubmit}>
            {/* EMRI */}
            <div style={styles.formGroup}>
              <label>
                Emri i Planetit:
              </label>

              <input
                type="text"
                value={formData.emriPlanetit}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    emriPlanetit:
                      e.target.value,
                  })
                }
                placeholder="p.sh. Mars"
                style={styles.input}
                required
              />
            </div>

            {/* TYPE */}
            <div style={styles.formGroup}>
              <label>Type:</label>

              <input
                type="text"
                value={formData.type}
                onChange={(e) =>
                  setFormData({
                    ...formData,
                    type: e.target.value,
                  })
                }
                placeholder="p.sh. Gas Giant"
                style={styles.input}
                required
              />
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
                {submitLoading
                  ? "Duke ruajtur..."
                  : "💾 Ruaj"}
              </button>

              <button
                type="button"
                onClick={() =>
                  setShowForm(false)
                }
                style={styles.btnCancel}
              >
                ✕ Anulo
              </button>
            </div>
          </form>
        </div>
      )}

      {/* LOADING */}
      {loading && (
        <p>⏳ Duke ngarkuar...</p>
      )}

      {/* ERROR */}
      {error && (
        <p style={{ color: "red" }}>
          ❌ {error}
        </p>
      )}

      {/* TABLE */}
      {!loading && !error && (
        <table style={styles.table}>
          <thead>
            <tr style={styles.tableHeader}>
              <th style={styles.th}>ID</th>

              <th style={styles.th}>
                Emri i Planetit
              </th>

              <th style={styles.th}>
                Type
              </th>

              <th style={styles.th}>
                Veprimet
              </th>
            </tr>
          </thead>

          <tbody>
            {planetet.length === 0 ? (
              <tr>
                <td
                  colSpan={4}
                  style={{
                    textAlign: "center",
                    padding: "20px",
                  }}
                >
                  Nuk ka planetë të regjistruar
                </td>
              </tr>
            ) : (
              planetet.map((p) => (
                <tr
                  key={p.ID_Planet}
                  style={styles.tableRow}
                >
                  <td style={styles.td}>
                    {p.ID_Planet}
                  </td>

                  <td style={styles.td}>
                    {p.emriPlanetit}
                  </td>

                  <td style={styles.td}>
                    {p.type}
                  </td>

                  <td style={styles.td}>
                    <button
                      onClick={() =>
                        handleOpenEdit(p)
                      }
                      style={styles.btnEdit}
                    >
                      ✏️ Edito
                    </button>

                    <button
                      onClick={() =>
                        handleDelete(
                          p.ID_Planet
                        )
                      }
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