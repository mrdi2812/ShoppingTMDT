var common = {
    init: function () {
        common.registerEvents();
    },
    registerEvents: function () {      
        $('#btnLogout').off('click').on('click', function (e) {
            e.preventDefault();
            $('#frmLogout').submit();
        });     
    }
}
common.init();