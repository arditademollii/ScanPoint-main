import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

interface ProtectedRouteProps {
  allowedRoles?: Array<"admin" | "manager" | "cashier">;
}

// ============================================================
// PROTECTED ROUTE
// Nëse përdoruesi nuk është kyçur → ridrejtohet te /signin
// Nëse ka role të kufizuara dhe roli nuk përputhet → 403
// ============================================================
export default function ProtectedRoute({ allowedRoles }: ProtectedRouteProps) {
  const { isAuthenticated, user } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/signin" replace />;
  }

  if (allowedRoles && user && !allowedRoles.includes(user.role)) {
    // Ridrejto te dashboard-i i rolit të tyre
    const roleHome: Record<string, string> = {
      admin: "/dashboard/home",
      manager: "/dashboard/moderatorhome",
      cashier: "/dashboard/userhome",
    };
    return <Navigate to={roleHome[user.role] ?? "/signin"} replace />;
  }

  return <Outlet />;
}
