using AVFoundation;
using Foundation;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PAUM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        int index0 { get; set; } = 0;
        int index1 { get; set; } = 1;
        public int numberOfLive { get; set; } = 3;
        public int numberOfMaxCorrectPasses { get; set; } = 5;
        public bool resetCorrectPasses { get; set; }
        public int interval { get; set; } = 5;
        public int numberOfDiff { get; set; } = 3;
        public int timeToThink { get; set; } = 7;
        int diff { get; set; } = 1;
        int numberOfCorrectPasses { get; set; } = 1;
        bool isRound { get; set; }
        bool isHandled { get; set; }
        public ObservableCollection<string> ListAudioFiles { get; set; } = new ObservableCollection<string>();
        Random rnd;
        Timer timer;
        ISimpleAudioPlayer player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;

        int MinSeconds
        {
            get
            {
                switch (this.diff)
                {
                    case 1: return 0;
                    case 2: return 2;
                    case 3: return 4;
                }

                return 0;
            }
        }

        int MinSecondsToThink
        {
            get
            {
                switch (this.diff)
                {
                    case 1: return 0;
                    case 2: return 1;
                    case 3: return 2;
                }

                return 0;
            }
        }

        public Page1()
        {
            this.rnd = new Random();
            InitializeComponent();
        }

        public async void RoundStart()
        {
            try
            {
                this.isRound = true;
                if(this.numberOfCorrectPasses == this.numberOfMaxCorrectPasses)
                {
                    this.numberOfMaxCorrectPasses = 0;
                    if (this.diff < this.numberOfDiff)
                    {
                        await TextToSpeech.SpeakAsync("New difficulty!");
                        this.diff++;
                    }
                    else if(this.diff == this.numberOfDiff)
                    {
                        await TextToSpeech.SpeakAsync("You won!");
                        var modalPage = new MainPage();
                        modalPage.ListAudioFiles = this.ListAudioFiles;
                        modalPage.NumberOfDiff = this.numberOfDiff;
                        modalPage.NumberOfLive = this.numberOfLive;
                        modalPage.NumberOfMaxCorrectPasses = this.numberOfMaxCorrectPasses;
                        modalPage.TimeToThink = this.timeToThink;
                        await PAUM.App.Current.MainPage.Navigation.PushModalAsync(modalPage);
                        return;
                    }
                }

                if (this.numberOfLive == 0)
                {
                    await TextToSpeech.SpeakAsync("You lost!");
                    Vibration.Vibrate();
                    var modalPage = new MainPage();
                    modalPage.ListAudioFiles = this.ListAudioFiles;
                    modalPage.NumberOfDiff = this.numberOfDiff;
                    modalPage.NumberOfLive = this.numberOfLive;
                    modalPage.NumberOfMaxCorrectPasses = this.numberOfMaxCorrectPasses;
                    modalPage.TimeToThink = this.timeToThink;
                    await PAUM.App.Current.MainPage.Navigation.PushModalAsync(modalPage);
                    return;
                }

                await Task.Delay(2000);
                this.PlayFirstSound();
                await Task.Delay((this.interval - this.MinSeconds) * 1000);
                this.PlaySecondSound();

                this.timer = new System.Timers.Timer((this.timeToThink - this.MinSecondsToThink) * 1000);
                this.timer.Elapsed += OnTimedEvent;
                this.timer.Start();
                await Task.Delay(3000);
                this.isRound = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        async void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if(this.isHandled)
            {
                this.isHandled = true;
                this.timer.Stop();
                await TextToSpeech.SpeakAsync("No time!");
                Vibration.Vibrate();
                this.numberOfLive--;
                if (this.resetCorrectPasses) this.numberOfMaxCorrectPasses = 0;
                this.RoundStart();
                this.isHandled = false;
            }
        }

        async void PlayFirstSound()
        {
            await TextToSpeech.SpeakAsync("First sound");
            player.Load(this.ListAudioFiles[this.RandomNumber()]);
            await Task.Delay(1000);
            player.Play();
        }

        async void PlayFirstSound(int Index)
        {
            await TextToSpeech.SpeakAsync("First sound");
            player.Load(this.ListAudioFiles[Index]);
            await Task.Delay(1000);
            player.Play();
        }

        async void PlaySecondSound()
        {
            await TextToSpeech.SpeakAsync("Second sound");
            player.Load(this.ListAudioFiles[this.RandomNumber(true)]);
            await Task.Delay(1000);
            player.Play();
        }

        async void PlaySecondSound(int Index)
        {
            await TextToSpeech.SpeakAsync("Second sound");
            player.Load(this.ListAudioFiles[Index]);
            await Task.Delay(1000);
            player.Play();
        }

        int RandomNumber(bool IsSecond = false)
        {
            int value = 0;

            if(IsSecond)
            {
                if (rnd.Next(0, 100) < 33)
                {
                    this.index1 = this.index0;
                    value = this.index1;
                }
                else
                {
                    this.index1 = rnd.Next(0, this.ListAudioFiles.Count());
                    value = this.index1;
                }
            }
            else
            {
                this.index0 = rnd.Next(0, this.ListAudioFiles.Count());
                value = this.index0;
            }
            return value;
        }

        async private void OnSwipedLeft(object sender, SwipedEventArgs e)
        {
            if (!this.isRound && !this.isHandled)
            {
                this.isHandled = true;
                this.timer.Stop();
                if (this.index0 != this.index1)
                {
                    await TextToSpeech.SpeakAsync("Correct!");
                    this.numberOfCorrectPasses++;
                }
                else
                {
                    await TextToSpeech.SpeakAsync("Wrong!");
                    Vibration.Vibrate();
                    this.numberOfLive--;
                    if (this.resetCorrectPasses) this.numberOfMaxCorrectPasses = 0;
                }
                this.isHandled = false;
                this.RoundStart();
            }
        }

        async private void OnSwipedRight(object sender, SwipedEventArgs e)
        {
            if (!this.isRound && !this.isHandled)
            {
                this.isHandled = true;
                this.timer.Stop();
                if (this.index0 == this.index1)
                {
                    await TextToSpeech.SpeakAsync("Correct!");
                    this.numberOfCorrectPasses++;
                }
                else
                {
                    await TextToSpeech.SpeakAsync("Wrong!");
                    Vibration.Vibrate();
                    this.numberOfLive--;
                    if (this.resetCorrectPasses) this.numberOfMaxCorrectPasses = 0;
                }
                this.isHandled = false;
                this.RoundStart();
            }
        }

        async private void OnSwipedDown(object sender, SwipedEventArgs e)
        {
            if (!this.isHandled)
            {
                this.isHandled = true;
                this.isRound = true;
                this.PlayFirstSound(this.index0);
                await Task.Delay((this.interval - this.MinSeconds) * 1000);
                this.PlaySecondSound(this.index1);
                await Task.Delay((this.interval - this.MinSeconds) * 1000);

                this.timer = new System.Timers.Timer((this.timeToThink - this.MinSecondsToThink) * 1000);
                this.timer.Elapsed += OnTimedEvent;
                this.timer.Start();
                await Task.Delay(3000);
                this.isRound = false;
                this.isHandled = false;
            }
        }

        async private void OnSwipedUp(object sender, SwipedEventArgs e)
        {
            if (!this.isHandled)
            {
                this.isHandled = true;
                var modalPage = new MainPage();
                modalPage.ListAudioFiles = this.ListAudioFiles;
                modalPage.NumberOfDiff = this.numberOfDiff;
                modalPage.NumberOfLive = this.numberOfLive;
                modalPage.NumberOfMaxCorrectPasses = this.numberOfMaxCorrectPasses;
                modalPage.TimeToThink = this.timeToThink;
                await PAUM.App.Current.MainPage.Navigation.PushModalAsync(modalPage);
                this.isHandled = false;
            }
        }
    }
}