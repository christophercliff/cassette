$(function(){
    
//    window.i18n = {
//        t: function () {
//            //return 'Hello';
//            return 'Hola';
//        }
//    };
    
    console.log(JST['test'].render({
        name: 'Chris'
    }));
});