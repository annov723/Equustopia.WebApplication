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
