import {
  render,
  screen,
  fireEvent,
  waitFor,
  expect,
} from "@testing-library/react";
import { describe, test, expect, vi, beforeEach } from "vitest";
import WeatherDashboard from "../pages/WeatherDashboard";
import {
  fetchAllWeather,
  fetchWeather,
  deleteWeather,
  addWeather,
} from "../services/weatherservice";
import { MemoryRouter } from "react-router-dom";
import "@testing-library/jest-dom";

vi.mock("../services/weatherservice", () => ({
  addWeather: vi.fn().mockResolvedValue(1),
  fetchAllWeather: vi.fn().mockResolvedValue([
    {
      id: 2,
      cityName: "Los Angeles",
      temperature: 22,
      weatherCondition: "Sunny",
      lastUpdated: new Date().toISOString(),
    },
  ]),
  fetchWeather: vi.fn(),
  deleteWeather: vi.fn(),
}));

describe("WeatherDashboard Component", () => {
  beforeEach(() => {
    vi.clearAllMocks();
  });

  test("renders Weather Dashboard title", () => {
    render(
      <MemoryRouter>
        <WeatherDashboard />
      </MemoryRouter>
    );

    expect(screen.getByText("Weather Dashboard")).toBeInTheDocument();
  });

  test("fetches and displays weather data", async () => {
    const mockWeatherData = [
      {
        id: 1,
        cityName: "New York",
        temperature: 25,
        weatherCondition: "Clear",
        lastUpdated: new Date().toISOString(),
      },
    ];

    fetchAllWeather.mockResolvedValue([mockWeatherData[0]]);

    render(
      <MemoryRouter>
        <WeatherDashboard />
      </MemoryRouter>
    );

    await waitFor(() => {
      expect(screen.getByText("New York")).toBeInTheDocument();
      expect(screen.getByText("Temperature (°C)")).toBeInTheDocument();
    });
  });

  test("adds a new city", async () => {
    addWeather.mockResolvedValue(true);
    fetchAllWeather.mockResolvedValue([
      {
        id: 2,
        cityName: "Los Angeles",
        temperature: 22,
        weatherCondition: "Sunny",
        lastUpdated: new Date().toISOString(),
      },
    ]);

    render(
      <MemoryRouter>
        <WeatherDashboard />
      </MemoryRouter>
    );

    const addCityInput = screen.getByLabelText("Add City");
    fireEvent.change(addCityInput, { target: { value: "Los Angeles" } });

    const addButton = screen.getByRole("button", { name: /add/i });
    fireEvent.click(addButton);

    await waitFor(() => {
      expect(screen.getByText("Los Angeles")).toBeInTheDocument();
    });
  });

  test("deletes a city", async () => {
    fetchAllWeather.mockResolvedValue([
      {
        id: 3,
        cityName: "Chicago",
        temperature: 15,
        weatherCondition: "Cloudy",
        lastUpdated: new Date().toISOString(),
      },
    ]);

    deleteWeather.mockResolvedValue(true);

    render(
      <MemoryRouter>
        <WeatherDashboard />
      </MemoryRouter>
    );

    const deleteButton = screen.getByRole("button", { name: /delete/i });
    fireEvent.click(deleteButton);

    await waitFor(() => {
      expect(screen.queryByText("Chicago")).not.toBeInTheDocument();
    });
  });

  test("fetches weather by city", async () => {
    fetchWeather.mockResolvedValue({
      id: 4,
      cityName: "Miami",
      temperature: 30,
      weatherCondition: "Hot",
      lastUpdated: new Date().toISOString(),
    });

    render(
      <MemoryRouter>
        <WeatherDashboard />
      </MemoryRouter>
    );

    const cityInput = screen.getByLabelText("Enter City");
    fireEvent.change(cityInput, { target: { value: "Miami" } });

    const fetchButton = screen.getByText("Fetch Weather");
    fireEvent.click(fetchButton);

    await waitFor(() => {
      expect(screen.getByText("Miami")).toBeInTheDocument();
    });
  });
});
