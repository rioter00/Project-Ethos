using System;
using System.Windows.Input;
using Ethos.Launcher.Infrastructure;
using Ethos.Launcher.ViewModels.PvP;

namespace Ethos.Launcher.ViewModels
{
    public class PvPViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        private string _playerDisplayName;

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

        public string PlayerDisplayName
        {
            get { return _playerDisplayName; }
            set
            {
                _playerDisplayName = value;
                OnPropertyChanged();
            }
        }

        public PvPViewModel()
        {
            CurrentViewModel = new PlayerViewModel();
            NavigationCommand = new RelayCommand<string>(OnNavigation);

            PlayerDisplayName = "SKT Marin";
        }

        private void OnNavigation(string destination)
        {
            switch (destination)
            {
                case "play":
                    CurrentViewModel = new PlayViewModel();
                    break;
                case "player":
                    CurrentViewModel = new PlayerViewModel();
                    break;
                case "customize":
                    CurrentViewModel = new CustomizeViewModel();
                    break;
                case "statistics":
                    CurrentViewModel = new StatisticsViewModel();
                    break;
                default:
                    throw new ArgumentException($"Failed to navigate to '{destination}', the destination was not recognized");
            }
        }
    }
}