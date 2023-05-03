using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace File_editor.Class
{
    public class MatchViewModel
    {

        private string _text;
        private bool _isChecked;
        public ObservableCollection<string> ComboBoxItems { get; set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged("Text");
            }
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }
       

        public MatchViewModel(string text)
        {
            Text = text;
            IsChecked = false;
            ComboBoxItems = new ObservableCollection<string>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}
