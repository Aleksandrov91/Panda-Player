var ConfirmDelete = function (id, artistAndTitle) {
    $("#songId").val(id);
    $("#playlistId").val(id);
    $("#Description").text(`Are you sure you want to delete: ${artistAndTitle}`);
    $("#myModal").modal('show').css({ margin: 0, padding: 0, border: 0 });
    var req = $.get("#myModal").done(function (data) {
        data.Success
    });  
};

var Delete = function () {

    var sId = $("#songId").val();
    var pId = $("#playlistId").val();
    var req = $.post("#myModal").done(function (data) {
        data.Success
    });    

    $.ajax({
        type: "POST",
        url: (sId) ? "/Songs/DeleteConfirmed" : "/Playlists/DeleteConfirmed",
            data: (sId) ? { id: sId } : { id: pId },
            success: function (data) {
                $("#myModal").modal("hide");
                $('.modal-backdrop').remove();
                $('#body').load(data.Url);
            }
    })   
}