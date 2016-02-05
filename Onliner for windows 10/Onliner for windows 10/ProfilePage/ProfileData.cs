using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Onliner_for_windows_10.ProfilePage
{
    public class ProfileData : INotifyPropertyChanged
    {
        private string acName="user";

        public string AccauntName { get { return acName; } set { acName = value; OnPropertyChanged("AccauntName"); } } 
        public string Avatar { get; set; }
        public string Status { get; set; } = "status";
        public string ProfileNumbers { get; set; }
        public string Money { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class ProfilePataList
    {
        public string Info { get; set; }
        public string Value { get; set; }
    }

    public class BirthDayDate
    {
        public string Day { get ; set; }
        public string Mounth { get; set; }
        public string Year { get; set; }
    }
}
