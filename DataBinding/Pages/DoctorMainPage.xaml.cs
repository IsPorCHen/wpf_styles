using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace DataBinding.Pages
{
    public partial class DoctorMainPage : Page, INotifyPropertyChanged
    {
        public Doctor CurrentDoctor { get; set; }
        public ObservableCollection<Patient> Patients { get; set; }
        public Patient SelectedPatient { get; set; }

        public DoctorMainPage(Doctor doctor)
        {
            InitializeComponent();
            CurrentDoctor = doctor;
            Patients = new ObservableCollection<Patient>();
            LoadPatients();
            DataContext = this;
        }

        private void LoadPatients()
        {
            Patients.Clear();
            foreach (string file in Directory.GetFiles(Directory.GetCurrentDirectory(), "P_*.json"))
            {
                try
                {
                    string jsonString = File.ReadAllText(file, Encoding.UTF8);
                    var patientData = JsonSerializer.Deserialize<Patient>(jsonString);

                    string fileName = Path.GetFileNameWithoutExtension(file);
                    patientData.Id = int.Parse(fileName.Substring(2));

                    Patients.Add(patientData);
                }
                catch
                {
                    continue;
                }
            }
        }

        private void CreatePatientButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CreatePatientPage(Patients));
        }

        private void StartAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для начала приема!");
                return;
            }
            NavigationService.Navigate(new AppointmentPage(SelectedPatient, CurrentDoctor, Patients));
        }

        private void EditInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedPatient == null)
            {
                MessageBox.Show("Выберите пациента для изменения информации!");
                return;
            }
            NavigationService.Navigate(new EditPatientPage(SelectedPatient, Patients));
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new LoginPage());
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}