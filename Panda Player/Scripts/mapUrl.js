

    function mapUrl(controller, action) {
        //URL will change to /Songs/MySongs
        var id = 0;

        history.replaceState({
        plate_id: true,
            plate: "action"
        }, null, `/${controller}/${action}`);

        showUrl("action");

    }
    $(window).bind("popstate", function () {

    });

    //function showUrl(name) {
    //    document.getElementById("body").innerHTML = name;        
    //}

