Handlebars.registerHelper('i18n', function(s){
    return (window.i18n && window.i18n.t) ? i18n.t(s) : s;
});