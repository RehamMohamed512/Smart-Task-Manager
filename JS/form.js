

// Toggle password visibility
const password = document.getElementById("password"); 
const togglePass = document.getElementById("togglePass");

togglePass.addEventListener("click", () => {
  if (password.type === "password") {
    password.type = "text";
    togglePass.innerHTML = `<i class="fa-solid fa-eye"></i>`;
  } else {
    password.type = "password";
    togglePass.innerHTML = `<i class="fa-solid fa-eye-slash"></i>`;
  }
});



document.getElementById("myForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const email = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();

  try {
    const response = await fetch("https://localhost:7251/api/User/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password })
    });

    const data = await response.json();

    if (response.ok) {
      // ✅ خزني UserId في LocalStorage
      localStorage.setItem("userId", data.user.id);
      alert("Login successful!");
      window.location.href = "mainPage.html"; // بعد login يروح لصفحة التاسكات
    } else {
      alert(data.message || "Invalid email or password");
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
});
