import { useCallback, useEffect, useRef, useState } from "react";
import { Link, useLocation } from "react-router";
import {
  ChevronDownIcon,
  GridIcon,
  HorizontaLDots,
  ListIcon,
  TableIcon,
  UserCircleIcon,
} from "../icons";
import { useSidebar } from "../context/SidebarContext";
import { useAuth } from "../context/AuthContext";

type NavItem = {
  name: string;
  icon: React.ReactNode;
  path?: string;
  subItems?: { name: string; path: string; pro?: boolean; new?: boolean }[];
};

const AppSidebar: React.FC = () => {
  const { isExpanded, isMobileOpen, isHovered, setIsHovered, toggleMobileSidebar } =
    useSidebar();
  const location = useLocation();

  const { user } = useAuth();
  const role = user?.role ?? null;

  const [openSubmenu, setOpenSubmenu] = useState<{ index: number } | null>(null);
  const [subMenuHeight, setSubMenuHeight] = useState<Record<string, number>>({});
  const subMenuRefs = useRef<Record<string, HTMLDivElement | null>>({});

  const isActive = useCallback(
    (path: string) => location.pathname === path,
    [location.pathname]
  );

  // Close mobile sidebar on route change
  useEffect(() => {
    if (isMobileOpen) {
      toggleMobileSidebar();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [location.pathname]);

  useEffect(() => {
    if (openSubmenu !== null) {
      const key = `main-${openSubmenu.index}`;
      if (subMenuRefs.current[key]) {
        setSubMenuHeight((prev) => ({
          ...prev,
          [key]: subMenuRefs.current[key]?.scrollHeight || 0,
        }));
      }
    }
  }, [openSubmenu]);

  const handleSubmenuToggle = (index: number) => {
    setOpenSubmenu((prev) => (prev?.index === index ? null : { index }));
  };

  const getNavItems = (): NavItem[] => {
    if (role === "manager") {
      return [
        {
          name: "Products and Invoices",
          icon: <TableIcon />,
          subItems: [
            { name: "Products in stock", path: "/basic-tables10" },
            { name: "Products out of stock", path: "/basic-tables4" },
          ],
        },
        {
          name: "Cashiers",
          icon: <UserCircleIcon />,
          subItems: [{ name: "Cashiers", path: "/basic-tables1" }],
        },
      ];
    } else if (role === "cashier") {
      return [
        {
          name: "POS Terminal",
          icon: <GridIcon />,
          subItems: [{ name: "POS Terminal", path: "/PosTerminal" }],
        },
        {
          name: "Products",
          icon: <TableIcon />,
          subItems: [{ name: "Products", path: "/basic-tables10" }],
        },
      ];
    } else if (role === "admin") {
      return [
        {
          icon: <GridIcon />,
          name: "Dashboard",
          subItems: [{ name: "Statistikat", path: "/dashboard/home" }],
        },
        {
          name: "User Management",
          icon: <TableIcon />,
          subItems: [
            { name: "Cashiers", path: "/CashiersA" },
            { name: "Managers", path: "/basic-tables2" },
          ],
        },
        {
          name: "Products and Invoices",
          icon: <TableIcon />,
          subItems: [
            { name: "Products in stock", path: "/basic-tables10" },
            { name: "Products out of stock", path: "/basic-tables4" },
            { name: "Invoices", path: "/invoices" },
            { name: "Invoices total amount", path: "/invoice-summary" },
          ],
        },
        {
          name: "Provimi",
          icon: <TableIcon />,
          subItems: [
            { name: "Shkollat", path: "/shkollat" },
{ name: "Nxënësit", path: "/nxenesit" },
          ],
        },
      ];
    } else {
      return [];
    }
  };

  const navItems = getNavItems();

  // Sidebar is "open" (showing text labels) in any of these states
  const isOpen = isExpanded || isHovered || isMobileOpen;

  const renderMenuItems = (items: NavItem[]) => (
    <ul className="flex flex-col gap-4">
      {items.map((nav, index) => (
        <li key={nav.name}>
          {nav.subItems ? (
            <button
              onClick={() => handleSubmenuToggle(index)}
              className={`menu-item group w-full ${
                openSubmenu?.index === index
                  ? "menu-item-active"
                  : "menu-item-inactive"
              } cursor-pointer`}
            >
              <span
                className={`menu-item-icon-size flex-shrink-0 ${
                  openSubmenu?.index === index
                    ? "menu-item-icon-active"
                    : "menu-item-icon-inactive"
                }`}
              >
                {nav.icon}
              </span>
              {isOpen && (
                <span className="menu-item-text truncate">{nav.name}</span>
              )}
              {isOpen && (
                <ChevronDownIcon
                  className={`ml-auto w-5 h-5 flex-shrink-0 transition-transform duration-200 ${
                    openSubmenu?.index === index ? "rotate-180 text-brand-500" : ""
                  }`}
                />
              )}
            </button>
          ) : (
            nav.path && (
              <Link
                to={nav.path}
                className={`menu-item group ${
                  isActive(nav.path) ? "menu-item-active" : "menu-item-inactive"
                }`}
              >
                <span
                  className={`menu-item-icon-size flex-shrink-0 ${
                    isActive(nav.path)
                      ? "menu-item-icon-active"
                      : "menu-item-icon-inactive"
                  }`}
                >
                  {nav.icon}
                </span>
                {isOpen && (
                  <span className="menu-item-text truncate">{nav.name}</span>
                )}
              </Link>
            )
          )}

          {/* Submenu */}
          {nav.subItems && isOpen && (
            <div
              ref={(el) => {
                subMenuRefs.current[`main-${index}`] = el;
              }}
              className="overflow-hidden transition-all duration-300"
              style={{
                height:
                  openSubmenu?.index === index
                    ? `${subMenuHeight[`main-${index}`]}px`
                    : "0px",
              }}
            >
              <ul className="mt-2 space-y-1 ml-9">
                {nav.subItems.map((subItem) => (
                  <li key={subItem.name}>
                    <Link
                      to={subItem.path}
                      className={`menu-dropdown-item ${
                        isActive(subItem.path)
                          ? "menu-dropdown-item-active"
                          : "menu-dropdown-item-inactive"
                      }`}
                    >
                      {subItem.name}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          )}
        </li>
      ))}
    </ul>
  );

  return (
    <aside
      className={`
        fixed top-0 left-0 z-50
        flex flex-col
        h-screen bg-white dark:bg-gray-900 text-gray-900
        border-r border-gray-200 dark:border-gray-800
        transition-all duration-300 ease-in-out
        px-5

        /* Mobile & Tablet: slide in/out as overlay */
        ${isMobileOpen ? "translate-x-0 w-[290px]" : "-translate-x-full w-[290px]"}

        /* Desktop: always visible, width based on expand state */
        lg:translate-x-0
        ${
          isExpanded
            ? "lg:w-[290px]"
            : isHovered
            ? "lg:w-[290px]"
            : "lg:w-[90px]"
        }
      `}
      onMouseEnter={() => !isExpanded && setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      {/* Logo area */}
      <div
        className={`py-8 flex ${
          !isExpanded && !isHovered ? "lg:justify-center" : "justify-start"
        }`}
      >
        <Link to="/">
          {isOpen ? (
            <img
              src="/images/logo/logo.png"
              alt="Logo"
              className="h-8 w-auto"
            />
          ) : (
            <img
              src="/images/logo/logo-icon.png"
              alt="Logo"
              className="h-8 w-8"
            />
          )}
        </Link>
      </div>

      {/* Navigation */}
      <div className="flex flex-col overflow-y-auto duration-300 ease-linear no-scrollbar">
        <nav className="mb-6">
          <div className="flex flex-col gap-4">
            <div>
              <h2 className="mb-4 text-xs uppercase flex leading-[20px] text-gray-400">
                {isOpen ? (
                  "Menu"
                ) : (
                  <HorizontaLDots className="size-6" />
                )}
              </h2>
              {renderMenuItems(navItems)}
            </div>
          </div>
        </nav>
      </div>
    </aside>
  );
};

export default AppSidebar;
