window.sessionStorageHelper = {
    set: function (key, value) {
        sessionStorage.setItem(key, value);
    },
    get: function (key) {
        return sessionStorage.getItem(key);
    },
    remove: function (key) {
        sessionStorage.removeItem(key);
    }
};

window.userDisplay = {
    showLoggedUser: function () {
        var user = sessionStorage.getItem("username");
        var userInfo = document.getElementById("user-info");
        var logoutBtn = document.getElementById("logout-btn");

        if (user) {
            userInfo.textContent = "👤 " + user;
            logoutBtn.classList.remove("d-none");
        } else {
            userInfo.textContent = "🔓 Visitante";
            logoutBtn.classList.add("d-none");
        }
    }
};

function logout() {
    sessionStorage.removeItem("authToken");
    sessionStorage.removeItem("username");
    window.location.href = "/";
}

