function BitskiSignInShow() {
    document.getElementById('bitski-button').click();
    if (document.getElementsByClassName('bitski-connect-button').length != 0)
        document.getElementsByClassName('bitski-connect-button')[0].click();
}

function hideTopSignInBtn() {
    var x = document.getElementById('topSignInBtn');
    x.style.display = "none";
}

function showTopSignInBtn() {
    var x = document.getElementById('topSignInBtn');
    x.style.display = "block";
}

function initialProfilePortfolioComponent() {
    let x, i, j, l, ll, selElmnt, a, b, c;
    /* Look for any elements with the class "custom-select": */
    x = document.getElementsByClassName("custom-select");
    l = x.length;
    for (i = 0; i < l; i++) {
        selElmnt = x[i].getElementsByTagName("select")[0];
        ll = selElmnt.length;
        /* For each element, create a new DIV that will act as the selected item: */
        a = document.createElement("DIV");
        a.setAttribute("class", "select-selected");
        a.innerHTML = selElmnt.options[selElmnt.selectedIndex].innerHTML;
        x[i].appendChild(a);
        /* For each element, create a new DIV that will contain the option list: */
        b = document.createElement("DIV");
        b.setAttribute("class", "select-items select-hide");
        for (j = 1; j < ll; j++) {
            /* For each option in the original select element,
            create a new DIV that will act as an option item: */
            c = document.createElement("DIV");
            c.innerHTML = selElmnt.options[j].innerHTML;
            c.addEventListener("click", function (e) {
                /* When an item is clicked, update the original select box,
                and the selected item: */
                let y, i, k, s, h, sl, yl;
                s = this.parentNode.parentNode.getElementsByTagName("select")[0];
                sl = s.length;
                h = this.parentNode.previousSibling;
                for (i = 0; i < sl; i++) {
                    if (s.options[i].innerHTML == this.innerHTML) {
                        s.selectedIndex = i;
                        h.innerHTML = this.innerHTML;
                        y = this.parentNode.getElementsByClassName("same-as-selected");
                        yl = y.length;
                        for (k = 0; k < yl; k++) {
                            y[k].removeAttribute("class");
                        }
                        this.setAttribute("class", "same-as-selected");
                        break;
                    }
                }
                h.click();
            });
            b.appendChild(c);
        }
        x[i].appendChild(b);
        a.addEventListener("click", function (e) {
            /* When the select box is clicked, close any other select boxes,
            and open/close the current select box: */
            e.stopPropagation();
            closeAllSelect(this);
            this.nextSibling.classList.toggle("select-hide");
            this.classList.toggle("select-arrow-active");
        });
    }

    function closeAllSelect(elmnt) {
        /* A function that will close all select boxes in the document,
        except the current select box: */
        let x, y, i, xl, yl, arrNo = [];
        x = document.getElementsByClassName("select-items");
        y = document.getElementsByClassName("select-selected");
        xl = x.length;
        yl = y.length;
        for (i = 0; i < yl; i++) {
            if (elmnt == y[i]) {
                arrNo.push(i)
            } else {
                y[i].classList.remove("select-arrow-active");
            }
        }
        for (i = 0; i < xl; i++) {
            if (arrNo.indexOf(i)) {
                x[i].classList.add("select-hide");
            }
        }
    }

    /* If the user clicks anywhere outside the select box,
    then close all select boxes: */
    document.addEventListener("click", closeAllSelect);
}

function OpenProfileUserPhotoEditDialog() {
    $("#profile_photo_image_fileUpload").trigger('click');
}

function OpenProfileUserBannerEditDialog() {
    $("#profile_banner_image").trigger('click');
}

function initializeIndexPageComponent() {

    //$(window).scroll(function () {
    //    $('header').toggleClass('scrolled', $(this).scrollTop() > 80);
    //});

    //$("#sidebar").mCustomScrollbar({
    //    theme: "minimal"
    //});

    //$('#dismiss, .overlay-body').on('click', function () {
    //// hide sidebar
    //$('#sidebar').removeClass('active');
    //    // hide overlay
    //    $('.overlay').removeClass('active');
    //});

    //$('#sidebarCollapse').on('click', function () {
    //// open sidebar
    //$('#sidebar').addClass('active');
    //    // fade in the overlay
    //    //$('.overlay').addClass('active');
    //    $('.collapse.in').toggleClass('in');
    //    $('a[aria-expanded=true]').attr('aria-expanded', 'false');
    //});

    // popular_seller_swiper swiper
    // jquery ui input datepicker
    $(function () {
        $(".datepicker").datepicker();
    });



    // popular_seller_swiper swiper
    new Swiper(".popular_seller_swiper", {
        slidesPerView: 1,
        spaceBetween: 1,
        pagination: {
            el: ".swiper-pagination",
            clickable: true,
        },
        navigation: {
            nextEl: '[data-swiper_popular_seller="next"]',
            prevEl: '[data-swiper_popular_seller="prev"]',
        },
        breakpoints: {
            0: {
                slidesPerView: 1.3,
                spaceBetween: 10,
            },
            640: {
                slidesPerView: 2,
                spaceBetween: 10,
            },
            768: {
                slidesPerView: 3,
                spaceBetween: 15,
            },
            1024: {
                slidesPerView: 4,
                spaceBetween: 20,
            },
            1200: {
                slidesPerView: 5,
                spaceBetween: 20,
            },
        },
    });

    // collectors_card_swiper swiper

    new Swiper(".collectors_card_swiper", {
        slidesPerView: 1,
        spaceBetween: 1,
        pagination: {
            el: ".swiper-pagination",
            clickable: true,
        },
        navigation: {
            nextEl: '[data-swiper_collectors_only="next"]',
            prevEl: '[data-swiper_collectors_only="prev"]',
        },
        breakpoints: {
            0: {
                slidesPerView: 1.2,
                spaceBetween: 5,
            },
            640: {
                slidesPerView: 2,
                spaceBetween: 10,
            },
            768: {
                slidesPerView: 3,
                spaceBetween: 15,
            },
            1024: {
                slidesPerView: 4,
                spaceBetween: 20,
            }
        },
    });


    // collectors_card_swiper swiper

    new Swiper(".swiper_video_gala_container", {
        navigation: {
            nextEl: '[data-swiper_video_gala="next"]',
            prevEl: '[data-swiper_video_gala="prev"]',
        }
    });



    //gala_collections_card_swiper

    new Swiper(".gala_collections_card_swiper", {
        slidesPerView: 1,
        spaceBetween: 1,
        pagination: {
            el: ".swiper-pagination",
            clickable: true,
        },
        navigation: {
            nextEl: '[data-swiper_gala_collections="next"]',
            prevEl: '[data-swiper_gala_collections="prev"]',
        },
        breakpoints: {
            640: {
                slidesPerView: 1,
                spaceBetween: 10,
            },
            768: {
                slidesPerView: 2,
                spaceBetween: 15,
            },
            1024: {
                slidesPerView: 3,
                spaceBetween: 20,
            }
        },
    });

    // 

    // source: https://css-tricks.com/value-bubbles-for-range-inputs/
    const allRanges = document.querySelectorAll(".range-wrap");
    allRanges.forEach(wrap => {
        const range = wrap.querySelector(".range");
        const bubble = wrap.querySelector(".bubble");

        range.addEventListener("input", () => {
            setBubble(range, bubble);
        });
        setBubble(range, bubble);
    });

    function setBubble(range, bubble) {
        const val = range.value;
        const min = range.min ? range.min : 0;
        const max = range.max ? range.max : 100;
        const newVal = Number(((val - min) * 100) / (max - min));
        bubble.innerHTML = val + ' ETH';

        // Sorta magic numbers based on size of the native UI thumb
        bubble.style.left = `calc(${newVal}% + (${8 - newVal * 0.15}px))`;
    }

    // timer
    // Set the date we're counting down to
    let countDownDate = new Date("Dec 5, 2025 15:37:25").getTime();

    // Update the count down every 1 second
    let updateCountTime = setInterval(function () {

        // Get today's date and time
        let now = new Date().getTime();

        // Find the distance between now and the count down date
        let distance = countDownDate - now;

        // Time calculations for days, hours, minutes and seconds
        let days = Math.floor(distance / (1000 * 60 * 60 * 24));
        let hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        let minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        let seconds = Math.floor((distance % (1000 * 60)) / 1000);

        // Display the result in the element with id="demo"

        $('.counter_wrapper .hours').text(hours);
        $('.counter_wrapper .minutes').text(minutes);
        $('.counter_wrapper .seconds').text(seconds);


        // If the count down is finished, write some 
        if (distance < 0) {
            clearInterval(updateCountTime);
            document.getElementById("demo").innerHTML = "EXPIRED";
        }
    }, 1000);
}