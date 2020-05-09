var accountController = function () {
    this.initialize = function () {
        registerEvents();
    };

    function registerEvents() {
        CKEDITOR.replace('txt_problem');
        CKEDITOR.replace('txt_note');

        CKEDITOR.on('instanceReady', function () {
            $.each(CKEDITOR.instances, function (instance) {
                CKEDITOR.instances[instance].document.on("keyup", CK_jQ);
                CKEDITOR.instances[instance].document.on("paste", CK_jQ);
                CKEDITOR.instances[instance].document.on("keypress", CK_jQ);
                CKEDITOR.instances[instance].document.on("blur", CK_jQ);
                CKEDITOR.instances[instance].document.on("change", CK_jQ);
            });
        });

        $('#btn_add_attachment').off('click').on('click', function () {
            $('#attachment_items').prepend('<p><input type="file" name="attachments" /></p>');
            return false;
        });

        $("#frm_new_kb").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.

            var form = $(this);
            form.validate();

            if (form.valid()) {
                var url = form.attr('action');
                var formData = false;
                if (window.FormData) {
                    formData = new FormData(form[0]);
                }

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: formData,
                    success: function () {
                        window.location.href = '/my-kbs';
                    },
                    enctype: 'multipart/form-data',
                    processData: false,  // Important!
                    contentType: false,
                    beforeSend: function () {
                        $('#contact-loader').show();
                    },
                    cache: false,
                    error: function (err) {
                        $('#contact-loader').hide();
                        $('#message-result').html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result').append(errMsgs[field] + '<br>');
                            }
                            resetCaptchaImage('img-captcha');
                        }
                    }
                });
            }
        });

        $("#frm_edit_kb").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.

            var form = $(this);
            form.validate();

            if (form.valid()) {
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
                    beforeSend: function () {
                        $('#contact-loader').show();
                    },
                    cache: false,
                    error: function (err) {
                        $('#contact-loader').hide();
                        $('#message-result').html('');
                        if (err.status === 400 && err.responseText) {
                            var errMsgs = JSON.parse(err.responseText);
                            for (field in errMsgs) {
                                $('#message-result').append(errMsgs[field] + '<br>');
                            }
                            resetCaptchaImage('img-captcha');
                        }
                    }
                });
            }
        });

        $('body').on('click', '#img-captcha', function (e) {
            resetCaptchaImage('img-captcha');
        });
    }

    function CK_jQ() {
        for (instance in CKEDITOR.instances) {
            CKEDITOR.instances[instance].updateElement();
        }
    }

    function resetCaptchaImage(id) {
        d = new Date();
        $("#" + id).attr("src", "/get-captcha-image?" + d.getTime());
    }
};