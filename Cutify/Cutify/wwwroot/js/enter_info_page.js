// Dolu saatları yeniləyir
function updateOccupiedTimes() {
    const barberId = @Model.BarberId;
    const selectedDate = document.getElementById("date-picker").value;

    if (!selectedDate) {
        fillAvailableTimes([]); // Tarix seçilməyibsə, bütün saatları göstər
        return;
    }

    fetch(`/Reservation/GetOccupiedTimes?barberId=${barberId}&date=${selectedDate}`, {
        method: 'GET'
    })
        .then(response => response.json())
        .then(occupiedTimes => {
            fillAvailableTimes(occupiedTimes);
        })
        .catch(error => {
            console.error("Xəta baş verdi:", error);
            fillAvailableTimes([]); // Xəta olarsa bütün saatları göstər
        });
}

// Mövcud saatları doldurur (dolu saatlar çıxarılır)
function fillAvailableTimes(occupiedTimes) {
    const allTimes = [
        "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
        "12:00", "12:30", "13:00", "13:30", "14:00", "14:30",
        "15:00", "15:30", "16:00", "16:30", "17:00", "17:30"
    ];

    // Mövcud saatları filtr edirik
    const availableTimes = allTimes.filter(time => !occupiedTimes.includes(time));

    const timeSelect = document.getElementById("time-picker");
    timeSelect.innerHTML = "<option value=''>Saat seçin</option>"; // Başlıq

    // Yalnız mövcud saatları əlavə edirik
    availableTimes.forEach(time => {
        const option = document.createElement("option");
        option.value = time;
        option.textContent = time;
        timeSelect.appendChild(option);
    });

    // Əgər heç bir mövcud saat yoxdursa
    if (availableTimes.length === 0) {
        const option = document.createElement("option");
        option.disabled = true;
        option.textContent = "Mövcud saat yoxdur";
        timeSelect.appendChild(option);
    }
}

// Səhifə yükləndikdə və tarix seçildikdə saatları yeniləyirik
document.addEventListener("DOMContentLoaded", function () {
    flatpickr("#date-picker", {
        dateFormat: "Y-m-d",
        minDate: "today", // Bu günün tarixindən əvvəl tarix seçilməsi mümkün deyil
        disableMobile: true,
        onChange: updateOccupiedTimes // Tarix dəyişildikdə mövcud saatları yenilə
    });

    updateOccupiedTimes(); // İlk açılışda saatları yüklə
});