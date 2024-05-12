using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;


namespace frstpracticekos
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            string baseCurrency = BaseCurrencyTextBox.Text.ToUpper();
            string targetCurrency = TargetCurrencyTextBox.Text.ToUpper();
            decimal amount;

            if (!decimal.TryParse(AmountTextBox.Text, out amount))
            {
                MessageBox.Show("Введите корректное число для суммы.");
                return;
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync($"https://api.exchangerate-api.com/v4/latest/{baseCurrency}");
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();

                    // Преобразуем JSON-ответ в объект JObject
                    JObject exchangeRates = JObject.Parse(responseBody);

                    // Получаем курс для целевой валюты
                    decimal exchangeRate = (decimal)exchangeRates["rates"][targetCurrency];

                    // Вычисляем конвертированную сумму
                    decimal convertedAmount = amount * exchangeRate;

                    ConvertedAmountLabel.Content = $"{amount} {baseCurrency} = {convertedAmount} {targetCurrency}";
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
                }
            }
        }
    }
}
