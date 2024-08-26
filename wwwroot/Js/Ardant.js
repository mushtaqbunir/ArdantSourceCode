$(function () {
    $('.selectpicker').selectpicker();
});

// Function to scroll to the first visible validation error message
function scrollToFirstValidationError() {
    // Select all validation message elements
    var validationMessages = document.querySelectorAll('.validation-message');

    // Loop through each validation message
    for (var i = 0; i < validationMessages.length; i++) {
        var message = validationMessages[i];

        // Check if the message is visible (display is not 'none')
        if (window.getComputedStyle(message).display !== 'none') {
            // Scroll to the message
            message.scrollIntoView({ behavior: 'smooth', block: 'start' });
            return; // Stop the function after scrolling to the first error
        }
    }
}

// Observer to detect changes in the DOM
var observer = new MutationObserver(function (mutationsList, observer) {
    // Call scrollToFirstValidationError whenever the DOM changes
    scrollToFirstValidationError();
});

// Start observing changes in the DOM
observer.observe(document.body, { attributes: true, childList: true, subtree: true });




