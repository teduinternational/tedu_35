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

        $("#frm_new_kb").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.

            var form = $(this);
            var url = form.attr('action');
            var formData = false;
            if (window.FormData) {
                formData = new FormData(form[0]);
            }

            $.ajax({
                url: url,
                type: 'POST',
                data: formData,
                success: function (data) {
                    window.location.href = '/my-kbs';
                },
                enctype: 'multipart/form-data',
                processData: false,  // Important!
                contentType: false,
                cache: false,
            });
        });
    }
};