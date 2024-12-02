const menuToggle = document.getElementById('sideMenuButton');
const sideMenu = document.getElementById('sideMenu');
const searchBar = document.getElementById("searchContainer");
const searchResults = document.getElementById("searchResults");

//reloading
function closeAllModals() {
    document.getElementById("logInModal").style.display = "none";
    document.getElementById("signUpModal").style.display = "none";

    ["logInEmail", "logInPassword", "signUpName", "signUpEmail", "signUpPassword", "signUpPasswordRepeat"].forEach(id => {
        document.getElementById(id).value = "";
    });

    document.getElementById("logInError").textContent = "";

    document.getElementById("signUpError").textContent = "";
    document.getElementById("signUpErrorName").textContent = "";
    document.getElementById("signUpErrorEmail").textContent = "";
    document.getElementById("signUpErrorPassword").textContent = "";
    document.getElementById("signUpErrorPasswordRepeat").textContent = "";

    document.getElementById("search-bar").value = "";
    searchResults.style.display = "none";
    searchResults.innerHTML = "";
}

window.onbeforeunload = closeAllModals;

//side menu
menuToggle.addEventListener('click', function(e){
   sideMenu.classList.toggle('open');
    searchResults.style.display = "none";
    searchResults.innerHTML = "";
   e.stopPropagation();
});

document.addEventListener('click', function(e){
    if(sideMenu.classList.contains('open') && !sideMenu.contains(e.target) && !menuToggle.contains(e.target)){
        sideMenu.classList.remove('open');
    }
});

//search bar
searchBar.addEventListener("blur", function() {
    searchResults.style.display = "none";
    searchResults.innerHTML = "";
});

document.addEventListener("click", function(event) {
    if (!searchBar.contains(event.target) && !searchResults.contains(event.target)) {
        searchResults.style.display = "none";
        searchResults.innerHTML = "";
    }
});

window.onblur = function() {
    searchResults.style.display = "none";
    searchResults.innerHTML = "";
};



function handleSearch(event) {
    if (event.key === "Enter") {
        const query = document.getElementById("search-bar").value.trim();

        if (!query) {
            return;
        }

        fetch(`/Search/Search?query=${encodeURIComponent(query)}`)
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    displaySearchResults(data.data);
                } else {
                    displayEmpty();
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

function displayEmpty(){
    const resultsContainer = document.getElementById("searchResults");
    resultsContainer.innerHTML = "";
    const item = document.createElement("div");
    item.classList.add("search-result-item");
    const elem = document.createElement("a");
    elem.textContent = `No results found.`;
    item.appendChild(elem);
    resultsContainer.appendChild(item);
    resultsContainer.style.display = "block";
}
