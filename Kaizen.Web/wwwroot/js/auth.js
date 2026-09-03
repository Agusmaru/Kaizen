(() => {
    const button = document.querySelector('[data-password-toggle]');
    const input = button?.parentElement.querySelector('input');
    if (!button || !input) return;
    button.addEventListener('click', () => {
        const reveal = input.type === 'password';
        input.type = reveal ? 'text' : 'password';
        button.setAttribute('aria-pressed', reveal.toString());
        button.setAttribute('aria-label', reveal ? 'Ocultar contraseña' : 'Mostrar contraseña');
        input.focus();
    });
})();
