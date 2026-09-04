(() => {
    const dialog = document.getElementById('notification-dialog');
    const openButton = document.getElementById('notification-open');
    if (!dialog || !openButton) return;

    const form = document.getElementById('notification-form');
    const enabledInput = document.getElementById('notification-enabled');
    const timeInput = document.getElementById('notification-time');
    const status = document.getElementById('notification-status');
    const indicator = document.getElementById('notification-indicator');
    const userKey = dialog.dataset.user.toLowerCase();
    const preferenceKey = `kaizen:recordatorio:${userKey}`;
    let timer = null;

    function readPreference() {
        try {
            return JSON.parse(localStorage.getItem(preferenceKey)) || { enabled: false, time: '09:00' };
        } catch {
            return { enabled: false, time: '09:00' };
        }
    }

    function showStatus(message, kind = '') {
        status.textContent = message;
        status.className = `notification-status${kind ? ` is-${kind}` : ''}`;
    }

    function refreshIndicator(preference = readPreference()) {
        const active = preference.enabled && 'Notification' in window && Notification.permission === 'granted';
        indicator.classList.toggle('d-none', !active);
        openButton.classList.toggle('is-active', active);
        openButton.title = active ? `Recordatorio activo a las ${preference.time}` : 'Configurar recordatorio diario';
    }

    function pendingActions() {
        return [...document.querySelectorAll('#today-actions .daily-action')]
            .filter(row => row.dataset.status === 'Pendiente')
            .map(row => row.querySelector('.action-title strong')?.textContent?.trim())
            .filter(Boolean);
    }

    function localDateKey(date) {
        return `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
    }

    function sendReminder(preference) {
        if (!preference.enabled || !('Notification' in window) || Notification.permission !== 'granted') return;
        const now = new Date();
        const sentKey = `kaizen:recordatorio-enviado:${userKey}:${localDateKey(now)}:${preference.time}`;
        if (localStorage.getItem(sentKey)) return;

        const actions = pendingActions();
        localStorage.setItem(sentKey, 'true');
        if (!actions.length) return;

        const extra = actions.length > 3 ? ` y ${actions.length - 3} más` : '';
        const notification = new Notification(`Kaizen · ${actions.length} ${actions.length === 1 ? 'acción pendiente' : 'acciones pendientes'}`, {
            body: `${actions.slice(0, 3).join(' · ')}${extra}`,
            icon: '/images/kaizen-logo-192.png',
            tag: `kaizen-${userKey}-${localDateKey(now)}`
        });
        notification.onclick = () => { window.focus(); notification.close(); };
    }

    function schedule(preference = readPreference()) {
        clearTimeout(timer);
        refreshIndicator(preference);
        if (!preference.enabled || !('Notification' in window) || Notification.permission !== 'granted') return;

        const [hours, minutes] = preference.time.split(':').map(Number);
        const now = new Date();
        const target = new Date(now);
        target.setHours(hours, minutes, 0, 0);
        if (target <= now) {
            sendReminder(preference);
            target.setDate(target.getDate() + 1);
        }
        timer = window.setTimeout(() => {
            sendReminder(preference);
            schedule(preference);
        }, Math.min(target - now, 2147483647));
    }

    openButton.addEventListener('click', () => {
        const preference = readPreference();
        enabledInput.checked = preference.enabled;
        timeInput.value = preference.time;
        showStatus(!('Notification' in window) ? 'Este navegador no admite notificaciones.' : Notification.permission === 'denied' ? 'Las notificaciones están bloqueadas en la configuración del navegador.' : '');
        dialog.showModal();
    });
    document.getElementById('notification-close').addEventListener('click', () => dialog.close());
    document.getElementById('notification-cancel').addEventListener('click', () => dialog.close());
    dialog.addEventListener('click', event => { if (event.target === dialog) dialog.close(); });

    form.addEventListener('submit', async event => {
        event.preventDefault();
        if (enabledInput.checked) {
            if (!('Notification' in window)) {
                showStatus('Este navegador no admite notificaciones.', 'error');
                return;
            }
            const permission = Notification.permission === 'granted' ? 'granted' : await Notification.requestPermission();
            if (permission !== 'granted') {
                showStatus('Necesitás permitir las notificaciones para activar el recordatorio.', 'error');
                return;
            }
        }
        const preference = { enabled: enabledInput.checked, time: timeInput.value || '09:00' };
        localStorage.setItem(preferenceKey, JSON.stringify(preference));
        schedule(preference);
        dialog.close();
    });

    window.addEventListener('pageshow', () => schedule());
    document.addEventListener('visibilitychange', () => { if (!document.hidden) schedule(); });
    schedule();
})();
