import { useState, ChangeEvent, useEffect } from "react";
import { useModal } from "../../hooks/useModal";
import { Modal } from "../ui/modal";
import Button from "../ui/button/Button";

export interface ShopInfoCardProps {
  shopId: string;
  name: string;
  address: string;
  vatNumber: string;
  fiscalNumber: string;
  adminName: string;
  onSave: (updated: {
    name: string;
    address: string;
    vatNumber: string;
    fiscalNumber: string;
  }) => Promise<void>;
  onDelete?: () => void;
}

interface ShopForm {
  name: string;
  address: string;
  vatNumber: string;
  fiscalNumber: string;
}

interface ShopFormErrors {
  name?: string;
  address?: string;
  vatNumber?: string;
  fiscalNumber?: string;
}

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

export default function ShopInfoCard({
  name,
  address,
  vatNumber,
  fiscalNumber,
  adminName,
  onSave,
  onDelete,
}: ShopInfoCardProps) {
  const { isOpen, openModal, closeModal } = useModal();

  const [formData, setFormData] = useState<ShopForm>({ name, address, vatNumber, fiscalNumber });
  const [errors, setErrors] = useState<ShopFormErrors>({});
  const [saveError, setSaveError] = useState("");
  const [saveLoading, setSaveLoading] = useState(false);

  // ✅ FIX KRYESOR: Sa herë që hapet modali, reseto formën me props aktuale
  // Kjo parandalon autofill-in e vjetër nga tentativa e dështuar
  useEffect(() => {
    if (isOpen) {
      setFormData({ name, address, vatNumber, fiscalNumber });
      setErrors({});
      setSaveError("");
    }
  }, [isOpen]);

  const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
    const { name: field, value } = e.target;
    setFormData(prev => ({ ...prev, [field]: value }));
    // ✅ Pastro errorin e fushës sa herë që shkruhet
    if (errors[field as keyof ShopFormErrors]) {
      setErrors(prev => ({ ...prev, [field]: undefined }));
    }
  };

  const handleSave = async () => {
    setSaveError("");

    const validationErrors = validateShopForm(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }
    setErrors({});
    setSaveLoading(true);

    try {
      await onSave(formData);
      closeModal();
    } catch (err: any) {
      // ✅ FIX: Shfaq mesazhin nga serveri — forma MBETET HAPUR me të dhënat aktuale
      setSaveError(
        err.response?.data?.message ||
        err.response?.data ||
        "Ruajtja dështoi. Provoni përsëri."
      );
    } finally {
      setSaveLoading(false);
    }
  };

  const handleClose = () => {
    // ✅ Kur mbyllet me "Close", reseto gjithçka
    setFormData({ name, address, vatNumber, fiscalNumber });
    setErrors({});
    setSaveError("");
    closeModal();
  };

  return (
    <div className="p-5 border border-gray-200 rounded-2xl dark:border-gray-800 lg:p-6">
      <div className="flex flex-col gap-6 lg:flex-row lg:items-start lg:justify-between">
        <div>
          <h4 className="text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-6">
            Informacioni i Dyqanit
          </h4>

          <div className="grid grid-cols-1 gap-4 lg:grid-cols-2 lg:gap-7">
            <div>
              <p className="mb-2 text-xs text-gray-500 dark:text-gray-400">Emri</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{name}</p>
            </div>
            <div>
              <p className="mb-2 text-xs text-gray-500 dark:text-gray-400">Adresa</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{address}</p>
            </div>
            <div>
              <p className="mb-2 text-xs text-gray-500 dark:text-gray-400">Numri TVSH</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{vatNumber}</p>
            </div>
            <div>
              <p className="mb-2 text-xs text-gray-500 dark:text-gray-400">Numri Fiskal</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{fiscalNumber}</p>
            </div>
            <div className="col-span-2">
              <p className="mb-2 text-xs text-gray-500 dark:text-gray-400">Admin</p>
              <p className="text-sm font-medium text-gray-800 dark:text-white/90">{adminName}</p>
            </div>
          </div>
        </div>

        <div className="flex gap-2">
          <Button variant="outline" size="sm" onClick={openModal}>
            Edito
          </Button>
          {onDelete && (
            <Button
              variant="destructive"
              size="sm"
              onClick={() => {
                if (confirm("A jeni të sigurt që dëshironi të fshini këtë dyqan?")) {
                  onDelete();
                }
              }}
            >
              Fshi
            </Button>
          )}
        </div>
      </div>

      <Modal isOpen={isOpen} onClose={handleClose} className="max-w-[600px] m-4">
        <div className="relative w-full overflow-y-auto rounded-3xl bg-white p-6 dark:bg-gray-900">
          <h4 className="mb-4 text-2xl font-semibold text-gray-800 dark:text-white/90">
            Edito Dyqanin
          </h4>

          <div className="grid grid-cols-1 gap-4">
            {/* Emri */}
            <div>
              <label className="text-xs text-gray-500">Emri i dyqanit <span className="text-red-500">*</span></label>
              <input
                name="name"
                value={formData.name}
                onChange={handleChange}
                className="w-full rounded-md border px-3 py-2 text-gray-800 dark:text-white/90 dark:border-gray-700 dark:bg-gray-800 mt-1"
              />
              {errors.name && <p className="text-xs text-red-500 mt-1">{errors.name}</p>}
            </div>

            {/* Adresa */}
            <div>
              <label className="text-xs text-gray-500">Adresa <span className="text-red-500">*</span></label>
              <input
                name="address"
                value={formData.address}
                onChange={handleChange}
                className="w-full rounded-md border px-3 py-2 text-gray-800 dark:text-white/90 dark:border-gray-700 dark:bg-gray-800 mt-1"
              />
              {errors.address && <p className="text-xs text-red-500 mt-1">{errors.address}</p>}
            </div>

            {/* TVSH & Fiskal */}
            <div className="grid grid-cols-2 gap-3">
              <div>
                <label className="text-xs text-gray-500">Numri TVSH <span className="text-red-500">*</span></label>
                <input
                  name="vatNumber"
                  value={formData.vatNumber}
                  onChange={handleChange}
                  className="w-full rounded-md border px-3 py-2 text-gray-800 dark:text-white/90 dark:border-gray-700 dark:bg-gray-800 mt-1"
                />
                {errors.vatNumber && <p className="text-xs text-red-500 mt-1">{errors.vatNumber}</p>}
              </div>
              <div>
                <label className="text-xs text-gray-500">Numri Fiskal <span className="text-red-500">*</span></label>
                <input
                  name="fiscalNumber"
                  value={formData.fiscalNumber}
                  onChange={handleChange}
                  className="w-full rounded-md border px-3 py-2 text-gray-800 dark:text-white/90 dark:border-gray-700 dark:bg-gray-800 mt-1"
                />
                {errors.fiscalNumber && <p className="text-xs text-red-500 mt-1">{errors.fiscalNumber}</p>}
              </div>
            </div>
          </div>

          {/* ✅ Mesazhi i gabimit nga serveri */}
          {saveError && (
            <div className="mt-4 px-3 py-2 bg-red-50 border border-red-200 rounded-lg text-xs text-red-600">
              {saveError}
            </div>
          )}

          <div className="flex justify-end gap-3 mt-6">
            <Button variant="outline" onClick={handleClose}>
              Anulo
            </Button>
            <Button onClick={handleSave} disabled={saveLoading}>
              {saveLoading ? "Duke ruajtur..." : "Ruaj ndryshimet"}
            </Button>
          </div>
        </div>
      </Modal>
    </div>
  );
}