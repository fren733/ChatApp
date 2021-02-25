const toggleSwitch = document.querySelector('.theme-switch input[type="checkbox"]');

$.ajax({
    dataType: "json",
    url: "/Home/GetUserSettings",
    traditional: true,
    success: (data) => {
        if (data) {
            document.documentElement.setAttribute('data-theme', 'dark');
            if (toggleSwitch != null) {
                toggleSwitch.setAttribute("checked", "true");
            }
        }
        else {
            document.documentElement.setAttribute('data-theme', 'light');
        }
    }
});

if (toggleSwitch != null) {
    function switchTheme(e) {
        if (e.target.checked) {
            document.documentElement.setAttribute('data-theme', 'dark');
        }
        else {
            document.documentElement.setAttribute('data-theme', 'light');
        }
    }

    toggleSwitch.addEventListener('change', switchTheme, false);
}


