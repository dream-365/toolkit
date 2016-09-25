using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DirectoryMan
{
    public class UserInteractionVM : INotifyPropertyChanged
    {
        private string rootDirectory;

        private string regex;

        private string targetExpression;

        private bool selectFile;

        private bool selectFolder;

        public String RootDirectory
        {
            get
            {
                return this.rootDirectory;
            }

            set
            {
                if (value != this.rootDirectory)
                {
                    this.rootDirectory = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public String Regex { 
            get { return this.regex; } 
            set {
                if (value != this.regex)
                {
                    this.regex = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public String TargetExpression {
            get { return this.targetExpression; }
            set {
                if (value != this.targetExpression)
                {
                    this.targetExpression = value;
                    NotifyPropertyChanged();
                }
            } 
        }

        public bool SelectFile
        {
            get { return this.selectFile; }
            set
            {
                if(value != this.selectFile)
                {
                    this.selectFile = value;

                    NotifyPropertyChanged();
                }
            }
        }

        public bool SelectFolder
        {
            get
            {
                return this.selectFolder;
            }
            set
            {
                if(value != this.selectFolder)
                {
                    this.selectFolder = value;

                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
