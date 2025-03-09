export async function register(Username: string, Password: string) {
  try {
    const response = await fetch("https://localhost:7103/auth/register", {
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
    const response = await fetch("https://localhost:7103/auth/login", {
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
    console.error("Error logging in:", error);
    return false;
  }
}
