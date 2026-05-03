import { useEffect, useRef, useState } from "react";
import { Link } from "react-router";
import { useSidebar } from "../context/SidebarContext";
import { ThemeToggleButton } from "../components/common/ThemeToggleButton";
import UserDropdown from "../components/header/UserDropdown";

const AppHeader: React.FC = () => {
  const [isApplicationMenuOpen, setApplicationMenuOpen] = useState(false);

  const { isMobileOpen, toggleSidebar, toggleMobileSidebar } = useSidebar();
  const inputRef = useRef<HTMLInputElement>(null);

  const handleToggle = () => {
    if (window.innerWidth >= 1024) {
      toggleSidebar();
    } else {
      toggleMobileSidebar();
    }
  };

  const toggleApplicationMenu = () => {
    setApplicationMenuOpen(!isApplicationMenuOpen);
  };

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if ((event.metaKey || event.ctrlKey) && event.key === "k") {
        event.preventDefault();
        inputRef.current?.focus();
      }
    };

    document.addEventListener("keydown", handleKeyDown);
    return () => document.removeEventListener("keydown", handleKeyDown);
  }, []);

  return (
    <header className="sticky top-0 flex w-full bg-white border-gray-200 z-99999 dark:border-gray-800 dark:bg-gray-900 lg:border-b">
      <div className="flex flex-col items-center justify-between grow lg:flex-row lg:px-6">
        {/* Top bar — visible on all screen sizes */}
        <div className="flex items-center justify-between w-full gap-2 px-3 py-3 border-b border-gray-200 dark:border-gray-800 lg:border-b-0 lg:px-0 lg:py-4">
          {/* Hamburger */}
          <button
            className="flex items-center justify-center w-10 h-10 text-gray-500 border rounded-lg border-gray-200 dark:border-gray-700 hover:bg-gray-100 dark:hover:bg-gray-800 transition-colors"
            onClick={handleToggle}
            aria-label="Toggle sidebar"
          >
            ☰
          </button>

          {/* Logo — center on mobile, hidden on desktop (sidebar handles it) */}
          <Link to="/" className="lg:hidden absolute left-1/2 -translate-x-1/2">
            <img
              src="./images/logo/logo.png"
              alt="Logo"
              className="h-8 w-auto"
            />
          </Link>

          {/* Mobile: right side actions (always visible) */}
          <div className="flex items-center gap-2 lg:hidden">
            <ThemeToggleButton />
            <UserDropdown />
          </div>

          {/* Desktop: toggle application menu button (hidden on lg+, we show actions inline) */}
          <button
            className="hidden"
            onClick={toggleApplicationMenu}
            aria-label="Toggle application menu"
          />
        </div>

        {/* Desktop actions — hidden on mobile (already shown above) */}
        <div className="hidden lg:flex items-center justify-end w-full gap-4 px-0 py-4">
          <ThemeToggleButton />
          <UserDropdown />
        </div>
      </div>
    </header>
  );
};

export default AppHeader;
