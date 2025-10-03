document.addEventListener('DOMContentLoaded', function () {
    // ===== Slider Section =====
    const slides = document.querySelector('.slides');
    const slide = document.querySelectorAll('.slide');
    const leftArrow = document.querySelector('.arrow.left');
    const rightArrow = document.querySelector('.arrow.right');
    const dotsContainer = document.querySelector('.dots');
    let currentIndex = 0;

    if (slides && slide.length > 0 && dotsContainer && leftArrow && rightArrow) {
        const totalSlides = slide.length;

        // Create dots
        for (let i = 0; i < totalSlides; i++) {
            const dot = document.createElement('span');
            dot.classList.add('dot');
            if (i === 0) dot.classList.add('active');
            dot.setAttribute('data-index', i);
            dotsContainer.appendChild(dot);
        }

        const dots = document.querySelectorAll('.dot');

        function updateSlider(index) {
            slides.style.transform = `translateX(-${index * 100}%)`;
            dots.forEach(dot => dot.classList.remove('active'));
            dots[index].classList.add('active');
            currentIndex = index;
        }

        setInterval(() => {
            const nextIndex = (currentIndex + 1) % totalSlides;
            updateSlider(nextIndex);
        }, 4000);

        leftArrow.addEventListener('click', () => {
            const newIndex = (currentIndex - 1 + totalSlides) % totalSlides;
            updateSlider(newIndex);
        });

        rightArrow.addEventListener('click', () => {
            const newIndex = (currentIndex + 1) % totalSlides;
            updateSlider(newIndex);
        });

        dots.forEach(dot => {
            dot.addEventListener('click', () => {
                const index = parseInt(dot.getAttribute('data-index'));
                updateSlider(index);
            });
        });
    }

    // ===== Three-dot Popup Menu =====
    const threeDotIcon = document.querySelector('.threedot');
    const popupMenu = document.querySelector('.popup-menu');

    if (threeDotIcon && popupMenu) {
        threeDotIcon.addEventListener('click', (e) => {
            popupMenu.style.display = popupMenu.style.display === 'flex' ? 'none' : 'flex';
            e.stopPropagation();
        });

        document.addEventListener('click', function (event) {
            if (!event.target.closest('.threedot-container')) {
                popupMenu.style.display = 'none';
            }
        });
    }

    // ===== Hamburger Sidebar =====
    const hamburger = document.querySelector('.hamburger-icon');
    const sidebar = document.querySelector('.sidebar-menu');
    const closeBtn = document.querySelector('.close-btn');

    if (hamburger && sidebar) {
        hamburger.addEventListener('click', () => {
            sidebar.classList.toggle('show');
        });
    }

    if (closeBtn && sidebar) {
        closeBtn.addEventListener('click', () => {
            sidebar.classList.remove('show');
        });
    }

    document.addEventListener('click', (e) => {
        if (!e.target.closest('.sidebar-menu') && !e.target.closest('.hamburger-icon')) {
            if (sidebar) sidebar.classList.remove('show');
        }
    });

    // ===== Cart Functions =====
    window.removeFromCart = function (id) {
        fetch(`/Home/RemoveCart/${id}`, { method: 'POST' })
            .then(response => {
                if (response.ok) {
                    alert("🛒 Product removed from cart!");
                    window.location.reload();
                } else {
                    alert("Something went wrong.");
                }
            });
    }

    window.addToCart = function (id) {
        fetch(`/Home/AddToCart/${id}`, { method: 'POST' })
            .then(response => {
                if (response.ok) {
                    alert("🛒 Product added to cart!");
                } else {
                    alert("Product already in cart.");
                }
            });
    }


    // call to add product (use on product page's Add to Cart buttons)
    ////function addToCart(productId) {
    ////    $.ajax({
    ////        url: '/Home/AddToCart/' + productId,   // matches your controller route
    ////        type: 'POST',
    ////        success: function () {
    ////            updateCartCount();
    ////            // optional UX: small message/toast instead of alert
    ////            // alert('Added to cart');
    ////        },
    ////        error: function () {
    ////            alert('Failed to add item to cart. Please try again.');
    ////        }
    ////    });
    ////}



    // call to remove product from cart (use on cart page)
    //function removeFromCart(itemId, domElement) {
    //    if (!confirm('Remove this item from cart?')) return;
    //    $.ajax({
    //        url: '/Home/RemoveCart/' + itemId,   // matches your controller route
    //        type: 'POST',
    //        success: function () {
    //            updateCartCount();
    //            // remove the item element from DOM if you passed it
    //            if (domElement) {
    //                $(domElement).closest('.cart-item-card').remove();
    //            } else {
    //                // fallback: reload cart page
    //                // location.reload();
    //            }
    //        },
    //        error: function () {
    //            alert('Failed to remove item. Please try again.');
    //        }
    //    });

    //}



    //async function toggleWishlist(button, productId) {
    //    const icon = button.querySelector("i");
    //    const wantsAdd = icon.classList.contains("fa-heart-o");

    //    const url = `/Home/Addwishlist/${productId}`; // <-- match backend
    //    const res = await fetch(url, { method: "POST" });

    //    if (res.ok) {
    //        // toggle only on success
    //        if (wantsAdd) {
    //            icon.classList.remove("fa-heart-o");
    //            icon.classList.add("fa-heart");
    //            icon.style.color = "red";
    //        } else {
    //            // (optional) if you also implement remove on the server
    //            icon.classList.remove("fa-heart");
    //            icon.classList.add("fa-heart-o");
    //            icon.style.color = "#bbb";
    //        }
    //    } else {
    //        console.error("Wishlist save failed", res.status);
    //    }
    //}
    //window.toggleWishlist = toggleWishlist; // ensure it's global


    // Toggle wishlist (add/remove)
    window.toggleWishlist = async function (button, productId) {
        const icon = button.querySelector("i");
        const res = await fetch(`/Home/AddWishlist/${productId}`, { method: "POST" });

        if (!res.ok) {
            alert("Please log in first.");
            return;
        }

        const data = await res.json();

        // Toggle heart style (Font Awesome 4)
        if (icon.classList.contains("fa-heart-o")) {
            icon.classList.remove("fa-heart-o");
            icon.classList.add("fa-heart");
            icon.style.color = "red";
        } else {
            icon.classList.remove("fa-heart");
            icon.classList.add("fa-heart-o");
            icon.style.color = "#bbb";
        }

        // Update wishlist count badge
        updateWishlistCount(data.count);
    };

    // Update count globally
    async function refreshWishlistCount() {
        const res = await fetch("/Home/GetWishlistCount");
        if (res.ok) {
            const data = await res.json();
            updateWishlistCount(data.count);
        }
    }

    function updateWishlistCount(count) {
        const badge = document.getElementById("wishlistCount");
        if (badge) {
            badge.innerText = count;
        }
    }

    // Remove from wishlist page
    window.removeFromWishlist = async function (productId, cardId) {
        const res = await fetch(`/Home/RemoveFromWishlist/${productId}`, { method: "POST" });

        if (!res.ok) {
            alert("Failed to remove.");
            return;
        }

        const data = await res.json();
        if (data.success) {
            document.getElementById(cardId).remove();
            updateWishlistCount(data.count);
        }
    };









    console.log("site.js fully loaded.");
});
