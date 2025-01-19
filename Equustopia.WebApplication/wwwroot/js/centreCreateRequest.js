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

        if (email === "") {
            alert("Please enter an email address to search.");

            const requestsTableMain = document.getElementById("requests-board");
            requestsTableMain.style.display = "table-row-group";
            const requestsTable = document.getElementById("requests-board-search");
            requestsTable.style.display = "none";
            return;
        }

        fetch(`/Index/GetRequestsByUserId`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ email: email })
        })
            .then(response => response.json())
            .then(data => {
                if (Array.isArray(data.userRequests) && data.userRequests.length > 0) {
                    console.log(email);
                    displayRequests(data.userRequests);
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
    const requestsTableMain = document.getElementById("requests-board");
    requestsTableMain.style.display = "none";
    const requestsTable = document.getElementById("requests-board-search");
    
    requestsTable.innerHTML = "";
    requests.forEach(request => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td>${request.equestrianCentreName}</td>
            <td>${getStatusName(request.status)}</td>
             <td>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 1)" ${request.Status === 1 ? "disabled" : ""}>in progress</button>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 2)" ${request.Status === 2 ? "disabled" : ""}>approve</button>
                <button class="classic-button" onclick="updateRequestStatus(${request.id}, 3)" ${request.Status === 3 ? "disabled" : ""}>decline</button>
             </td>
        `;

        requestsTable.appendChild(row);
    });
    requestsTable.style.width = "100%";
    requestsTable.style.display = "table-row-group";
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