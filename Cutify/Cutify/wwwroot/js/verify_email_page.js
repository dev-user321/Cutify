document.addEventListener('DOMContentLoaded', function () {
    // Timer functionality
    let timeLeft = 59;
    const timerElement = document.querySelector('.timer');

    const timer = setInterval(() => {
        timeLeft--;
        if (timeLeft < 0) {
            clearInterval(timer);
            timerElement.textContent = '00:00';
            document.querySelector('.resend-link').style.color = '#000';
            document.querySelector('.resend-link').style.cursor = 'pointer';
        } else {
            const seconds = String(timeLeft).padStart(2, '0');
            timerElement.textContent = `00:${seconds}`;
        }
    }, 1000);

    // Auto-focus and auto-tab between input fields
    const inputs = document.querySelectorAll('.code-input');
    inputs.forEach((input, index) => {
        input.addEventListener('input', function () {
            if (this.value.length >= this.maxLength && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
        });

        input.addEventListener('keydown', function (e) {
            if (e.key === 'Backspace' && !this.value && index > 0) {
                inputs[index - 1].focus();
            }
        });
    });

    // Resend code functionality
    document.querySelector('.resend-link').addEventListener('click', function () {
        // Send AJAX request to resend code
        fetch('/Account/VerifyEmail', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (response.ok) {
                    // Reset timer
                    timeLeft = 59;
                    timerElement.textContent = '00:59';
                }
            });
    });
});