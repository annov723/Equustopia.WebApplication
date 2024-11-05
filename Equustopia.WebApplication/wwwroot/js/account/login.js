function openLoginModal() {
    closeAllModals();
    document.getElementById("loginModal").style.display = "block";
}

function openSignupModal() {
    closeAllModals();
    document.getElementById("signupModal").style.display = "block";
}

function closeAllModals() {
    document.getElementById("loginModal").style.display = "none";
    document.getElementById("signupModal").style.display = "none";
}

window.onbeforeunload = closeAllModals;

function attemptLogin() {
    // Get the form data
    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    // Send login data via AJAX
    fetch("/Account/Login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email: email, password: password })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Hide the login modal and login button
                document.getElementById("loginModal").style.display = "none";
                document.getElementById("loginButton").style.display = "none";

                // Optionally show the logged-in user's information
                alert("Login successful!");
            } else {
                // Show error message in modal
                document.getElementById("loginError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error during login:", error));
}