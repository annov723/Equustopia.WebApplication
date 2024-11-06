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

    ["loginEmail", "loginPassword", "signupName", "signupEmail", "signupPassword", "signupPasswordRepeat"].forEach(id => {
        document.getElementById(id).value = "";
    });

    document.getElementById("loginError").textContent = "";
}

window.onbeforeunload = closeAllModals;

function attemptLogin() {
    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;

    fetch("/Login/Login", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email: email, password: password })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("loginModal").style.display = "none";
                document.getElementById("loginButton").style.display = "none";
                document.getElementById("signupButton").style.display = "none";
                window.location.reload(true);
            } else {
                document.getElementById("loginError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error during login:", error));
}

function logout(){
    fetch("/Login/Logout", { //NameOfController/NameOfAction
        method: "POST"
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.reload(true);
            } else {
                console.error("Error during logout:", data.message);
            }
        })
        .catch(error => console.error("Error during logout:", error));
}

function attemptSignUp(){
    const name = document.getElementById("signupName").value;
    const email = document.getElementById("signupEmail").value;
    const password = document.getElementById("signupPassword").value;
    const passwordRepeat = document.getElementById("signupPasswordRepeat").value;

    if(password !== passwordRepeat){
        document.getElementById("signupError").textContent = "Passwords do not match";
        return;
    }

    fetch("/Login/Signup", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ name: name, email: email, password: password })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("loginModal").style.display = "none";
                document.getElementById("loginButton").style.display = "none";
                document.getElementById("signupButton").style.display = "none";
                window.location.reload(true);
            } else {
                document.getElementById("signupError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error during login:", error));
}