Handlebars.registerHelper('i18n', function(s){
    return (i18n != undefined ? i18n.t(s) : s);
});