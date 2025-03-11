import { use, useEffect, useRef, useState } from "react";
import {
  TextField,
  Button,
  Card,
  CardContent,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Box,
} from "@mui/material";
import { Add, Delete, Logout, Update } from "@mui/icons-material";
import {
  deleteWeather,
  fetchWeather,
  fetchAllWeather,
  UpdateWeather,
  addWeather,
} from "../services/weatherservice";
import { useNavigate } from "react-router-dom";

interface WeatherData {
  id: number;
  cityName: string;
  temperature: number;
  weatherCondition: string;
  lastUpdated: string;
}

export default function WeatherDashboard() {
  const [city, setCity] = useState("");
  const [weatherList, setWeatherList] = useState<WeatherData[]>([]);
  const [addCity, setAddCity] = useState("");
  const navigate = useNavigate();
  useEffect(() => {
    async function loadWeather() {
      if (!city) {
        const weatherData = await fetchAllWeather();
        if (weatherData) {
          setWeatherList(weatherData);
        }
      } else {
        const weatherData = await fetchWeather(city);
        if (weatherData) {
          setWeatherList([weatherData]);
        }
      }
    }
    loadWeather();
  }, [city]);

  const handleFetchWeather = async () => {
    if (!city) return;
    const weatherData = await fetchWeather(city);
    if (weatherData) {
      setWeatherList((prev) => [...prev, weatherData]);
    }
  };

  const handleDeleteWeather = async (city: string) => {
    const success = await deleteWeather(city);
    if (success) {
      setWeatherList((prev) => prev.filter((w) => w.cityName !== city));
    }
  };

  const handleUpdateWeather = async (city: string) => {
    const weatherData = await UpdateWeather(city);
  };

  const handleAddWeather = async (): Promise<void> => {
    const isSuccess = await addWeather(addCity);
    if (isSuccess) {
      fetchAllWeather().then((data) => {
        setWeatherList(data);
        alert("City added successfully");
      });
    }
  };

  return (
    <Card
      sx={{
        width: "100vw",
        display: "flex",
        height: "100vh",
        justifyContent: "center",
        alignItems: "center",
        mx: "auto",
        mt: 5,
        p: 2,
      }}
    >
      <CardContent>
        <Typography variant="h5" textAlign="center" mb={2}>
          Weather Dashboard
        </Typography>

        <Box
          display="flex"
          justifyContent="flex-end"
          alignItems="center"
          width="100%"
          mb={2}
          sx={{ top: 16, right: 16, position: "absolute" }}
        >
          <TextField
            label="Add City"
            size="small"
            sx={{ height: "40px", mr: 1 }}
            value={addCity}
            onChange={(e) => setAddCity(e.target.value)}
          />
          <Button
            variant="contained"
            color="primary"
            size="small"
            startIcon={<Add />}
            sx={{
              height: "40px",
              width: "40px",
              textTransform: "none",
              px: 2,
            }}
            onClick={handleAddWeather}
          ></Button>
          <Button
            variant="contained"
            color="error"
            size="small"
            startIcon={<Logout />}
            sx={{
              height: "40px",
              width: "80px",
              textTransform: "none",
              px: 2,
              ml: 2,
            }}
            onClick={() => {
              localStorage.removeItem("token");
              navigate("/login");
            }}
          >
            Logout
          </Button>
        </Box>

        <TextField
          label="Enter City"
          fullWidth
          margin="normal"
          value={city}
          onChange={(e) => setCity(e.target.value)}
        />
        <Button
          fullWidth
          variant="contained"
          sx={{ mt: 2 }}
          onClick={handleFetchWeather}
        >
          Fetch Weather
        </Button>

        {weatherList.length > 0 && (
          <TableContainer component={Paper} sx={{ mt: 3 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>City</TableCell>
                  <TableCell>Temperature (°C)</TableCell>
                  <TableCell>Condition</TableCell>
                  <TableCell>Last Updated</TableCell>
                  <TableCell>Action</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {weatherList.map((weather) => (
                  <TableRow key={weather.id}>
                    <TableCell>{weather.cityName}</TableCell>
                    <TableCell>{weather.temperature}</TableCell>
                    <TableCell>{weather.weatherCondition}</TableCell>
                    <TableCell>
                      {new Date(weather.lastUpdated).toLocaleString()}
                    </TableCell>
                    <TableCell>
                      <IconButton
                        onClick={() => handleUpdateWeather(weather.cityName)}
                      >
                        <Update />
                      </IconButton>
                    </TableCell>
                    <TableCell>
                      <IconButton
                        onClick={() => handleDeleteWeather(weather.cityName)}
                        color="error"
                      >
                        <Delete />
                      </IconButton>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </CardContent>
    </Card>
  );
}
