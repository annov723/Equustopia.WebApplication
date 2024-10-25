const menuToggle = document.getElementById('side-menu-toggle');
const sideMenu = document.getElementById('side-menu');

menuToggle.addEventListener('click', function(e){
   sideMenu.classList.toggle('open');
   e.stopPropagation();
});

document.addEventListener('click', function(e){
    if(sideMenu.classList.contains('open') && !sideMenu.contains(e.target) && !menuToggle.contains(e.target)){
        sideMenu.classList.remove('open');
    }
});
