var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: 'red',
    progressColor: 'purple',
});

// load default track
wavesurfer.load('https://ia902606.us.archive.org/35/items/shortpoetry_047_librivox/song_cjrg_teasdale_64kb.mp3');

// load M3U playlist    
var myPlaylist = wavesurfer.Playlist;
$('body').on('click', '.loadM3U', function () {
    myPlaylist.init({
        wavesurfer: wavesurfer,
        playlistFile: 'https://cdn.rawgit.com/katspaugh/wavesurfer.js/gh-pages/example/playlist/sample.m3u',
        playlistType: 'm3u'
    });
});

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