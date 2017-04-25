'use strict';

/* Playlist Parser */
window.PlaylistParser = {
    init: function (params) {
        this.params = params;

        // parse playlist and set params
        this.playlistFileGET = this.params.playlistFile || null;
        this.playlistType = this.params.playlistType || null;

        if (this.playlistFileGET !== null) {
            var xhr = new XMLHttpRequest();
            xhr.open('GET', this.playlistFileGET, true);
            xhr.responseType = 'text';
            if (xhr.status = 200) {
                this.playlistFile = xhr.response;
                this.parse();
            } else {
                throw new Error('Error reading the playlist file');
            }
        } else{
            throw new Error('No playlist file provided');
        }
    },

    parse: function() {
        // check if playlist type is given
        var playlist = [];
        if (this.playlistType === 'm3u' || 'audio/mpegurl') {
            playlist = this.playlistFile.replace(/^.*#.*$|#EXTM3U|#EXTINF:/mg, '').split('\n');
        } else if (this.playlistType === 'pls' || this.playlistType === 'audio/x-scpls') {
            // to do
        } else if (this.playlistType === 'smil' || this.playlistType === 'application/smil') {
            // to do
        } else if (this.playlistType === 'json' || this.playlistType === 'application/json') {
            // to do
        } else {
            throw new Error('No valid playlist file provided, valid formats are m3u pls smil json or their valid mime types');
        }

        // playlist type is set return the playlist
        var outputArray = [];
        for (var i = 0; i < playlist.length; i++) {
            if (playlist[i]) {
                // check if file name has .mp3 or .wav before adding the playlist array
                if(playlist[i].indexOf('.mp3') !== -1 || playlist[i].indexOf('.wav') !== -1) {
                    outputArray.push(playlist[i]);
                }
            }
        }
        return outputArray;
    }

};
