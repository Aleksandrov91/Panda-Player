var wavesurfer = WaveSurfer.create({
    container: '#waveform',
    waveColor: 'red',
    progressColor: 'purple',
});

wavesurfer.load('/Uploads/Home.mp3');