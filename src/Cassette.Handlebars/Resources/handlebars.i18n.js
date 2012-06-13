Handlebars.registerHelper('i18n', function(s, b, c){
    return (window.i18n && typeof window.i18n.translate === 'function') ? i18n.translate(s) : s;
});