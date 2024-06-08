document.addEventListener('DOMContentLoaded', () => {
    let toastBox = document.getElementById('toastBox');
    let successMsg = '<i class="fa-solid fa-circle-check"></i>';
    let errorMsg = '<i class="fa-solid fa-circle-exclamation"></i>'
    window.showToast = function (msg) {
        let toast = document.createElement('div');
        toast.classList.add('toast');
        toast.classList.add('show');
        toast.innerHTML = successMsg + msg;
        if (toastBox) {
            toastBox.appendChild(toast);
        } else {
            console.error("Không tìm thấy phần tử toastBox!");
        }

        setTimeout(() => {
            toast.remove();
        }, 3000);
    };
    window.showToastErr = function (msg) {
        let toast = document.createElement('div');
        toast.classList.add('toast');
        toast.classList.add('show');
        toast.classList.add('error');
        toast.innerHTML = errorMsg + msg;
        if (toastBox) {
            toastBox.appendChild(toast);
        } else {
            console.error("Không tìm thấy phần tử toastBox!");
        }

        setTimeout(() => {
            toast.remove();
        }, 3000);
    };

    const modalBox = document.querySelector('.js-modalBox');
    const modalBoxBody = document.querySelector('.js-modalBox-body');

    window.showBox = function (inputContent) {
        modalBoxBody.innerHTML = inputContent;
        modalBox.style.display = 'flex';
    };

    document.addEventListener('click', (event) => {
        if (event.target.classList.contains('js-modalBox-close') ||
            event.target.classList.contains('js-accept') ||
            event.target === modalBox) {
            closeModal(event.target.closest('.js-modalBox'));
        }
        if (event.target.classList.contains('js-accept')) {
            const redirectUrl = event.target.dataset.redirectUrl;
            if (redirectUrl) {
                window.location.href = redirectUrl;
            } else {
                console.warn('No redirect URL found on js-accept button.');
            }
        }
    });

    function closeModal(modal) {
        modal.style.display = 'none';
    }
});