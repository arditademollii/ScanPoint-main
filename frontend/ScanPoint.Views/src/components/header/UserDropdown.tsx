import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Dropdown from "../ui/dropdown/Dropdown";
import { DropdownItem } from "../ui/dropdown/DropdownItem";
import { useAuth } from "../../context/AuthContext";
import api from "../../api/axiosInstance";

export default function UserDropdown() {
  const [isOpen, setIsOpen] = useState(false);
  const [isLoggingOut, setIsLoggingOut] = useState(false);

  // ✅ userInfo vjen direkt nga AuthContext — përditësohet automatikisht
  const { user, userInfo, logout } = useAuth();
  const navigate = useNavigate();

  const closeDropdown = () => setIsOpen(false);

  const handleLogout = async () => {
    if (!user?.refreshToken || isLoggingOut) return;
    setIsLoggingOut(true);
    try {
      await api.post("/api/auth/logout", { refreshToken: user.refreshToken });
    } catch {}
    finally {
      logout();
      navigate("/signin", { replace: true });
    }
  };

  const photoSrc = userInfo?.profileImagePath
    ? `http://localhost:5055/${userInfo.profileImagePath}`
    : "/images/user/owner.jpg";

  return (
    <div className="relative">
      <button
        onClick={() => setIsOpen((prev) => !prev)}
        className="flex items-center gap-2"
        aria-label="Menu i përdoruesit"
      >
        <img
          src={photoSrc}
          alt="Profili"
          className="w-9 h-9 rounded-full object-cover border border-gray-200"
        />
        <span className="hidden sm:block text-sm font-medium text-gray-700 dark:text-gray-300">
          {userInfo?.username ?? "..."}
        </span>
      </button>

      <Dropdown
        isOpen={isOpen}
        onClose={closeDropdown}
        className="absolute right-0 mt-[17px] w-[240px] rounded-2xl border bg-white dark:bg-gray-900 p-3 shadow-theme-lg"
      >
        <div className="px-3 py-2 border-b border-gray-100 dark:border-gray-800 mb-2">
          <p className="font-medium text-gray-800 dark:text-white text-sm">
            {userInfo?.username ?? "—"}
          </p>
          <p className="text-xs text-gray-500 dark:text-gray-400">
            {userInfo?.email ?? "—"}
          </p>
          <span className="inline-block mt-1 text-xs px-2 py-0.5 rounded-full bg-brand-50 text-brand-600 capitalize">
            {user?.role}
          </span>
        </div>

        <ul className="flex flex-col gap-1 pb-2 border-b border-gray-100 dark:border-gray-800">
          {user?.role === "admin" && (
            <li>
              <DropdownItem
                onItemClick={closeDropdown}
                tag="a"
                to="/profile"
                className="flex items-center gap-3 px-3 py-2 font-medium text-gray-700 rounded-lg text-theme-sm hover:bg-gray-100 dark:text-gray-400 dark:hover:bg-white/5"
              >
                Menaxho dyqanet
              </DropdownItem>
            </li>
          )}
        </ul>

        <button
          onClick={handleLogout}
          disabled={isLoggingOut}
          className="w-full text-left px-3 py-2 mt-2 rounded-lg text-sm font-medium text-red-600 hover:bg-red-50 dark:hover:bg-red-900/20 transition-colors disabled:opacity-50"
        >
          {isLoggingOut ? "Duke dalur..." : "Dil nga llogaria"}
        </button>
      </Dropdown>
    </div>
  );
}
