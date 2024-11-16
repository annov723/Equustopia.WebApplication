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
}

window.onbeforeunload = closeAllModals;

//side menu
menuToggle.addEventListener('click', function(e){
   sideMenu.classList.toggle('open');
   e.stopPropagation();
});

document.addEventListener('click', function(e){
    if(sideMenu.classList.contains('open') && !sideMenu.contains(e.target) && !menuToggle.contains(e.target)){
        sideMenu.classList.remove('open');
    }
});

//search bar
searchBar.addEventListener("blur", function() {
    // Use a small delay to ensure results are not hidden immediately after selection
    setTimeout(() => {
        searchResults.style.display = "none";  // Hide the results container
    }, 100);
});

document.addEventListener("click", function(event) {
    if (!searchBar.contains(event.target) && !searchResults.contains(event.target)) {
        searchResults.style.display = "none";
    }
});


// hiding footer
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
    const elem = document.createElement("p");
    elem.textContent = `No results found.`;
    item.appendChild(elem);
    resultsContainer.appendChild(item);
    resultsContainer.style.display = "block";
}
