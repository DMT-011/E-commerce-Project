const modalConfirmDelete = document.getElementById('modal-confirm-delete');
const formDelete = document.querySelector("#form-delete-account");
const btnConfirmDelete = document.querySelector(".btn-confirm-delete");

if (modalConfirmDelete) {
    modalConfirmDelete.addEventListener('show.bs.modal', event => {
        const contentBody = modalConfirmDelete.querySelector(".slider-name");
        const userId = event.relatedTarget.dataset.userId;
        const userName = event.relatedTarget.dataset.userName;

        formDelete.action = `/User/Delete/${userId}`;
        contentBody.textContent = userName;
    })
}

btnConfirmDelete.onclick = function () {
    formDelete.submit();
}
