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




    $("#artikl-dodaj").click(function () {
        event.preventDefault();
        dodajZadatak();
    });
});

function dodajZadatak() {
    var sifra = $("#zadatak-sifra").val();

    console.log("jdhkjfhlkJH");
    if (sifra != '') {
        if ($("[name='Zadatci[" + sifra + "].IdZadatak'").length > 0) {
            alert('Zadatak je već u dokumentu');
            return;
        }

        var oib = $("#zadatak-oibnositelj").val();
        var idzahtjev = $("zadtak-zahtjev").val();
        var naziv = $("#zadatak-naziv").val();
        var status = $("#zadatak-status").val();
        var vrpoc = new Date($("#zadatak-pocetak").val());
        var vrkraj = new Date($("#zadatak-kraj").val());
        var vrockraj = new Date($("#zadatak-ockraj").val());


        var template = $('#template').html();

        //Alternativa ako su hr postavke sa zarezom //http://haacked.com/archive/2011/03/19/fixing-binding-to-decimals.aspx/
        //ili ovo http://intellitect.com/custom-model-binding-in-asp-net-core-1-0/

        template = template.replace(/--sifra--/g, sifra)
            .replace(/--status--/g, status)
            .replace(/--oib--/g, oib)
            .replace(/--zahtjev--/g, idzahtjev)
            .replace(/--naziv--/g, naziv)
            .replace(/--vrpoc--/g, vrpoc)
            .replace(/--vrkraj--/g, vrkraj)
            .replace(/--vrockraj--/g, vrockraj)

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