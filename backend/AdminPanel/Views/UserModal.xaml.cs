using System.Windows;

namespace AdminPanel;

public partial class UserModal : Window
{
    public string Login => LoginTextBox.Text;
    public string Password => PasswordTextBox.Text;

    public UserModal(string login = "", string password = "")
    {
        InitializeComponent();
        LoginTextBox.Text = login;
        PasswordTextBox.Text = password;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}