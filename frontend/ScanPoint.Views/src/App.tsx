import { lazy, Suspense } from "react";
import { BrowserRouter as Router, Routes, Route, Navigate } from "react-router-dom";

import ProtectedRoute from "./components/auth/ProtectedRoute";
import { ScrollToTop } from "./components/common/ScrollToTop";
import { AuthProvider } from "./context/AuthContext";
import InvoicesPage from "./pages/InvoicesPage";



// ============================================================
// LAZY IMPORTS — çdo faqe ngarkohet vetëm kur vizitohet
// (code splitting = aplikacioni hapet më shpejt fillimisht)
// ============================================================

// Auth
const SignIn = lazy(() => import("./pages/AuthPages/SignIn"));
const SignUp = lazy(() => import("./pages/AuthPages/SignUp"));

// Layout
const AppLayout = lazy(() => import("./layout/AppLayout"));

// Dashboard
const Home           = lazy(() => import("./pages/Dashboard/Home"));


// Tables
const CashiersTable         = lazy(() => import("./pages/Tables/CashiersTable"));
const ManagersTable         = lazy(() => import("./pages/Tables/ManagersTable"));
const ProductsInStockA      = lazy(() => import("./pages/Tables/ProductsInStockA"));
const ProductsOutOfStock    = lazy(() => import("./pages/Tables/ProductsOutOfStockTable"));
const PosTerminal           = lazy(() => import("./pages/Tables/PosTerminal"));
const CashiersA             = lazy(() => import("./pages/Tables/CashiersA"));

// Forms
const FormElements1 = lazy(() => import("./pages/Forms/FormElements"));
const FormElements2 = lazy(() => import("./pages/Forms/Add-Products"));

// Other
const UserProfiles       = lazy(() => import("./pages/UserProfiles"));
const Calendar           = lazy(() => import("./pages/Calendar"));
const InvoiceSummaryPage = lazy(() => import("./pages/InvoiceSummaryPage"));
const NotFound           = lazy(() => import("./pages/OtherPage/NotFound"));

// Charts
const LineChart = lazy(() => import("./pages/Charts/LineChart"));
const BarChart  = lazy(() => import("./pages/Charts/BarChart"));

// UI Elements
const Alerts  = lazy(() => import("./pages/UiElements/Alerts"));
const Avatars = lazy(() => import("./pages/UiElements/Avatars"));
const Badges  = lazy(() => import("./pages/UiElements/Badges"));
const Buttons = lazy(() => import("./pages/UiElements/Buttons"));
const Images  = lazy(() => import("./pages/UiElements/Images"));
const Videos  = lazy(() => import("./pages/UiElements/Videos"));

// ============================================================
// FAQET E REJA — Shkolla & Nxënës (Provimi)
// I importojmë lazy ashtu si çdo faqe tjetër në projekt
// ==================================================================================================================================================================




// ============================================================
// LOADING FALLBACK — shfaqet ndërkohë që faqja po ngarkohet
// ============================================================
function PageLoader() {
  return (
    <div className="flex items-center justify-center min-h-screen">
      <div className="w-8 h-8 border-4 border-brand-500 border-t-transparent rounded-full animate-spin" />
    </div>
  );
}

// ============================================================
// APP — komponenti rrënjë
// AuthProvider → mbështjell gjithçka për qasje te useAuth()
// Router       → menaxhon URL-në e browser-it
// Suspense     → tregon PageLoader derisa faqja lazy të ngarkohet
// ============================================================
export default function App() {
  return (
    <AuthProvider>
      <Router>
        <ScrollToTop />
        <Suspense fallback={<PageLoader />}>
          <Routes>

            {/* ================================================
                RRUGËT PUBLIKE — nuk kërkohet login
                Çdo vizitor mund t'i hapë këto URL
            ================================================ */}
            <Route path="/signin" element={<SignIn />} />
            <Route path="/signup" element={<SignUp />} />

            {/* ================================================
                RRUGËT E MBROJTURA — kërkohet login
                ProtectedRoute pa role = çdo user i loguar
                ProtectedRoute me allowedRoles = vetëm ato role
            ================================================ */}
            <Route element={<ProtectedRoute />}>
              <Route element={<AppLayout />}>

                {/* Root → ridrejto te dashboard-i kryesor */}
                <Route index element={<Navigate to="/dashboard/home" replace />} />

                {/* --- Dashboard sipas rolit --- */}

                {/* Vetëm Admin */}
                <Route element={<ProtectedRoute allowedRoles={["admin"]} />}>
                  <Route path="/dashboard/home" element={<Home />} />
                </Route>

                {/* Vetëm Cashier */}
                <Route element={<ProtectedRoute allowedRoles={["cashier"]} />}>
                  <Route path="/dashboard/userhome" element={<PosTerminal />} />
                </Route>

                {/* Vetëm Manager */}
                <Route element={<ProtectedRoute allowedRoles={["manager"]} />}>
                  <Route path="/dashboard/moderatorhome" element={<ProductsInStockA />} />
                </Route>

                {/* --- Faqe të përbashkëta (të gjitha rolet) --- */}
                <Route path="/profile"         element={<UserProfiles />} />
                <Route path="/calendar"        element={<Calendar />} />
                <Route path="/invoices"        element={<InvoicesPage />} />
                <Route path="/invoice-summary" element={<InvoiceSummaryPage />} />

                {/* --- Tabela --- */}
                <Route path="/basic-tables1"  element={<CashiersTable />} />
                <Route path="/basic-tables2"  element={<ManagersTable />} />
                <Route path="/basic-tables4"  element={<ProductsOutOfStock />} />
                <Route path="/basic-tables10" element={<ProductsInStockA />} />
                <Route path="/PosTerminal"    element={<PosTerminal />} />
                <Route path="/CashiersA"      element={<CashiersA />} />

                {/* --- Forma --- */}
                <Route path="/form-elements1" element={<FormElements1 />} />
                <Route path="/form-elements2" element={<FormElements2 />} />

                {/* --- Charts --- */}
                <Route path="/line-chart" element={<LineChart />} />
                <Route path="/bar-chart"  element={<BarChart />} />

                {/* --- UI Elements --- */}
                <Route path="/alerts"  element={<Alerts />} />
                <Route path="/avatars" element={<Avatars />} />
                <Route path="/badge"   element={<Badges />} />
                <Route path="/buttons" element={<Buttons />} />
                <Route path="/images"  element={<Images />} />
                <Route path="/videos"  element={<Videos />} />

                {/* ================================================
                    RRUGËT E REJA — Shkolla & Nxënës (Provimi)
                    Vendosura brenda AppLayout → marrin sidebar/header
                    Vendosura brenda ProtectedRoute → kërkohet login

                    URL-të:
                      /shkollat  → ShkollaPage (lista + CRUD i shkollave)
                      /nxenesit  → NxenesiPage (lista + CRUD + filtrim sipas shkollës)
                ================================================ =============================================================================================*/}
                
              </Route>
            </Route>

            {/* 404 — çdo URL tjetër */}
            <Route path="*" element={<NotFound />} />

          </Routes>
        </Suspense>
      </Router>
    </AuthProvider>
  );
}
