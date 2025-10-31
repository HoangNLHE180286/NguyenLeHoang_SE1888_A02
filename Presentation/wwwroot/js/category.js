const MODAL_TYPES_CONST = {
    create: {
        name: "Create",
        title: "Create A New Category",
        sumbmitText: "Create"
    },
    update: {
        name: "Update",
        title: "Edit Category Information",
        sumbmitText: "Save"
    },
    delete: {
        name: "Delete",
        title: "Delete Category",
        sumbmitText: "OK"
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

        const url = '/Category/Index?' + query;
        history.pushState(null, null, url);

        const container = $('#category-table-container');
        container.html('<div class="text-center p-4"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">Loading...</p></div>');

        $.ajax({
            url: '/Category/Index?handler=SearchCategory&' + query,
            type: 'GET',
            success: function (html) {
                container.html(html);
            },
            error: function () {
                container.html('<div class="alert alert-danger">Failed to load category list.</div>');
            }
        });
    });

    $('#search-form').trigger('submit');

    $('#reset-btn').on('click', function (e) {
        e.preventDefault();
        $('#search-form')[0].reset();
        $('#search-form').trigger('submit');
    });

    $('.modal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const modalType = button.data('action-type');
        const categoryId = button.data('id');
        const modal = Object.values(MODAL_TYPES_CONST).find(modal => modal.name == modalType);
        $(this).find('.modal-title').text(modal.title);

        const btnSubmit = $(this).find('.btn-submit');
        btnSubmit
            .text(modal.sumbmitText)
            .removeClass('btn-danger btn-primary')
            .addClass(modal.name === MODAL_TYPES_CONST.delete.name ? 'btn-danger' : 'btn-primary')
            .data('action-type', modalType)

        switch (modalType) {
            case MODAL_TYPES_CONST.create.name:
                break;
            case MODAL_TYPES_CONST.update.name:
            case MODAL_TYPES_CONST.delete.name:
                btnSubmit.data('id', categoryId);
                break;
        }

        $.ajax({
            url: '/Category/Index?handler=OpenModal',
            type: 'GET',
            data: { actionType: modalType, categoryId: categoryId },
            success: function (html) {
                $('#modal .modal-body').html(html);

                $.validator.unobtrusive.parse('#category-form');

                $.ajaxSetup({
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    }
                });
            }
        });
    });

    $('#category-form').on('submit', function (e) {
        e.preventDefault();
        const btnSubmit = $(this).closest('.modal').find('.btn-submit');
        const modalType = btnSubmit.data('action-type');
        const categoryId = btnSubmit.data('id');

        switch (modalType) {
            case MODAL_TYPES_CONST.create.name:
                if (!$(this).valid()) {
                    return;
                }

                const createParams = $(this).serializeArray();
                createParams.push({ name: "actionType", value: modalType });
                $.ajax({
                    url: '/Category/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: createParams,
                    success: function (response) {
                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('A category has been created successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#category-form'));
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
                updateParams.push({ name: "Id", value: categoryId }, { name: "actionType", value: modalType });

                $.ajax({
                    url: '/Category/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: updateParams,
                    success: function (response) {
                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('A category has been updated successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#category-form'));
                        } else {
                            toastr.error('Something went wrong!');
                        }
                    }
                });

                break;
            case MODAL_TYPES_CONST.delete.name:
                $.ajax({
                    url: '/Category/Index?handler=Delete',
                    type: 'POST',
                    data: { categoryId },
                    success: function (response) {
                        if (response.success) {
                            $('#search-form').trigger('submit');
                            $('#modal').modal('hide');
                            toastr.success('A category has been deleted successfully!');
                        } else {
                            toastr.error(response.message);
                        }
                    },
                    error: function (xhr) {
                        toastr.error('Unable to delete this category. Please try again!');
                    }
                });
                break;
        }
    });
});

$(document).on('change', '.form-check-input', function () {
    const categoryId = Number($(this).data('id'));
    const isActive = $(this).is(':checked');

    $(this).closest('tr').find('.form-check-label').text(isActive ? STATUS_CONST.active.name : STATUS_CONST.inactive.name);

    $.ajax({
        url: '/Category/Index?handler=UpdateStatus',
        type: 'POST',
        data: { categoryId: categoryId, isActive: isActive },
        success: function (response) {

            toastr.success('Category status has been updated successfully!');
        },
        error: function (xhr) {
            toastr.error(xhr.responseText || 'Something went wrong!');
        }
    });
});