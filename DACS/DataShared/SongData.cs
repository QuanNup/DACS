using SharedClassLibrary.Entities;

namespace DACS.DataShared
{
    public class SongData
    {

        private Song? song;

        public Song? Song
        {
            get => song;
            set
            {
                if (song != value)
                {
                    song = value;
                    NotifyStateChanged();
                }
            }
        }

        public event Action OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();


    }
}
