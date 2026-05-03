// ===================== IMPORTS =====================
import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { ChevronLeftIcon, EyeCloseIcon, EyeIcon } from "../../icons";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Checkbox from "../form/input/Checkbox";
import FileInput from "../form/input/FileInput";
import axios from "axios";

export default function SignUpForm() {
  const navigate = useNavigate();

  const [showPassword, setShowPassword] = useState(false);
  const [isChecked, setIsChecked] = useState(false);

  // FORM STATE
  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
  });

  const [profileImage, setProfileImage] = useState<File | null>(null);
  const [errors, setErrors] = useState<any>({});
  const [serverError, setServerError] = useState<string | null>(null);

  const allowedTypes = ["image/jpeg", "image/png", "image/jpg"];
  const MAX_SIZE = 2 * 1024 * 1024; // 2MB

  // ===================== VALIDATION =====================
  const validate = () => {
    let newErrors: any = {};

    // 1. Username
    if (!form.username.trim()) {
      newErrors.username = "Username është i detyrueshëm.";
    }

    // 2. Email (Validim strikt)
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!form.email.trim()) {
      newErrors.email = "Email është i detyrueshëm.";
    } else if (!emailRegex.test(form.email)) {
      newErrors.email = "Format i gabuar i email-it.";
    }

    // 3. Password (Kriteret e reja: 8+, Shkronjë e madhe, Numër, Simbol)
    if (!form.password) {
      newErrors.password = "Fjalëkalimi është i detyrueshëm.";
    } else if (form.password.length < 8) {
      newErrors.password = "Minimumi 8 karaktere.";
    } else if (!/[A-Z]/.test(form.password)) {
      newErrors.password = "Duhet së paku një shkronjë e madhe.";
    } else if (!/[0-9]/.test(form.password)) {
      newErrors.password = "Duhet së paku një numër.";
    } else if (!/[!@#$%^&*(),.?":{}|<>]/.test(form.password)) {
      newErrors.password = "Duhet së paku një simbol (!@#$%^&*).";
    }

    // 4. Terms
    if (!isChecked) {
      newErrors.terms = "Duhet të pranoni Kushtet dhe Privatësinë.";
    }

    // 5. Profile Image
    if (!profileImage) {
      newErrors.profileImage = "Fotoja e profilit është e detyrueshme.";
    } else {
      if (!allowedTypes.includes(profileImage.type)) {
        newErrors.profileImage = "Lejohen vetëm formatet JPG/PNG.";
      }
      if (profileImage.size > MAX_SIZE) {
        newErrors.profileImage = "Fotoja nuk duhet të kalojë 2MB.";
      }
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // ===================== HANDLE SUBMIT =====================
  const handleSubmit = async (e: any) => {
    e.preventDefault();
    setServerError(null);

    if (!validate()) return;

    try {
      const data = new FormData();
      data.append("Username", form.username);
      data.append("Email", form.email);
      data.append("Password", form.password);
      if (profileImage) data.append("ProfileImage", profileImage);

      const res = await axios.post(
        "http://localhost:5055/api/Auth/register",
        data,
        { headers: { "Content-Type": "multipart/form-data" } }
      );

      // Kontrollojmë përgjigjen sipas strukturës së backend-it tënd
      if (res.status === 200 || res.data.success) {
        navigate("/signin");
      } else {
        setServerError(res.data.message || "Diçka shkoi keq gjatë regjistrimit.");
      }
    } catch (err: any) {
      setServerError(
        err.response?.data?.message || err.response?.data || "Gabim në server. Provo përsëri."
      );
    }
  };

  return (
    <div className="flex flex-col flex-1 w-full overflow-y-auto lg:w-1/2 no-scrollbar">
      <div className="w-full max-w-md mx-auto mb-5 sm:pt-10">
        <Link
          to="/"
          className="inline-flex items-center text-sm text-gray-500 transition-colors hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-300"
        >
          <ChevronLeftIcon className="size-5" />
          Kthehu prapa
        </Link>
      </div>

      <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
        <div>
          <div className="mb-5 sm:mb-8">
            <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90 sm:text-title-md">
              Sign Up
            </h1>
            <p className="text-sm text-gray-500 dark:text-gray-400">
              Krijoni llogarinë tuaj duke plotësuar të dhënat e mëposhtme.
            </p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="space-y-5">
              {/* USERNAME */}
              <div>
                <Label>Username<span className="text-error-500">*</span></Label>
                <Input
                  type="text"
                  placeholder="Shkruani username-in"
                  value={form.username}
                  onChange={(e) => {
                    setForm({ ...form, username: e.target.value });
                    if (errors.username) setErrors({ ...errors, username: null });
                  }}
                />
                {errors.username && <p className="text-red-500 text-xs mt-1">{errors.username}</p>}
              </div>

              {/* EMAIL */}
              <div>
                <Label>Email<span className="text-error-500">*</span></Label>
                <Input
                  type="email"
                  placeholder="shembull@email.com"
                  value={form.email}
                  onChange={(e) => {
                    setForm({ ...form, email: e.target.value });
                    if (errors.email) setErrors({ ...errors, email: null });
                  }}
                />
                {errors.email && <p className="text-red-500 text-xs mt-1">{errors.email}</p>}
              </div>

              {/* PASSWORD */}
              <div>
                <Label>Password<span className="text-error-500">*</span></Label>
                <div className="relative">
                  <Input
                    placeholder="Së paku 8 karaktere"
                    type={showPassword ? "text" : "password"}
                    value={form.password}
                    onChange={(e) => {
                      setForm({ ...form, password: e.target.value });
                      if (errors.password) setErrors({ ...errors, password: null });
                    }}
                  />
                  <span
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
                  >
                    {showPassword ? (
                      <EyeIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                    ) : (
                      <EyeCloseIcon className="fill-gray-500 dark:fill-gray-400 size-5" />
                    )}
                  </span>
                </div>
                {errors.password && <p className="text-red-500 text-xs mt-1">{errors.password}</p>}
              </div>

              {/* FILE INPUT */}
              <div>
                <Label>Foto Profili<span className="text-error-500">*</span></Label>
                <FileInput
                  onChange={(event) => {
                    setProfileImage(event.target.files?.[0] ?? null);
                    if (errors.profileImage) setErrors({ ...errors, profileImage: null });
                  }}
                />
                {errors.profileImage && <p className="text-red-500 text-xs mt-1">{errors.profileImage}</p>}
              </div>

              {/* TERMS */}
              <div className="flex flex-col gap-2">
                <div className="flex items-center gap-3">
                  <Checkbox
                    className="w-5 h-5"
                    checked={isChecked}
                    onChange={(val) => {
                      setIsChecked(val);
                      if (errors.terms) setErrors({ ...errors, terms: null });
                    }}
                  />
                  <p className="text-sm font-normal text-gray-500 dark:text-gray-400">
                    Duke krijuar llogari, pajtoheni me{" "}
                    <span className="text-gray-800 dark:text-white font-medium cursor-pointer underline">
                      Terms & Conditions
                    </span>
                  </p>
                </div>
                {errors.terms && <p className="text-red-500 text-xs">{errors.terms}</p>}
              </div>

              {/* SERVER ERROR */}
              {serverError && (
                <div className="p-3 bg-red-50 border border-red-200 rounded-lg">
                  <p className="text-red-600 text-sm text-center">{serverError}</p>
                </div>
              )}

              {/* SUBMIT */}
              <button
                type="submit"
                className="flex items-center justify-center w-full px-4 py-3 text-sm font-medium text-white transition rounded-lg bg-blue-600 hover:bg-blue-700 shadow-theme-xs"
              >
                Sign Up
              </button>
            </div>
          </form>

          {/* FOOTER */}
          <div className="mt-5">
            <p className="text-sm font-normal text-center text-gray-700 dark:text-gray-400 sm:text-start">
              Keni llogari?{" "}
              <Link to="/signin" className="text-blue-600 hover:underline font-medium">
                Kyçu këtu
              </Link>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}