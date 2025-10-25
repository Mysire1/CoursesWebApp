// Site-wide JavaScript functionality

// Show loading spinner during AJAX requests
$(document).ready(function() {
    // Add loading indicators to buttons during form submissions
    $('form').on('submit', function() {
        const submitBtn = $(this).find('button[type="submit"], input[type="submit"]');
        const originalText = submitBtn.text();
        
        submitBtn.prop('disabled', true)
                 .html('<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Завантаження...');
        
        // Re-enable after 5 seconds as fallback
        setTimeout(function() {
            submitBtn.prop('disabled', false).text(originalText);
        }, 5000);
    });
    
    // Auto-dismiss alerts after 5 seconds
    $('.alert:not(.alert-permanent)').each(function() {
        const alert = $(this);
        setTimeout(function() {
            alert.fadeOut('slow');
        }, 5000);
    });
    
    // Smooth scrolling for anchor links
    $('a[href*="#"]:not([href="#"])').click(function() {
        if (location.pathname.replace(/^\//, '') == this.pathname.replace(/^\//, '') && location.hostname == this.hostname) {
            var target = $(this.hash);
            target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');
            if (target.length) {
                $('html, body').animate({
                    scrollTop: target.offset().top - 100
                }, 500);
                return false;
            }
        }
    });
    
    // Add confirmation to delete buttons
    $('a[href*="/Delete/"], button:contains("Видалити")').click(function(e) {
        if (!$(this).hasClass('confirmed')) {
            e.preventDefault();
            const confirmText = $(this).data('confirm') || 'Ви впевнені, що хочете видалити цей елемент?';
            
            if (confirm(confirmText)) {
                $(this).addClass('confirmed').click();
            }
        }
    });
    
    // Add tooltips to elements with title attribute
    $('[title]').tooltip();
    
    // Format numbers in Ukrainian locale
    $('.currency').each(function() {
        const value = parseFloat($(this).text());
        if (!isNaN(value)) {
            $(this).text(value.toLocaleString('uk-UA', { 
                style: 'currency', 
                currency: 'UAH' 
            }));
        }
    });
    
    // Add active class to current navigation item
    const currentPath = window.location.pathname;
    $('.navbar-nav .nav-link').each(function() {
        const href = $(this).attr('href');
        if (href && currentPath.startsWith(href) && href !== '/') {
            $(this).addClass('active');
        } else if (href === '/' && currentPath === '/') {
            $(this).addClass('active');
        }
    });
});