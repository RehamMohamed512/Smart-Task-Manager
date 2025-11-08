const API_URL = "https://localhost:7251/api/Task";

// Get userId من LocalStorage
const userId = localStorage.getItem("userId");

const tasksDiv = document.getElementById("tasks");
const titleInput = document.getElementById("title");
const descInput = document.getElementById("description");
const dateInput = document.getElementById("dueDate");
const submitBtn = document.getElementById("submit");
const taskForm = document.getElementById("taskForm");

let editId = null;


if (!userId) {
  alert("You must login first!");
  window.location.href = "index.html";
 
}


taskForm.addEventListener("submit", function (e) {
  e.preventDefault();
  console.log("Form submitted without reload ");
});

// ✅ Fetch all tasks
async function fetchTasks() {
  try {
    const res = await fetch(`${API_URL}/user/${userId}`);
    const result = await res.json();

    if (res.ok) {
      displayTasks(result.data);
    } else {
      tasksDiv.innerHTML = `<p>No tasks yet</p>`;
    }
  } catch (err) {
    alert("Error fetching tasks: " + err.message);
  }
}

// ✅ Display tasks in UI
function displayTasks(tasks) {
  tasksDiv.innerHTML = "";
  if (!tasks || tasks.length === 0) {
    tasksDiv.innerHTML = "<p>No tasks yet.</p>";
    return;
  }

  tasks.forEach((task) => {
    const taskEl = document.createElement("div");
    taskEl.className = "task";

    if (task.isCompleted) {
      taskEl.style.backgroundColor = "#d4edda";
    }

    taskEl.innerHTML = `
  <div class="task-left">
    <div class="complete-btn ${task.isCompleted ? "checked" : ""}" 
         onclick="toggleComplete(${task.id}, ${task.isCompleted})">
      ${task.isCompleted ? "✔" : ""}
    </div>
    <div class="task-details">
      <h3>${task.title}</h3>
      <p>${task.description}</p>
      <small>Due: ${task.dueDate ? task.dueDate.split("T")[0] : "No date"}</small>
    </div>
  </div>
  <div class="task-actions">
    <button onclick="editTask(${task.id}, '${task.title}', '${task.description}', '${task.dueDate || ""}')"  id="edit">Edit</button>
    <button onclick="deleteTask(${task.id})"  id="del">Delete</button>
  </div>
`;
    tasksDiv.appendChild(taskEl);
  });
}

// ✅ Add or Update task
submitBtn.onclick = async function () {
  let title = titleInput.value.trim();
  let description = descInput.value.trim();
  let dueDate = dateInput.value || null;

  if (!title || !description) {
    return alert("Please fill both Title and Description");
  }

  const task = {
    title,
    description,
    dueDate,
    // userId: parseInt(userId),
    // isCompleted: false
  };

  try {
    let res;
    if (editId === null) {
      // 🆕 إضافة مهمة جديدة
      res = await fetch(`${API_URL}/user/${userId}`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
      });
    } else {
      // ✏️ تحديث مهمة موجودة
      res = await fetch(`${API_URL}/${editId}/user/${userId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(task)
      });
      editId = null;
      submitBtn.textContent = "Add Task";
    }

    if (res.ok) {
      titleInput.value = "";
      descInput.value = "";
      dateInput.value = "";
      fetchTasks();
    } else {
      const result = await res.json();
      alert(result.message || "Failed to save task");
       titleInput.value = "";
      descInput.value = "";
      dateInput.value = "";
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
};

// ✅ Edit task
function editTask(id, title, description, dueDate) {
  titleInput.value = title;
  descInput.value = description;
  dateInput.value = dueDate ? dueDate.split("T")[0] : "";
  editId = id;
  submitBtn.textContent = "Update Task";
}

// ✅ Toggle Complete/Undo
async function toggleComplete(id, isCompleted) {
  try {
    const res = await fetch(`${API_URL}/${id}/toggle/user/${userId}`, {
      method: "PATCH"
    });

    if (res.ok) {
      fetchTasks();
    } else {
      const result = await res.json();
      alert(result.message || "Failed to update task");
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
}

// ✅ Delete task
async function deleteTask(id) {
  if (!confirm("Are you sure you want to delete this task?")) return;

  try {
    const res = await fetch(`${API_URL}/${id}/user/${userId}`, {
      method: "DELETE"
    });
    if (res.ok) {
      fetchTasks();
    } else {
      const result = await res.json();
      alert(result.message || "Failed to delete task");
    }
  } catch (err) {
    alert("Error: " + err.message);
  }
}

// ✅ Load tasks on page load
fetchTasks();
