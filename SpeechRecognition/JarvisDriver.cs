#region using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Windows;
using System.Diagnostics;

#endregion


namespace SpeechRecognition
{
    class JarvisDriver
    {
        #region locals

        /// <summary>
        /// the engine
        /// </summary>
        SpeechRecognitionEngine speechRecognitionEngine = null;

        /// <summary>
        /// The speech synthesizer
        /// </summary>
        SpeechSynthesizer speechSynthesizer = null;

        /// <summary>
        /// list of predefined commands
        /// </summary>
        List<Word> words = new List<Word>();

        /// <summary>
        /// The last command
        /// </summary>
        string lastCommand = "";

        /// <summary>
        /// The name to call commands
        /// </summary>
        string aiName = "Jarvis";

        #endregion

        #region ctor

        public void Start()
        {

            try
            {
                // create the engine
                speechRecognitionEngine = createSpeechEngine("en-US");

                // hook to event
                speechRecognitionEngine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(engine_SpeechRecognized);

                // load dictionary
                loadGrammarAndCommands();

                // use the system's default microphone
                speechRecognitionEngine.SetInputToDefaultAudioDevice();

                // start listening
                speechRecognitionEngine.RecognizeAsync(RecognizeMode.Multiple);

                //Create the speech synthesizer
                speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.Rate = -5;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Voice recognition failed " + ex.Message);
            }

            //Keeps the command prompt going until you say jarvis quit
            while(lastCommand.ToLower() != "quit")
            {

            }

        }

        #endregion

        #region internal functions and methods

        /// <summary>
        /// Creates the speech engine.
        /// </summary>
        /// <param name="preferredCulture">The preferred culture.</param>
        /// <returns></returns>
        private SpeechRecognitionEngine createSpeechEngine(string preferredCulture)
        {
            foreach (RecognizerInfo config in SpeechRecognitionEngine.InstalledRecognizers())
            {
                if (config.Culture.ToString() == preferredCulture)
                {
                    speechRecognitionEngine = new SpeechRecognitionEngine(config);
                    break;
                }
            }

            // if the desired culture is not found, then load default
            if (speechRecognitionEngine == null)
            {
                Console.WriteLine("The desired culture is not installed on this machine, the speech-engine will continue using "
                    + SpeechRecognitionEngine.InstalledRecognizers()[0].Culture.ToString() + " as the default culture.",
                    "Culture " + preferredCulture + " not found!");
                speechRecognitionEngine = new SpeechRecognitionEngine(SpeechRecognitionEngine.InstalledRecognizers()[0]);
            }

            return speechRecognitionEngine;
        }

        /// <summary>
        /// Loads the grammar and commands.
        /// </summary>
        private void loadGrammarAndCommands()
        {
            try
            {
                Choices texts = new Choices();
                texts.Add("Javis");
                string[] lines = File.ReadAllLines(Environment.CurrentDirectory + "\\example.txt");
                foreach (string line in lines)
                {
                    // skip commentblocks and empty lines..
                    if (line.StartsWith("--") || line == String.Empty) continue;

                    // split the line
                    var parts = line.Split(new char[] { '|' });

                    // add commandItem to the list for later lookup or execution
                    words.Add(new Word() { Text = parts[0], AttachedText = parts[1], IsShellCommand = (parts[2] == "true") });

                    // add the text to the known choices of speechengine
                    texts.Add(parts[0]);
                }
                Grammar wordsList = new Grammar(new GrammarBuilder(texts));
                speechRecognitionEngine.LoadGrammar(wordsList);

                DictationGrammar dict = new DictationGrammar();
                speechRecognitionEngine.LoadGrammar(dict);
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Gets the known command.
        /// </summary>
        /// <param name="command">The order.</param>
        /// <returns></returns>
        private string getKnownTextOrExecute(string command)
        {
            if (!command.StartsWith("Jarvis "))
                return "";
            else
                command = command.Replace("Jarvis ", "");

            Console.WriteLine(command);
            try
            {
                var cmd = words.Where(c => c.Text == command).First();

                if (cmd.IsShellCommand)
                {
                    Process proc = new Process();
                    proc.EnableRaisingEvents = false;
                    proc.StartInfo.FileName = cmd.AttachedText;
                    proc.Start();
                    lastCommand = command;

                    if (command.ToLower() == "i have a burn victim")
                        return "Fetching list of burn centers for you sir";
                    else
                        return "I've started : " + command;
                }
                else
                {
                    lastCommand = command;
                    return cmd.AttachedText;
                }
            }
            catch (Exception)
            {
                lastCommand = command;
                return command;
            }

            
        }

        #endregion

        #region speechEngine events

        /// <summary>
        /// Handles the SpeechRecognized event of the engine control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Speech.Recognition.SpeechRecognizedEventArgs"/> instance containing the event data.</param>
        void engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string command = getKnownTextOrExecute(e.Result.Text);

            if (command != "")
                speechSynthesizer.SpeakAsync(command);
        }

        #endregion

    }
}
