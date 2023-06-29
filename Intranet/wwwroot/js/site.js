// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $('.tabs').tabs();
    $('.modal').modal();
    $('select').formSelect();
    $('input#input_text, textarea#textarea2').characterCounter();

    $(".datepicker").datepicker({
        dateFormat: "yy-mm-dd",
        altFormat: "yy-mm-dd'T'HH:mm:ss"
    });

});