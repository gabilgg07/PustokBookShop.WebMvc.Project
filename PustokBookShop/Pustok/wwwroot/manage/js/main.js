
$(document).ready(function () {
    $(document).on("change", "#lblImageFile input", function (e) {
        if ($(this)[0].files) {
            let inputName;
            for (var i = 0; i < $(this)[0].files.length; i++) {
                inputName = $(this)[0].files[0].name;

                if (i==0) {
                    $(this).parent().children()[0].innerHTML = inputName;
                } else if (i<3) {
                    $(this).parent().children()[0].innerHTML += ", " + inputName;
                }
            }
            if ($(this)[0].files.length>3) {
                $(this).parent().children()[0].innerHTML += "... +" + ($(this)[0].files.length - 3);
            }
        }
    });

    $(document).on("click", "span#deleteImage", function (e) {
        console.log($("form.user .image-poster"));

        $(this).parent().parent().parent().remove();
    });

    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
});