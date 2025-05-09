 //const searchInput = document.getElementById("search-input");
 //   const barberCards = document.querySelectorAll(".barber-card");

 //   searchInput.addEventListener("input", function () {
 //       const query = this.value.toLowerCase();

 //       barberCards.forEach(card => {
 //           const name = card.querySelector(".barber-name").textContent.toLowerCase();
 //           if (name.includes(query)) {
 //               card.parentElement.style.display = "block";
 //           } else {
 //               card.parentElement.style.display = "none";
 //           }
 //       });
//   });
document.addEventListener('DOMContentLoaded', function () {
    const radioInputs = document.querySelectorAll('.radio-input');
    const submitButton = document.querySelector('.submit-button');

    radioInputs.forEach(input => {
        input.addEventListener('change', function () {
            if (this.checked) {
                submitButton.classList.add('active');
            }
        });
    });
});