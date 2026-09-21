using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        private string currentNumber = "";
        private double firstNumber = 0;
        private string operation = "";
        private bool newNumber = true;
        private string logFile = "log.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadHistory();
            StandardMode_Click(null, null);
        }

        private void StandardMode_Click(object sender, RoutedEventArgs e)
        {
            StandardButtons.Visibility = Visibility.Visible;
            EngineerButtons.Visibility = Visibility.Collapsed;
            Background = new SolidColorBrush(Colors.WhiteSmoke);
            DisplayTextBox.Background = new SolidColorBrush(Colors.White);
            DisplayTextBox.Foreground = new SolidColorBrush(Colors.Black);
            HistoryListBox.Background = new SolidColorBrush(Colors.White);
            ClearAll();
            this.Height = 480;
        }

        private void EngineerMode_Click(object sender, RoutedEventArgs e)
        {
            StandardButtons.Visibility = Visibility.Collapsed;
            EngineerButtons.Visibility = Visibility.Visible;
            Background = new SolidColorBrush(Color.FromRgb(40, 40, 50));
            DisplayTextBox.Background = new SolidColorBrush(Color.FromRgb(60, 60, 70));
            DisplayTextBox.Foreground = new SolidColorBrush(Colors.LightGreen);
            HistoryListBox.Background = new SolidColorBrush(Color.FromRgb(50, 50, 60));
            HistoryListBox.Foreground = new SolidColorBrush(Colors.LightGray);
            ClearAll();
            this.Height = 540;
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string num = btn.Content.ToString();
            if (newNumber)
            {
                currentNumber = "";
                newNumber = false;
            }
            currentNumber += num;
            DisplayTextBox.Text = currentNumber;
        }

        private void Operation_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string op = btn.Content.ToString();
            if (currentNumber != "")
            {
                firstNumber = double.Parse(currentNumber);
                operation = op;
                newNumber = true;
            }
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            if (operation == "" || currentNumber == "")
                return;
            double secondNumber = double.Parse(currentNumber);
            double result = 0;
            string expression = "";
            if (operation == "+")
            {
                result = firstNumber + secondNumber;
                expression = $"{firstNumber} + {secondNumber} = {result}";
            }
            else if (operation == "-")
            {
                result = firstNumber - secondNumber;
                expression = $"{firstNumber} - {secondNumber} = {result}";
            }
            else if (operation == "*")
            {
                result = firstNumber * secondNumber;
                expression = $"{firstNumber} * {secondNumber} = {result}";
            }
            else if (operation == "/")
            {
                if (secondNumber != 0)
                {
                    result = firstNumber / secondNumber;
                    expression = $"{firstNumber} / {secondNumber} = {result}";
                }
                else
                {
                    DisplayTextBox.Text = "Ошибка";
                    MessageBox.Show("Ошибка: Деление на ноль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (operation == "xʸ")
            {
                result = Math.Pow(firstNumber, secondNumber);
                expression = $"{firstNumber} ^ {secondNumber} = {result}";
            }
            DisplayTextBox.Text = result.ToString();
            AddToHistory(expression);
            SaveToLog(expression);
            currentNumber = result.ToString();
            operation = "";
            newNumber = true;
        }

        private void Function_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string func = btn.Content.ToString();
            if (currentNumber == "")
                return;
            double num = double.Parse(currentNumber);
            double result = 0;
            string expression = "";
            if (func == "x²")
            {
                result = num * num;
                expression = $"{num}² = {result}";
            }
            else if (func == "√")
            {
                if (num >= 0)
                {
                    result = Math.Sqrt(num);
                    expression = $"√{num} = {result}";
                }
                else
                {
                    DisplayTextBox.Text = "Ошибка";
                    MessageBox.Show("Ошибка: Корень из отрицательного числа!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (func == "1/x")
            {
                if (num != 0)
                {
                    result = 1 / num;
                    expression = $"1/{num} = {result}";
                }
                else
                {
                    DisplayTextBox.Text = "Ошибка";
                    MessageBox.Show("Ошибка: Деление на ноль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else if (func == "sin")
            {
                result = Math.Sin(num * Math.PI / 180);
                expression = $"sin({num}°) = {result}";
            }
            else if (func == "cos")
            {
                result = Math.Cos(num * Math.PI / 180);
                expression = $"cos({num}°) = {result}";
            }
            DisplayTextBox.Text = result.ToString();
            AddToHistory(expression);
            SaveToLog(expression);
            currentNumber = result.ToString();
            newNumber = true;
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            if (currentNumber == "")
                return;
            double num = double.Parse(currentNumber);
            double result = num / 100;
            string expression = $"{num}% = {result}";
            DisplayTextBox.Text = result.ToString();
            AddToHistory(expression);
            SaveToLog(expression);
            currentNumber = result.ToString();
            newNumber = true;
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            currentNumber = "";
            newNumber = true;
            DisplayTextBox.Text = "0";
        }

        private void ClearAll()
        {
            currentNumber = "";
            firstNumber = 0;
            operation = "";
            newNumber = true;
            DisplayTextBox.Text = "0";
        }

        private void AddToHistory(string expression)
        {
            HistoryListBox.Items.Insert(0, expression);
            if (HistoryListBox.Items.Count > 15)
                HistoryListBox.Items.RemoveAt(15);
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HistoryListBox.Items.Clear();
                if (File.Exists(logFile))
                {
                    File.WriteAllText(logFile, "");
                    MessageBox.Show("Лог-файл успешно очищен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveToLog(string expression)
        {
            try
            {
                string entry = DateTime.Now.ToString("HH:mm:ss") + " - " + expression;
                File.AppendAllText(logFile, entry + Environment.NewLine);
            }
            catch { }
        }

        private void LoadHistory()
        {
            try
            {
                if (File.Exists(logFile))
                {
                    string[] lines = File.ReadAllLines(logFile);
                    for (int i = lines.Length - 1; i >= 0; i--)
                    {
                        if (!string.IsNullOrWhiteSpace(lines[i]))
                            HistoryListBox.Items.Add(lines[i]);
                    }
                }
            }
            catch { }
        }
    }
}