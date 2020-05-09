var knowledgeBaseController = function () {
    this.initialize = function () {
        var kbId = parseInt($('#hid_knowledge_base_id').val());
        loadComments(kbId);
        registerEvents();
    };

    function registerEvents() {
        // this is the id of the form
        $("#commentform").submit(function (e) {
            e.preventDefault(); // avoid to execute the actual submit of the form.
            var form = $(this);
            var url = form.attr('action');

            $.post(url, form.serialize()).done(function (response) {
                var content = $("#txt_new_comment_content").val();

                var template = $('#tmpl_comments').html();
                var newComment = Mustache.render(template, {
                    id: response.id,
                    content: content,
                    createDate: formatRelativeTime(),
                    ownerName: $('#hid_current_login_name').val()
                });
                $("#txt_new_comment_content").val('');
                $("#txt_captcha").val('');

                $('#comment_list').prepend(newComment);
                var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                $('#hid_number_comments').val(numberOfComments);
                $('#comments-title').text('(' + numberOfComments + ') bình luận');

                $('#message-result').removeClass('alert-danger')
                    .addClass('alert-success')
                    .html('Bình luận thành công')
                    .show();
            }).error(function (err) {
                $('#message-result').html('');
                if (err.status === 400 && err.responseText) {
                    var errMsgs = JSON.parse(err.responseText);
                    for (field in errMsgs) {
                        $('#message-result').append(errMsgs[field] + '<br>');
                    }
                    $('#message-result')
                        .removeClass('alert-success"')
                        .addClass('alert-danger')
                        .show();
                    resetCaptchaImage('img-captcha');
                }
            });
        });

        //Binding reply comment event
        $('body').on('click', '.comment-reply-link', function (e) {
            e.preventDefault();
            var commentId = $(this).data('commentid');
            var template = $('#tmpl_reply_comment').html();
            var html = Mustache.render(template, {
                commentId: commentId
            });
            $('#reply_comment_' + commentId).html(html);

            // this is the id of the form
            $("#frm_reply_comment_" + commentId).submit(function (e) {
                e.preventDefault(); // avoid to execute the actual submit of the form.
                var form = $(this);
                var url = form.attr('action');

                $.post(url, form.serialize()).done(function (response) {
                    var content = $("#txt_reply_content_" + commentId).val();
                    var template = $('#tmpl_children_comments').html();

                    var newComment = Mustache.render(template, {
                        id: commentId,
                        content: content,
                        createDate: formatRelativeTime(),
                        ownerName: $('#hid_current_login_name').val()
                    });

                    //Reset reply comment
                    $("#txt_reply_content_" + commentId).val('');
                    $('#reply_comment_' + commentId).html('');
                    $('#txt_captcha_reply_' + commentId).html('');

                    //Prepend new comment to children
                    $('#children_comments_' + commentId).prepend(newComment);

                    //Update number of comment
                    var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                    $('#hid_number_comments').val(numberOfComments);
                    $('#comments-title').text('(' + numberOfComments + ') bình luận');

                    $('#message-result-reply-' + commentId).removeClass('alert-danger')
                        .addClass('alert-success')
                        .html('Bình luận thành công')
                        .show();
                }).error(function (err) {
                    $('#message-result-reply-' + commentId).html('');
                    if (err.status === 400 && err.responseText) {
                        var errMsgs = JSON.parse(err.responseText);
                        for (field in errMsgs) {
                            $('#message-result-reply-' + commentId).append(errMsgs[field] + '<br>');
                        }
                        $('#message-result-reply-' + commentId)
                            .removeClass('alert-success"')
                            .addClass('alert-danger')
                            .show();
                        resetCaptchaImage('img-captcha-reply-' + commentId);
                    }
                });
            });
        });
        $('#frm_vote').submit(function (e) {
            e.preventDefault();
            var form = $(this);
            $.post('/knowledgeBase/postVote', form.serialize()).done(function (response) {
                $('.like-it').text(response);
                $('.like-count').text(response);
            });
        });
        $('#frm_vote .like-it').click(function () {
            $('#frm_vote').submit();
        });

        $('#btn_send_report').off('click').on('click', function (e) {
            e.preventDefault();
            var form = $('#frm_report');
            $.post('/knowledgeBase/postReport', form.serialize())
                .done(function () {
                    $('#reportModal').modal('hide');
                    $('#txt_report_content').val('');
                }).error(function (err) {
                    $('#message-result-report').html('');
                    if (err.status === 400 && err.responseText) {
                        var errMsgs = JSON.parse(err.responseText);
                        for (field in errMsgs) {
                            $('#message-result-report').append(errMsgs[field] + '<br>');
                        }
                        $('#message-result-report')
                            .removeClass('alert-success"')
                            .addClass('alert-danger')
                            .show();
                        resetCaptchaImage('img-captcha-report');
                    }
                });
        });

        $("#img-captcha").click(function () {
            resetCaptchaImage('img-captcha');
        });
        $('body').on('click', '.img-captcha', function (e) {
            var id = $(this).data('id');
            resetCaptchaImage('img-captcha-reply-' + id);
        });

        $('body').on('click', '#img-captcha-report', function (e) {
            resetCaptchaImage('img-captcha-report');
        });
    }

    function loadComments(id) {
        $.get('/knowledgeBase/GetCommentByKnowledgeBaseId?knowledgeBaseId=' + id).done(function (response, statusText, xhr) {
            if (xhr.status === 200) {
                var template = $('#tmpl_comments').html();
                var childrenTemplate = $('#tmpl_children_comments').html();
                if (response) {
                    var html = '';
                    $.each(response, function (index, item) {
                        var childrenHtml = '';
                        if (item.children.length > 0) {
                            $.each(item.children, function (childIndex, childItem) {
                                childrenHtml += Mustache.render(childrenTemplate, {
                                    id: childItem.id,
                                    content: childItem.content,
                                    createDate: formatRelativeTime(childItem.createDate),
                                    ownerName: childItem.ownerName
                                });
                            });
                        }
                        html += Mustache.render(template, {
                            childrenHtml: childrenHtml,
                            id: item.id,
                            content: item.content,
                            createDate: formatRelativeTime(item.createDate),
                            ownerName: item.ownerName
                        });
                    });
                    $('#comment_list').html(html);
                }
            }
        });
    }

    function resetCaptchaImage(id) {
        d = new Date();
        $("#" + id).attr("src", "/get-captcha-image?" + d.getTime());
    }
};