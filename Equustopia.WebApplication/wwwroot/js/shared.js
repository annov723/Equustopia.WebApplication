function viewHorseDetails(horseId){
    window.location.href = `/Horse/Details/${horseId}`;
}

function viewCentreDetails(centreId){
    window.location.href = `/EquestrianCentre/Details/${centreId}`;
}

function populateCentreDropdown(selectName) {
    fetch("/User/GetEquestrianCentres")
        .then(response => response.json())
        .then(data => {
            const stableSelect = document.getElementById(selectName);
            stableSelect.innerHTML = '<option value="-1">choose a centre</option>'; // Reset options

            data.forEach(stable => {
                const option = document.createElement("option");
                option.value = stable.id;
                option.textContent = stable.name;
                stableSelect.appendChild(option);
            });
        })
        .catch(error => console.error("Error fetching stables:", error));
}