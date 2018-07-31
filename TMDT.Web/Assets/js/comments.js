var comment = {
    init: function () {
        var saveComment = function (data) {

            // Convert pings to human readable format
            $(data.pings).each(function (index, id) {
                var user = usersArray.filter(function (user) { return user.id == id })[0];
                data.content = data.content.replace('@' + id, '@' + user.fullname);
            });

            return data;
        }
        $('#comments-container').comments({
            profilePictureURL: 'https://viima-app.s3.amazonaws.com/media/user_profiles/user-icon.png',
            currentUserId: 1,
            roundProfilePictures: true,
            textareaRows: 1,
            enableAttachments: true,
            enableHashtags: true,
            enablePinging: true,
            postCommentOnEnter: true,
            textareaPlaceholderText: "Viết một bình luận",
            defaultNavigationSortKey: 'newest',
            maxRepliesVisible: 3,
            newestText: 'Mới nhất',
            oldestText: 'Cũ nhất',
            popularText: 'Phổ biến nhất',
            attachmentsText: 'Hiển thị đính kèm',
            sendText: 'Đăng',
            replyText: 'Trả lời',
            editText: 'Chỉnh sửa',
            editedText: 'Đã chỉnh sửa',
            youText: 'Tôi',
            saveText: 'Cập nhật',
            deleteText: 'Xóa',
            viewAllRepliesText: 'Hiển thị tất cả trả lời (__replyCount__)',
            hideRepliesText: 'Ẩn',
            noCommentsText: 'Chưa có bình luận nào',
            noAttachmentsText: 'Không có file đính kèm',
            attachmentDropText: 'Thả vào đây',
            highlightColor: '#23A6F0',
            deleteButtonColor: 'red',
            getUsers: function (success, error) {
                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: { id: $('#hidProductId').val() },
                    url: '/Product/GetAllUser',
                    success: function (usersArray) {
                        var users = [];
                        $.each(usersArray, function (i, item) {
                            users.push({
                                id: item.Id,
                                fullname: item.FullName,
                                email: item.Email,
                                profile_picture_url: comment.getUserAvatar(item.Avatar),
                            });
                        });
                        success(users);
                    },
                    error: error
                });
            },
            getComments: function (success, error) {
                var id = $('#hidProductId').val();
                var currentUserId = $('#hidCurrentUserId').val();
                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    contentType: 'application/json',
                    data: { id },
                            url: '/Product/GetAll',
                    success: function (commentsArray) {
                        var comments = [];
                        if (commentsArray.status == true) {
                            $.each(commentsArray.data, function (i, item) {
                                var itemId = item.ID;
                                comments.push({
                                    id: item.ID,
                                    created: comment.dateFormatJson(item.CreateDate),
                                    modified: comment.dateFormatJson(item.ModifiedDate),
                                    content: item.Content,
                                    fullname: item.AppUser.FullName,
                                    parent: item.ParentId,
                                    pings: [],
                                    creator: item.UserId,
                                    profile_picture_url: comment.getUserAvatar(item.AppUser.Avatar),
                                    created_by_admin: item.AppUser.UserName == 'administrator',
                                    created_by_current_user: item.UserId == currentUserId,
                                    upvote_count: item.CommentVotes.length,
                                    user_has_upvoted: item.CommentVotes.filter(function (obj) {
                                        return obj.UserId == currentUserId;
                                    }).length > 0

                                });
                            });
                        }

                        success(comments);
                    },
                    error: error
                });
            },
            postComment: function (commentJSON, success, error) {
                $.ajax({
                    url: '/Product/Insert/',
                    type: 'post',
                    dataType:'json',
                    data: {
                        content: commentJSON.content,
                        productId: $('#hidProductId').val(),
                        parentId: commentJSON.parent
                    },
                    success: function(commentObj){
                        var commentVm = commentObj.data;
                        var obj = {
                            id: commentVm.ID,
                            created: comment.dateFormatJson(commentVm.CreateDate),
                            modified: comment.dateFormatJson(commentVm.ModifiedDate),
                            content: commentVm.Content,
                            fullname: commentVm.AppUser.FullName,
                            parent: commentVm.ParentId,
                            pings: [],
                            creator: commentVm.UserId,
                            profile_picture_url: comment.getUserAvatar(commentVm.AppUser.Avatar),
                            created_by_admin: commentVm.AppUser.UserName == 'administrator',
                            created_by_current_user: commentVm.UserId == $('#hidCurrentUserId').val(),
                            upvote_count: 0,
                            user_has_upvoted: false

                        }
                        success(obj);
                    },
                    error:error
                });
            },
            putComment: function (data, success, error) {
                $.ajax({
                    url: '/Product/Update/',
                    type: 'post',               
                    data: {
                        id: data.id,
                        content: data.content
                    },
                    success: function(commentObj){
                            var commentVm = commentObj.data;
                            var obj = {
                                id: commentVm.ID,
                                created: comment.dateFormatJson(commentVm.CreateDate),
                                modified: comment.dateFormatJson(commentVm.ModifiedDate),
                                content: commentVm.Content,
                                fullname: commentVm.AppUser.FullName,
                                parent: commentVm.ParentId,
                                pings: [],
                                creator: commentVm.UserId,
                                profile_picture_url: comment.getUserAvatar(commentVm.AppUser.Avatar),
                                created_by_admin: commentVm.AppUser.UserName == 'administrator',
                                created_by_current_user: commentVm.UserId == $('#hidCurrentUserId').val(),
                                upvote_count: 0,
                                user_has_upvoted: false

                            }
                            success(obj);
                    },
                    error: error
                });
            },
            deleteComment: function (data, success, error) {
                var id = data.id;
                $.ajax({
                    url: '/Product/Delete',
                    type: 'post',                   
                    data: { id: id },
                    success: function () {
                        success();
                    },
                    error: error
                });
                
            },
            upvoteComment: function (commentJSON, success, error) {
                var currentUserId = $('#hidCurrentUserId').val();
                    $.ajax({
                        type: 'post',
                        url: '/Product/Vote',
                        data: {
                            currentUserId,
                            commentId: commentJSON.id
                        },
                        success: function () {
                            success(commentJSON)
                        },
                        error: error
                    }); 
            },
            timeFormatter: function (time) {
                return moment(time).locale('vi').fromNow();
            },
        });
    },
    getUserAvatar: function (avatar) {
        if (avatar == null || avatar == undefined || avatar == '') {
            return "/Assets/images/user-icon.png";
        }
        else
            return avatar;
    },
    dateFormatJson: function (datetime) {
        if (datetime == null || datetime == '')
            return '';
        var newdate = new Date(parseInt(datetime.substr(6)));
        var month = newdate.getMonth() + 1;
        var day = newdate.getDate();
        var year = newdate.getFullYear();
        var hh = newdate.getHours();
        var mm = newdate.getMinutes();
        if (month < 10)
            month = "0" + month;
        if (day < 10)
            day = "0" + day;
        if (hh < 10)
            hh = "0" + hh;
        if (mm < 10)
            mm = "0" + mm;
        return month + "/" + day + "/" + year + " " + hh + ":" + mm;
    }
}
comment.init();