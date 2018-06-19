
$(document).ready(function () {
    myPopOvers();
    $(".exampleModal").attr("display", "none");
});

/*Popovers for the input field on the Registration forms*/
function myPopOvers() {


    $(".reg-password")
        .popover({ trigger: 'focus', title: 'Jelszó', content: 'A jelszónak legalább 6 karakter hosszúnak kell lennie!', placement: 'top'});
    $(".reg-confirm-password")
        .popover({ trigger: 'focus', title: 'Jelszó', content: 'A két jelszónak egyeznie kell!', placement: 'top' });

    /*Personal Account Popovers*/
    $("#reg-person-name")
        .popover({ trigger: 'focus', title: 'Név', content: 'A név megadása kötelező!', placement: 'top' });
    $("#reg-person-identification")
        .popover({ trigger: 'focus', title: 'Személyi igazolvány', content: 'Szem. Ig. megadási formátum: "123456AB"', placement: 'top' })
    $("#reg-person-mothername")
        .popover({ trigger: 'focus', title: 'Anyja neve', content: 'Anyja leánykori neve', placement: 'top' });
    $("#reg-person-address")
        .popover({ trigger: 'focus', title: 'Lakcím', content: 'Pl.: Budapest 1267 Példa utca 12/a', placement: 'top' });
    $("#reg-person-telephone")
        .popover({ trigger: 'focus', title: 'Telefonszáma', content: 'A telefonszám megadása kötelező ebben a formátumba : "+3620/11-22-333"', placement: 'top' });

    /*Company Account Popover information*/
    $("#reg-company-name")
        .popover({ trigger: 'focus', title: 'Cég neve', content: 'Cégnév megadása kötelező', placemenet: 'top' });
    $("#reg-company-taxnumber")
        .popover({ trigger: 'focus', title: 'Adószám', content: 'Az adószám megadása ebben a formában kötelező: "12345678-1-23"', placement: 'top' });
    $("#reg-company-contactname")
        .popover({ trigger: 'focus', title: 'Kapcsolattartó', content: 'A cég kapcsolattartójának nevét kötelező megadni!', placement: 'top' });
    $("#reg-company-registry")
        .popover({ trigger: 'focus', title: 'Cégjegyzékszám', content: 'A cégjegyzék számát ebben a formában kötelező megadni: "00-00-000000"', placement: 'top' });
    $("#reg-company-address")
        .popover({ trigger: 'focus', title: 'Cég telephelye', content: 'A cég telephelyének címét kötelező megadni!', placement: 'top' });
    $("#reg-company-telephone")
        .popover({ trigger: 'focus', title: 'Cég Telefonszáma', content: 'A céges kapcsolattartó telefonszáma a következő formátumban: "+3620/11-22-333"', placement: 'top' });



};