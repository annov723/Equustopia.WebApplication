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