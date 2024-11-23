﻿function openEditHorseView() {
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
                document.getElementById("editHorseModal").style.display = "none";
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
        .catch(error => console.error("Error editing horse:", error));
}

function openRemoveHorseView() {
    
}

function removeHorse() {

}