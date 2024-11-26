function openEditCentreView() {
    closeAllModals();
    const inputs = document.querySelectorAll("#editCentreModal [data-default-value]");
    inputs.forEach(input => {
        input.value = input.getAttribute("data-default-value");
    });

    document.getElementById("editCentreModal").style.display = "block";
}

function closeEditCentreView(){
    document.getElementById("editCentreModal").style.display = "none";
    document.getElementById("editCentreError").textContent = "";
}

function editEquestrianCentre(id) {
    const name = document.getElementById("editCentreName").value;
    const address = document.getElementById("editAddress").value;

    fetch(`/EquestrianCentre/Edit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ Id: id, Name: name, Address: address,  })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                closeEditCentreView();
                window.location.reload(true);
            } else {
                if(data.message === ""){
                    switch (data.constraintName){
                        case centreNameConstraint:
                            document.getElementById("editCentreError").textContent = "Name must be between 2 and 250 characters.";
                            document.getElementById("editCentreName").value = "";
                            break;
                        case centreAddressConstraint:
                            document.getElementById("editCentreError").textContent = "Address must be between 2 and 250 characters.";
                            document.getElementById("editAddress").value = "";
                            break;
                        default:
                            document.getElementById("editCentreError").textContent = "An error occurred while creating a new centre.";
                    }
                    return;
                }
                document.getElementById("editCentreError").textContent = data.message;
            }
        })
        .catch(error => console.error("Error editing centre:", error));
}



function openRemoveCentreView() {
    document.getElementById("removeCentreModal").style.display = "block";
}

function closeRemoveCentreView(){
    document.getElementById("removeCentreModal").style.display = "none";
    document.getElementById("removeCentreError").textContent = "";
}

function removeEquestrianCentre(id) {
    fetch(`/EquestrianCentre/Remove`, {
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
                closeRemoveCentreView();
            } else {
                document.getElementById("removeCentreError").textContent = "An error occurred while removing a centre.";
            }
        })
        .catch(error => console.error("Error removing centre:", error));
}

const centreNameConstraint = "chk_name_length";
const centreAddressConstraint = "chk_address_length";