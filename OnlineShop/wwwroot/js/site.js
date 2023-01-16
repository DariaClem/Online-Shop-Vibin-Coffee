// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/* Atunci cand se da scroll sa se dezavaluie pozele de prezentare */
function reveal() {
    var reveals = document.querySelectorAll(".anim");

    for (var i = 0; i < reveals.length; i++) {
        var windowHeight = window.innerHeight;
        var elementTop = reveals[i].getBoundingClientRect().top;
        var elementVisible = 250;

        if (elementTop < windowHeight - elementVisible) {
            reveals[i].classList.add("act");
        } else {
            reveals[i].classList.remove("act");
        }
    }
}

window.addEventListener("scroll", reveal);

/* Atunci cand se selecteaza o rubrica din Manage sa fie subliniata */
document.addEventListener('DOMContentLoaded', function () {
    const selector = '.verticalA';
    const elems = Array.from(document.querySelectorAll(selector));
    const navigation = document.querySelector('verticalNavbar');

    function makeActive(evt) {
        const target = evt.target;

        if (!target || !target.matches(selector)) {
            return;
        }

        elems.forEach(elem => elem.classList.remove('verticalAActive'));

        evt.target.classList.add('verticalAActive');
    };
    if (navigation != null) {
        navigation.addEventListener('mousedown', makeActive);
    }

});

/* Cand apare o eroare sa poata fi inchisa din X sau sa dispara dupa 2 secunde */

/* Cand dai click pe un buton sa se schimbe imaginea */

var slideIndex = 1;

showDivs(slideIndex);

function plusDivs(n) {
    showDivs(slideIndex += n);
}

function showDivs(n) {
    var i;
    var x = document.getElementsByClassName("images");
    if (x.length > 0) {
        if (n > x.length) { slideIndex = 1 }
        if (n < 1) { slideIndex = x.length }
        for (i = 0; i < x.length; i++) {
            x[i].style.display = "none";
        }
        var imagine = document.querySelectorAll(".mySlides");
        var src = imagine[slideIndex - 1].getAttribute('src');
        var imaginePop = document.querySelectorAll('.popup-image img')[0];
        imaginePop.src = src;
        x[slideIndex - 1].style.display = "block";
    }
}

window.onload = function () {
    var ok = 0;
    var inchis = document.getElementById("inchis");
    var alert = document.getElementById("alert");
    console.log("orice");
    if (alert != null) {
        inchis.addEventListener("click", function () {
            alert.style.display = "none";
        })
        var int = setTimeout(function () { alert.style.display = "none"; }, 2000);
    }

    /* La click pe imagine se deschide imaginea */
    var mySlides = document.querySelectorAll('.images');
    if (mySlides.length > 0) {
        for (var image of mySlides) {
            image.addEventListener("click", function (event) {
                if (event.target === event.currentTarget) {
                    event.stopPropagation();
                    var poze = document.querySelectorAll('.popup-image')[0];
                    var imaginePop = document.querySelectorAll('.popup-image img')[0];
                    var imagine = document.querySelectorAll(".mySlides");
                    if (imagine[slideIndex - 1] != null) {
                        var src = imagine[slideIndex - 1].getAttribute('src');
                        imaginePop.src = src;
                        poze.style.display = "block";
                    }
                }
            });
        }
    }
    /* La apasarea pe poza deschisa sa nu se inchida */
    var popUpImageDiv = document.querySelectorAll(".pop .butonPoze");
    if (popUpImageDiv.length > 0) {
        popUpImageDiv[0].onclick = function (event) {
            plusDivs(-1);
            event.stopPropagation();
        }
        popUpImageDiv[1].onclick = function (event) {
            plusDivs(1);
            event.stopPropagation();
        }
    }

    /* La apasarea sagetilor dreapta/stanga sa se schimbe imaginea */
    var butoane = document.querySelectorAll(".popup-image .butonPoze");
    if (butoane.length > 0) {
        var poze = document.querySelectorAll('.popup-image')[0];
        window.onkeydown = function (e) {
            if (e.key === 'ArrowRight' && poze.style.display == "block") {
                plusDivs(1);
            }
            else if (e.key === 'ArrowLeft' && poze.style.display == "block") {
                plusDivs(-1);
            }
        }
    }

    var pozaPop = document.querySelectorAll(".pozaPopUp");
    pozaPop[0].onclick = function (e) {
        if (e.currentTarget === e.target) {
            e.stopPropagation();
        }
    }

    /* La apasarea in exteriorul imaginii popUp sa se inchida */
    var popup_image = document.getElementsByClassName("popup-image");
    if (popup_image.length > 0) {
        popup_image[0].onclick = function () {
            popup_image[0].style.display = "none";
        }
    }

    /* La apasarea pe x sa se inchida imaginea popUp */
    document.querySelector('.popup-image span').onclick = () => {
        document.querySelector('.popup-image').style.display = "none";
    }
}