using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Mvvm.UI.Interactivity;

namespace DevExpress.Mvvm.UI {
    [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
    [TargetType(typeof(System.Windows.Controls.UserControl)), TargetType(typeof(Window))]
    public class SaveFileDialogService : FileDialogServiceBase, ISaveFileDialogService {
        protected interface ISaveFileDialog : IFileDialog {
            bool CreatePrompt { get; set; }
            bool OverwritePrompt { get; set; }
        }

        protected class SaveFileDialogAdapter : FileDialogAdapter<SaveFileDialog>, ISaveFileDialog {
            public SaveFileDialogAdapter(SaveFileDialog fileDialog) : base(fileDialog) { }

            bool ISaveFileDialog.CreatePrompt {
                get { return fileDialog.CreatePrompt; }
                set { fileDialog.CreatePrompt = value; }
            }
            bool ISaveFileDialog.OverwritePrompt {
                get { return fileDialog.OverwritePrompt; }
                set { fileDialog.OverwritePrompt = value; }
            }
        }

        public static readonly DependencyProperty CreatePromptProperty =
            DependencyProperty.Register("CreatePrompt", typeof(bool), typeof(SaveFileDialogService), new PropertyMetadata(false));
        public static readonly DependencyProperty OverwritePromptProperty =
            DependencyProperty.Register("OverwritePrompt", typeof(bool), typeof(SaveFileDialogService), new PropertyMetadata(true));
        public static readonly DependencyProperty DefaultExtProperty =
            DependencyProperty.Register("DefaultExt", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty DefaultFileNameProperty =
            DependencyProperty.Register("DefaultFileName", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(string), typeof(SaveFileDialogService), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty FilterIndexProperty =
            DependencyProperty.Register("FilterIndex", typeof(int), typeof(SaveFileDialogService), new PropertyMetadata(1));
        public bool CreatePrompt {
            get { return (bool)GetValue(CreatePromptProperty); }
            set { SetValue(CreatePromptProperty, value); }
        }
        public bool OverwritePrompt {
            get { return (bool)GetValue(OverwritePromptProperty); }
            set { SetValue(OverwritePromptProperty, value); }
        }
        public string DefaultExt {
            get { return (string)GetValue(DefaultExtProperty); }
            set { SetValue(DefaultExtProperty, value); }
        }
        public string DefaultFileName {
            get { return (string)GetValue(DefaultFileNameProperty); }
            set { SetValue(DefaultFileNameProperty, value); }
        }
        public string Filter {
            get { return (string)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }
        public int FilterIndex {
            get { return (int)GetValue(FilterIndexProperty); }
            set { SetValue(FilterIndexProperty, value); }
        }

        ISaveFileDialog SaveFileDialog { get { return (ISaveFileDialog)GetFileDialog(); } }
        public SaveFileDialogService() {
            CheckFileExists = false;
        }
        protected override object CreateFileDialog() {
            return new SaveFileDialog();
        }
        protected override IFileDialog CreateFileDialogAdapter() {
            return new SaveFileDialogAdapter((SaveFileDialog)CreateFileDialog());
        }
        protected override void InitFileDialog() {
            SaveFileDialog.CreatePrompt = CreatePrompt;
            SaveFileDialog.OverwritePrompt = OverwritePrompt;
            SaveFileDialog.FileName = DefaultFileName;
            SaveFileDialog.DefaultExt = DefaultExt;
            SaveFileDialog.Filter = Filter;
            SaveFileDialog.FilterIndex = FilterIndex;
        }
        protected override List<FileInfoWrapper> GetFileInfos() {
            List<FileInfoWrapper> res = new List<FileInfoWrapper>();
            foreach(string fileName in SaveFileDialog.FileNames)
                res.Add(FileInfoWrapper.Create(fileName));
            return res;
        }
        IFileInfo ISaveFileDialogService.File { get { return GetFiles().FirstOrDefault(); } }
        bool ISaveFileDialogService.ShowDialog(Action<CancelEventArgs> fileOK, string directoryName, string fileName) {
            if(directoryName != null)
                InitialDirectory = directoryName;
            if(fileName != null)
                DefaultFileName = fileName;
            var res = Show(fileOK);
            FilterIndex = SaveFileDialog.FilterIndex;
            return res;
        }
    }
}