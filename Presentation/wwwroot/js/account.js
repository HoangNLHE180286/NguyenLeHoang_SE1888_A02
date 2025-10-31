const MODAL_TYPES_CONST = {
    create: {
        name: "Create",
        title: "Create A New Account",
        sumbmitText: "Create"
    },
    update: {
        name: "Update",
        title: "Edit Account Information",
        sumbmitText: "Save"
    },
    delete: {
        name: "Delete",
        title: "Delete Account",
        sumbmitText: "OK"
    },
    updatePassword: {
        name: "UpdatePassword",
        title: "Update Password",
        sumbmitText: "Save"
    },
}

$(document).ready(function () {
    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
        }
    });

    // Search Accounts
    $('#search-form').on('submit', function (e) {
        e.preventDefault();

        const form = $(this);
        const query = form.serialize();

        const url = '/Account/Index?' + query;
        history.pushState(null, null, url);

        const container = $('#account-table-container');
        container.html('<div class="text-center p-4"><div class="spinner-border text-primary" role="status"></div><p class="mt-2">Loading...</p></div>');

        $.ajax({
            url: '/Account/Index?handler=SearchAccount&' + query,
            type: 'GET',
            data: query,
            success: function (html) {
                container.html(html);
            },
            error: function () {
                container.html('<div class="alert alert-danger">Failed to load account list.</div>');
            }
        });
    });

    $('#search-form').trigger('submit');

    $('#reset-btn').on('click', function (e) {
        e.preventDefault();
        $('#search-form')[0].reset();
        $('#search-form').trigger('submit');
    });

    // Open Modal
    $('.modal').on('show.bs.modal', function (event) {
        const button = $(event.relatedTarget);
        const modalType = button.data('action-type');
        const accountId = button.data('id');
        const modal = Object.values(MODAL_TYPES_CONST).find(modal => modal.name === modalType);
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
            case MODAL_TYPES_CONST.updatePassword.name:
            case MODAL_TYPES_CONST.delete.name:
                btnSubmit.data('id', accountId);
                break;
        }

        $.ajax({
            url: '/Account/Index?handler=OpenModal',
            type: 'GET',
            data: { actionType: modalType, accountId: accountId },
            success: function (html) {
                $('#modal .modal-body').html(html);

                $.validator.unobtrusive.parse('#account-form');

                $.ajaxSetup({
                    headers: {
                        'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
                    }
                });
            }
        });
    });


    // Submit Modal
    $('#account-form').on('submit', function (e) {
        e.preventDefault();
        const btnSubmit = $(this).closest('.modal').find('.btn-submit');
        const modalType = btnSubmit.data('action-type');
        const accountId = btnSubmit.data('id');

        switch (modalType) {
            case MODAL_TYPES_CONST.create.name:
                const isPassValid = validateNewConfirm('#password', '#confirm-password', '.err-pass-msg', '.err-confirm-msg');
                if (!$(this).valid() || !isPassValid) {
                    return;
                }

                const createParams = $(this).serializeArray();
                createParams.push({ name: "actionType", value: modalType });

                $.ajax({
                    url: '/Account/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: createParams,
                    success: function () {

                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('An account has been created successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#account-form'));
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
                updateParams.push({ name: "Id", value: accountId }, { name: "actionType", value: modalType });

                $.ajax({
                    url: '/Account/Index?handler=CreateUpdate',
                    type: 'POST',
                    data: updateParams,
                    success: function () {
                       
                        $('#search-form').trigger('submit');
                        $('#modal').modal('hide');
                        toastr.success('Account saved successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#account-form'));
                        } else {
                            toastr.error('Something went wrong!');
                        }
                    }
                });

                break;
            case MODAL_TYPES_CONST.updatePassword.name:
                if (!$(this).valid()) {
                    return;
                }

                const updatePassParams = $(this).serializeArray();
                updatePassParams.push({ name: "Id", value: accountId });

                $.ajax({
                    url: '/Account/Index?handler=UpdatePassword',
                    type: 'POST',
                    data: updatePassParams,
                    success: function () {

                        $('#modal').modal('hide');
                        toastr.success('Password saved successfully!');
                    },
                    error: function (xhr) {
                        if (xhr.status === 400) {
                            $('#modal .modal-body').html(xhr.responseText);
                            $.validator.unobtrusive.parse($('#account-form'));
                        } else {
                            toastr.error('Something went wrong!');
                        }
                    }
                });
                break;
            case MODAL_TYPES_CONST.delete.name:
                $.ajax({
                    url: '/Account/Index?handler=Delete',
                    type: 'POST',
                    data: { accountId },
                    success: function (response) {
                        if (response.success) {
                            $('#search-form').trigger('submit');
                            $('#modal').modal('hide');
                            toastr.success('An account has been deleted successfully!');
                        } else {
                            toastr.error(response.message);
                        }
                    },
                    error: function () {
                        toastr.error('Unable to delete this account. Please try again!');
                    }
                });
                break;
        }
    });


});

function validateNewConfirm(newSelector, confirmSelector, errorNewSelector, errorConfirmSelector) {
    const newPass = $(newSelector).val().trim();
    const confirmPass = $(confirmSelector).val().trim();
    const $errorNew = $(errorNewSelector);
    const $errorConfirm = $(errorConfirmSelector);
    let isValid = true;

    $errorNew.text('');
    $errorConfirm.text('');

    if (!newPass) {
        $errorNew.text('Password is required!');
        isValid = false;
    } else if (newPass.length < 2) {
        $errorNew.text('Password must be at least 2 characters long!');
        isValid = false;
    }

    if (!confirmPass) {
        $errorConfirm.text('Password is required!');
        isValid = false;
    } else if (confirmPass.length < 2) {
        $errorConfirm.text('Password must be at least 2 characters long!');
        isValid = false;
    } else if (newPass !== confirmPass) {
        $errorConfirm.text('Confirm password does not match!');
        isValid = false;
    }

    if (!isValid) {
        return false;
    }
    return true;
}