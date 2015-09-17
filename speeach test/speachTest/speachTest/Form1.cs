using System;
using System.Windows.Forms;
using System.Speech.Recognition;


namespace speachTest
{
    public partial class Form1 : Form
    {
        SpeechRecognitionEngine recEngine = new SpeechRecognitionEngine();
        

        public Form1()
        {
            InitializeComponent();
        }

        private void btnEnable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsync(RecognizeMode.Multiple);
            btnDisable.Enabled = true;
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
                    MessageBox.Show("Hello Denis. How are you?"); break;
                
                case "print my name":
                    richTextBox1.Text += "\nDenis"; break;

            }
        }

        private void btnDisable_Click(object sender, EventArgs e)
        {
            recEngine.RecognizeAsyncStop();
            btnDisable.Enabled = false;
        }
    }
}
