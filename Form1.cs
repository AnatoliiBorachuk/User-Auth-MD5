using System;
using System.Windows.Forms;

namespace UserAuth
{
    public partial class UserAuthForm : Form
    {
        public const int MIN_PASS_LENGTH = 3;
        public const int MAX_PASS_LENGTH = 8;

        private UserDatabase users;

        public UserAuthForm()
        {
            InitializeComponent();
            users = new UserDatabase();
        }

        private void showPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (showPassword.Checked)
                password.UseSystemPasswordChar = false;
            else
                password.UseSystemPasswordChar = true;
        }

        private string loginText =>
            login.Text;
        private string passwordText =>
            password.Text;

        private bool ValidatePassword =>
            passwordText.Length >= MIN_PASS_LENGTH &&
            passwordText.Length <= MAX_PASS_LENGTH;

        private bool ValidateLogin =>
            loginText.Length > MIN_PASS_LENGTH;

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (ValidateAllFields() == false)
                return;

            if(users.TryLogin(loginText, passwordText))
                MessageBox.Show($"Success login: {loginText}");
            else
                MessageBox.Show($"Login failed");
        }

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (ValidateAllFields() == false)
                return;

            if(users.ContainsUser(loginText))
            {
                MessageBox.Show($"User with login {loginText} is registred. Use login");
                return;
            }
            else
            {
                users.AddUser(new UserCredentials(loginText, UserDatabase.GetMd5Hash(passwordText)));
                MessageBox.Show($"User with login {loginText} is success registred");
            }
        }

        private bool ValidateAllFields ()
        {
            if (string.IsNullOrEmpty(loginText) || string.IsNullOrEmpty(passwordText))
            {
                MessageBox.Show($"Fill all fields");
                return false;
            }

            if (ValidatePassword == false)
            {
                MessageBox.Show($"Wrong password. Password length must be in range [{MIN_PASS_LENGTH}, {MAX_PASS_LENGTH}]");
                return false;
            }

            if (ValidateLogin == false)
            {
                MessageBox.Show($"Wrong login. Login length must be greater than {MIN_PASS_LENGTH}");
                return false;
            }

            return true;
        }
    }
}
