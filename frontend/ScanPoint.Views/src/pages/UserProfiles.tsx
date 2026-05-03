import { useEffect, useState, useMemo, useCallback } from "react";
import PageBreadcrumb from "../components/common/PageBreadCrumb";
import PageMeta from "../components/common/PageMeta";
import ShopInfoCard, { ShopInfoCardProps } from "../components/UserProfile/ShopInfoCard";
import { Modal } from "../components/ui/modal";
import { useModal } from "../hooks/useModal";
import Button from "../components/ui/button/Button";
import Label from "../components/form/Label";
import Input from "../components/form/input/InputField";
import api from "../api/axiosInstance";
import { useAuth } from "../context/AuthContext";

// ─────────────────────────────────────────────
// TYPES
// ─────────────────────────────────────────────
interface UserProfile {
  username: string;
  email: string;
  profileImagePath?: string;
}

interface ShopItem extends ShopInfoCardProps {
  isDeleted: boolean;
  deletedAt?: string;
}

interface ProfileFormErrors {
  username?: string;
  email?: string;
  password?: string;
}

interface ShopFormErrors {
  name?: string;
  address?: string;
  vatNumber?: string;
  fiscalNumber?: string;
}

interface ShopForm {
  name: string;
  address: string;
  vatNumber: string;
  fiscalNumber: string;
}

// ─────────────────────────────────────────────
// VALIDATION HELPERS
// ─────────────────────────────────────────────
const validateEmail = (email: string) =>
  /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);

const validateProfileForm = (
  form: { username: string; email: string; password: string }
): ProfileFormErrors => {
  const errors: ProfileFormErrors = {};
  if (!form.username.trim())
    errors.username = "Username-i është i detyrueshëm.";
  else if (form.username.trim().length < 3)
    errors.username = "Username-i duhet të ketë të paktën 3 karaktere.";

  if (!form.email.trim())
    errors.email = "Email-i është i detyrueshëm.";
  else if (!validateEmail(form.email))
    errors.email = "Email-i nuk është i vlefshëm.";

  if (form.password && form.password.length < 6)
    errors.password = "Fjalëkalimi duhet të ketë të paktën 6 karaktere.";

  return errors;
};

// ✅ FIX: Validim me regex për adresën — duhet të ketë të paktën një shkronjë
const validateShopForm = (form: ShopForm): ShopFormErrors => {
  const errors: ShopFormErrors = {};
  if (!form.name.trim())
    errors.name = "Emri i dyqanit është i detyrueshëm.";
  if (!form.address.trim())
    errors.address = "Adresa është e detyrueshme.";
  else if (!/.*[a-zA-ZÀ-ÿ].*/.test(form.address))
    errors.address = "Adresa duhet të përmbajë të paktën një shkronjë.";
  if (!form.vatNumber.trim())
    errors.vatNumber = "Numri i TVSH-së është i detyrueshëm.";
  if (!form.fiscalNumber.trim())
    errors.fiscalNumber = "Numri fiskal është i detyrueshëm.";
  return errors;
};

// ─────────────────────────────────────────────
// COMPONENT
// ─────────────────────────────────────────────
export default function UserProfiles() {
  const { user, userInfo, refreshUser } = useAuth();
  const isAdmin = user?.role === "admin";

  // Profile state
  const [profile, setProfile] = useState<UserProfile | null>(null);
  const [editForm, setEditForm] = useState({ username: "", email: "", password: "" });
  const [profileErrors, setProfileErrors] = useState<ProfileFormErrors>({});
  const [profileImage, setProfileImage] = useState<File | null>(null);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [saveError, setSaveError] = useState("");
  const [saveSuccess, setSaveSuccess] = useState("");
  const [saveLoading, setSaveLoading] = useState(false);
  const [imageTimestamp, setImageTimestamp] = useState(Date.now());

  // Edit profile modal
  const { isOpen: isEditOpen, openModal: openEdit, closeModal: closeEdit } = useModal();

  // Add shop modal
  const { isOpen: isAddShopOpen, openModal: openAddShop, closeModal: closeAddShop } = useModal();
  const [shopForm, setShopForm] = useState<ShopForm>({
    name: "", address: "", vatNumber: "", fiscalNumber: "",
  });
  const [shopErrors, setShopErrors] = useState<ShopFormErrors>({});
  const [shopSaveError, setShopSaveError] = useState("");
  const [shopSaveLoading, setShopSaveLoading] = useState(false);

  // Shops state
  const [shops, setShops] = useState<ShopItem[]>([]);
  const [showDeletedShops, setShowDeletedShops] = useState(false);

  // ─────────────────────────────────────────────
  // INIT
  // ─────────────────────────────────────────────
  useEffect(() => {
    if (userInfo) {
      setProfile(userInfo);
      setEditForm({
        username: userInfo.username ?? "",
        email: userInfo.email ?? "",
        password: "",
      });
    }
  }, [userInfo]);

  useEffect(() => { fetchShops(); }, [isAdmin]);

  // ─────────────────────────────────────────────
  // PROFILE IMAGE
  // ─────────────────────────────────────────────
  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      if (!file.type.startsWith("image/")) {
        setSaveError("Vetëm imazhe janë të lejuara.");
        return;
      }
      if (file.size > 5 * 1024 * 1024) {
        setSaveError("Imazhi nuk mund të jetë më i madh se 5MB.");
        return;
      }
      setProfileImage(file);
      setPreviewUrl(URL.createObjectURL(file));
      setSaveError("");
    }
  };

  const handleSaveProfile = async () => {
    setSaveError("");
    setSaveSuccess("");

    const errors = validateProfileForm(editForm);
    if (Object.keys(errors).length > 0) {
      setProfileErrors(errors);
      return;
    }
    setProfileErrors({});
    setSaveLoading(true);

    try {
      const formData = new FormData();
      if (editForm.username?.trim()) formData.append("Username", editForm.username.trim());
      if (editForm.email?.trim()) formData.append("Email", editForm.email.trim());
      if (editForm.password) formData.append("Password", editForm.password);
      if (profileImage) formData.append("File", profileImage);

      await api.put("/api/profile/me", formData);

      await refreshUser();
      setImageTimestamp(Date.now());
      setProfileImage(null);
      setPreviewUrl(null);
      setEditForm(f => ({ ...f, password: "" }));
      setSaveSuccess("Profili u ruajt me sukses.");
      closeEdit();
    } catch (err: any) {
      setSaveError(
        err.response?.data?.message ||
        err.response?.data ||
        "Ruajtja dështoi."
      );
    } finally {
      setSaveLoading(false);
    }
  };

  const handleCloseEdit = () => {
    setProfileErrors({});
    setSaveError("");
    setPreviewUrl(null);
    setProfileImage(null);
    // ✅ FIX: Reseto me të dhënat aktuale nga profile (jo nga editForm i vjetër)
    setEditForm({
      username: profile?.username ?? "",
      email: profile?.email ?? "",
      password: "",
    });
    closeEdit();
  };

  // ✅ FIX: openEdit e mbështjellë — reseton formën me gjendjen aktuale para hapjes
  const handleOpenEdit = () => {
    setProfileErrors({});
    setSaveError("");
    setPreviewUrl(null);
    setProfileImage(null);
    setEditForm({
      username: profile?.username ?? "",
      email: profile?.email ?? "",
      password: "",
    });
    openEdit();
  };

  // ─────────────────────────────────────────────
  // FETCH SHOPS
  // ─────────────────────────────────────────────
  const fetchShops = useCallback(async () => {
    if (!isAdmin) return;
    try {
      const res = await api.get("/api/shops/my-shops/all");
      const data = res.data as any[];

      const mappedShops: ShopItem[] = data.map((shop) => ({
        shopId: shop.id,
        name: shop.name,
        address: shop.address,
        vatNumber: shop.vatNumber,
        fiscalNumber: shop.fiscalNumber,
        adminName: shop.adminName ?? "Admin",
        isDeleted: shop.isDeleted,
        deletedAt: shop.deletedAt,
        // ✅ FIX: onSave kthen mesazhin e gabimit nga serveri te ShopInfoCard
        onSave: async (updated: any) => {
          await api.put(`/api/shops/${shop.id}`, updated);
          fetchShops();
        },
        onDelete: async () => {
          if (window.confirm("A jeni të sigurt që dëshironi të fshini këtë dyqan?")) {
            await api.delete(`/api/shops/${shop.id}`);
            fetchShops();
          }
        },
      }));

      setShops(mappedShops);
    } catch (err) {
      console.error("Gabim gjatë ngarkimit të dyqaneve", err);
    }
  }, [isAdmin]);

  // ─────────────────────────────────────────────
  // RESTORE SHOP
  // ─────────────────────────────────────────────
  const handleRestoreShop = async (id: string) => {
    try {
      await api.post(`/api/shops/${id}/restore`);
      await fetchShops();
    } catch {
      alert("Rikthimi i dyqanit dështoi.");
    }
  };

  // ─────────────────────────────────────────────
  // ADD SHOP
  // ─────────────────────────────────────────────
  const handleAddShop = async () => {
    setShopSaveError("");

    const errors = validateShopForm(shopForm);
    if (Object.keys(errors).length > 0) {
      setShopErrors(errors);
      return;
    }
    setShopErrors({});
    setShopSaveLoading(true);

    try {
      await api.post("/api/shops", {
        name: shopForm.name.trim(),
        address: shopForm.address.trim(),
        vatNumber: shopForm.vatNumber.trim(),
        fiscalNumber: shopForm.fiscalNumber.trim(),
      });

      await fetchShops();
      setShopForm({ name: "", address: "", vatNumber: "", fiscalNumber: "" });
      closeAddShop();
    } catch (err: any) {
      // ✅ FIX: Lexo mesazhin saktë nga backend-i
      setShopSaveError(
        err.response?.data?.message ||
        err.response?.data ||
        "Shtimi i dyqanit dështoi."
      );
    } finally {
      setShopSaveLoading(false);
    }
  };

  const handleCloseAddShop = () => {
    setShopErrors({});
    setShopSaveError("");
    setShopForm({ name: "", address: "", vatNumber: "", fiscalNumber: "" });
    closeAddShop();
  };

  // ─────────────────────────────────────────────
  // PHOTO SRC
  // ─────────────────────────────────────────────
  const photoSrc = useMemo(() => {
    if (profile?.profileImagePath) {
      const baseUrl = import.meta.env.VITE_API_URL || "http://localhost:5055/";
      const cleanPath = profile.profileImagePath.replace(/^\/+/, "");
      return `${baseUrl}${cleanPath}?t=${imageTimestamp}`;
    }
    return "/images/user/owner.jpg";
  }, [profile?.profileImagePath, imageTimestamp]);

  const activeShops = shops.filter(s => !s.isDeleted);
  const deletedShops = shops.filter(s => s.isDeleted);

  // ─────────────────────────────────────────────
  // RENDER
  // ─────────────────────────────────────────────
  return (
    <>
      <PageMeta title="Profili | ScanPoint" description="Menaxhimi i të dhënave të profilit" />
      <PageBreadcrumb pageTitle="Profili" />

      {/* ── HEADER PROFILIT ── */}
      <div className="p-6 mb-6 bg-white border border-gray-200 rounded-2xl dark:bg-white/[0.03] dark:border-gray-800 shadow-sm">
        <div className="flex flex-col items-center gap-6 lg:flex-row lg:justify-between">
          <div className="flex flex-col items-center gap-4 lg:flex-row">
            <div className="relative w-24 h-24 overflow-hidden rounded-full border-2 border-brand-500 bg-gray-50">
              <img src={photoSrc} alt="User" className="w-full h-full object-cover" />
            </div>
            <div className="text-center lg:text-left">
              <h3 className="text-xl font-bold text-gray-800 dark:text-white">
                {profile?.username || "Përdorues"}
              </h3>
              <p className="text-sm text-gray-500">{profile?.email}</p>
              <span className="px-3 py-1 mt-2 inline-block text-xs font-medium bg-brand-50 text-brand-600 rounded-full capitalize">
                {user?.role}
              </span>
            </div>
          </div>
          <div className="flex items-center gap-2">
            {saveSuccess && (
              <span className="text-xs text-green-600 font-medium">{saveSuccess}</span>
            )}
            {/* ✅ FIX: Përdor handleOpenEdit në vend të openEdit direkt */}
            <Button size="sm" onClick={handleOpenEdit}>Ndrysho Profilin</Button>
          </div>
        </div>
      </div>

      {/* ── MODAL: EDITO PROFILIN ── */}
      <Modal isOpen={isEditOpen} onClose={handleCloseEdit} className="max-w-[500px]">
        <div className="p-6 bg-white dark:bg-gray-900 rounded-3xl">
          <h3 className="text-lg font-bold mb-1 dark:text-white">Përditëso Profilin</h3>
          <p className="text-xs text-gray-400 mb-5">Ndrysho të dhënat e llogarisë tënde.</p>

          <div className="space-y-4">
            {/* Foto */}
            <div className="flex items-center gap-4 p-4 bg-gray-50 dark:bg-white/5 rounded-xl border border-gray-100 dark:border-gray-800">
              <div className="w-16 h-16 rounded-full overflow-hidden border-2 border-white shadow-sm bg-gray-200 flex-shrink-0">
                <img src={previewUrl || photoSrc} alt="Preview" className="w-full h-full object-cover" />
              </div>
              <div className="flex-1">
                <Label>Foto e profilit</Label>
                <input
                  type="file"
                  accept="image/*"
                  onChange={handleFileChange}
                  className="mt-1 text-xs w-full cursor-pointer text-gray-500"
                />
                <p className="text-[10px] text-gray-400 mt-1">PNG, JPG deri në 5MB</p>
              </div>
            </div>

            {/* Username */}
            <div>
              <Label>Username <span className="text-red-500">*</span></Label>
              <Input
                placeholder="p.sh. admin_test"
                value={editForm.username}
                onChange={e => {
                  setEditForm({ ...editForm, username: e.target.value });
                  if (profileErrors.username) setProfileErrors(p => ({ ...p, username: undefined }));
                }}
              />
              {profileErrors.username && (
                <p className="text-xs text-red-500 mt-1">{profileErrors.username}</p>
              )}
            </div>

            {/* Email */}
            <div>
              <Label>Email <span className="text-red-500">*</span></Label>
              <Input
                type="email"
                placeholder="p.sh. admin@scanpoint.com"
                value={editForm.email}
                onChange={e => {
                  setEditForm({ ...editForm, email: e.target.value });
                  if (profileErrors.email) setProfileErrors(p => ({ ...p, email: undefined }));
                }}
              />
              {profileErrors.email && (
                <p className="text-xs text-red-500 mt-1">{profileErrors.email}</p>
              )}
            </div>

            {/* Password */}
            <div>
              <Label>Fjalëkalimi i ri</Label>
              <Input
                type="password"
                placeholder="Lëre bosh nëse nuk ndryshon"
                value={editForm.password}
                onChange={e => {
                  setEditForm({ ...editForm, password: e.target.value });
                  if (profileErrors.password) setProfileErrors(p => ({ ...p, password: undefined }));
                }}
              />
              {profileErrors.password && (
                <p className="text-xs text-red-500 mt-1">{profileErrors.password}</p>
              )}
              <p className="text-[10px] text-gray-400 mt-1">Minimumi 6 karaktere nëse ndryshon.</p>
            </div>

            {saveError && (
              <div className="px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600">
                {saveError}
              </div>
            )}

            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" size="sm" onClick={handleCloseEdit}>Anulo</Button>
              <Button size="sm" onClick={handleSaveProfile} disabled={saveLoading}>
                {saveLoading ? "Duke ruajtur..." : "Ruaj ndryshimet"}
              </Button>
            </div>
          </div>
        </div>
      </Modal>

      {/* ── DYQANET (vetëm Admin) ── */}
      {isAdmin && (
        <div className="mt-6">
          <div className="flex justify-between items-center mb-4 px-1">
            <h3 className="text-lg font-bold dark:text-white">Dyqanet e mia</h3>
            <div className="flex items-center gap-3">
              {deletedShops.length > 0 && (
                <button
                  onClick={() => setShowDeletedShops(!showDeletedShops)}
                  className="text-sm text-gray-500 hover:text-brand-600 hover:underline font-medium transition-colors"
                >
                  {showDeletedShops ? "Fshih arkivën" : `Arkiva (${deletedShops.length})`}
                </button>
              )}
              <Button size="sm" onClick={openAddShop}>+ Shto Dyqan</Button>
            </div>
          </div>

          <div className="grid grid-cols-1 gap-6">
            {activeShops.length > 0 ? (
              activeShops.map(shop => <ShopInfoCard key={shop.shopId} {...shop} />)
            ) : (
              <div className="flex flex-col items-center justify-center py-12 border border-dashed border-gray-200 dark:border-gray-700 rounded-2xl text-center">
                <p className="text-gray-400 text-sm mb-3">Nuk keni asnjë dyqan aktiv.</p>
                <Button size="sm" variant="outline" onClick={openAddShop}>
                  + Shto dyqanin e parë
                </Button>
              </div>
            )}
          </div>

          {showDeletedShops && deletedShops.length > 0 && (
            <div className="mt-10 border-t border-dashed pt-6 border-gray-200 dark:border-gray-800">
              <h4 className="text-red-500 font-bold mb-4 flex items-center gap-2">
                🗑️ Arkiva e Dyqaneve
              </h4>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {deletedShops.map(shop => (
                  <div
                    key={shop.shopId}
                    className="p-4 bg-red-50/50 dark:bg-red-900/10 border border-red-100 dark:border-red-900/20 rounded-xl flex justify-between items-center"
                  >
                    <div>
                      <h5 className="font-bold text-gray-800 dark:text-gray-200 line-through">{shop.name}</h5>
                      <p className="text-xs text-gray-500">{shop.address}</p>
                      {shop.deletedAt && (
                        <p className="text-[10px] text-red-400 mt-1">
                          Fshirë më: {new Date(shop.deletedAt).toLocaleDateString("sq-AL")}
                        </p>
                      )}
                    </div>
                    <Button
                      size="sm"
                      className="bg-green-600 hover:bg-green-700 h-8 text-xs"
                      onClick={() => handleRestoreShop(shop.shopId)}
                    >
                      Rikthe
                    </Button>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      )}

      {/* ── MODAL: SHTO DYQAN ── */}
      <Modal isOpen={isAddShopOpen} onClose={handleCloseAddShop} className="max-w-[480px]">
        <div className="p-6 bg-white dark:bg-gray-900 rounded-3xl">
          <h3 className="text-lg font-bold mb-1 dark:text-white">Shto Dyqan të Ri</h3>
          <p className="text-xs text-gray-400 mb-5">Plotëso të dhënat e dyqanit të ri.</p>

          <div className="space-y-4">
            <div>
              <Label>Emri i dyqanit <span className="text-red-500">*</span></Label>
              <Input
                placeholder="p.sh. ScanPoint Prishtinë"
                value={shopForm.name}
                onChange={e => {
                  setShopForm({ ...shopForm, name: e.target.value });
                  if (shopErrors.name) setShopErrors(p => ({ ...p, name: undefined }));
                }}
              />
              {shopErrors.name && <p className="text-xs text-red-500 mt-1">{shopErrors.name}</p>}
            </div>

            <div>
              <Label>Adresa <span className="text-red-500">*</span></Label>
              <Input
                placeholder="p.sh. Rr. Agim Ramadani, Prishtinë"
                value={shopForm.address}
                onChange={e => {
                  setShopForm({ ...shopForm, address: e.target.value });
                  if (shopErrors.address) setShopErrors(p => ({ ...p, address: undefined }));
                }}
              />
              {shopErrors.address && <p className="text-xs text-red-500 mt-1">{shopErrors.address}</p>}
            </div>

            <div className="grid grid-cols-2 gap-3">
              <div>
                <Label>Numri i TVSH-së <span className="text-red-500">*</span></Label>
                <Input
                  placeholder="p.sh. 331234567"
                  value={shopForm.vatNumber}
                  onChange={e => {
                    setShopForm({ ...shopForm, vatNumber: e.target.value });
                    if (shopErrors.vatNumber) setShopErrors(p => ({ ...p, vatNumber: undefined }));
                  }}
                />
                {shopErrors.vatNumber && <p className="text-xs text-red-500 mt-1">{shopErrors.vatNumber}</p>}
              </div>
              <div>
                <Label>Numri fiskal <span className="text-red-500">*</span></Label>
                <Input
                  placeholder="p.sh. 601234567"
                  value={shopForm.fiscalNumber}
                  onChange={e => {
                    setShopForm({ ...shopForm, fiscalNumber: e.target.value });
                    if (shopErrors.fiscalNumber) setShopErrors(p => ({ ...p, fiscalNumber: undefined }));
                  }}
                />
                {shopErrors.fiscalNumber && <p className="text-xs text-red-500 mt-1">{shopErrors.fiscalNumber}</p>}
              </div>
            </div>

            {shopSaveError && (
              <div className="px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600">
                {shopSaveError}
              </div>
            )}

            <div className="flex justify-end gap-2 pt-2">
              <Button variant="outline" size="sm" onClick={handleCloseAddShop}>Anulo</Button>
              <Button size="sm" onClick={handleAddShop} disabled={shopSaveLoading}>
                {shopSaveLoading ? "Duke shtuar..." : "Shto dyqanin"}
              </Button>
            </div>
          </div>
        </div>
      </Modal>
    </>
  );
}