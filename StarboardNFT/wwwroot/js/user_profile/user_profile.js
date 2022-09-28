new Swiper(".user_profile_slider1", {
    slidesPerView: 4,
    spaceBetween: 5,
    //centeredSlides: true,
    breakpoints: {

        800: {
            slidesPerView: 3,
        },
        1200: {
            slidesPerView: 4,
        }
    },
});

new Swiper(".user_profile_slider_centered", {
    slidesPerView: 4,
    spaceBetween: 5,
    centeredSlides: true,
    breakpoints: {

        800: {
            slidesPerView: 3,
        },
        1200: {
            slidesPerView: 4,
        }
    },
});