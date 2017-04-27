var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: '#000000',
    progressColor: '#1ca532',
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

    var count = myList[0];
    wavesurfer.load(count);
    setTimeout(play, delay);

    wavesurfer.on('finish', function () {   
        wavesurfer.load(myList[1]);
        setTimeout(play, delay);
        console.log("YEa")
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

    var count = myList[$(this)];
    console.log(count);

    wavesurfer.on('finish', function () {
        count++;
        wavesurfer.load(myList[2]);
        setTimeout(play, delay);
        console.log("YEa")
    });
});