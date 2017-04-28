var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: '#225622',
    progressColor: '#1ca532',
    hideScrollbar: true,
    scrollParent: true,
    maxCanvasWidth: 4000,
    scrollParent: true
});

// load M3U playlist    
var myPlaylist = wavesurfer.Playlist;
$('body').on('click', '.playlist', function () {
    myPlaylist.init({
        wavesurfer: wavesurfer,
        playlistFile: 'http://localhost:4522/Uploads/Playlists/currentPlaylist.m3u',
        playlistType: 'm3u'
    });
});

var LoadPlaylistName = function () {

    var sId = $("#songId").val();
    var pId = $("#playlistId").val();

    $.ajax({
        type: "GET",
        url: (sId) ? "/Playlist/LoadPlaylist" : "/Playlists/DeleteConfirmed",
        data: (sId) ? { id: sId } : { id: pId },
        success: function () {
            $("#myModal").modal("hide");
            window.location.reload();
        }
    })
}

// on playlist parsed with event playlist-ready
var myList;
var allList;
wavesurfer.on('playlist-ready', function () {
    myList = myPlaylist.getPlaylist();
    allList = myPlaylist;
    var myRegexp = /(.*?_)/g;

    for (var i = 0; i < myList.length; i++) {
        var song = myList[i].replace(myRegexp, '');
        var songNumber = `${i + 1}. `;
        if (myList[i]) {
            $('.playlistbox').append('<li class="playTrack" data-id="' + i + '">' + songNumber + song + '</li>');
        }
    }
    console.log(myList);
});

// on waveform ready
wavesurfer.on('waveform-ready', function () {
    wavesurfer.play();
});

// on playlist track click
$('body').on('click', '.playTrack', function () {
    wavesurfer.load(myList[$(this).data('id')]);
});