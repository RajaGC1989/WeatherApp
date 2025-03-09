import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import LoginPage from "./pages/Login";
import Dashboard from "./pages/WeatherDashboard";
import { AuthProvider } from "./context/AuthContext";
import TestCss from "./pages/TestCss";
import Register from "./pages/Register";

function App() {
  return (
    <AuthProvider>
      <Router>
        <Routes>
          <Route path="/" element={<LoginPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/dashboard" element={<Dashboard />} />
          <Route path="/register" element={<Register />} />
          <Route path="/test" element={<TestCss />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
}

export default App;
