// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
let filterActive = false;
let filterPaying = false;

const btnActive = document.getElementById('btn-active');
const btnPaying = document.getElementById('btn-paying');

btnActive.addEventListener('click', () => {
    filterActive = !filterActive;
    btnActive.classList.toggle('btn-success', filterActive);
    btnActive.classList.toggle('btn-primary', !filterActive);
    filterDoctors();
});

btnPaying.addEventListener('click', () => {
    filterPaying = !filterPaying;
    btnPaying.classList.toggle('btn-success', filterPaying);
    btnPaying.classList.toggle('btn-primary', !filterPaying);
    filterDoctors();
});

function filterDoctors() {
    const doctors = Array.from(document.querySelectorAll('.doctor-card'));

    doctors.forEach(doc => {
        const isActive = doc.getAttribute('data-active') === 'true';
        const promotionLevel = parseInt(doc.getAttribute('data-promotionlevel')) || 10;

        let show = true;

        if (filterActive && filterPaying) {
            if (!isActive || promotionLevel > 5) {
                show = false;
            }
        }
        else if (filterActive) {
            if (!isActive) {
                show = false;
            }
        }
        else if (filterPaying) {
            if (promotionLevel > 5) {
                show = false;
            }
        }

        doc.style.display = show ? '' : 'none';
    });
}

document.querySelectorAll('.contact-button').forEach(button => {
    button.addEventListener('click', function () {
        var contactModal = new bootstrap.Modal(document.getElementById('contactModal'));
        contactModal.show();
    });
});

document.getElementById('contactForm').addEventListener('submit', async function (e) {
    e.preventDefault();

    const fullName = document.getElementById('fullName').value.trim();
    const phone = document.getElementById('phone').value.trim();
    const email = document.getElementById('email').value.trim();

    clearValidationErrors();

    let isValid = true;

    if (fullName === '') {
        showValidationError('fullName', 'נא למלא שם מלא');
        isValid = false;
    }

    const phoneRegex = /^[0-9\-]+$/;
    if (phone === '') {
        showValidationError('phone', 'נא למלא מספר טלפון');
        isValid = false;
    } else if (!phoneRegex.test(phone)) {
        showValidationError('phone', 'מספר טלפון חייב להכיל רק ספרות ומקפים');
        isValid = false;
    }

    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (email === '') {
        showValidationError('email', 'נא למלא כתובת אימייל');
        isValid = false;
    } else if (!emailRegex.test(email)) {
        showValidationError('email', 'כתובת אימייל לא תקינה');
        isValid = false;
    }

    if (!isValid) {
        return;
    }

    const response = await fetch('/Doctors/SaveLead', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ fullName, phone, email })
    });

    if (response.ok) {
        alert('פרטים נשמרו בהצלחה!');
        var contactModal = bootstrap.Modal.getInstance(document.getElementById('contactModal'));
        contactModal.hide();
        document.getElementById('contactForm').reset();
    } else {
        alert('שגיאה בשליחת הפרטים.');
    }
});

function showValidationError(inputId, message) {
    const input = document.getElementById(inputId);
    if (!input) return;
    input.classList.add('is-invalid');

    let error = document.createElement('div');
    error.className = 'invalid-feedback';
    error.innerText = message;
    input.parentNode.appendChild(error);
}

function clearValidationErrors() {
    document.querySelectorAll('.is-invalid').forEach(el => el.classList.remove('is-invalid'));
    document.querySelectorAll('.invalid-feedback').forEach(el => el.remove());
}
