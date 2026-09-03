(() => {
    document.querySelectorAll('[data-password-toggle]').forEach(button => {
        const input = button.parentElement?.querySelector('input');
        if (!input) return;

        button.addEventListener('click', () => {
            const reveal = input.type === 'password';
            input.type = reveal ? 'text' : 'password';
            button.setAttribute('aria-pressed', reveal.toString());
            button.setAttribute('aria-label', reveal ? 'Ocultar contraseña' : 'Mostrar contraseña');
            input.focus();
        });
    });
})();
