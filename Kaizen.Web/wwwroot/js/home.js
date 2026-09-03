(() => {
    const feedback = document.getElementById('daily-feedback');
    const container = document.getElementById('today-actions');
    if (!container) return;
    const busy = new Set();
    const statusClass = status => status === 'Completada' ? 'success' : status === 'NoRealizada' ? 'danger' : 'secondary';
    function refresh() {
        const rows = [...container.querySelectorAll('.daily-action')];
        const done = rows.filter(row => row.dataset.status !== 'Pendiente').length;
        document.getElementById('today-summary').textContent = `${done} de ${rows.length} registradas`;
        document.getElementById('day-complete').classList.toggle('d-none', done !== rows.length);
    }
    function message(text, ok) {
        feedback.textContent = text;
        feedback.className = `alert alert-${ok ? 'success' : 'danger'}`;
        feedback.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    }
    async function send(row, form, status, undo = false) {
        const id = row.dataset.entryId;
        if (busy.has(id)) return;
        busy.add(id);
        const buttons = [...row.querySelectorAll('button')];
        buttons.forEach(button => button.disabled = true);
        const data = new FormData(form);
        if (status) data.set('status', status);
        try {
            const response = await fetch(undo ? form.dataset.undoUrl : form.action, { method: 'POST', body: data, headers: { Accept: 'application/json' } });
            const body = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(body.message || 'No se pudo actualizar la acción.');
            row.dataset.status = body.status;
            const badge = row.querySelector('.status-badge');
            badge.textContent = body.statusLabel;
            badge.className = `status-badge status-${statusClass(body.status)}`;
            row.querySelector('.undo-log').classList.toggle('d-none', body.status === 'Pendiente');
            row.querySelector('.action-check').classList.toggle('is-complete', body.status === 'Completada');
            message(body.message, true);
            refresh();
        } catch (error) {
            message(error.message || 'No se pudo actualizar la acción.', false);
        } finally {
            buttons.forEach(button => button.disabled = false);
            busy.delete(id);
        }
    }
    container.addEventListener('submit', event => {
        const form = event.target.closest('.daily-log-form');
        if (!form) return;
        event.preventDefault();
        send(form.closest('.daily-action'), form, event.submitter?.value);
    });
    container.addEventListener('click', event => {
        const button = event.target.closest('.undo-log');
        if (!button) return;
        const form = button.closest('form');
        send(form.closest('.daily-action'), form, null, true);
    });
})();
