using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System;
using System.Collections.Generic;
using File_editor.Class;
using System.Windows.Media;
using System.Text;


namespace File_editor.Frame
{
    /// <summary>
    /// Логика взаимодействия для Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public static string Newfilepath;
        public static string pathfile;
        public static string pattern = @"(?s)СекцияДокумент=Платежное поручение.*?КонецДокумента";
        public static MatchCollection matches;
        public ObservableCollection<MatchViewModel> matchViewModels;
        public ObservableCollection<string> selectedItems;
        public static List<string> a;

        public Menu()
        {
            InitializeComponent();
            matchViewModels = new ObservableCollection<MatchViewModel>();
            listView.ItemsSource = matchViewModels;
            selectedItems = new ObservableCollection<string>();
            a = new List<string>();
            upList.IsEnabled = false;
            BtnCreateNewFile.IsEnabled = false;
        }
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            upList.IsEnabled = true;
            BtnCreateNewFile.IsEnabled = true;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "выберите файл";
            ofd.Filter = "Text|*.txt|All|*.*";
            ofd.InitialDirectory = @"C:\";
            // Отображение диалогового окна и ожидание результата
            bool? result = ofd.ShowDialog();

            if (result == true)
            {
                pathfile = ofd.FileName;

                ConvertToUtf8(pathfile);


                MessageBox.Show("Файл выбран");

                //очистка коллекции 
                matchViewModels.Clear();
                // обращение к бибилиотеки 
                string doc = File.ReadAllText(pathfile);
                // взаимодействие с регулярными выражениями 
                matches = Regex.Matches(doc, pattern, RegexOptions.Multiline);
                // Добавление найденных совпадений в коллекцию моделей данных
                foreach (string match in ShowSupplier(matches))
                {
                    matchViewModels.Add(new MatchViewModel(match));
                }
            }
        }
        private static void ConvertToUtf8(string pathToFile)
        {
            string tempFilePath = Path.Combine(Path.GetDirectoryName(pathToFile), "temp_file.tmp");

            // Определяем кодировку файла
            Encoding encoding = GetFileEncoding(pathToFile);

            if (encoding != Encoding.UTF8) // Если кодировка не UTF-8
            {
                // Читаем содержимое файла с текущей кодировкой
                string fileContent;
                using (var reader = new StreamReader(pathToFile, encoding))
                {
                    fileContent = reader.ReadToEnd();
                }

                // Записываем содержимое файла во временный файл с кодировкой UTF-8
                using (var writer = new StreamWriter(tempFilePath, false, Encoding.UTF8))
                {
                    writer.Write(fileContent);
                }

                // Заменяем исходный файл на временный
                File.Delete(pathToFile);
                File.Move(tempFilePath, pathToFile);
            }
        }
        private static Encoding GetFileEncoding(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                byte[] buffer = new byte[4];
                fs.Read(buffer, 0, 4);

                if (buffer[0] <= 0x7F) // ASCII
                {
                    if (buffer[1] <= 0x7F && buffer[2] <= 0x7F && buffer[3] <= 0x7F)
                    {
                        return Encoding.Default; // ANSI encoding
                    }
                }
            }

            return Encoding.UTF8; // default to UTF-8
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.IsChecked == true)
            {
                MatchViewModel matchViewModel = checkBox.DataContext as MatchViewModel;
                if (matchViewModel != null)
                {
                    selectedItems.Add(matchViewModel.Text); // Добавление выбранного элемента в другой список
                }
            }
        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox != null && checkBox.IsChecked == false)
            {
                MatchViewModel matchViewModel = checkBox.DataContext as MatchViewModel;
                if (matchViewModel != null)
                {
                    selectedItems.Remove(matchViewModel.Text); // Удаление отмененного выбранного элемента из другого списка
                }
            }
        }
        public static List<string> ShowSupplier(MatchCollection matches)
        {

            string doc = File.ReadAllText(pathfile);
            matches = Regex.Matches(doc, pattern, RegexOptions.Multiline);

            foreach (Match match in matches)
            {
                string section = match.Value;
                string platelshik1Value = Regex.Match(section, @"(?<=Плательщик1\s*=\s*).*").Value.Trim();
                a.Add(platelshik1Value);
            }
            return a;
        }


        public void All_Elemnt_Check(object sender, RoutedEventArgs e)
        {
            // Проходим по всем элементам ListView
            foreach (var item in listView.Items)
            {
                // Находим контейнер элемента ListView
                var container = listView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;

                // Находим CheckBox в контейнере элемента
                var checkBox = FindChild<CheckBox>(container);

                // Если найден CheckBox, устанавливаем его значение в true
                if (checkBox != null)
                {
                    checkBox.IsChecked = true;
                }
            }
        }
        public void All_Elemnt_UnCheck(object sender, RoutedEventArgs e)
        {
            // Проходим по всем элементам ListView
            foreach (var item in listView.Items)
            {
                // Находим контейнер элемента ListView
                var container = listView.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;

                // Находим CheckBox в контейнере элемента
                var checkBox = FindChild<CheckBox>(container);

                // Если найден CheckBox, устанавливаем его значение в true
                if (checkBox != null)
                {
                    checkBox.IsChecked = false;
                }
            }
        }

        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;

            int count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child == null) continue;

                T found = child as T;
                if (found != null)
                    return found;

                found = FindChild<T>(child);
                if (found != null)
                    return found;
            }

            if (parent is MenuItem menuItem)
            {
                count = menuItem.Items.Count;
                for (int i = 0; i < count; i++)
                {
                    DependencyObject child = menuItem.ItemContainerGenerator.ContainerFromIndex(i);

                    if (child == null) continue;

                    T found = child as T;
                    if (found != null)
                        return found;

                    found = FindChild<T>(child);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }
        public void CreateNewFile(object sender, RoutedEventArgs e)
        {
            upList.IsEnabled = false;
            BtnCreateNewFile.IsEnabled = false;
            try
            {
                SaveFileDialog ofd = new SaveFileDialog();
                ofd.Title = "выберите файл";
                ofd.Filter = "Text|*.txt|All|*.*";
                ofd.InitialDirectory = @"C:\";

                bool? result = ofd.ShowDialog();

                if (result == true)
                {
                    Newfilepath = ofd.FileName;

                    // Определяем кодировку исходного файла
                    Encoding srcEncoding = GetFileEncoding(pathfile);

                    // Читаем исходный файл в кодировке srcEncoding
                    string doc;
                    using (StreamReader reader = new StreamReader(pathfile, srcEncoding))
                    {
                        doc = reader.ReadToEnd();
                    }

                    string cleanedDoc = Regex.Replace(doc, pattern, match =>
                    {
                        string str = match.Value;

                        string platelshik1 = Regex.Match(str, @"(?<=Плательщик1\s*=\s*).*").Value.Trim();

                        if (selectedItems.Contains(platelshik1))
                        {
                            // Если значение содержится в списке, то удаляем раздел целиком
                            return string.Empty;
                        }
                        else
                        {
                            // Иначе, оставляем раздел без изменений
                            return match.Value;
                        }

                    }, RegexOptions.Multiline);

                    cleanedDoc = Regex.Replace(cleanedDoc, @"^\s*$\n|\r", "", RegexOptions.Multiline);

                    // Смещение следующих значений на пустую строку
                    cleanedDoc = cleanedDoc.Replace("\n\na", "\na");

                    // Записываем очищенные данные в файл с кодировкой ANSI
                    File.WriteAllText(Newfilepath, cleanedDoc, Encoding.ASCII);

                    matchViewModels.Clear();
                    MessageBox.Show("Файл создан");
                }
                else
                {
                    MessageBox.Show("отмена операции");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show("Ошибка при создание файла : " + ex.Message);
            }
        }
    }

  }
