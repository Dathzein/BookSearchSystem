/**
 * Sistema de Búsqueda de Libros - Validaciones y Funcionalidades del Cliente
 */

$(document).ready(function() {
    // Inicializar validaciones
    initializeValidations();
    
    // Inicializar funcionalidades
    initializeFeatures();
});

/**
 * Inicializa las validaciones del formulario
 */
function initializeValidations() {
    const searchForm = $('#searchForm');
    const authorInput = $('#Author');
    const searchBtn = $('#searchBtn');
    
    if (searchForm.length) {
        // Validación en tiempo real
        authorInput.on('input', function() {
            validateAuthorField($(this), searchBtn);
        });
        
        // Validación al enviar el formulario
        searchForm.on('submit', function(e) {
            if (!validateForm(authorInput, searchBtn)) {
                e.preventDefault();
                return false;
            }
            
            // Mostrar estado de carga
            showLoadingState(searchBtn);
        });
        
        // Auto-focus en el campo de autor
        authorInput.focus();
    }
}

/**
 * Valida el campo de autor en tiempo real
 */
function validateAuthorField(input, button) {
    const value = input.val().trim();
    const errorSpan = input.siblings('.text-danger');
    
    // Limpiar errores previos
    input.removeClass('is-invalid is-valid');
    errorSpan.hide();
    
    if (value === '') {
        // Campo vacío
        button.prop('disabled', true)
              .html('<i class="fas fa-search me-2"></i>Buscar Libros');
        return false;
    } else if (value.length < 2) {
        // Muy corto
        input.addClass('is-invalid');
        showFieldError(input, 'El nombre del autor debe tener al menos 2 caracteres');
        button.prop('disabled', true);
        return false;
    } else if (value.length > 255) {
        // Muy largo
        input.addClass('is-invalid');
        showFieldError(input, 'El nombre del autor no puede exceder 255 caracteres');
        button.prop('disabled', true);
        return false;
    } else if (!isValidAuthorName(value)) {
        // Caracteres inválidos
        input.addClass('is-invalid');
        showFieldError(input, 'El nombre del autor contiene caracteres no válidos');
        button.prop('disabled', true);
        return false;
    } else {
        // Válido
        input.addClass('is-valid');
        button.prop('disabled', false)
              .html('<i class="fas fa-search me-2"></i>Buscar Libros');
        return true;
    }
}

/**
 * Valida el formulario completo antes del envío
 */
function validateForm(authorInput, searchBtn) {
    const author = authorInput.val().trim();
    
    if (!author) {
        showAlert('error', 'Por favor, ingrese el nombre de un autor');
        authorInput.focus();
        return false;
    }
    
    if (author.length < 2) {
        showAlert('error', 'El nombre del autor debe tener al menos 2 caracteres');
        authorInput.focus();
        return false;
    }
    
    if (author.length > 255) {
        showAlert('error', 'El nombre del autor no puede exceder 255 caracteres');
        authorInput.focus();
        return false;
    }
    
    if (!isValidAuthorName(author)) {
        showAlert('error', 'El nombre del autor contiene caracteres no válidos');
        authorInput.focus();
        return false;
    }
    
    return true;
}

/**
 * Valida si el nombre del autor contiene caracteres válidos
 */
function isValidAuthorName(name) {
    // Permitir letras, espacios, guiones, apostrofes y puntos
    const validPattern = /^[a-zA-ZÀ-ÿ\u00f1\u00d1\s\-'\.]+$/;
    return validPattern.test(name);
}

/**
 * Muestra un error específico en un campo
 */
function showFieldError(input, message) {
    let errorSpan = input.siblings('.text-danger');
    if (errorSpan.length === 0) {
        errorSpan = $('<span class="text-danger small"></span>');
        input.after(errorSpan);
    }
    errorSpan.text(message).show();
}

/**
 * Muestra el estado de carga en el botón
 */
function showLoadingState(button) {
    button.html('<i class="fas fa-spinner fa-spin me-2"></i>Buscando...')
          .prop('disabled', true)
          .addClass('loading');
}

/**
 * Inicializa funcionalidades adicionales
 */
function initializeFeatures() {
    // Tooltips
    initializeTooltips();
    
    // Confirmaciones para acciones
    initializeConfirmations();
    
    // Funcionalidades de tabla
    initializeTableFeatures();
    
    // Atajos de teclado
    initializeKeyboardShortcuts();
}

/**
 * Inicializa tooltips de Bootstrap
 */
function initializeTooltips() {
    $('[title]').tooltip({
        placement: 'top',
        trigger: 'hover'
    });
}

/**
 * Inicializa confirmaciones para acciones importantes
 */
function initializeConfirmations() {
    // Las búsquedas desde historial ahora se ejecutan directamente sin confirmación
    // Los botones "Buscar Nuevamente" abren automáticamente en nueva ventana
}

/**
 * Inicializa funcionalidades de tabla
 */
function initializeTableFeatures() {
    // Resaltar filas al hacer hover
    $('.table-hover tbody tr').on('mouseenter', function() {
        $(this).addClass('table-active');
    }).on('mouseleave', function() {
        $(this).removeClass('table-active');
    });
    
    // Hacer filas clickeables (opcional)
    $('.table tbody tr[data-href]').on('click', function() {
        window.location = $(this).data('href');
    });
}

/**
 * Inicializa atajos de teclado
 */
function initializeKeyboardShortcuts() {
    $(document).on('keydown', function(e) {
        // Ctrl + Enter para buscar
        if (e.ctrlKey && e.keyCode === 13) {
            const searchForm = $('#searchForm');
            if (searchForm.length) {
                searchForm.submit();
            }
        }
        
        // Escape para limpiar formulario
        if (e.keyCode === 27) {
            const authorInput = $('#Author');
            if (authorInput.length && authorInput.is(':focus')) {
                authorInput.val('').trigger('input');
            }
        }
    });
}

/**
 * Muestra alertas personalizadas
 */
function showAlert(type, message, duration = 5000) {
    const alertClass = type === 'error' ? 'alert-danger' : 
                      type === 'success' ? 'alert-success' : 
                      type === 'warning' ? 'alert-warning' : 'alert-info';
    
    const icon = type === 'error' ? 'fa-exclamation-triangle' : 
                type === 'success' ? 'fa-check-circle' : 
                type === 'warning' ? 'fa-exclamation-triangle' : 'fa-info-circle';
    
    const alertHtml = `
        <div class="alert ${alertClass} alert-dismissible fade show position-fixed" 
             style="top: 20px; right: 20px; z-index: 9999; min-width: 300px;" role="alert">
            <i class="fas ${icon} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('body').append(alertHtml);
    
    // Auto-remover después del tiempo especificado
    setTimeout(function() {
        $('.alert').fadeOut(500, function() {
            $(this).remove();
        });
    }, duration);
}

/**
 * Utilidades adicionales
 */
const BookSearchUtils = {
    /**
     * Limpia y formatea el nombre del autor
     */
    cleanAuthorName: function(name) {
        return name.trim()
                  .replace(/\s+/g, ' ')  // Múltiples espacios a uno solo
                  .replace(/^\w/, c => c.toUpperCase()); // Primera letra mayúscula
    },
    
    /**
     * Valida si una cadena está vacía o solo contiene espacios
     */
    isEmpty: function(str) {
        return !str || str.trim().length === 0;
    },
    
    /**
     * Escapa HTML para prevenir XSS
     */
    escapeHtml: function(text) {
        const map = {
            '&': '&amp;',
            '<': '&lt;',
            '>': '&gt;',
            '"': '&quot;',
            "'": '&#039;'
        };
        return text.replace(/[&<>"']/g, function(m) { return map[m]; });
    }
};

// Exponer utilidades globalmente
window.BookSearchUtils = BookSearchUtils;
