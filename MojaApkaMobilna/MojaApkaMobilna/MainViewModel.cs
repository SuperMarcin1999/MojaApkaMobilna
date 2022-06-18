using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace MojaApkaMobilna
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        private string _name;
        private string _password;
        private string _topText;

        public ObservableCollection<Person> Persons { get; } = new ObservableCollection<Person>();
        public string TopText
        {
            get => _topText;
            set
            {
                _topText = value;
                OnPropertyChanged(nameof(TopText));
            }
        }
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
                OnClickCommand.ChangeCanExecute();
            }
        }
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
                OnClickCommand.ChangeCanExecute();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public Command OnClickCommand { get; }

        public Command OnClickCommandOdbierzBtn { get; }


        public MainViewModel()
        {
           // Persons.Add(new Person("dsadsada","Fasffsa"));
            OnClickCommand = new Command(Button_Clicked, Validate);
            OnClickCommandOdbierzBtn = new Command(ButtonOdbierz_Clicked);
        }
        private void Button_Clicked()
        {
            TopText = "Wpisane imie: " + Name + ", Haslo: " + Password;

            try
            {
                Person person = new Person(Name, Password);
                Add(person);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
                TopText = "Nie powiódł się zapis nowego użytkownika";
            }
        }

        private void ButtonOdbierz_Clicked()
        {
            try
            {
                IEnumerable<Person> collectionResultPersons = GetAll();
                foreach (var person in collectionResultPersons)
                {
                    Console.WriteLine(person.Name);
                    Console.WriteLine(person.Password);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                TopText = "Problem z pobraniem danych z API";
            }
        }

        private IEnumerable<Person> GetAll()
        {
            HttpClient httpClient = new HttpClient();

            var response = httpClient.GetAsync($"http://10.0.2.2:5262/Person").GetAwaiter().GetResult();
            
            response.EnsureSuccessStatusCode();

            IEnumerable<Person> _persons = JsonSerializer.DeserializeAsync<IEnumerable<Person>>(
                response.Content.ReadAsStreamAsync().Result,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }).GetAwaiter().GetResult();

            //foreach (var item in _persons)
            //{
            //    Console.WriteLine(item.Name);
            //    Console.WriteLine(item.Password);
            //}

            return _persons;
        }
        private async Task Add(Person person)
        {
            HttpClient httpClient = new HttpClient();

            var requestContent = new StringContent(JsonSerializer.Serialize(person),
                Encoding.UTF8,
                "application/json");

            var response = await httpClient.PostAsync("http://10.0.2.2:5262/Person", requestContent);

            response.EnsureSuccessStatusCode();
        }
        private async Task Fetch(int index)
        {
            HttpClient httpClient = new HttpClient();

            var response = await httpClient.GetAsync($"http://10.0.2.2:5262/Person/{index}");

            response.EnsureSuccessStatusCode();

            IEnumerable<Person> persons = await JsonSerializer.DeserializeAsync<IEnumerable<Person>>(
                response.Content.ReadAsStreamAsync().Result,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            foreach (var item in persons)
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(item.Password);
            }
        }


        /* Informowanie widoku o zmianach */
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool Validate() => !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Password);
        
    }
}
