// Perfil modal AJAX submit
document.addEventListener('DOMContentLoaded', function () {
    var form = document.querySelector('#profileModal form');
    if (!form) return;

    // Previsualizar al seleccionar archivo
    var fileInput = form.querySelector('#FotoPerfil');
    var imgEl = document.querySelector('#profileModal img');
    if (fileInput && imgEl) {
        fileInput.addEventListener('change', function () {
            var f = fileInput.files && fileInput.files[0];
            if (!f) return;
            var url = URL.createObjectURL(f);
            imgEl.src = url;
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
                // Actualizar imagen del modal y cache-busting
                if (imgEl && data.photoUrl) {
                    var bust = (data.photoUrl.indexOf('?') === -1 ? '?' : '&') + 'v=' + Date.now();
                    imgEl.src = data.photoUrl + bust;
                }
                // Actualizar avatar del navbar si existe
                var navImg = document.getElementById('navbarAvatar');
                if (navImg && data.photoUrl) {
                    var bust2 = (data.photoUrl.indexOf('?') === -1 ? '?' : '&') + 'v=' + Date.now();
                    navImg.src = data.photoUrl + bust2;
                }
                var modalEl = document.getElementById('profileModal');
                if (modalEl) {
                    var modal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
                    modal.hide();
                }
                // Limpiar passwords
                var cur = form.querySelector('#CurrentPassword'); if (cur) cur.value = '';
                var np = form.querySelector('#NewPassword'); if (np) np.value = '';
                var cp = form.querySelector('#ConfirmPassword'); if (cp) cp.value = '';
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

