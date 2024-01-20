$(document).on('click', '.deleterow', function () {
    event.preventDefault();
    var tr = $(this).parents("tr");
    tr.remove();
    clearOldMessage();
});

$(function () {
    $(".form-control").bind('keydown', function (event) {
        if (event.which === 13) {
            event.preventDefault();
        }
    });




    $("#zadatak-dodaj").click(function () {
        event.preventDefault();
        dodajZadatak();
    });
});

function dodajZadatak() {
    //debugger;
    var sifra = $("#zadatak-sifra").val();

    if (sifra != '') {
        if ($("[name='Zadatci[" + sifra + "].IdZadatak'").length > 0) {
            alert('Zadatak je već u dokumentu');
            return;
        }

        var oib = $("#zadatak-oibnositelj").val();
        var idzahtjev = $("zadatak-zahtjev").val();
        var naziv = $("#zadatak-naziv").val();
        var status = $("#zadatak-status").val();
        var vrpoc = ($("#zadatak-pocetak").val());
        var vrkraj = ($("#zadatak-kraj").val());
        var vrockraj = ($("#zadatak-ockraj").val());
        var nazivstatusa = ($("#zadatak-status option:selected").text());
        var punioib = ($("#zadatak-oibnositelj option:selected").text());




        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        var newIndex = Date.now();

        template = template.replace(/--sifra--/g, sifra)
            .replace(/--status--/g, status)
            .replace(/--oib--/g, oib)
            .replace(/--zahtjev--/g, idzahtjev)
            .replace(/--naziv--/g, naziv)
            .replace(/--vrpoc--/g, vrpoc)
            .replace(/--vrkraj--/g, vrkraj)
            .replace(/--vrockraj--/g, vrockraj)
            .replace(/--nazivstatus--/g, nazivstatusa)
            .replace(/--punioib--/g, punioib)
                    

        $(template).find('.dynamic-oib').val(oib);

        $(template).find('tr').insertBefore($("#table-zadatci").find('tr').last());



        $("#zadatak-sifra").val('');
        $("#zadatak-status").val('');
        $("#zadatak-oibnositelj").val('');
        $("#zadatak-zahtjev").val('');
        $("#zadatak-naziv").val('');
        $("#zadatak-pocetak").val('');
        $("#zadatak-kraj").val('');
        $("#zadatak-ockraj").val('');


        clearOldMessage();
    }
}


