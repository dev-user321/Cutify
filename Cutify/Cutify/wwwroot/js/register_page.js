document.getElementById('Image').addEventListener('change', function (e) {
    const file = e.target.files[0];
    if (file) {
        const reader = new FileReader();

        reader.onload = function (event) {
            const profileImage = document.getElementById('profile-image-preview');
            const defaultIcon = document.getElementById('default-icon');

            // Display the image
            // Sekilin gosterilmesi
            profileImage.src = event.target.result;
            profileImage.style.display = 'block';

            // Hide the default icon
            defaultIcon.style.display = 'none';
        }

        reader.readAsDataURL(file);
    }
});


// Password visibility toggle
// 
document.querySelector('.password-toggle').addEventListener('click', function () {
    const passwordField = document.getElementById('password');
    const icon = document.getElementById('password-toggle-icon');

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
