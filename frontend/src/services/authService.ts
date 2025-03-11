const apiBaseUrl = import.meta.env.VITE_API_BASE_URL;
export async function register(Username: string, Password: string) {
  try {
    const response = await fetch(`${apiBaseUrl}/api/auth/register`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ Username, Password }),
    });
    if (!response.ok) {
      throw new Error("Failed to register");
    }
    return true;
  } catch (error) {
    console.error("Error registering:", error);
    return false;
  }
}

export async function login(Username: string, Password: string) {
  try {
    const response = await fetch(`${apiBaseUrl}/api/auth/login`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ Username, Password }),
    });
    if (!response.ok) {
      throw new Error("Failed to login");
    }
    const data = await response.json();
    localStorage.setItem("token", data.token);
    return true;
  } catch (error) {
    alert("Invalid username or password");
    console.error("Error logging in:", error);
    return false;
  }
}
