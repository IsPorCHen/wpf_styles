using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DataBinding.Pages
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(SpecialisationTextBox.Text))
            {
                MessageBox.Show("Все поля обязательны для заполнения!");
                return;
            }

            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return;
            }

            if (PasswordBox.Password.Length < 4)
            {
                MessageBox.Show("Пароль должен содержать минимум 4 символа!");
                return;
            }

            if (IsDoctorExists(LastNameTextBox.Text, NameTextBox.Text, MiddleNameTextBox.Text))
            {
                MessageBox.Show("Пользователь с такими ФИО уже существует!");
                return;
            }

            var random = new System.Random();
            int newId;
            bool idExists;

            do
            {
                newId = random.Next(10000, 99999);
                idExists = File.Exists($"D_{newId}.json");
            } while (idExists);

            var newDoctor = new Doctor
            {
                Id = newId,
                LastName = LastNameTextBox.Text,
                Name = NameTextBox.Text,
                MiddleName = MiddleNameTextBox.Text,
                Specialisation = SpecialisationTextBox.Text,
                Password = PasswordBox.Password
            };

            SaveDoctorToJson(newDoctor);
            MessageBox.Show($"Доктор зарегистрирован с ID: {newDoctor.Id}");

            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool IsDoctorExists(string lastName, string name, string middleName)
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "D_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var doctorData = JsonSerializer.Deserialize<Doctor>(jsonString);

                    if (doctorData.LastName == lastName &&
                        doctorData.Name == name &&
                        doctorData.MiddleName == middleName)
                    {
                        return true;
                    }
                }
                catch
                {
                    continue;
                }
            }
            return false;
        }

        private void SaveDoctorToJson(Doctor doctor)
        {
            var doctorData = new
            {
                doctor.Id,
                doctor.LastName,
                doctor.Name,
                doctor.MiddleName,
                doctor.Specialisation,
                doctor.Password
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(doctorData, options);
            string fileName = $"D_{doctor.Id}.json";
            File.WriteAllText(fileName, jsonString, Encoding.UTF8);
        }
    }
}