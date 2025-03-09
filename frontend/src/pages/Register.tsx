import { useState } from "react";
import {
  TextField,
  Button,
  Card,
  CardContent,
  Typography,
} from "@mui/material";
import { useNavigate } from "react-router-dom";
import { register } from "../services/authService";

export default function Register() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();

  const handleRegister = async () => {
    // Mock API request
    if (username && password) {
      var response = await register(username, password);
      if (response) {
        alert("Registration successful! Please log in.");
        navigate("/login");
      } else {
        alert("failed to register user.");
      }
    }
  };

  return (
    <Card
      sx={{
        mx: "auto",
        mt: 5,
        p: 2,
        width: "100vw",
        height: "100vh",
        display: "flex",
        justifyContent: "center",
      }}
    >
      <CardContent>
        <Typography variant="h5" textAlign="center" mb={2}>
          Register
        </Typography>
        <TextField
          label="Username"
          fullWidth
          margin="normal"
          value={username}
          onChange={(e) => setUsername(e.target.value)}
        />
        <TextField
          label="Password"
          type="password"
          fullWidth
          margin="normal"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
        />
        <Button
          fullWidth
          variant="contained"
          sx={{ mt: 2 }}
          onClick={handleRegister}
        >
          Register
        </Button>
      </CardContent>
    </Card>
  );
}
