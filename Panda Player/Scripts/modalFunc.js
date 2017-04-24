var ConfirmDelete = function (id, artistAndTitle) {
    $("#songId").val(id);
    $("#playlistId").val(id);
    $("#Description").text(`Are you sure you want to delete: ${artistAndTitle}`);
    $("#myModal").modal('show').css({ margin: 0, padding: 0, border: 0 });
};

var Delete = function () {

    var sId = $("#songId").val();
    var pId = $("#playlistId").val();

    $.ajax({
        type: "POST",
        url: (sId) ? "/Songs/DeleteConfirmed" : "/Playlists/DeleteConfirmed",
            data: (sId) ? { id: sId } : { id: pId },
            success: function () {
                $("#myModal").modal("hide");
                window.location.reload();
            }
        })

    
}

var BanUser = function (id, fullName) {
    $("#userId").val(id);
    $("#Description").text(`Izberi za kolko vreme iskash da bannesh: ${fullName}`);
    $("#myModal").modal('show').css({ margin: 0, padding: 0, border: 0 });
};

var Ban = function (data) {
    var start = document.getElementsByName('start').value;
    var duration = document.getElementsByName('duration').value;

    console.log(start);
    console.log(duration);
    var userId = $("#userId").val();
    $.ajax({
        type: "POST",
        url: "/Account/BanUser",
        data: { id: userId },
        success: function () {
            $("#myModal").modal("hide");
            window.location.reload();
        }
    })


}