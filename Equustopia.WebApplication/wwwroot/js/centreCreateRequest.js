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





document.addEventListener("DOMContentLoaded", function () {
    // Populate user dropdown
    fetch('/Moderator/GetUsersWithCentres')
        .then(response => response.json())
        .then(users => {
            const userSelect = document.getElementById("request-user-select");
            users.forEach(user => {
                const option = document.createElement("option");
                option.value = user.id;
                option.textContent = user.email;
                userSelect.appendChild(option);
            });
        })
        .catch(() => alert("Failed to load users."));

    // Handle user selection change
    document.getElementById("request-user-select").addEventListener("change", function () {
        const userId = this.value;

        if (userId === "-1") {
            // Clear the table if no user is selected
            document.querySelector(".moderator-requests-table tbody").innerHTML = "";
            return;
        }

        // Fetch filtered requests
        fetch(`/Moderator/GetRequestsByUserId?userId=${userId}`)
            .then(response => response.json())
            .then(requests => {
                const tbody = document.querySelector(".moderator-requests-table tbody");
                tbody.innerHTML = "";

                requests.forEach(request => {
                    const row = document.createElement("tr");
                    row.innerHTML = `
                        <td>${request.centreName}</td>
                        <td>${request.status}</td>
                        <td>
                            <button class="classic-button" onclick="updateRequestStatus(${request.id}, 1)" ${request.status === 1 ? "disabled" : ""}>in progress</button>
                            <button class="classic-button" onclick="updateRequestStatus(@request.id, 2)" @(request.status === 2 ? "disabled" : "")>approve</button>
                            <button class="classic-button" onclick="updateRequestStatus(@request.id, 3)" @(request.status === 3 ? "disabled" : "")>decline</button>
                        </td>
                    `;
                    tbody.appendChild(row);
                });
            })
            .catch(() => alert("Failed to load requests."));
    });
});