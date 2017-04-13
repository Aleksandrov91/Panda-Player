var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: 'red',
    progressColor: 'purple'
});

wavesurfer.load('https://ia902606.us.archive.org/35/items/shortpoetry_047_librivox/song_cjrg_teasdale_64kb.mp3');