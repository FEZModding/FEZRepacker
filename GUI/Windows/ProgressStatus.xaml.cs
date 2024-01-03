using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace FEZRepacker.GUI.Windows
{
    public partial class ProgressStatus : Window
    {
        private readonly BackgroundWorker worker;

        private readonly IProgress<string> statusTextProgess;
        private readonly IProgress<string> statusSubTextProgess;
        private readonly IProgress<float> statusBarProgress;

        private int stagesCount;
        private int currentStage;
        private float stageProgress;

        public Action? Task;
        public Action? OnComplete;

        public ProgressStatus(string title)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Title = Title + title;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            stagesCount = 1;
            currentStage = 0;
            stageProgress = 0.0f;

            worker = new BackgroundWorker();
            statusTextProgess = new Progress<string>(data => statusText.Content = data);
            statusSubTextProgess = new Progress<string>(data => statusSubText.Content = data);
            statusBarProgress = new Progress<float>(data => statusBar.Value = data);
            worker.DoWork += Work;
            worker.RunWorkerCompleted += OnCompleted;
        }

        private void Work(object? sender, DoWorkEventArgs e)
        {
            Task?.Invoke();
        }

        private void OnCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            Close();

            if (e.Error != null)
            {
                MessageBox.Show("There was an error! " + e.Error.ToString());
            }
            else
            {
                OnComplete?.Invoke();
            }
        }

        private void ReportProgressBar()
        {
            float progress = ((float)currentStage + stageProgress) / (float)stagesCount;
            float progressClamped = Math.Clamp(progress, 0.0f, 1.0f);
            statusBarProgress.Report(progressClamped * 100.0f);
        }


        public void Run()
        {
            worker.RunWorkerAsync();
            ShowDialog();
        }
        public void SetStageText(string text)
        {
            statusSubTextProgess.Report(text);
        }
        public void SetStageName(string label)
        {
            statusTextProgess.Report(label);
        }
        public void SetStageCompletion(float completion)
        {
            stageProgress = completion;
            ReportProgressBar();
        }
        public void IncrementStage(string label)
        {
            SetStageName(label);
            currentStage++;
            stageProgress = 0.0f;
            ReportProgressBar();
        }
        public void SetStageCount(int count)
        {
            stagesCount = count;
            stageProgress = 0.0f;
            ReportProgressBar();
        }

    }
}
