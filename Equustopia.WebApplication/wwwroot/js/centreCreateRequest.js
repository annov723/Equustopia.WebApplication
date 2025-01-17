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





function handleSearchRequests(event) {
    if (event.key === "Enter") {
        const email = document.getElementById("search-bar-requests").value.trim();
        console.log(email);

        if (email === "") {
            alert("Please enter an email address to search.");

            const requestsTableMain = document.getElementById("requests-board-search");
            requestsTableMain.style.display = "block";
            const requestsTable = document.getElementById("requests-board-search");
            requestsTable.style.display = "none";
            return;
        }

        fetch(`/IndexController/GetRequestsByUserId`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email })
        })
            .then(response => response.json())
            .then(data => {
                if (Array.isArray(data) && data.length > 0) {
                    displayRequests(data);
                } else {
                    alert("No requests found for this user.");
                }
            })
            .catch(error => {
                alert(error.message);
            });
    }
}

function displayRequests(requests) {
    const requestsTableMain = document.getElementById("requests-board-search");
    requestsTableMain.style.display = "none";
    const requestsTable = document.getElementById("requests-board-search");
    
    requestsTable.innerHTML = "";
    requests.forEach(request => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${request.EquestrianCentre.name}</td>
            <td>${getStatusName(request.status)}</td>
             <td>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 1)" ${request.status === 1 ? "disabled" : ""}>in progress</button>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 2)" ${request.status === 2 ? "disabled" : ""}>approve</button>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 3)" ${request.status === 3 ? "disabled" : ""}>decline</button>
             </td>
        `;

        requestsTable.appendChild(row);
    });
    requestsTableMain.style.display = "block";
}

function getStatusName(statusCode) {
    const statusMap = {
        0: "New",
        1: "In Progress",
        2: "Approved",
        3: "Declined"
    };
    return statusMap[statusCode];
}