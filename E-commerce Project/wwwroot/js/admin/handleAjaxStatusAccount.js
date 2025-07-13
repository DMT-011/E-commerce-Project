const listBtnToogleStatus = document.querySelectorAll(".btn-toggle-status");
const listLabelToggles = document.querySelectorAll(".label-toggle");

listBtnToogleStatus.forEach((btn) => {
    btn.addEventListener("click", handleToogleBtn)
});

listLabelToggles.forEach((item) => {
    const btnToogleStatus = item.previousElementSibling;
    const isChecked = btnToogleStatus.checked;

    if (isChecked) {
        item.setAttribute("data-bs-title", "Đang kích hoạt");
    } else {
        item.setAttribute("data-bs-title", "Đã vô hiệu hóa")
    }
});

function changeContentTooltip(item, isActive) {
    const tooltip = bootstrap.Tooltip.getOrCreateInstance(item);
    if (isActive) {
        tooltip.setContent({'.tooltip-inner': "Đang kích hoạt"});
        tooltip.show();
    } else {
        tooltip.setContent({'.tooltip-inner': "Đã vô hiệu hóa"});
        tooltip.show();
    }
}

function handleToogleBtn(e) {
    const userId = this.dataset.idUser;
    const isChecked = this.checked;
    const labelToggle = this.nextElementSibling;

    if (isChecked) {
        const active = 1;
        updateStatusAcount(userId, active, this);
        changeContentTooltip(labelToggle, active);
    } else {
        const banned = 0;
        updateStatusAcount(userId, banned, this);
        changeContentTooltip(labelToggle, banned);
    }

    async function updateStatusAcount(userId, status, btnStatus) {

        const formData = {
            userId: userId,
            status: status,
        }
        fetch("/Admin/UpdateStatus", {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formData)
        })
            .then((res) => {
                return res.json();
            })
            .then((data) => {
                showNoticeUpdateStatus(data, btnStatus);
            });
    }

    async function showNoticeUpdateStatus(data, btnStatus) {
        var statusCode = await data.statusCode;
        if (statusCode === 200) {
            notice(
                title = "Cập nhật thành công",
                message = `Trạng thái tài khoản có ID = ${userId} đã được cập nhật.`,
                type = "info",
                icon = "fas fa-edit"
            );
        } else if (statusCode == 304) {
            setTimeout(function () {
                btnStatus.checked = true;
            }, 1000)
            notice(
                title = "Cập nhật thất bại",
                message = `Không thể tự vô hiệu tài khoản của bản thân.`,
                type = "danger",
                icon = "fas fa-times"
            );
        }
    }
}
