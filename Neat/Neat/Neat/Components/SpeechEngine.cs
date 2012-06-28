#if SPEECH
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Kinect;

namespace Neat.Components
{
    public class SpeechEngine : GameComponent
    {
        #region FIELDS
        public SpeechRecognitionEngine Recognizer;
        public KinectAudioSource KinectAudioSource;

        public Choices Choices = new Choices();
        public bool Recognizing { get { return recognizing; } }
        public int AudioLevel { get { return audioLevel; } }

        public Action<SpeechRecognitionRejectedEventArgs> SpeechRejectedFunc = null;
        public Action<SpeechHypothesizedEventArgs> SpeechHypothesizedFunc = null;
        public Action<SpeechRecognizedEventArgs> SpeechRecognizedFunc = null;

        public string SpeechRejectedCommand = null;
        public string SpeechHypothesizedCommand = null;
        public string SpeechRecognizedCommand = null;

        bool recSpeech = true;
        Thread t;
        bool recognizing = false;
        int audioLevel=5; 
        Console Console;
        #endregion

        #region Initialize
        public SpeechEngine(NeatGame game)
            : base(game)
        {
            this.Console = game.Console;

        }

        public override void Initialize()
        {
            AttachToConsole(Console);
            base.Initialize();
        }
        #endregion

        #region Functions
        public void StartListening(bool speech = true)
        {
            if (recognizing)
            {
                Debug.Write("Speech engine is already running.");
                return;
            }

            RecognizerInfo ri = GetKinectRecognizer();
            if (ri == null)
            {
                Debug.WriteLine("Could not find Kinect speech recognizer.");
                return;
            }

            this.Recognizer = new SpeechRecognitionEngine(ri);

            recSpeech = speech;

            if (recSpeech)
            {
                var grammar = new GrammarBuilder();
                grammar.Append(Choices);
                grammar.Culture = Recognizer.RecognizerInfo.Culture;
                Recognizer.LoadGrammar(new Grammar(grammar));
            }

            Recognizer.AudioLevelUpdated += Recognizer_AudioLevelUpdated;
            Recognizer.SpeechRecognized += SreSpeechRecognized;
            Recognizer.SpeechHypothesized += SreSpeechHypothesized;
            Recognizer.SpeechRecognitionRejected += SreSpeechRecognitionRejected;

            var sensor = (Game as NeatGame).Kinect.Sensor;
            KinectAudioSource = sensor.AudioSource;
            KinectAudioSource.AutomaticGainControlEnabled = true;
            KinectAudioSource.BeamAngleMode = BeamAngleMode.Automatic;
            KinectAudioSource.NoiseSuppression = true;

            var kinectStream = KinectAudioSource.Start();
            //Recognizer.SetInputToAudioStream(
            //    kinectStream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            Recognizer.SetInputToDefaultAudioDevice();
            Recognizer.RecognizeAsync(RecognizeMode.Multiple);
            /*
            t = new Thread(new ThreadStart(StartListening));
            t.SetApartmentState(ApartmentState.MTA);
            t.Start();*/
            recognizing = true;
        }

        void ListenThread()
        {

        }

        /*
        void StartListening()
        {
            if (recognizing)
            {
                Debug.Write("Speech engine is already running.");
                return;
            }

            try
            {

                using (var source = new KinectAudioSource())
                {
                    source.FeatureMode = true;
                    source.AutomaticGainControl = false;
                    source.SystemMode = SystemMode.OptibeamArrayOnly;

                    RecognizerInfo ri = GetKinectRecognizer();

                    if (ri == null)
                    {
                        Debug.WriteLine("Could not find Kinect speech recognizer.");
                        return;
                    }

                    Recognizer = new SpeechRecognitionEngine(ri.Id);
                    Recognizer.SetInputToDefaultAudioDevice();
                    if (recSpeech)
                    {
                        var gb = new GrammarBuilder();
                        //Specify the culture to match the recognizer in case we are running in a different culture.                                 
                        gb.Culture = ri.Culture;
                        gb.Append(Choices);

                        // Create the actual Grammar instance, and then load it into the speech recognizer.
                        Grammar g;

                        try
                        {
                            g = new Grammar(gb);
                        }
                        catch
                        {
                            Debug.WriteLine("Grammar is empty.");
                            return;
                        }

                        Recognizer.LoadGrammar(g);
                    }

                    Debug.WriteLine("Recognizing.");

                    while (Recognizer != null)
                    {
                        if (!recognizing)
                        {
                            Recognizer.RecognizeAsync(RecognizeMode.Multiple);
                            recognizing = true;
                        }
                        else Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                Console.Run("et_echo Error initializing speech.");
                recognizing = false;
            }
        }
        */

        int sp_c = 0;
        void Recognizer_AudioLevelUpdated(object sender, AudioLevelUpdatedEventArgs e)
        {
            Debug.WriteLineIf((sp_c++ % 30) == 0, "Audio Level: " + e.AudioLevel, "Recognizer_AudioLevelUpdated");
            audioLevel = e.AudioLevel;
        }

        public void StopListening()
        {
            if (Recognizer != null)
            {
                if (KinectAudioSource != null)
                    KinectAudioSource.Stop();
                Recognizer.RecognizeAsyncCancel();
                Recognizer.RecognizeAsyncStop();

                Recognizer.AudioLevelUpdated -= Recognizer_AudioLevelUpdated;
                Recognizer.SpeechRecognized -= SreSpeechRecognized;
                Recognizer.SpeechHypothesized -= SreSpeechHypothesized;
                Recognizer.SpeechRecognitionRejected -= SreSpeechRecognitionRejected;

                Recognizer = null;
                recognizing = false;
            }
        }

        public void AddChoice(string text)
        {
            Choices.Add(text);
        }
        #endregion

        #region Helpers
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }
        #endregion

        #region Events
        void SreSpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine("Speech Rejected: " + e.Result.Text);
            if (SpeechRejectedFunc != null) SpeechRejectedFunc(e);
            if (Console != null && SpeechRecognizedCommand != null) Console.Run(SpeechRejectedCommand + ' ' + e.Result.Text);
        }

        void SreSpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine("Speech Hypothesized: " + e.Result.Text + " - Confidence: %" + e.Result.Confidence * 100f);
            if (SpeechHypothesizedFunc != null) SpeechHypothesizedFunc(e);
            if (Console != null && SpeechHypothesizedCommand != null) Console.Run(SpeechHypothesizedCommand + ' ' + e.Result.Text);
        }

        void SreSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Debug.WriteLine("Speech Recognized: " + e.Result.Text + " - Confidence: %" + e.Result.Confidence * 100f);
            if (SpeechRecognizedFunc != null) SpeechRecognizedFunc(e);
            if (Console != null && SpeechRecognizedCommand != null) Console.Run(SpeechRecognizedCommand + ' ' + e.Result.Text);
        }
        #endregion

        #region Console
        public void AttachToConsole(Console console = null)
        {
            if (console == null) return;
            console.AddCommand("sp_start", sp_start);
            console.AddCommand("sp_stop", sp_stop);
            console.AddCommand("sp_clear", sp_clear);
            console.AddCommand("sp_add", sp_add);
            console.AddCommand("sp_rejectcmd", sp_rejectcmd);
            console.AddCommand("sp_hypocmd", sp_hypofunc);
            console.AddCommand("sp_recognizecmd", sp_recognizecmd);
        }

        void sp_start(IList<string> args)
        {
            if (args.Count == 1)
                StartListening();
            else
                StartListening(bool.Parse(args[1]));
        }

        void sp_stop(IList<string> args)
        {
            StopListening();
        }

        void sp_clear(IList<string> args)
        {
            Choices = new Choices();
        }

        void sp_add(IList<string> args)
        {
            Choices.Add(Console.Args2Str(args, 1));
        }

        void sp_rejectcmd(IList<string> args)
        {
            SpeechRejectedCommand = Console.Args2Str(args, 1);
        }

        void sp_hypofunc(IList<string> args)
        {
            SpeechHypothesizedCommand = Console.Args2Str(args, 1);
        }

        void sp_recognizecmd(IList<string> args)
        {
            SpeechRecognizedCommand = Console.Args2Str(args, 1);
        }
        #endregion
    }
}
#endif