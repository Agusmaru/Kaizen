(() => {
    const feedback = document.getElementById('daily-feedback');
    const container = document.getElementById('today-actions');
    if (!container) return;
    const busy = new Set();
    let ordering = false;
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
    function refreshOrderControls() {
        [...container.querySelectorAll('.daily-action')].forEach((row, index) => {
            const handle = row.querySelector('.action-order-handle');
            handle.disabled = ordering;
            handle.setAttribute('aria-label', `Ordenar ${row.querySelector('.action-title strong').textContent}. Posición ${index + 1}`);
        });
    }
    async function saveOrder(originalRows) {
        ordering = true;
        refreshOrderControls();
        const data = new FormData();
        const token = container.querySelector('input[name="__RequestVerificationToken"]')?.value;
        if (token) data.append('__RequestVerificationToken', token);
        [...container.querySelectorAll('.daily-action')].forEach(row => data.append('ids', row.dataset.entryId));
        try {
            const response = await fetch(container.dataset.orderUrl, { method: 'POST', body: data, headers: { Accept: 'application/json' } });
            const body = await response.json().catch(() => ({}));
            if (!response.ok) throw new Error(body.message || 'No se pudo guardar el orden.');
            message(body.message, true);
        } catch (error) {
            originalRows.forEach(row => container.append(row));
            message(error.message || 'No se pudo guardar el orden.', false);
        } finally {
            ordering = false;
            refreshOrderControls();
        }
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
    container.addEventListener('keydown', event => {
        const handle = event.target.closest('.action-order-handle');
        if (!handle || ordering || !['ArrowUp', 'ArrowDown'].includes(event.key)) return;
        event.preventDefault();
        const row = handle.closest('.daily-action');
        const originalRows = [...container.querySelectorAll('.daily-action')];
        const sibling = event.key === 'ArrowUp' ? row.previousElementSibling : row.nextElementSibling;
        if (!sibling) return;
        if (event.key === 'ArrowUp') container.insertBefore(row, sibling);
        else container.insertBefore(sibling, row);
        refreshOrderControls();
        saveOrder(originalRows).then(() => handle.focus());
    });
    container.querySelectorAll('.action-order-handle').forEach(handle => {
        handle.addEventListener('pointerdown', event => {
            if (ordering || event.button > 0) return;
            const row = handle.closest('.daily-action');
            const originalRows = [...container.querySelectorAll('.daily-action')];
            const otherRows = originalRows.filter(item => item !== row);
            const initialIndex = originalRows.indexOf(row);
            const startY = event.clientY;
            let destinationIndex = initialIndex;
            let moved = false;
            handle.setPointerCapture(event.pointerId);
            row.classList.add('is-dragging');
            document.body.classList.add('daily-actions-dragging');

            const move = moveEvent => {
                moveEvent.preventDefault();
                const distance = moveEvent.clientY - startY;
                if (!moved && Math.abs(distance) < 5) return;
                moved = true;
                row.style.transform = `translateY(${distance}px) scale(1.01)`;
                destinationIndex = otherRows.filter(item => {
                    const bounds = item.getBoundingClientRect();
                    return moveEvent.clientY > bounds.top + bounds.height / 2;
                }).length;
            };
            const finish = () => {
                handle.removeEventListener('pointermove', move);
                handle.removeEventListener('pointerup', finish);
                handle.removeEventListener('pointercancel', cancel);
                row.style.removeProperty('transform');
                row.classList.remove('is-dragging');
                document.body.classList.remove('daily-actions-dragging');
                if (!moved || destinationIndex === initialIndex) return;
                const reference = otherRows[destinationIndex];
                if (reference) container.insertBefore(row, reference);
                else container.append(row);
                refreshOrderControls();
                saveOrder(originalRows);
            };
            const cancel = () => {
                originalRows.forEach(originalRow => container.append(originalRow));
                moved = false;
                finish();
                refreshOrderControls();
            };
            handle.addEventListener('pointermove', move);
            handle.addEventListener('pointerup', finish);
            handle.addEventListener('pointercancel', cancel);
        });
    });
    refreshOrderControls();
})();
