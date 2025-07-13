const formRestore = document.querySelector("#form-restore-account");
const btnRestore = document.querySelectorAll(".btn-restore-account");

btnRestore.forEach(item => {
    item.addEventListener("click", function () {
        const id = this.dataset.userId;
        formRestore.action = `/Account/Restore/${id}`;
        formRestore.submit();
    })
});
