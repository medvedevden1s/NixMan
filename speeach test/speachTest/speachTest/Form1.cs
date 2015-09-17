using System;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace speachTest
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        SpeechSynthesizer syncSpeechSynthesizer = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
            btnDisable.Enabled = true;
            btnEnable.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Choices commands = new Choices();
            commands.Add(new string[ ] {"say hello", "print my name"}); // добавляем команды

            GrammarBuilder gBuilder = new GrammarBuilder();
            gBuilder.Append(commands); // добавляем команды в словарь

            Grammar grammar = new Grammar(gBuilder); // создаем словарь 

            recEngine.LoadGrammarAsync(grammar); // загружаем словарь 
            recEngine.SetInputToDefaultAudioDevice(); // указывем микрофон
            recEngine.SpeechRecognized += recEngine_SpeachSpeechRecognized;
        }

        void recEngine_SpeachSpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            switch (e.Result.Text)
            {
                case "say hello": 
                    //MessageBox.Show("Hello Denis. How are you?"); break;

                    PromptBuilder promtBuilder = new PromptBuilder();
                    promtBuilder.StartSentence();
                    promtBuilder.AppendText("Hello Denis");
                    promtBuilder.EndSentence();

                    promtBuilder.AppendBreak(PromptBreak.ExtraSmall);
                    promtBuilder.AppendText("How are you?");

                    syncSpeechSynthesizer.SpeakAsync("Hello Denis. How are you?"); break;
                case "print my name":
                    richTextBox1.Text += "\nDenis"; break;
                case "speak selected text":
                    syncSpeechSynthesizer.SpeakAsync(richTextBox1.SelectedText); break;
            }
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
            btnEnable.Enabled = true;
            btnDisable.Enabled = false;
        }
    }
}
