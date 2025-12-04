using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace DataBinding.Pages
{
    public partial class CreatePatientPage : Page
    {
        private ObservableCollection<Patient> _patients;

        public CreatePatientPage(ObservableCollection<Patient> patients)
        {
            InitializeComponent();
            _patients = patients;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(MiddleNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(BirthDatePicker.Text))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            if (IsPatientExists(LastNameTextBox.Text, NameTextBox.Text, MiddleNameTextBox.Text, BirthDatePicker.Text))
            {
                MessageBox.Show("Пациент с такими данными уже существует!");
                return;
            }

            var random = new System.Random();
            int newId;
            bool idExists;

            do
            {
                newId = random.Next(1000000, 9999999);
                idExists = File.Exists($"P_{newId}.json");
            } while (idExists);

            var newPatient = new Patient
            {
                Id = newId,
                LastName = LastNameTextBox.Text,
                Name = NameTextBox.Text,
                MiddleName = MiddleNameTextBox.Text,
                Birthday = BirthDatePicker.Text,
                PhoneNumber = PhoneTextBox.Text,
                AppointmentStories = new ObservableCollection<Appointment>()
            };

            SavePatientToJson(newPatient);
            _patients.Add(newPatient);
            MessageBox.Show($"Пациент создан с ID: {newPatient.Id}");
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool IsPatientExists(string lastName, string name, string middleName, string birthday)
        {
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "P_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var patientData = JsonSerializer.Deserialize<Patient>(jsonString);

                    if (patientData.LastName == lastName &&
                        patientData.Name == name &&
                        patientData.MiddleName == middleName &&
                        patientData.Birthday == birthday)
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

        private void SavePatientToJson(Patient patient)
        {
            var patientData = new
            {
                patient.LastName,
                patient.Name,
                patient.MiddleName,
                patient.Birthday,
                patient.PhoneNumber,
                AppointmentStories = patient.AppointmentStories
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string jsonString = JsonSerializer.Serialize(patientData, options);
            string fileName = $"P_{patient.Id}.json";
            File.WriteAllText(fileName, jsonString, Encoding.UTF8);
        }
    }
}