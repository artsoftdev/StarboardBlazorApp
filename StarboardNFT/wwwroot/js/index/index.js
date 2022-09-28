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