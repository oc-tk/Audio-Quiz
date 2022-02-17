using PAUM.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace PAUM
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.ListDiff.ItemsSource = this.ListDiffLevels;
            this.ListDiff.SelectedItem = this.NumberOfDiff;
            this.ListAudio.ItemsSource = this.ListAudioFiles;
            this.ListAudio.SelectionMode = ListViewSelectionMode.None;
            this.ListInterval.ItemsSource = this.ListIntervalValues;
            this.ListInterval.SelectedItem = this.IntervalValue;
            this.ListLive.ItemsSource = this.ListOfLive;
            this.ListLive.SelectedItem = this.NumberOfLive;
            this.ListCorrect.ItemsSource = this.ListOfCorrectPasses;
            this.ListCorrect.SelectedItem = this.NumberOfMaxCorrectPasses;
            this.ListToThink.ItemsSource = this.ListOfTimeToThink;
            this.ListToThink.SelectedItem = this.TimeToThink;
        }

        public int TimeToThink { get; set; } = 7;
        public int IntervalValue { get; set; } = 5;
        public int NumberOfLive { get; set; } = 3;
        public int NumberOfMaxCorrectPasses { get; set; } = 5;
        public int NumberOfDiff { get; set; } = 1;

        public List<int> ListOfTimeToThink { get; set; } = new List<int>() { 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public List<int> ListOfCorrectPasses { get; set; } = new List<int>() { 3, 4, 5, 6, 7, 8, 9, 10 };
        public List<int> ListOfLive { get; set; } = new List<int>() { 1, 2, 3, 4, 5 };
        public List<int> ListDiffLevels { get; set; } = new List<int>() { 1, 2, 3 };
        public List<int> ListIntervalValues { get; set; } = new List<int>() { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        public ObservableCollection<string> ListAudioFiles { get; set; } = new ObservableCollection<string>() { "1.mp3", "2.mp3", "3.mp3", "4.mp3", "5.mp3", "6.mp3" };

        async void ButtonClickedAudio(object sender, EventArgs e)
        {
            try
            {
                var customFileType =
                    new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                    {
                        { DevicePlatform.Android, new[] { "audio/*" } },
                        { DevicePlatform.UWP, new[] { "mp3" } }
                    });

                var options = new PickOptions
                {
                    PickerTitle = "Please select a audio files",
                    FileTypes = customFileType
                };
                var result = await FilePicker.PickMultipleAsync(options);
                if (result != null)
                {
                    this.ListAudioFiles.Clear();
                    foreach(var item in result)
                    {
                        this.ListAudioFiles.Add(item.FullPath);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async void ButtonClicked(object sender, EventArgs e)
        {
            if(this.ListAudioFiles.Any())
            {
                var modalPage = new Page1();
                modalPage.ListAudioFiles = this.ListAudioFiles;
                modalPage.numberOfDiff = this.NumberOfDiff;
                modalPage.resetCorrectPasses = this.CheckPoprawnosc.IsChecked;
                modalPage.numberOfLive = this.NumberOfLive;
                modalPage.numberOfMaxCorrectPasses = this.NumberOfMaxCorrectPasses;
                modalPage.timeToThink = this.TimeToThink;
                await PAUM.App.Current.MainPage.Navigation.PushModalAsync(modalPage);
                modalPage.RoundStart();
            }
            else
            {
                this.ButtonClickedAudio(null, null);
            }
        }

        private async void ButtonHowToPlayClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Jak grać?",
                "W ekranie ustawień można dobrać wiele parametrów aby ułatwić rozgrywkę lub ją utrudnić." +
                "Rozgrywka jest podzielona na rundy, każda runda rozpoczyna się puszczeniem dwóch dźwięków a potem następuje czas na zastanowienie." +
                "Po odpowiedniej ilości poprawnych odpowiedzi gracz awansuje na wyższy poziom trudności." +
                 "W grze występują 4 typy wskaźników:" +
                "\n1. Swipe w lewo: puszczone dźwięki się róźnią" +
                "\n2. Swipe w prawo: puszczone dźwięki są takie same" +
                "\n3. Swipe w górę: powrót do ekranu ustawień" +
                "\n4. Swipe w dół: powtarza dwa ostatnie dźwięki", "OK");
        }
    }
}
