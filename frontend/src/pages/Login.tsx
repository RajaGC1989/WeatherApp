import { Box, TextField, Button, Typography } from "@mui/material";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";
import { login } from "../services/authService";

export default function Login() {
  const navigate = useNavigate();
  const auth = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleLogin = async () => {
    auth.logout();
    const response = await login(username, password);
    if (response) {
      navigate("/dashboard");
      auth.login(username);
    } else {
      setError("Invalid username or password");
    }
  };

  return (
    <Box
      display="flex"
      justifyContent="center"
      alignItems="center"
      height="100vh" // Takes full viewport height
      width="100vw" // Ensures full width
      bgcolor="#f4f4f4" // Optional background color
    >
      <Box
        width={320}
        p={3}
        borderRadius={2}
        boxShadow={3}
        textAlign="center"
        bgcolor="white"
      >
        <Typography variant="h5" mb={2}>
          Login
        </Typography>
        <TextField
          label="Username"
          fullWidth
          margin="normal"
          onChange={(e) => {
            setUsername(e.target.value);
          }}
        />
        <TextField
          label="Password"
          type="password"
          fullWidth
          margin="normal"
          onChange={(e) => {
            setPassword(e.target.value);
          }}
        />
        <Button variant="contained" fullWidth onClick={handleLogin}>
          LOGIN
        </Button>
        <p>
          New User? <Link to="/register">Register Here</Link>
        </p>
      </Box>
    </Box>
  );
}
