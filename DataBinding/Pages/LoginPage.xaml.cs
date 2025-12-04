using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace DataBinding.Pages
{
    public partial class LoginPage : Page, INotifyPropertyChanged
    {
        private string _loginDoctorId = "";

        public string LoginDoctorId
        {
            get => _loginDoctorId;
            set
            {
                _loginDoctorId = value;
                OnPropertyChanged(nameof(LoginDoctorId));
            }
        }

        public LoginPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginDoctorId))
            {
                MessageBox.Show("Введите ID доктора!");
                return;
            }

            if (!int.TryParse(LoginDoctorId, out int doctorId))
            {
                MessageBox.Show("Введите корректный ID доктора! ID должен быть числом.");
                return;
            }

            string fileName = $"D_{doctorId}.json";
            if (!File.Exists(fileName))
            {
                MessageBox.Show("Доктор с таким ID не найден!");
                return;
            }

            try
            {
                string jsonString = File.ReadAllText(fileName, Encoding.UTF8);
                var doctorData = JsonSerializer.Deserialize<Doctor>(jsonString);

                if (doctorData.Password == loginPassword.Password)
                {
                    NavigationService.Navigate(new DoctorMainPage(doctorData));
                }
                else
                {
                    MessageBox.Show("Неверный пароль!");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}