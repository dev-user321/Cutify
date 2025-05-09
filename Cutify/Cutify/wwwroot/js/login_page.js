document.querySelector('.password-toggle').addEventListener('click', function () {
    const passwordField = document.querySelector('input[name="Password"]');
    const icon = this.querySelector('i');

    if (passwordField.type === 'password') {
        passwordField.type = 'text';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    } else {
        passwordField.type = 'password';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    }
});
