// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Configure toastr
toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": false,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
};

// Update cart count
function updateCartCount(count) {
    $('.cart-count').text(count);
}

// Add to cart
function addToCart(productId, quantity = 1) {
    $.post('/Guest/Order/AddToCart', { productId: productId, quantity: quantity })
        .done(function (response) {
            if (response.success) {
                toastr.success('Product added to cart');
                updateCartCount(response.cartCount);
            } else {
                toastr.error(response.message || 'Failed to add product to cart');
            }
        })
        .fail(function () {
            toastr.error('Failed to add product to cart');
        });
}

// Update cart quantity
function updateCartQuantity(productId, quantity) {
    $.ajax({
        url: '/Guest/Order/UpdateQuantity',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ productId: productId, quantity: quantity }),
        success: function (response) {
            if (response.success) {
                updateCartCount(response.cartCount);
                location.reload();
            } else {
                toastr.error(response.message || 'Failed to update quantity');
            }
        },
        error: function () {
            toastr.error('Failed to update quantity');
        }
    });
}

// Remove from cart
function removeFromCart(productId) {
    $.post('/Guest/Order/RemoveFromCart', { productId: productId })
        .done(function (response) {
            if (response.success) {
                toastr.success('Product removed from cart');
                updateCartCount(response.cartCount);
                location.reload();
            } else {
                toastr.error(response.message || 'Failed to remove product from cart');
            }
        })
        .fail(function () {
            toastr.error('Failed to remove product from cart');
        });
}

// Initialize cart count on page load
$(document).ready(function () {
    // Get initial cart count
    $.get('/Guest/Order/GetCartCount')
        .done(function (response) {
            updateCartCount(response.count);
        });
});
