document.querySelectorAll('form[data-confirm-before-submit]').forEach(function (element) {
    const message = element.getAttribute('data-confirm-before-submit');
    element.addEventListener('submit', function (event) {
        if (!confirm(message)) {
            event.preventDefault();
            return false;
        }
    });
});