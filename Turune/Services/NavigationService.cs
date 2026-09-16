using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;

namespace Turune.Services
{
    public class NavigationService : INotifyPropertyChanged
    {
        private readonly Stack<UserControl> _history = new();
        private UserControl _currentView;

        public UserControl CurrentView
        {
            get => _currentView;
            private set { _currentView = value; OnPropertyChanged(); }
        }

        public bool CanGoBack => _history.Count > 0;

        public void NavigateTo(UserControl view)
        {
            if (_currentView != null)
                _history.Push(_currentView);
            CurrentView = view;
        }

        public void GoBack()
        {
            if (CanGoBack)
                CurrentView = _history.Pop();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
