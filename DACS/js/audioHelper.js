window.audioHelper = {
    play: function (audioElement) {
        audioElement.play();
    },
    pause: function (audioElement) {
        audioElement.pause();
    }, 
    playAudio: function () {
        var audioPlayer = document.getElementById('audioPlayer');
        if (audioPlayer) {
            audioPlayer.play();
        } else {
            console.error('Audio player element not found');
        }
    },
    pauseAudio: function () {
        var audioPlayer = document.getElementById('audioPlayer');
        if (audioPlayer) {
            audioPlayer.pause();
        } else {
            console.error('Audio player element not found');
        }
    },
    getCurrentTime: function (audioElement) {
        return audioElement.currentTime || 0;
    },
    setCurrentTime: function (audioElement, time) {
        audioElement.currentTime = time;
    },
    getDuration: function (audioElement) {
        return new Promise((resolve) => {
            if (audioElement.duration > 0) {
                resolve(audioElement.duration);
            } else {
                audioElement.addEventListener('loadedmetadata', () => {
                    resolve(audioElement.duration);
                });
            }
        });
    },
    getAudioDuration: function (url) {
        return new Promise((resolve, reject) => {
            let audio = new Audio(url);
            audio.onloadedmetadata = function () {
                resolve(audio.duration);
            };
            audio.onerror = function () {
                reject("Error loading audio file.");
            };
        });
    },

    addTimeUpdateEvent: function (audioElement, dotNetHelper) {
        audioElement.addEventListener('timeupdate', () => {
            dotNetHelper.invokeMethodAsync('UpdateCurrentTime', audioElement.currentTime);
        });
    },
    initializeProgressBar: function (progressContainer, audioElement, dotNetHelper) {
        let isDragging = false;

        progressContainer.addEventListener('mousedown', (e) => {
            isDragging = true;
            updateProgress(e);
        });

        window.addEventListener('mousemove', (e) => {
            if (isDragging) {
                updateProgress(e);
            }
        });

        window.addEventListener('mouseup', (e) => {
            if (isDragging) {
                isDragging = false;
                updateProgress(e);
            }
        });

        progressContainer.addEventListener('click', (e) => {
            updateProgress(e);
        });

        function updateProgress(e) {
            const rect = progressContainer.getBoundingClientRect();
            const offsetX = e.clientX - rect.left;
            const progress = Math.max(0, Math.min(1, offsetX / rect.width));
            const percentage = progress * 100;
            dotNetHelper.invokeMethodAsync('UpdateProgress', percentage);
            const newTime = progress * audioElement.duration;
            audioElement.currentTime = newTime;
        }
    },
    setAudioSource: function (audioElement, url) {
        audioElement.src = url;
        audioElement.load();
        audioElement.oncanplaythrough = function () {
            audioElement.play().catch(error => {
                console.error("Error playing audio:", error);
            });
        };
    },
    
   
};
window.volumeHelper = {
    initializeVolumeSlider: function (sliderContainer, dotNetHelper) {
        let isDragging = false;

        sliderContainer.addEventListener('mousedown', (e) => {
            isDragging = true;
            updateVolume(e);
        });

        window.addEventListener('mousemove', (e) => {
            if (isDragging) {
                updateVolume(e);
            }
        });

        window.addEventListener('mouseup', () => {
            if (isDragging) {
                isDragging = false;
                
            }
        });
        sliderContainer.addEventListener('click', (e) => {
            updateVolume(e);
        });
        function updateVolume(e) {
            const rect = sliderContainer.getBoundingClientRect();
            const offsetX = e.clientX - rect.left;
            const volume = Math.max(0, Math.min(1, offsetX / rect.width));
            const percentage = volume * 100;
            dotNetHelper.invokeMethodAsync('UpdateVolume', percentage);
            const newVolume = percentage;
            sliderContainer.volumePercentage = newVolume;

        }
    },
    setVolume: function (volume) {
        const audioElements = document.querySelectorAll('audio');
        audioElements.forEach(audio => {
            audio.volume = volume;
        });
    },
};






