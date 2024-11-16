function openLogInModal() {
    closeAllModals();
    document.getElementById("logInModal").style.display = "block";
}

function openSignUpModal() {
    closeAllModals();
    document.getElementById("signUpModal").style.display = "block";
}

function closeAllModals() {
    document.getElementById("logInModal").style.display = "none";
    document.getElementById("signUpModal").style.display = "none";
    document.getElementById("searchResultsModal").style.display = "none";

    ["logInEmail", "logInPassword", "signUpName", "signUpEmail", "signUpPassword", "signUpPasswordRepeat"].forEach(id => {
        document.getElementById(id).value = "";
    });

    document.getElementById("logInError").textContent = "";
    
    document.getElementById("signUpError").textContent = "";
    document.getElementById("signUpErrorName").textContent = "";
    document.getElementById("signUpErrorEmail").textContent = "";
    document.getElementById("signUpErrorPassword").textContent = "";
    document.getElementById("signUpErrorPasswordRepeat").textContent = "";
    
    document.getElementById("search-bar").value = "";
}

window.onbeforeunload = closeAllModals;

function attemptLogIn() {
    const email = document.getElementById("logInEmail").value;
    const password = document.getElementById("logInPassword").value;

    fetch("/Account/logIn", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ email: email, password: password })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("logInModal").style.display = "none";
                window.location.reload(true);
            } else {
                document.getElementById("logInError").textContent = data.message;
                document.getElementById("logInPassword").value = "";
            }
        })
        .catch(error => console.error("Error during logIn:", error));
}

function logOut(){
    fetch("/Account/LogOut", { //NameOfController/NameOfAction
        method: "POST"
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = "/";
            } else {
                console.error("Error during logout:", data.message);
            }
        })
        .catch(error => console.error("Error during logout:", error));
}

function attemptSignUp(){
    const name = document.getElementById("signUpName").value;
    const email = document.getElementById("signUpEmail").value;
    const password = document.getElementById("signUpPassword").value;
    const passwordRepeat = document.getElementById("signUpPasswordRepeat").value;

    document.getElementById("signUpError").textContent = "";
    document.getElementById("signUpErrorName").textContent = "";
    document.getElementById("signUpErrorEmail").textContent = "";
    document.getElementById("signUpErrorPassword").textContent = "";
    document.getElementById("signUpErrorPasswordRepeat").textContent = "";

    if(password !== passwordRepeat){
        document.getElementById("signUpErrorPasswordRepeat").textContent = "Passwords do not match.\n Password must be between 4 and 60 characters.";
        document.getElementById("signUpPasswordRepeat").value = "";
        return;
    }

    fetch("/Account/signUp", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ name: name, email: email, password: password })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("signUpModal").style.display = "none";
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case logInNameConstraint:
                            document.getElementById("signUpErrorName").textContent = "Name must be between 2 and 50 characters.";
                            document.getElementById("signUpName").value = "";
                            break;
                        case logInEmailConstraint:
                            document.getElementById("signUpErrorEmail").textContent = "Email must be between 5 and 255 characters.";
                            document.getElementById("signUpEmail").value = "";
                            break;
                        case logInPasswordConstraint:
                            document.getElementById("signUpErrorPassword").textContent = "Password must be between 4 and 60 characters.";
                            document.getElementById("signUpPassword").value = "";
                            document.getElementById("signUpPasswordRepeat").value = "";
                            break;
                        case logInEmailExistsConstraint:
                            document.getElementById("signUpErrorEmail").textContent = "User with this email already exists.";
                            document.getElementById("signUpEmail").value = "";
                            document.getElementById("signUpPassword").value = "";
                            document.getElementById("signUpPasswordRepeat").value = "";
                            break;
                        default:
                            document.getElementById("signUpError").textContent = "An error occurred while creating a new user.";
                    }
                    return;
                }
                document.getElementById("signUpError").textContent = data.message;
                document.getElementById("signUpPassword").value = "";
                document.getElementById("signUpPasswordRepeat").value= "";
            }
        })
        .catch(error => console.error("Error during sign up:", error));
}



const logInNameConstraint = "chk_name_length";
const logInEmailConstraint = "chk_email_length";
const logInPasswordConstraint = "chk_password_length";
const logInEmailExistsConstraint = "uzytkownik_email_key";