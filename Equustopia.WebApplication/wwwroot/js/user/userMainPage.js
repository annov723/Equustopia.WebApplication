function addNewHorse(){
    const name = document.getElementById("name").value;
    const birthDateValue = document.getElementById("birthDate").value;
    const birthDate = birthDateValue ? new Date(birthDateValue).toISOString() : null;

    fetch("/User/AddHorse", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Name: name, BirthDate: birthDate })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("logInModal").style.display = "none";
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case horseNameConstraint:
                            document.getElementById("addHorseError").textContent = "Name must be between 2 and 50 characters.";
                            document.getElementById("name").value = "";
                            break;
                        default:
                            document.getElementById("addHorseError").textContent = "An error occurred while creating a new horse.";
                    }
                    return;
                }
                document.getElementById("addHorseError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error adding horse:", error));
}

function addNewCentre(){
    
}

function openAddHorseView(){
    closeAllModals();
    document.getElementById("addHorseModal").style.display = "block";
}

function closeAddHorseView(){
    document.getElementById("addHorseModal").style.display = "none";

    document.getElementById("name").textContent = "";
    document.getElementById("birthDate").value = "";
}



const horseNameConstraint = "chk_name_length";