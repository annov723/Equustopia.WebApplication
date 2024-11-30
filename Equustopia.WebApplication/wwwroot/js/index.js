function viewDetails(id, type) {
    if (type === "Horse") {
        window.location.href = `/Horse/Details/${id}`;
    } else if (type === "EquestrianCentre") {
        window.location.href = `/EquestrianCentre/Details/${id}`;
    } else {
        console.error("Error opening items details page.");
    }
}
