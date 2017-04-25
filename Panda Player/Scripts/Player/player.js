var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: 'red',
    progressColor: 'purple',
});

// load default track
wavesurfer.load('https://ia902606.us.archive.org/35/items/shortpoetry_047_librivox/song_cjrg_teasdale_64kb.mp3');

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
wavesurfer.on('playlist-ready', function () {
    myList = myPlaylist.getPlaylist();
    for (var i = 0; i < myList.length; i++) {
        if (myList[i]) {
            $('.playlistbox').append('<li class="playTrack" data-id="' + i + '">' + myList[i] + '</li>');
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