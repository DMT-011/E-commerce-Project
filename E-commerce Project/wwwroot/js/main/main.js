const nivoControls = document.querySelectorAll(".nivo-control");

// Slide
$("#main-slider").nivoSlider({
  effect: "random", 
  animSpeed: 600, 
  pauseTime: 2500,
  startSlide: 0,
  directionNav: false, 
  controlNav: true, 
  manualAdvance: false, 
});


// Carouse brand
$('.brand-carousel').owlCarousel({
  item: 5,
  loop: true,
  mouseDrag: true,
  nav: false,
  autoWidth: true,
  margin: 32,
  dots: false,
  // navContainer: ".nav-carouse-controls",
  navText: [
    "<i class=\"bi icon-controls bi-chevron-left\"></i>",
    "<i class=\"bi icon-controls bi-chevron-right\"></i>"
  ],
  responsive: {
    1000: {
      items: 5
    }
  },
  autoplay: true,
  autoplayTimeout: 2200,
  autoplayHoverPause: true,
});


// Click item to link assign on sidebar
const listItemSidebar = document.querySelectorAll(".single-product-item");

listItemSidebar.forEach(itemSidebar => {
    itemSidebar.addEventListener("click", function() {
      const itemSidebarLink = itemSidebar.querySelector(".sidebar-product-link").href;

      if (itemSidebarLink != null) {
        location.href = itemSidebarLink;
      }
     
    });
});


// Click item to link assign on product 
const listProducts = document.querySelectorAll(".product-item");

listProducts.forEach(productItem => {
    productItem.addEventListener("click", function( ) {
      const itemProductLink = productItem.querySelector(".product-link").href;

      console.log(itemProductLink);
      if (itemProductLink != null) {
        location.href = itemProductLink;
      }
    });
});

const totalBlock = document.querySelector(".total-price-block");
const shoppingCartElement = document.querySelector(".shopping-cart");
const cartBlock = document.querySelector(".cart-product-list");


shoppingCartElement.addEventListener("mouseover", function() {
  const possitionTotalBlock = cartBlock.offsetHeight + 53;
  totalBlock.style.top = possitionTotalBlock + "px";
});



// Nav control user 
const btnShowControl = document.querySelector('.user-verified .control-user-name');
const controlUser = document.querySelector(".user-verified .user-controls-nav");

if (btnShowControl) {
  btnShowControl.onclick = function(e) {
    e.preventDefault();
    controlUser.classList.toggle("active");
  }
}

if (controlUser) {
  document.addEventListener("click", function(e) {
    const isActive = controlUser.classList.contains("active");

    if (!e.target.closest(".control-user-name") && isActive) {
      controlUser.classList.remove("active");
    }
  })
}