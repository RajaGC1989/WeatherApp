const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
export async function fetchWeather(city: string) {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No token found");
    }
    const response = await fetch(` ${apiBaseUrl}/api/weather/${city}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      method: "GET",
    });
    const result = await response.json();

    if (!response.ok) {
      throw new Error(result.message || "Failed to fetch weather data");
    }

    return result.data;
  } catch (error) {
    console.error("Error fetching weather:", error);
    return null;
  }
}

export async function UpdateWeather(city: string) {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No token found");
    }
    const response = await fetch(` ${apiBaseUrl}/api/weather/${city}`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      method: "PUT",
    });
    if (!response.ok) {
      throw new Error("Failed to update weather data");
    }
    return await response.json();
  } catch (error) {
    console.error("Error updating weather:", error);
    return null;
  }
}

export async function deleteWeather(city: string) {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No token found");
    }
    const response = await fetch(`${apiBaseUrl}/api/weather/${city}`, {
      method: "DELETE",
      headers: {
        Authorization: `Bearer ${token}`,
      },
    });
    if (!response.ok) {
      throw new Error("Failed to delete weather data");
    }
    return true;
  } catch (error) {
    console.error("Error deleting weather:", error);
    return false;
  }
}

export async function fetchAllWeather() {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No token found");
    }
    const response = await fetch(`${apiBaseUrl}/api/weather`, {
      headers: {
        Authorization: `Bearer ${token}`,
      },
      method: "GET",
    });
    const result = await response.json();

    if (!response.ok) {
      throw new Error(result.message || "Failed to fetch weather data");
    }

    return result.data;
  } catch (error) {
    console.error("Error fetching weather:", error);
    return null;
  }
}

export async function addWeather(city: string) {
  try {
    const token = localStorage.getItem("token");
    if (!token) {
      throw new Error("No token found");
    }

    const response = await fetch(`${apiBaseUrl}/api/weather/fetch/${city}`, {
      method: "GET",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
    });

    if (response.status === 404) {
      alert("City not found");
      throw new Error("City not found");
    }

    if (!response.ok) {
      const result = await response.json();
      throw new Error(result.message || "Failed to add weather data");
    }

    return true;
  } catch (error) {
    console.error("Error adding weather:", error);
    return false;
  }
}
