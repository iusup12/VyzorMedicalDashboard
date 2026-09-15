$(document).ready(function () {

    /*
     * ==========================================
     * ION RANGE SLIDER
     * ==========================================
     */

    /*
     * RANGE 01
     */
    $("#range_01").ionRangeSlider();


    /*
     * RANGE 02
     */
    $("#range_02").ionRangeSlider({
        min: 100,
        max: 1000,
        from: 550
    });


    /*
     * RANGE 03
     * DOCTORS PRICE FILTER
     *
     * Используется:
     * MinPrice
     * MaxPrice
     */
    const priceSlider = $("#range_03");

    if (priceSlider.length) {

        const minPriceInput = $("#minPrice");
        const maxPriceInput = $("#maxPrice");

        const minPrice =
            minPriceInput.val() !== ""
                ? Number(minPriceInput.val())
                : 0;

        const maxPrice =
            maxPriceInput.val() !== ""
                ? Number(maxPriceInput.val())
                : 5000;

        priceSlider.ionRangeSlider({

            type: "double",

            grid: true,

            min: 0,

            max: 5000,

            from: minPrice,

            to: maxPrice,

            prefix: "$",

            onStart: function (data) {

                updatePriceValues(data);

            },

            onChange: function (data) {

                updatePriceValues(data);

            }

        });

    }


    /*
     * RANGE 04
     */
    $("#range_04").ionRangeSlider({
        type: "double",
        grid: true,
        min: -1000,
        max: 1000,
        from: -500,
        to: 500
    });


    /*
     * RANGE 05
     */
    $("#range_05").ionRangeSlider({
        type: "double",
        grid: true,
        min: -1000,
        max: 1000,
        from: -500,
        to: 500,
        step: 250
    });


    /*
     * RANGE 06
     */
    $("#range_06").ionRangeSlider({
        grid: true,
        from: 3,
        values: [
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December"
        ]
    });


    /*
     * RANGE 07
     */
    $("#range_07").ionRangeSlider({
        grid: true,
        min: 1000,
        max: 1000000,
        from: 200000,
        step: 1000,
        prettify_enabled: true
    });


    /*
     * RANGE 08
     */
    $("#range_08").ionRangeSlider({
        min: 100,
        max: 1000,
        from: 550,
        disable: true
    });


    /*
     * RANGE 09
     */
    $("#range_09").ionRangeSlider({
        grid: true,
        min: 18,
        max: 70,
        from: 30,
        prefix: "Age ",
        max_postfix: "+"
    });


    /*
     * RANGE 10
     */
    $("#range_10").ionRangeSlider({
        type: "double",
        min: 100,
        max: 200,
        from: 145,
        to: 155,
        prefix: "Weight: ",
        postfix: " million pounds",
        decorate_both: true
    });


    /*
     * RANGE 11
     */
    $("#range_11").ionRangeSlider({
        type: "single",
        grid: true,
        min: -90,
        max: 90,
        from: 0,
        postfix: "°"
    });


    /*
     * RANGE 12
     */
    $("#range_12").ionRangeSlider({
        type: "double",
        min: 1000,
        max: 2000,
        from: 1200,
        to: 1800,
        hide_min_max: true,
        hide_from_to: true,
        grid: true
    });


    /*
     * RANGE 13
     */
    $("#range_13").ionRangeSlider({
        skin: "modern"
    });


    /*
     * RANGE 14
     */
    $("#range_14").ionRangeSlider({
        skin: "sharp"
    });


    /*
     * RANGE 15
     */
    $("#range_15").ionRangeSlider({
        skin: "round"
    });


    /*
     * RANGE 16
     */
    $("#range_16").ionRangeSlider({
        skin: "square"
    });


    /*
     * ==========================================
     * PRICE HELPERS
     * ==========================================
     */

    function updatePriceValues(data) {

        const minPrice = data.from;
        const maxPrice = data.to;


        /*
         * Записываем значения
         * в hidden input
         */
        $("#minPrice").val(minPrice);

        $("#maxPrice").val(maxPrice);


        /*
         * Обновляем текст
         * под slider
         */
        $("#priceMinLabel").text(minPrice);

        $("#priceMaxLabel").text(maxPrice);

    }

});