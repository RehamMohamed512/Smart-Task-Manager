// --------------------
// 1) Toggle Password
// --------------------
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

// --------------------
// 2) Register Form Submit
// --------------------
document.getElementById("registerForm").addEventListener("submit", async (e) => {
  e.preventDefault();

  const name = document.getElementById("name").value.trim();
  const email = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value.trim();
  const confirmPassword = document.getElementById("confirmPassword").value.trim();

  if (password !== confirmPassword) {
    alert("Passwords do not match!");
    return;
  }

  try {
    const response = await fetch("https://localhost:7251/api/User/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ name, email, password })
    });

    const data = await response.json();
    console.log(data);


    if (response.ok) {
      alert("Registration successful! Please login now.");
      window.location.href = "index.html";
    } else {
      alert(data.message || "Registration failed");
    }
  } catch (err) {
    alert("Error: " + err.message);
    console.log(err)
  }
});
