var accountController = function () {
    this.initialize = function () {
        registerEvents();
    };

    function registerEvents() {
        CKEDITOR.replace('txt_environment');
        CKEDITOR.replace('txt_problem');
        //CKEDITOR.replace('txt_step_reproduce');
        //CKEDITOR.replace('txt_workaround');
        CKEDITOR.replace('txt_note');
        //CKEDITOR.replace('txt_description');

        $('#btn_add_attachment').off('click').on('click', function () {
            $('#attachment_items').prepend('<p><input type="file" name="attachments" /></p>');
            return false;
        });
    }
};