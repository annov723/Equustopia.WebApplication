const menuToggle = document.getElementById('sideMenuButton');
const sideMenu = document.getElementById('sideMenu');

menuToggle.addEventListener('click', function(e){
   sideMenu.classList.toggle('open');
   e.stopPropagation();
});

document.addEventListener('click', function(e){
    if(sideMenu.classList.contains('open') && !sideMenu.contains(e.target) && !menuToggle.contains(e.target)){
        sideMenu.classList.remove('open');
    }
});



window.addEventListener('scroll', function () {
    var footer = document.querySelector('.footer-content');

    if ((window.innerHeight + window.scrollY) >= document.body.offsetHeight) {
        footer.classList.add('show');
    } else {
        footer.classList.remove('show');
    }
});

function handleSearch(event) {
    if (event.key === "Enter") {
        const query = document.getElementById("search-bar").value.trim();

        if (!query) {
            alert("Please enter a search term.");
            return;
        }

        fetch(`/Search/Search?query=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    displaySearchResults(data.data);
                } else {
                    alert("No results found.");
                }
            })
            .catch(error => console.error("Error during search:", error));
    }
}

function displaySearchResults(results) {
    const resultsContainer = document.getElementById("search-results");
    resultsContainer.innerHTML = "";

    results.forEach(result => {
        const item = document.createElement("div");
        item.classList.add("search-result-item");

        // Create a link based on the type of the result
        const link = document.createElement("a");
        link.textContent = `${result.Type}: ${result.Name}`;
        link.href = `/${result.Type}/Details/${result.Id}`;
        item.appendChild(link);

        resultsContainer.appendChild(item);
    });

    // Show the results container
    resultsContainer.style.display = "block";
}
