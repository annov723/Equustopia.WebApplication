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
        body: JSON.stringify({ Id: id })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                window.location.href = `/User/UserMainPage`;
                closeRemoveCentreView();
            } else {
                document.getElementById("removeCentreError").textContent = "An error occurred while removing a centre. " + data.message;
            }
        })
        .catch(error => console.error("Error removing centre:", error));
}



function generateChart(id){
    const selectedOption = document.getElementById("chartSelect").value;
    
    switch(selectedOption){
        case 'dailyViews':
            generateHourlyViewsChart(id);
            break;
        case 'weeklyViews':
            const endDateWeek = new Date();
            const startDateWeek = new Date();
            startDateWeek.setDate(endDateWeek.getDate() - 7);
            generateViewsChart(id, startDateWeek, endDateWeek);
            break;
        case 'monthlyViews':
            const endDateMonth = new Date();
            const startDateMonth = new Date();
            startDateMonth.setMonth(endDateMonth.getMonth() - 1);
            generateViewsChart(id, startDateMonth, endDateMonth);
            break;
        case 'horseAge':
            generateHorseAgeChart(id);
            break;
        case 'horseBreed':
            generateHorseBreedChart(id);
            break;
        default:
            console.error('Invalid option selected.');
            return;
    }
}

function generateHourlyViewsChart(centreId) {
    fetch(`/EquestrianCentre/GetHourlyViews?centreId=${centreId}`)
        .then(response => response.json())
        .then(data => {
            const labels = data.map(item => new Date(item.date).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', hour12: false }));
            const views = data.map(item => item.viewsCount);

            if (window.myChart) {
                window.myChart.destroy();
            }

            const ctx = document.getElementById('chartCanvas').getContext('2d');
            window.myChart = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'views count',
                        data: views,
                        fill: true,
                        borderColor: '#007bff',
                        backgroundColor: 'rgba(0,123,255,0.2)',
                        tension: 0.3
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top'
                        },
                        title: {
                            display: true,
                            text: 'Centre Views'
                        }
                    },
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: 'Hour'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: 'Number of Views'
                            },
                            beginAtZero: true
                        }
                    }
                }
            });

            document.getElementById("chartCanvas").style.display = "block";
        })
        .catch(error => {
            if (error.message.includes("NotFound")) {
                document.getElementById("chartCanvas").innerText = "No views found for the last 24 hours.";
            } else {
                console.error("Error fetching hourly views data:", error);
            }
        });
}

function generateViewsChart(id, startDate, endDate){
    let startDateFormatted = startDate.toISOString();
    let endDateFormatted = endDate.toISOString();
    
    fetch(`/EquestrianCentre/GetCentreViews?centreId=${id}&startDate=${startDateFormatted}&endDate=${endDateFormatted}`)
        .then(response => response.json())
        .then(data => {
            const labels = data.map(item => item.date.split('T')[0]);
            const views = data.map(item => item.viewsCount);


            console.log("data:", data);
            console.log("labels:", labels);

            if (window.myChart) {
                window.myChart.destroy();
            }

            const ctx = document.getElementById('chartCanvas').getContext('2d');
            window.myChart = new Chart(ctx, {
                type: 'bar',
                data: {
                    labels: labels,
                    datasets: [{
                        label: `views count`,
                        data: views,
                        backgroundColor: '#2895b5',
                        borderColor: '#1c6d8c',
                        borderWidth: 1
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top'
                        },
                        title: {
                            display: true,
                            text: 'Centre Views'
                        }
                    },
                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: 'Date'
                            }
                        },
                        y: {
                            title: {
                                display: true,
                                text: 'Number of Views'
                            },
                            beginAtZero: true
                        }
                    }
                }
            });
            document.getElementById("chartCanvas").style.display = "block";
        })
        .catch(error => {
            if (error.message.includes("NotFound")) {
                document.getElementById("chartCanvas").innerText = "No views found.";
            } else {
                console.error("Error fetching views data:", error);
            }
        });
}

function generateHorseAgeChart(id){
    fetch(`/EquestrianCentre/GetHorsesAgeGroups?centreId=${id}`)
        .then(response => response.json())
        .then(data => {
            const ctx = document.getElementById('chartCanvas').getContext('2d');

            if (window.myChart) {
                window.myChart.destroy();
            }

            window.myChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: ['0-3 years', '3-10 years', '10-19 years', '19+ years'],
                    datasets: [{
                        label: 'count',
                        data: [
                            data.ageGroup_0_3,
                            data.ageGroup_3_10,
                            data.ageGroup_10_19,
                            data.ageGroup_19_Plus
                        ],
                        backgroundColor: [
                            '#3cb3d7', '#f4b400', '#f15c24', '#6a737b'
                        ],
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top'
                        },
                        title: {
                            display: true,
                            text: 'Horse Age'
                        }
                    }
                }
            });
            document.getElementById("chartCanvas").style.display = "block";
        })
        .catch(error => {
            if (error.message.includes("NotFound")) {
                document.getElementById("chartCanvas").innerText = "No horses found.";
            } else {
                console.error("Error fetching horse age data:", error);
            }});
}

function generateHorseBreedChart(id){
    fetch(`/EquestrianCentre/GetHorsesBreedGroups?centreId=${id}`)
        .then(response => response.json())
        .then(data => {
            const labels = data.map(group => group.breed);
            const counts = data.map(group => group.horse_count);
            
            const ctx = document.getElementById('chartCanvas').getContext('2d');

            if (window.myChart) {
                window.myChart.destroy();
            }

            window.myChart = new Chart(ctx, {
                type: 'pie',
                data: {
                    labels: labels,
                    datasets: [{
                        label: 'count',
                        data: counts,
                        backgroundColor: generateColors(labels.length),
                        hoverOffset: 4
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: {
                            position: 'top'
                        },
                        title: {
                            display: true,
                            text: 'Horse Breed'
                        }
                    }
                }
            });
            document.getElementById("chartCanvas").style.display = "block";
        })
        .catch(error => {
            if (error.message.includes("NotFound")) {
                document.getElementById("chartCanvas").innerText = "No horses found.";
            } else {
                console.error("Error fetching horse breed data:", error);
            }
        });
}

function generateColors(count) {
    const colors = ['#3cb3d7', '#f4b400', '#f15c24', '#6a737b', '#8e44ad', '#27ae60', '#e67e22', '#d35400'];
    while (colors.length < count) {
        colors.push(`#${Math.floor(Math.random() * 16777215).toString(16)}`);
    }
    return colors.slice(0, count);
}




function openCentreVerificationView(centreId) {
    fetch(`/EquestrianCentre/GetCentreRequestView?centreId=${centreId}`, {
        method: "GET"
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("Failed to load centre verification view.");
            }
            return response.json();
        })
        .then(data => {
            if (data.success) {
                if(data.id !== 0){
                    console.log(data);
                    document.getElementById("verification-modal-body").innerHTML = `
                        <p><strong>Centre Name:</strong> ${data.centreName}</p>
                        <p><strong>Status:</strong> ${data.status}</p>
                        <p><strong>Created At:</strong> ${data.createdAt}</p>
                        <p><strong>Last Updated:</strong> ${data.updatedAt}</p>
                    `;
                    document.getElementById("new-request-button").style.display = "none";
                } else {
                    console.log(data);
                    document.getElementById("verification-modal-body").innerHTML = `<p>Request is not created yet for this centre.</p>`;
                    document.getElementById("new-request-button").style.display = "block";
                }
                document.getElementById("verification-modal").style.display = "block";
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            alert(error.message);
        });
}

function closeCentreVerificationView() {
    document.getElementById("verification-modal").style.display = "none";
}

function createNewCentreRequest(centreId) {
    fetch(`/EquestrianCentre/CreateNewCentreRequest`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(centreId)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                document.getElementById("verification-modal-body").innerHTML = `
                <p><strong>Centre Name:</strong> ${data.centreName}</p>
                <p><strong>Status:</strong> ${data.status}</p>
                <p><strong>Created At:</strong> ${data.createdAt}</p>
                <p><strong>Last Updated:</strong> ${data.updatedAt}</p>
            `;
                document.getElementById("new-request-button").style.display = "none";
            } else {
                alert(data.message);
            }
        })
        .catch(error => {
            alert("Failed to create a new request. " + error.message);
        });
}



const centreNameConstraint = "chk_name_length";
const centreAddressConstraint = "chk_address_length";