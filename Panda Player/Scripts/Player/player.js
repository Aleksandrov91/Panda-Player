var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: '#225622',
    progressColor: '#1ca532',
    hideScrollbar: true,
    scrollParent: true,
    maxCanvasWidth: 4000,
    scrollParent: true
});

// load default track
//wavesurfer.load('https://ia902606.us.archive.org/35/items/shortpoetry_047_librivox/song_cjrg_teasdale_64kb.mp3');

// load M3U playlist    
var myPlaylist = wavesurfer.Playlist;
$('body').on('click', '.playlist', function () {
    myPlaylist.init({
        wavesurfer: wavesurfer,
        playlistFile: 'http://localhost:4522/Uploads/Playlists/currentPlaylist.m3u',
        playlistType: 'm3u'
    });
});

// on playlist parsed with event playlist-ready
var myList;
//var allList;
wavesurfer.on('playlist-ready', function () {
    myList = myPlaylist.getPlaylist();
    //allList = myPlaylist;
    var myRegexp = /(.*?_)/g;

    for (var i = 0; i < myList.length; i++) {
        var song = myList[i].replace(myRegexp, '');
        var songNumber = `${i + 1}. `;
        if (myList[i]) {
            $('.playlistbox').append('<li class="playTrack" data-id="' + i + '">' + songNumber + song + '</li>');
        }
    }

    var counter = 0;
    wavesurfer.load(myList[counter]);
    setTimeout(play, delay);

    /// play all song in playlist
    var reqursion = wavesurfer.on('finish', function () {
        counter++;
        if (counter < myList.length) {
            wavesurfer.load(myList[counter]);
            setTimeout(play, delay);
            return reqursion;
        }
    });  
});

// on waveform ready
wavesurfer.on('waveform-ready', function () {
    wavesurfer.play();
});

// on playlist track click
$('body').on('click', '.playTrack', function () {
    wavesurfer.load(myList[$(this).data('id')]);
    setTimeout(play, delay);
        
    var counter = $(this).data('id');

    var reqursion = wavesurfer.on('finish', function () {
        counter++;
        if (counter < myList.length) {
            wavesurfer.load(myList[counter]);
            setTimeout(play, delay);
            return reqursion;
        }
    }); 
});