using Prism.Commands;
using Prism.Mvvm;

namespace BlankApp1.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Hello Prism 9!";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public DelegateCommand ChangeTitleCommand { get; }

        public MainWindowViewModel()
        {
            ChangeTitleCommand = new DelegateCommand(ExcuteChangeTitle);
            ClickMeCommand = new DelegateCommand(Execute, CanExecute);
        }
        private void ExcuteChangeTitle()
        {
            Title = "Title Changed!";
        }

        public bool _isActionEnabled;
        public bool IsActionEnabled
        {
            get { return _isActionEnabled; }
            set {
                if (SetProperty(ref _isActionEnabled, value))
                {
                    ClickMeCommand.RaiseCanExecuteChanged();
                } 
            }
        }

        public DelegateCommand ClickMeCommand { get; }

        private void Execute() => Title = "Button Clicked!";
        private bool CanExecute() => IsActionEnabled;
    }
}
