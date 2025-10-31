const MODAL_TYPES_CONST = {
    create: {
        name: "Create",
        title: "Create A New Article",
        submitText: "Create"
    },
    update: {
        name: "Update",
        title: "Edit Article Information",
        submitText: "Save"
    },
    delete: {
        name: "Delete",
        title: "Delete Article",
        submitText: "OK"
    },
    duplicate: {
        name: "Duplicate",
        title: "Duplicate Article",
        submitText: "OK"
    }
}

const STATUS_CONST = {
    active: {
        name: "Active",
        value: 1
    },
    inactive: {
        name: "Inactive",
        value: 0
    }
}

$(document).ready(function () {
    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        }
    });

    $('#search-form').on('submit', function (e) {
        e.preventDefault();
        const form = $(this);
        const query = form.serialize();

        const url = '/Article/Index?' + query;
        history.pushState(null, null, url);

        const container = $('#article-table-container');
        container.html('<div class="text-center p-4"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">Loading...</p></div>');

        $.ajax({
            url: '/Article/Index?handler=SearchArticle&' + query,
            type: 'GET',
            success: function (html) {
                container.html(html);
            },
            error: function () {
                container.html('<div class="alert alert-danger">Failed to load article list.</div>');
            }
        });
    });

    $('#search-form').trigger('submit');

    $('#reset-btn').on('click', function (e) {
        e.preventDefault();
        $('#search-form')[0].reset();
        $('.category-select').val('').trigger('change');
        $('#search-form').trigger('submit');
    });

    $('.modal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const modalType = button.data('action-type');
        const articleId = button.data('id');
        const modal = Object.values(MODAL_TYPES_CONST).find(modal => modal.name == modalType);
        $(this).find('.modal-title').text(modal.title);

        const btnSubmit = $(this).find('.btn-submit');
        btnSubmit
            .text(modal.submitText)
            .removeClass('btn-danger btn-primary')
            .addClass(modal.name === MODAL_TYPES_CONST.delete.name ? 'btn-danger' : 'btn-primary')
            .data('action-type', modalType)

        switch (modalType) {
            case MODAL_TYPES_CONST.create.name:
                break;
            case MODAL_TYPES_CONST.update.name:
            case MODAL_TYPES_CONST.delete.name:
            case MODAL_TYPES_CONST.duplicate.name:
                btnSubmit.data('id', articleId);
                break;
        }

        $.ajax({
            url: '/Article/Index?handler=OpenModal',
            type: 'GET',
            data: { actionType: modalType, articleId: articleId},
            success: function (html) {
                $('#modal .modal-body').html(html);

                $.validator.unobtrusive.parse('#article-form');

                $.ajaxSetup({
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    }
                });

                if ($.fn.select2) {
                    $('#Category, #TagIds').select2({
                        placeholder: 'Select options',
                        width: '100%'
                    });
                }
            },
        });
    });

    $('#article-form').on('submit', function (e) {
        e.preventDefault();
        const btnSubmit = $(this).closest('.modal').find('.btn-submit');
        const modalType = btnSubmit.data('action-type');
        const articleId = btnSubmit.data('id');

        switch (modalType) {
            case MODAL_TYPES_CONST.create.name:
                if (!$(this).valid()) {
                    return;
                }

                const createParams = $(this).serializeArray();
                createParams.push({ name: "actionType", value: modalType });
                $.ajax({
                    url: '/Article/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: createParams,
                    success: function (response) {
                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('An article has been created successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#article-form'));
                        } else {
                            toastr.error('Something went wrong!');
                        }
                    }
                });

                break;
            case MODAL_TYPES_CONST.update.name:
                if (!$(this).valid()) {
                    return;
                }

                const updateParams = $(this).serializeArray();
                updateParams.push({ name: "Id", value: articleId }, { name: "actionType", value: modalType });

                $.ajax({
                    url: '/Article/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: updateParams,
                    success: function (response) {
                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('An article has been updated successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#article-form'));
                        } else {
                            toastr.error('Something went wrong!');
                        }
                    }
                });

                break;
            case MODAL_TYPES_CONST.delete.name:
                $.ajax({
                    url: '/Article/Index?handler=Delete',
                    type: 'POST',
                    data: { articleId },
                    success: function (response) {
                        if (response.success) {
                            $('#search-form').trigger('submit');
                            $('#modal').modal('hide');
                            toastr.success('An article has been deleted successfully!');
                        } else {
                            toastr.error(response.mesage);
                        }
                    },
                    error: function () {
                        toastr.error('Unable to delete this article. Please try again!');
                    }
                });
                break;
            case MODAL_TYPES_CONST.duplicate.name:
                $.ajax({
                    url: '/Article/Index?handler=Duplicate',
                    type: 'POST',
                    data: { articleId },
                    success: function (response) {
                        if (response.success) {
                            $('#search-form').trigger('submit');
                            $('#modal').modal('hide');
                            toastr.success('An article has been duplicated successfully!');
                        } else {
                            toastr.error(response.mesage);
                        }
                    },
                    error: function () {
                        toastr.error('Unable to duplicate this article. Please try again!');
                    }
                });
                break;
        }
    });
});

$(document).on('change', '.form-check-input', function () {
    const articleId = $(this).data('id'); // articleId là string
    const isActive = $(this).is(':checked');

    // Cập nhật label hiển thị
    $(this)
        .closest('tr')
        .find('.form-check-label')
        .text(isActive ? STATUS_CONST.active.name : STATUS_CONST.inactive.name);

    $.ajax({
        url: '/Article/Index?handler=UpdateStatus',
        type: 'POST',
        data: { articleId: articleId, isActive: isActive },
        success: function (response) {
            if (response.success) {
                toastr.success('Article status has been updated successfully!');
            } else {
                toastr.error(response.message || 'Failed to update article status!');
            }
        },
        error: function (xhr) {
            toastr.error(xhr.responseText || 'Something went wrong!');
        }
    });
});
