$(document).ready(function () {

    getUserInfo();
    checkPromotion();


    $('form#registerForm input[name="username"]').on('keyup', function (e) {
        $(this).removeClass('is-invalid');
        let label = $(this).parent().find('#basic-addon1');
        let validationFeedback = $(this).parent().find('#validationUsernameFeedback');
        $('form#registerForm button[type="submit"]').attr('disabled', true);
        $('form#registerForm input[name="password"]').attr('disabled', true);

        $(validationFeedback).removeClass('invalid-feedback');
        $(validationFeedback).removeClass('valid-feedback');
        $(validationFeedback).empty();

        let username = $(this).val();
        if (username.length > 3) {
            if (!validateEmail(username)) {
                $(this).addClass('is-invalid');
                $(validationFeedback).addClass('invalid-feedback').text('Podana wartość posiada niepoprawy format');
                return;
            }
            else {

                let formData = new FormData();
                formData.append("username", username);
                $.ajax(
                    {
                        url: "/Account/CheckUsername",
                        type: "POST",
                        data: formData,
                        processData: false,
                        contentType: false,
                        beforeSend: function () {
                            $('form#registerForm input[name="username"]').attr('disabled', true);
                            $(label).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span><span class="visually-hidden">Loading...</span>')
                        },
                        success: function (response) {
                            $('form#registerForm input[name="username"]').addClass('is-valid');
                            $(validationFeedback).addClass('valid-feedback').text('Podana wartość jest prawidłowa');
                            $('form#registerForm input[name="username"]').attr('disabled', false);
                            $('form#registerForm input[name="password"]').attr('disabled', false);
                            $(label).html('Adres e-mail');
                            return;
                        },
                        error: function (response) {
                            $('form#registerForm input[name="username"]').attr('disabled', false);
                            $('form#registerForm input[name="username"]').addClass('is-invalid');
                            $(validationFeedback).addClass('invalid-feedback').text(response.responseJSON.message);
                            $(label).html('Adres e-mail');
                            return;
                        }
                    }
                )

                
            }
        }
    });

    $('form#registerForm input[name="password"]').on('keyup', function (e) {
        $(this).removeClass('is-invalid');
        let validationFeedback = $(this).parent().find('#validationPasswordFeedback');

        $(validationFeedback).removeClass('invalid-feedback');
        $(validationFeedback).removeClass('valid-feedback');
        $(validationFeedback).empty();

        let password = $(this).val();

        if (password.length < 8) {
            $(this).addClass('is-invalid');
            $(validationFeedback).addClass('invalid-feedback').text('Hasło powinno mieć co najmniej 8 znaków');
            $('form#registerForm button[type="submit"]').attr('disabled', true);
            return;
        }

        // Sprawdzenie zawartości hasła (przykładowo: musi zawierać małe i duże litery oraz cyfry)
        let lowercaseRegex = /[a-z]/;
        let uppercaseRegex = /[A-Z]/;
        let digitRegex = /\d/;

        if (!lowercaseRegex.test(password) || !uppercaseRegex.test(password) || !digitRegex.test(password)) {
            $(this).addClass('is-invalid');
            $(validationFeedback).addClass('invalid-feedback').text('Hasło powinno zawierać małe i duże litery oraz cyfry');
            $('form#registerForm button[type="submit"]').attr('disabled', true);
            return;
        }

  
        $(this).addClass('is-valid');
        $(validationFeedback).addClass('valid-feedback').text('Hasło jest bezpieczne');

        $('form#registerForm button[type="submit"]').attr('disabled', false);
        
    });

    $('form#registerForm').on('submit', function (e) {
        e.preventDefault();

        const chartModal = new bootstrap.Modal(document.getElementById('chartModal'));
        chartModal.show();

        let formData = new FormData();
        formData.append('username', $('form#registerForm input[name="username"]').val());
        formData.append('password', $('form#registerForm input[name="password"]').val());
        $.ajax(
            {
                url: "/Account/Store",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('form#registerForm input').attr('disabled', true);
                    $('form#registerForm button[type="submit"]').attr('disabled', true);
                    $('#chartModal .modal-title').text('Rejestracja konta użytkownika');
                    $('#chartModal .modal-body').empty().html('<div class="spinner-border m-3" role="status"><span class="visually-hidden">Loading...</span></div>');
                },
                success: function (response) {
                    window.location.href = "/Account/Panel";
                    return;
                },
                error: function (response) {
                    $('#chartModal .modal-body').empty().html(response.responseJSON.message);
                },
            }
        )
        
    });

    $('form#loginForm input[name="username"]').on('keyup', function (e) {
        $(this).removeClass('is-invalid');
        let validationFeedback = $(this).parent().find('#validationUsernameFeedback');
        $('form#loginForm button[type="submit"]').attr('disabled', true);
        $('form#loginForm input[name="password"]').attr('disabled', true);

        $(validationFeedback).removeClass('invalid-feedback');
        $(validationFeedback).removeClass('valid-feedback');
        $(validationFeedback).empty();

        let username = $(this).val();
        if (username.length > 3) {
            if (!validateEmail(username)) {
                $(this).addClass('is-invalid');
                $(validationFeedback).addClass('invalid-feedback').text('Podana wartość posiada niepoprawy format');
                return;
            }
            else {
                //$(this).addClass('is-valid').attr('disabled', true);
                $(validationFeedback).addClass('valid-feedback').text('Podana adres e-mail jest poprawny');
                $('form#loginForm input[name="password"]').attr('disabled', false);
                $('form#loginForm button[type="submit"]').attr('disabled', false);
                return;
            }
        }
    });

    $('form#loginForm').on('submit', function (e) {
        e.preventDefault();

        const chartModal = new bootstrap.Modal(document.getElementById('chartModal'));
        chartModal.show();

        let formData = new FormData();
        formData.append('username', $('form#loginForm input[name="username"]').val());
        formData.append('password', $('form#loginForm input[name="password"]').val());
        $.ajax(
            {
                url: "/Account/Signin",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('form#loginForm input').attr('disabled', true);
                    $('form#loginForm button[type="submit"]').attr('disabled', true);
                    $('#chartModal .modal-title').text('Autoryzacja użytkownika');
                    $('#chartModal .modal-body').empty().html('<div class="spinner-border m-3" role="status"><span class="visually-hidden">Loading...</span></div>');
                },
                success: function (response) {
                    window.location.href = "/Account/Panel";
                    return;
                },
                error: function (response) {
                    $('#chartModal .modal-body').empty().html(response.responseJSON.message);
                    $('form#loginForm')[0].reset();
                    $('form#loginForm input[name="username"]').attr('disabled', false);

                    $('form#loginForm input[name="username"]').removeClass('is-valid');
                    let validationFeedback = $('form#loginForm input[name="username"]').parent().find('#validationUsernameFeedback');
                    $(validationFeedback).removeClass('invalid-feedback');
                    $(validationFeedback).removeClass('valid-feedback');
                    $(validationFeedback).empty();
                },
            }
        )
    })

    $('form.addToCart').on('submit', function (e) {
        e.preventDefault();

        const chartModal = new bootstrap.Modal(document.getElementById('chartModal'));
        chartModal.show();

        $('#chartModal .modal-body').empty();

        let input = $(this).find('input[name="product"]');
        if (input !== undefined) {
            let formData = new FormData();
            formData.append("product", input.val());

            $.ajax(
                {
                    url: "/Ecommerce/AddToCart",
                    type: "POST",
                    data: formData,
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        $('#chartModal .modal-title').text('Dodaje produkt do koszyka');
                        $('#chartModal .modal-body').html('<div class="spinner-border m-3" role="status"><span class="visually-hidden">Loading...</span></div>');
                    },
                    success: function (response) {
                        $('#chartModal .modal-title').text('Gratulacje!');
                        $('#chartModal .modal-body').html(response.message);

                        let cartCounterPill = $('#cartButton').find('#cartCounterPill');
                        if (cartCounterPill.length > 0) {
                            let counterValue = parseInt($('#cartCounterPill span.cartItemsCounter').text());
                            counterValue += 1;
                            $('#cartCounterPill span.cartItemsCounter').text(counterValue);
                        }
                        else {
                            $('#cartButton').prepend('<div id="cartCounterPill" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-secondary"><span class="cartItemsCounter">1</span><span class="visually-hidden">Produkty w koszyku</span></div>');
                        }

                    },
                    error: function (response) {
                        $('#chartModal .modal-title').text('Wystąpił problem');
                        if (response.status == 400) {
                            $('#chartModal .modal-body').html('<p>Niestety ale wystąpił problem z dodaniem tego produktu do koszyka.<br/>Spróbuj ponownie później.</p>')
                        }
                        else if (response.status == 403) {
                            $('#chartModal .modal-body').html(response.responseJSON.message);
                        }
                    },
                }
            )
        }


    });

    if ($('.cart-content').length) {

            var sum = 0.0;
            let items = $('.cart-content').find('.price');
            $.each(items, function (i, v) {
                var price = parseFloat($(v).data('price').toString().replace(",", "."));
                sum += price;
            });

        $('#cartSummary').text(sum.toFixed(2))
    }

    $('#storeOrder').on('click', function (e) {
        const chartModal = new bootstrap.Modal(document.getElementById('chartModal'));
        chartModal.show();

        $.ajax(
            {
                url: "/Account/CheckLoginAccount",
                type: "GET",
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('#storeOrder').attr('disabled', true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Trwa pobieranie danych')
                },
                success: function (response) {
                    makeUserOrder();
                    $('#storeOrder').attr('disabled', false).html('Finalizuj zamówienie')
                },
                error: function (response) {
                    if (response.status == 403) {
                        $('#chartModal .modal-title').text('Wymagana akcja użytkownika');
                        $('#chartModal .modal-body').empty().html('<p>Niestety ale finalizacja zamówienia dostępna jest jedynie dla zalogowanych użytkowników.</p><div class="btn-group" role="group" aria-label="Basic example"><a href="/Account/LogIn" type="button" class="btn btn-warning">Zaloguj się</a><a href="/Account/Register" type="button" class="btn btn-outline-warning">Utwórz konto</a></div>')
                    }
                    $('#storeOrder').attr('disabled', false).html('Finalizuj zamówienie')
                }
            }
        )
    });

    if ($('.userPanel').length) {
        getUserOrders();

        $('.ordersList').on('click', 'button.orderDetails', function () {
            var orderId = $(this).data('id');
            if (orderId !== undefined) {

                const chartModal = new bootstrap.Modal(document.getElementById('chartModal'));
                chartModal.show();

                $.ajax(
                    {
                        url: "/Account/GetOrderDetails?id=" + orderId,
                        type: "GET",
                        processData: false,
                        contentType: false,
                        beforeSend: function () {
                            $('#chartModal .modal-title').text('Trwa pobieranie danych');
                            $('#chartModal .modal-body').html('<div class="spinner-border m-3" role="status"><span class="visually-hidden">Loading...</span></div>');
                        },
                        success: function (response) {
                            $('#chartModal .modal-title').text('Lista zamówionych kursów (' + Object.keys(response).length +')');
                            $('#chartModal .modal-body').empty();
                            if (response.length > 0 && Object.keys(response).length > 0) {
                                $.each(response, function (index, value) {
                                    $('#chartModal .modal-body').append('<div class="row mb-3"><div class="col-8"><strong>' + value.productTitle + '</strong></div><div class="col-4">' + value.productPrice.toFixed(2) + ' PLN</div></div>')
                                });
                            }
                            else {
                                $('#chartModal .modal-body').html('<p>Brak treści do wyświetlenia</p>');
                            }
                        },
                        error: function (response) {}
                    }
                )

            }
            
        });

    }

    $('#searchInput').on('keyup', function (e) {
        $(".searchResults").empty();
        let query = e.target.value;
        if (query.length > 2) {
            $.ajax(
                {
                    url: "/Search/GetSearchTitle?id=" + query,
                    type: "GET",
                    processData: false,
                    contentType: false,
                    beforeSend: function () {
                        $(".searchResults").html('<div class="d-flex justify-content-center"><div class="spinner-border m-3" role="status"></div></div>')
                    },
                    success: function (response) {
                        $(".searchResults").html('<ul class="list-unstyled"></ul>');
                        if (response.length > 0 && Object.keys(response).length > 0) {


                            var filteredData = Object.values(response).filter(function (item) {
                                return item.value.toLowerCase().includes(query.toLowerCase());
                            });

  
                            $.each(filteredData, function (index, value) {
                       
                                $('.searchResults ul').append('<li>'+value.value+'</li>');
                            });
                        }
                    },
                    error: function (response) { }
                }
            )
        }
    });

    $('body').on('click', '.searchResults li', function (e) {
        var value = $(this).text();
        if (value !== undefined) {
            window.location.href = "/Search?query=" + value;
        }
        $(".searchResults").empty();
    })

    const targetDate = new Date($(".promotionInfo").data('end'));
    function countdown() {
    
        const currentDate = new Date();
        const remainingTime = targetDate.getTime() - currentDate.getTime();

        const days = Math.floor(remainingTime / (1000 * 60 * 60 * 24));
        const hours = Math.floor((remainingTime % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const minutes = Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60));
        const seconds = Math.floor((remainingTime % (1000 * 60)) / 1000);

        $(".promotionInfo span.counter").html(`${days} dni, ${hours} godzin, ${minutes} minut, ${seconds} sekund`);

        if (remainingTime <= 0) {
            console.log('Odliczanie zakończone!');
           
            return;
        }
        setTimeout(countdown, 1000);
    }

    countdown();

    function getUserInfo() {
        $.ajax(
            {
                url: "/Account/CheckLoginAccount",
                type: "GET",
                processData: false,
                contentType: false,
                beforeSend: function () { 
                    $("#userMenu").html('<button class="btn btn-outline-info" type="button" disabled><span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span><span class="visually-hidden">Loading...</span>  </button>');
                },
                success: function (response) {
                    $("#userMenu").html('<div class="dropdown"><button class="btn btn-outline-info dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false" type="button">' + response.email + '</button><ul class="dropdown-menu"> <li><a class="dropdown-item" href="/Account/Panel">Moje konto</a></li> <li><a class="dropdown-item" href="/Account/Logout">Wyloguj</a></li></ul></div>')
                },
                error: function (response) {
                    if (response.status == 403) {
                        $("#userMenu").html('<a href="/Account/LogIn" type="button" class="btn btn-outline-danger">Zaloguj się lub utwórz konto</a>');
                    }
                }
            }
        )
    }

    function validateEmail(email) {
        let emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailPattern.test(email);
    }

    function makeUserOrder() {
        $.ajax(
            {
                url: "/Ecommerce/MakeOrder",
                type: "POST",
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('#chartModal .modal-title').text('Trwa zapis Twojego zamówenia');
                    $('#chartModal .modal-body').empty().html('<div class="d-flex justify-content-center"><div class="spinner-border" role="status"><span class="visually-hidden">Loading...</span></div></div>')
                },
                success: function (response) {
                    window.location.href = "/Account/Panel";
                },
                error: function (response) {
                    $('#chartModal .modal-title').text('Niestety, wystąpił błąd');
                    if (response.status == 403) {
                        $('#chartModal .modal-body').empty().html('<p>' + response.responseJSON.message +'</p>');
                    }
                }
            }
        )
    }

    function getUserOrders() {
        $.ajax(
            {
                url: "/Account/GetAccountOrders",
                type: "GET",
                processData: false,
                contentType: false,
                beforeSend: function () {
                    $('.ordersList').empty().html('<div class="d-flex justify-content-center"><div class="spinner-border m-5" role="status"><span class="visually-hidden">Loading...</span></div></div>')
                },
                success: function (response) {
                    $('.ordersList').empty();
                    if (response.length > 0 && Object.keys(response).length > 0) {
                        $.each(response, function (index, value) {
                            $('.ordersList').append('<div class="row mb-3"><div class="col-5"><strong>' + value.orderId + '</strong></div><div class="col-2">' + value.orderSum.toFixed(2) + ' PLN</div><div class="col-3">' + value.orderCdate + '</div><div class="col-2"><button data-id="' + value.orderId +'" type="button" class="btn btn-primary orderDetails">Szczegóły</button></div></div>')
                        });
                    }
                    else {
                        $('.ordersList').html('<p>Brak treści do wyświetlenia</p>');
                    }
                },
                error: function (response) {}
            }
        )
    }

    function checkPromotion() {
        if ($('#promoBox').length > 0) {

            let promoValue = $('#promoBox').data('value');

            var isPromoted = $('body').find('.isPromoted');
            $.each(isPromoted, function (index, value) {
                var value = $(value).find('.card-body');
                var item = $(value).children(2).children(2);
                var price = $(item).first().children().first().text();
                price = parseFloat(price.toString().replace(",", "."));
                var discountedPrice = price * (100 - promoValue) / 100;
                $(item).first().children().first().text(discountedPrice.toFixed(2));
                $(item).parent().prepend('<span class="origPrice">' + price.toFixed(2) + ' zł</span>')
            })

            var isPromotedRow = $('body').find('.isPromotedRow');
            $.each(isPromotedRow, function (index, value) {
                var value = $(value).find('.card-body');
                var item = $(value).children(2).children(2);
                var price = $(item).children(2).first().text();

                price = parseFloat(price.toString().replace(",", "."));
                var discountedPrice = price * (100 - promoValue) / 100;
                // console.log($(item).children(2).first());
                $(item).children(2).first().text(discountedPrice.toFixed(2));
                //$(item).children(2).prepend('<span class="origPrice">' + price.toFixed(2) + ' zł</span>')
            })

            var productPrice = $('body').find('.productPrice');
            if (productPrice !== undefined) {
                var price = $(productPrice).find('.promoValue').text();
                price = parseFloat(price.toString().replace(",", "."));
                var discountedPrice = price * (100 - promoValue) / 100;
                $(productPrice).find('.promoValue').text(discountedPrice.toFixed(2))
                $(productPrice).append('<p class="origPrice"> Stara cena: <strong>' + price.toFixed(2) + ' zł</strong></p>');
            }
        }
        else {
            $('.isPromotedRow strong').css({ color: '#000' });
            $('.isPromoted strong').css({ color: '#000' });
            $('.productPrice strong').css({ color: '#000' });
        }
    }


});