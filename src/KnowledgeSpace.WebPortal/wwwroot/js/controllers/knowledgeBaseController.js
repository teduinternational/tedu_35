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
                    content: content,
                    createDate: new Date(),
                    ownerName: $('#hid_current_login_name').val()
                });
                $("#txt_new_comment_content").val('');
                $('#comment_list').prepend(newComment);
                var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                $('#hid_number_comments').val(numberOfComments);
                $('#comments-title').text('(' + numberOfComments + ') bình luận');
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
                        createDate: new Date(),
                        ownerName: $('#hid_current_login_name').val()
                    });

                    //Reset reply comment
                    $("#txt_reply_content_" + commentId).val('');
                    $('#reply_comment_' + commentId).html('');

                    //Prepend new comment to children
                    $('#children_comments_' + commentId).prepend(newComment);

                    //Update number of comment
                    var numberOfComments = parseInt($('#hid_number_comments').val()) + 1;
                    $('#hid_number_comments').val(numberOfComments);
                    $('#comments-title').text('(' + numberOfComments + ') bình luận');
                });
            });
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
                                    createDate: childItem.createDate,
                                    ownerName: childItem.ownerName
                                });
                            });
                        }
                        html += Mustache.render(template, {
                            childrenHtml: childrenHtml,
                            id: item.id,
                            content: item.content,
                            createDate: item.createDate,
                            ownerName: item.ownerName
                        });
                    });
                    $('#comment_list').html(html);
                }
            }
        });
    }
};