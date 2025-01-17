function viewDetails(id, type) {
    if (type === "Horse") {
        window.location.href = `/Horse/Details/${id}`;
    } else if (type === "EquestrianCentre") {
        window.location.href = `/EquestrianCentre/Details/${id}`;
    } else {
        console.error("Error opening items details page.");
    }
}

function updateRequestStatus(requestId, status) {
    fetch(`/EquestrianCentre/UpdateRequestStatus`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ requestId, status })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                location.reload();
            } else {
                alert("Failed to update status: " + data.message);
            }
        })
        .catch(error => {
            alert("Error: " + error.message);
        });
}
