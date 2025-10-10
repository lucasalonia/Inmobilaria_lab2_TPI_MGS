// Perfil modal AJAX submit
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('#profileModal form');
    if (!form) return;

    var fileInput = form.querySelector('#FotoPerfil');
    var imgEl = document.querySelector('#profileModal img');
    var removeCheckbox = form.querySelector('#RemoveFotoPerfil');
    var defaultAvatar = '/img/avatar-default.svg';
    var originalPhoto = imgEl ? imgEl.src : defaultAvatar;

    if (fileInput && imgEl) {
        fileInput.addEventListener('change', function () {
            var f = fileInput.files && fileInput.files[0];
            if (!f) return;
            var url = URL.createObjectURL(f);
            imgEl.src = url;
            if (removeCheckbox && removeCheckbox.checked) {
                removeCheckbox.checked = false;
            }
        });
    }

    if (removeCheckbox && fileInput && imgEl) {
        removeCheckbox.addEventListener('change', function () {
            if (removeCheckbox.checked) {
                fileInput.disabled = true;
                fileInput.value = '';
                imgEl.src = defaultAvatar;
            } else {
                fileInput.disabled = false;
                imgEl.src = originalPhoto;
            }
        });
    }
    form.addEventListener('submit', async function (e) {
        e.preventDefault();
        var url = form.getAttribute('action');
        var formData = new FormData(form);
        var tokenInput = form.querySelector('input[name="__RequestVerificationToken"]');
        var headers = { 'X-Requested-With': 'XMLHttpRequest' };
        if (tokenInput && tokenInput.value) {
            headers['RequestVerificationToken'] = tokenInput.value;
        }
        try {
            var resp = await fetch(url, { method: 'POST', headers: headers, body: formData });
            var data = await resp.json();
            if (data && data.success) {
                if (window.toastr) toastr.success(data.message || 'Perfil actualizado');
                var photoUrl = data.photoUrl || defaultAvatar;
                if (imgEl) {
                    if (photoUrl === defaultAvatar) {
                        imgEl.src = defaultAvatar;
                    } else {
                        var bust = (photoUrl.indexOf('?') === -1 ? '?' : '&') + 'v=' + Date.now();
                        imgEl.src = photoUrl + bust;
                    }
                }
                var navImg = document.getElementById('navbarAvatar');
                if (navImg) {
                    if (photoUrl === defaultAvatar) {
                        navImg.src = defaultAvatar;
                    } else {
                        var bust2 = (photoUrl.indexOf('?') === -1 ? '?' : '&') + 'v=' + Date.now();
                        navImg.src = photoUrl + bust2;
                    }
                }
                var modalEl = document.getElementById('profileModal');
                if (modalEl) {
                    var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                    modal.hide();
                }
                setTimeout(function() {
                    location.reload();
                }, 500);
            } else {
                var msg = (data && data.message) ? data.message : 'No se pudo actualizar el perfil';
                if (window.toastr) toastr.error(msg); else alert(msg);
            }
        } catch (err) {
            if (window.toastr) toastr.error('Error de red al actualizar perfil');
            console.error(err);
        }
    });
});

