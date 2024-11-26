function addNewHorse(){
    const name = document.getElementById("horseName").value;
    const birthDateValue = document.getElementById("birthDate").value;
    const birthDate = birthDateValue ? new Date(birthDateValue).toISOString() : null;
    const centre = document.getElementById("equestrianCentreSelect").value;
    let centreId = parseInt(centre, 10);
    if (Number.isNaN(centreId) || centreId === -1){
        centreId =  null;
    }

    fetch("/User/AddHorse", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Name: name, BirthDate: birthDate, EquestrianCentreId: centreId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("addHorseModal").style.display = "none";
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case horseNameConstraint:
                            document.getElementById("addHorseError").textContent = "Name must be between 2 and 50 characters.";
                            document.getElementById("horseName").value = "";
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

function openAddHorseView(){
    closeAllModals();
    populateCentreDropdown("equestrianCentreSelect");
    document.getElementById("addHorseModal").style.display = "block";
}

function closeAddHorseView(){
    document.getElementById("addHorseModal").style.display = "none";

    document.getElementById("horseName").textContent = "";
    document.getElementById("birthDate").value = "";
    document.getElementById("addHorseError").textContent = "";
}

function addNewEquestrianCentre(){
    const name = document.getElementById("centreName").value;
    const address = document.getElementById("address").value;

    fetch("/User/AddEquestrianCentre", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Name: name, Address: address })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("addCentreModal").style.display = "none";
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case centreNameConstraint:
                            document.getElementById("addCentreError").textContent = "Name must be between 2 and 250 characters.";
                            document.getElementById("centreName").value = "";
                            break;
                        case centreAddressConstraint:
                            document.getElementById("addCentreError").textContent = "Address must be between 2 and 250 characters.";
                            document.getElementById("address").value = "";
                            break;
                        default:
                            document.getElementById("addCentreError").textContent = "An error occurred while creating a new equestrian centre.";
                    }
                    return;
                }
                document.getElementById("addCentreError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error adding centre:", error));
}

function openAddEquestrianCentreView(){
    closeAllModals();
    document.getElementById("addCentreModal").style.display = "block";
}

function closeAddEquestrianCentreView(){
    document.getElementById("addCentreModal").style.display = "none";

    document.getElementById("centreName").textContent = "";
    document.getElementById("address").textContent = "";
    document.getElementById("addCentreError").textContent = "";
}


function openRemoveUserView() {
    document.getElementById("removeUserModal").style.display = "block";
}

function removeUser(id) {
    fetch(`/User/Remove`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Id: id })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = `/`;
                closeRemoveHorseView();
            } else {
                document.getElementById("removeUserError").textContent = "An error occurred while removing your user profile. " + data.message;
            }
        })
        .catch(error => console.error("Error removing user profile:", error));
}

function closeRemoveUserView(){
    document.getElementById("removeUserModal").style.display = "none";
    document.getElementById("removeUserError").textContent = "";
}



const horseNameConstraint = "chk_name_length";
const centreNameConstraint = "chk_name_length";
const centreAddressConstraint = "chk_address_length";