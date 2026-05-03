import { useEffect, useState } from "react";
import api from "../../api/axiosInstance";
import Dropdown from "../ui/dropdown/Dropdown";
import { DropdownItem } from "../ui/dropdown/DropdownItem";
import { Link } from "react-router";

const API_BASE = "http://localhost:5055";

interface UserDto {
  username: string;
  email: string;
  profileImage?: string;
}

export default function UserDropdown() {
  const [isOpen, setIsOpen] = useState(false);
  const [isEditing, setIsEditing] = useState(false);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);

  const [user, setUser] = useState<UserDto>({
    username: "",
    email: "",
    profileImage: "",
  });

  /* =============================
     LOAD USER FROM BACKEND
     ============================= */
  useEffect(() => {
    loadMe();
  }, []);

  async function loadMe() {
    try {
      const res = await api.get<UserDto>("/api/profile/me");
      setUser(res.data);
    } catch (err) {
      console.error("Error loading user:", err);
    }
  }

  function toggleDropdown() {
    setIsOpen((p) => !p);
  }

  function closeDropdown() {
    setIsOpen(false);
    setIsEditing(false);
  }

  function handleFileChange(e: React.ChangeEvent<HTMLInputElement>) {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
    }
  }

  /* =============================
     UPLOAD PROFILE IMAGE
     ============================= */
  async function handleSave() {
    if (!selectedFile) {
      alert("Please select an image");
      return;
    }

    try {
      const formData = new FormData();
      formData.append("file", selectedFile);

      await api.post("/api/Admin/upload-profile-image", formData, {
        headers: {
          "Content-Type": "multipart/form-data",
        },
      });

      await loadMe();
      setIsEditing(false);
      closeDropdown();
    } catch (err) {
      console.error(err);
      alert("Failed to upload image");
    }
  }

  const profileImageUrl = user.profileImage
    ? `${API_BASE}/${user.profileImage}`
    : "/images/user/owner.jpg";

  return (
    <div className="relative">
      {/* ===== BUTTON ===== */}
      <button
        onClick={toggleDropdown}
        className="flex items-center text-gray-700 dark:text-gray-400"
      >
        <span className="mr-3 overflow-hidden rounded-full h-11 w-11">
          <img
            src={profileImageUrl}
            alt="User"
            className="h-full w-full object-cover"
          />
        </span>
        <span className="block mr-1 font-medium text-theme-sm">
          {user.username}
        </span>
        <svg
          className={`stroke-gray-500 transition-transform ${
            isOpen ? "rotate-180" : ""
          }`}
          width="18"
          height="20"
          viewBox="0 0 18 20"
          fill="none"
        >
          <path
            d="M4.3125 8.65625L9 13.3437L13.6875 8.65625"
            stroke="currentColor"
            strokeWidth="1.5"
            strokeLinecap="round"
            strokeLinejoin="round"
          />
        </svg>
      </button>

      {/* ===== DROPDOWN ===== */}
      <Dropdown
        isOpen={isOpen}
        onClose={closeDropdown}
        className="absolute right-0 mt-[17px] w-[260px] rounded-2xl border bg-white p-3 shadow-lg"
      >
        {isEditing ? (
          <div className="flex flex-col gap-3">
            <input
              type="file"
              accept="image/*"
              onChange={handleFileChange}
              className="border p-2 rounded-md"
            />

            <div className="flex justify-end gap-2">
              <button
                onClick={handleSave}
                className="px-3 py-1 bg-blue-500 text-white rounded-md"
              >
                Save
              </button>
              <button
                onClick={() => setIsEditing(false)}
                className="px-3 py-1 bg-gray-300 rounded-md"
              >
                Cancel
              </button>
            </div>
          </div>
        ) : (
          <>
            {/* HEADER */}
            <div>
              <span className="block font-medium text-gray-700">
                {user.username}
              </span>
              <span className="block text-sm text-gray-500">
                {user.email}
              </span>
            </div>

            {/* MENU */}
            <ul className="flex flex-col gap-1 pt-4 pb-3 border-b">
              <li>
                <DropdownItem
                  tag="a"
                  to="/profile"
                  onItemClick={closeDropdown}
                >
                  Edit shops
                </DropdownItem>
              </li>

              <li>
                <DropdownItem
                  tag="button"
                  onItemClick={() => setIsEditing(true)}
                >
                  Edit profile
                </DropdownItem>
              </li>
            </ul>

            {/* LOGOUT */}
            <Link
              to="/signin"
              className="block px-3 py-2 mt-3 rounded-lg hover:bg-gray-100"
            >
              Sign out
            </Link>
          </>
        )}
      </Dropdown>
    </div>
  );
}
