using System;
using System.Windows.Input;
using Ethos.Launcher.Infrastructure;

namespace Ethos.Launcher.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigationCommand { get; private set; }

        public MainWindowViewModel()
        {
            CurrentViewModel = new HomeViewModel();
            NavigationCommand = new RelayCommand<string>(OnNavigation);
        }

        private void OnNavigation(string destination)
        {
            switch (destination)
            {
                case "home":
                    CurrentViewModel = new HomeViewModel();
                    break;
                case "pvp":
                    CurrentViewModel = new PvPViewModel();
                    break;
                case "world":
                    CurrentViewModel = new WorldViewModel();
                    break;
                case "store":
                    CurrentViewModel = new StoreViewModel();
                    break;
                case "account":
                    CurrentViewModel = new AccountViewModel();
                    break;
                default:
                    throw new ArgumentException($"Failed to navigate to '{destination}', the destination was not recognized");
            }
        }
    }
}