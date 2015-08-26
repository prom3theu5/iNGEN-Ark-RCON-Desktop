using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace iNGen.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<NavigationViewModel>();
            SimpleIoc.Default.Register <ScheduledCommandsViewModel>();
            SimpleIoc.Default.Register<ChatViewModel>();
            SimpleIoc.Default.Register<ConsoleViewModel>();
            SimpleIoc.Default.Register<PlayersViewModel>();
            SimpleIoc.Default.Register<HomeViewModel>();
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<SettingsViewModels.GeneralSettingsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModels.ConsoleSettingsViewModel>();
            SimpleIoc.Default.Register<SettingsViewModels.ChatSettingsViewModel>();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        
        public ConsoleViewModel Console => ServiceLocator.Current.GetInstance<ConsoleViewModel>();
        public PlayersViewModel Players => ServiceLocator.Current.GetInstance<PlayersViewModel>();
        public ChatViewModel Chat => ServiceLocator.Current.GetInstance<ChatViewModel>();
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public ScheduledCommandsViewModel Scheduled => ServiceLocator.Current.GetInstance<ScheduledCommandsViewModel>();
        public NavigationViewModel Navigation => ServiceLocator.Current.GetInstance<NavigationViewModel>();
        public HomeViewModel Home => ServiceLocator.Current.GetInstance<HomeViewModel>();
        public SettingsViewModels.GeneralSettingsViewModel GeneralSettings => ServiceLocator.Current.GetInstance<SettingsViewModels.GeneralSettingsViewModel>();
        public SettingsViewModels.ChatSettingsViewModel ChatSettings => ServiceLocator.Current.GetInstance<SettingsViewModels.ChatSettingsViewModel>();
        public SettingsViewModels.ConsoleSettingsViewModel ConsoleSettings => ServiceLocator.Current.GetInstance<SettingsViewModels.ConsoleSettingsViewModel>();

    }
}
