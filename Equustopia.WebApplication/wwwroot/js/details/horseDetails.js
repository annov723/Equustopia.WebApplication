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

function editHorse() {
    const horseId = document.getElementById("editHorseId").value;
    const horseName = document.getElementById("editHorseName").value;
    const equestrianCentreId = document.getElementById("editEquestrianCentreSelect").value;
    const birthDate = document.getElementById("editBirthDate").value;

    const horseData = {
        id: horseId,
        name: horseName,
        centreId: equestrianCentreId,
        birthDate: birthDate
    };

    fetch(`/Horse/Edit/${horseId}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(horseData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert("Horse details updated successfully.");
                window.location.href = `/Horse/Details/${horseId}`;  // Redirect after success
            } else {
                alert("Error updating horse details.");
            }
        })
        .catch(error => console.error("Error:", error));
}

function openRemoveHorseView() {
    
}

function removeHorse() {

}