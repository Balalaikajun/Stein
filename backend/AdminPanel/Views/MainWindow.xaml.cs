using System.Windows;
using System.Collections.ObjectModel;
using Application.DTOs.User;
using Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AdminPanel;

public partial class MainWindow : Window
{
    private readonly IUserService _userService;
    private ObservableCollection<UserGetDto> _users;

    public MainWindow()
    {
        InitializeComponent();
        _userService = App.ServiceProvider.GetRequiredService<IUserService>();
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        var users = await _userService.GetUsersAsync();
        _users = new ObservableCollection<UserGetDto>(users);
        UsersGrid.ItemsSource = _users;
    }

    private void AddUser_Click(object sender, RoutedEventArgs e)
    {
        var modal = new UserModal();
        if (modal.ShowDialog() == true)
        {
            var userPostDto = new UserPostDto(modal.Login, modal.Password);
            _userService.AddUserAsync(userPostDto).ContinueWith(async _ =>
            {
                await LoadUsersAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    private void EditUser_Click(object sender, RoutedEventArgs e)
    {
        var selectedUser = (UserGetDto)UsersGrid.SelectedItem;
        if (selectedUser == null) return;

        var modal = new UserModal(selectedUser.Login);
        if (modal.ShowDialog() == true)
        {
            var userUpdateDto = new UserUpdateDto(
                selectedUser.Id,
                modal.Login,
                modal.Password
            );
            _userService.UpdateUserAsync(userUpdateDto).ContinueWith(async _ =>
            {
                await LoadUsersAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }

    private void DeleteUser_Click(object sender, RoutedEventArgs e)
    {
        var selectedUser = (UserGetDto)UsersGrid.SelectedItem;
        if (selectedUser == null) return;

        var result = MessageBox.Show(
            $"Delete user '{selectedUser.Login}'?",
            "Confirm",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );

        if (result == MessageBoxResult.Yes)
        {
            _userService.DeleteUserAsync(selectedUser.Id).ContinueWith(async _ =>
            {
                await LoadUsersAsync();
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}