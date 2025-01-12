function openEditHorseView() {
    closeAllModals();
    populateCentreDropdown("editEquestrianCentreSelect");
    const inputs = document.querySelectorAll("#editHorseModal [data-default-value]");
    inputs.forEach(input => {
        input.value = input.getAttribute("data-default-value");
    });
    
    document.getElementById("editHorseModal").style.display = "block";
}

function closeEditHorseView(){
    document.getElementById("editHorseModal").style.display = "none";
    document.getElementById("editHorseError").textContent = "";
}

function editHorse(id) {
    const name = document.getElementById("editHorseName").value;
    const birthDateValue = document.getElementById("editBirthDate").value;
    const birthDate = birthDateValue ? new Date(birthDateValue).toISOString() : null;
    const centre = document.getElementById("editEquestrianCentreSelect").value;
    let centreId = parseInt(centre, 10);
    if (Number.isNaN(centreId) || centreId === -1){
        centreId =  null;
    }

    fetch(`/Horse/Edit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Id: id, Name: name, BirthDate: birthDate, EquestrianCentreId: centreId })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                closeEditHorseView();
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case horseNameConstraint:
                            document.getElementById("editHorseError").textContent = "Name must be between 2 and 50 characters.";
                            document.getElementById("horseName").value = "";
                            break;
                        default:
                            document.getElementById("editHorseError").textContent = "An error occurred while creating a new horse.";
                    }
                    return;
                }
                document.getElementById("editHorseError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error editing horse:", error));
}



function openRemoveHorseView() {
    document.getElementById("removeHorseModal").style.display = "block";
}

function closeRemoveHorseView(){
    document.getElementById("removeHorseModal").style.display = "none";
    document.getElementById("removeHorseError").textContent = "";
}

function removeHorse(id) {
    fetch(`/Horse/Remove`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ id: id })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = `/User/UserMainPage`;
                closeRemoveHorseView();
            } else {
                document.getElementById("removeHorseError").textContent = "An error occurred while removing a horse.";
            }
        })
        .catch(error => console.error("Error removing horse:", error));
}

function horsePrivacyChange(id) {

    fetch(`/Horse/PrivacyChange`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Id: id })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const button1 = document.getElementById('horse-privacy-1');
                const button2 = document.getElementById('horse-privacy-2');
                if(button1.disabled) {
                    button1.disabled=false;
                    button2.disabled=true;
                }
                else {
                    button1.disabled=true;
                    button2.disabled=false;
                }
            } else {
                alert("An error occurred while updating privacy. " + data.message);
            }
        })
        .catch(error => console.error("Error updating privacy:", error));
}

const horseNameConstraint = "chk_name_length";