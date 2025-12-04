using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Globalization;

namespace DataBinding.Pages
{
    public partial class EditPatientPage : Page, INotifyPropertyChanged
    {
        private Patient _currentPatient;
        private ObservableCollection<Patient> _patients;
        private string _birthday;

        public Patient CurrentPatient => _currentPatient;

        public string Birthday
        {
            get => _birthday;
            set
            {
                _birthday = value;
                OnPropertyChanged(nameof(Birthday));

                if (DateTime.TryParseExact(value, "dd.MM.yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out DateTime parsedDate))
                {
                    BirthDate = parsedDate;
                }
                else
                {
                    BirthDate = null;
                }
            }
        }
        public DateTime? BirthDate { get; set; }

        public EditPatientPage(Patient patient, ObservableCollection<Patient> patients)
        {
            _currentPatient = patient;
            _patients = patients;
            _birthday = patient.Birthday;

            InitializeComponent();
            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_currentPatient.LastName) ||
                string.IsNullOrWhiteSpace(_currentPatient.Name) ||
                string.IsNullOrWhiteSpace(_currentPatient.MiddleName))
            {
                MessageBox.Show("Заполните все обязательные поля!");
                return;
            }

            _currentPatient.Birthday = Birthday;

            if (BirthDate.HasValue)
            {
                _currentPatient.Birthday = BirthDate.Value.ToString("dd.MM.yyyy");
            }

            SavePatientToJson(_currentPatient);
            var index = _patients.IndexOf(_currentPatient);
            if (index >= 0)
            {
                _patients[index] = _currentPatient;
            }
            MessageBox.Show("Информация о пациенте обновлена!");
            NavigationService.GoBack();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}