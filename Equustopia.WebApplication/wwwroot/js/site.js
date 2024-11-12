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
    let footer = document.querySelector('.footer-content');

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
    console.log("Received search results:", results);
    
    const resultsContainer = document.getElementById("searchResults");
    resultsContainer.innerHTML = "";

    results.forEach(result => {
        const item = document.createElement("div");
        item.classList.add("search-result-item");

        if (result.id && result.type) {
            const link = document.createElement("a");
            link.textContent = `${result.type}: ${result.name}`;
            link.href = `/${result.type}/Details/${result.id}`;
            item.appendChild(link);
        } else {
            item.textContent = result.name + " " + result.id + " " + result.type;
        }

        resultsContainer.appendChild(item);
    });

    resultsContainer.style.display = "block";
}
