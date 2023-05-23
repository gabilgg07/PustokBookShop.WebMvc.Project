$(document).ready(function () {
    //#region Modal
    $(document).on("click", ".show-book-modal", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");

        async function ResponseHtml() {
            const response = await fetch(url)
                .then(resp => resp.text())
                .then(data =>

                    $("#quickModal .product-details-modal").html(data)
                    );
        }

        ResponseHtml();

        $("#quickModal").modal("show");
        

    });

    //#endregion 


    //#region Basket

    $(document).on("click", ".add-basket", function (e) {
        e.preventDefault();
        
        var url = $(this).attr("href");

        console.log(url);

        async function ResponseHtml() {
            const response = await fetch(url)
                .then(resp => resp.text())
                .then(data =>

                    $(".cart-widget .cart-block").html(data)
                );
        }

        ResponseHtml();


    });
    

    $(document).on("click", ".cross-btn", function (e) {
        e.preventDefault();

        var url = $(this).attr("href");

        console.log(url);

        async function ResponseHtml() {
            const response = await fetch(url)
                .then(resp => resp.text())
                .then(data =>

                    $(".cart-widget .cart-block").html(data)
                );
        }

        ResponseHtml();
    });

    //#endregion


    //#region Show Books by Category

    $(document).on("click", "[data-target='grid']", function (e) {
        $(".first-toolbar .sorting-btn").each(function (e) {
            if ($(this).attr("data-target") == "grid") {
                console.log("no list");
                console.log($(this));
                $(".product-grid-content").css("display", "block");
            } 
        });
    });

    $(document).on("click", "[data-target='grid-four']", function (e) {

        $(".first-toolbar .sorting-btn").each(function (e) {
            
            if ($(this).attr("data-target") == "grid-four") {
                $(".product-grid-content").css({
                    "height": "500px",
                    "display": "flex",
                    "flex-direction": "column",
                    "justify-content": "space-between"
                });
            }

        });
    });

    $(document).on("click", "[data-target='list']", function (e) {

        $(".first-toolbar .sorting-btn").each(function (e) {
            if ($(this).attr("data-target") != "list") {
                $(".product-grid-content").css("display","none");
            }
        });
    });

    //#endregion


    //#region Loading Comments

    $(document).on("click", "#loadcomment", function (e) {
        e.preventDefault();

        var nextPage = $(this).attr("data-nextPage");
        var url = $(this).attr("href") + "&page=" + nextPage;
        console.log(url);
        fetch(url)
            .then(response => response.text())
            .then(data => {
                $("#comments").html(data)
            });

        nextPage = +nextPage + 1;
        var maxPage = $(this).attr("data-maxPage");

        if (nextPage > maxPage) {
            $(this).remove()
        }
        else {
            $(this).attr("data-nextPage", nextPage)
        }
    })

    //#endregion


    // initialize with defaults

    //$("#input-id").rating({ min: 0, max: 5, step: 1, size: 'md' });

    toastr.options = {
        "closeButton": false,
        "debug": false,
        "newestOnTop": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "5000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }

    if ($("#toast-message").length) {
        if ($("#toast-message").attr("data-succeded") == "true") {
            toastr["success"]($("#toast-message").attr("data-text"))
        }
        else {
            toastr["error"]($("#toast-message").attr("data-text"))
        }
    }

})

