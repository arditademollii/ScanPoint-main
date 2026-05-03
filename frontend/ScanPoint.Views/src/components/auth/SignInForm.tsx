import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { EyeCloseIcon, EyeIcon } from "../../icons";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import api from "../../api/axiosInstance";
import { useAuth } from "../../context/AuthContext";

export default function SignInForm() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [errors, setErrors] = useState<{ email?: string; password?: string }>({});
  const [serverError, setServerError] = useState("Provoni përsëri.");
  const [loading, setLoading] = useState(false);

  const navigate = useNavigate();
  const { login } = useAuth();

  // ===========================
  // LOGJIKA E VALIDIMIT
  // ===========================
  const validate = (): boolean => {
    const newErrors: { email?: string; password?: string } = {};

    // Validimi i Email-it
    const emailRegex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (!email) {
      newErrors.email = "Email është i detyrueshëm";
    } else if (!emailRegex.test(email)) {
      newErrors.email = "Format i gabuar i email-it";
    }

    // Validimi i Fjalëkalimit (Sipas rregullave të reja)
    if (!password) {
      newErrors.password = "Fjalëkalimi është i detyrueshëm";
    } else if (password.length < 8) {
      newErrors.password = "Minimumi 8 karaktere";
    } else if (!/[A-Z]/.test(password)) {
      newErrors.password = "Duhet së paku një shkronjë e madhe";
    } else if (!/[0-9]/.test(password)) {
      newErrors.password = "Duhet së paku një numër";
    } else if (!/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
      newErrors.password = "Duhet së paku një simbol (!@#$%^&*)";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // ===========================
  // HANDLE SUBMIT
  // ===========================
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setServerError("");
  
    if (!validate()) return;
  
    setLoading(true);
  
    // 🔹 Debug: shiko çfarë po dërgohet
    console.log("Login payload:", { email, password });
  
    try {
      const response = await api.post("/api/auth/login", { email, password });
  
      // 🔹 Debug: shiko përgjigjen e serverit
      console.log("Login response:", response.data);
  
      const { accessToken, refreshToken, role } = response.data;
  
      await login(accessToken, refreshToken, role);
  
      const userRole = role?.toLowerCase();
      if (userRole === "admin") navigate("/dashboard/home");
      else if (userRole === "cashier") navigate("/dashboard/userhome");
      else if (userRole === "manager") navigate("/dashboard/moderatorhome");
      else setServerError("Rol i panjohur");
  
    } catch (error: any) {
      // 🔹 Debug: shiko gabimin nga serveri
      console.error("Login error:", error.response?.data || error);
  
      setServerError(
        error.response?.data?.message ||
        error.response?.data?.Message ||
        JSON.stringify(error.response?.data) ||
        "Login dështoi. Provo përsëri."
      );
      setPassword("");
    } finally {
      setLoading(false);
    }
  };
  return (
    <div className="flex flex-col justify-center flex-1 w-full max-w-md mx-auto">
      <div>
        <h1 className="mb-2 font-semibold text-gray-800 text-title-sm dark:text-white/90">
          Sign In
        </h1>
        <p className="text-sm text-gray-500 dark:text-gray-400">
          Vendos email-in dhe fjalëkalimin për t'u kyçur.
        </p>

        <form onSubmit={handleSubmit} className="mt-5 space-y-6" noValidate>
          {/* FUSHA E EMAIL-IT */}
          <div>
            <Label>Email <span className="text-error-500">*</span></Label>
            <Input
              type="email"
              placeholder="info@gmail.com"
              value={email}
              onChange={(e) => {
                setEmail(e.target.value);
                if (errors.email) setErrors((prev) => ({ ...prev, email: undefined }));
              }}
            />
            {errors.email && (
              <p className="text-error-500 text-sm mt-1">{errors.email}</p>
            )}
          </div>

          {/* FUSHA E FJALËKALIMIT */}
          <div>
            <Label>Fjalëkalimi <span className="text-error-500">*</span></Label>
            <div className="relative">
              <Input
                type={showPassword ? "text" : "password"}
                placeholder="Vendos fjalëkalimin"
                value={password}
                onChange={(e) => {
                  setPassword(e.target.value);
                  if (errors.password) setErrors((prev) => ({ ...prev, password: undefined }));
                }}
              />
              <span
                onClick={() => setShowPassword(!showPassword)}
                className="absolute z-30 -translate-y-1/2 cursor-pointer right-4 top-1/2"
              >
                {showPassword ? <EyeIcon className="size-5" /> : <EyeCloseIcon className="size-5" />}
              </span>
            </div>
            {errors.password && (
              <p className="text-error-500 text-sm mt-1">{errors.password}</p>
            )}
          </div>

          {/* ERROR-ET NGA SERVERI */}
          {serverError && (
            <p className="text-error-500 text-sm" role="alert">{serverError}</p>
          )}

          <Button type="submit" className="w-full" size="sm" disabled={loading}>
            {loading ? "Duke u kyçur..." : "Kyçu"}
          </Button>
        </form>

        <p className="mt-5 text-sm text-center text-gray-700 dark:text-gray-400">
          Kontakto administratorin për të krijuar llogari.
        </p>
      </div>
    </div>
  );
}