using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Юмагулов_Глазки_save
{

    public partial class AddEditPage : Page
    {
        private Agent _currentAgent = new Agent();
        private Юмагуловглазки2Entities _db = new Юмагуловглазки2Entities();
        public AddEditPage(Agent selectedAgent = null)
        {
            InitializeComponent();

            if (selectedAgent != null)
            {
                _currentAgent = _db.Agent.FirstOrDefault(p => p.ID == selectedAgent.ID);
            }
            else
            {
                _currentAgent = new Agent();
            }

            DataContext = _currentAgent;
            ComboType.ItemsSource = _db.AgentType.ToList();
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentAgent.Title))
                errors.AppendLine("Укажите наименование агента");
            if (_currentAgent.AgentType == null)
                errors.AppendLine("Выберите тип агента");
            if (_currentAgent.Priority < 0)
                errors.AppendLine("Приоритет должен быть положительным числом");
            if (string.IsNullOrWhiteSpace(_currentAgent.INN))
                errors.AppendLine("Укажите ИНН");
            if (string.IsNullOrWhiteSpace(_currentAgent.KPP))
                errors.AppendLine("Укажите КПП");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }

            if (_currentAgent.ID == 0)
                _db.Agent.Add(_currentAgent);

            try
            {
                _db.SaveChanges();
                MessageBox.Show("Данные сохранены");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_currentAgent.AgentType == null)
            {
                MessageBox.Show("Выберите тип агента!");
                return;
            }
            if(_currentAgent.Priority < 0)
            {
                MessageBox.Show("Приоритет не может быть отрицательным!");
                return;
            }

            if (_currentAgent.ID == 0)
                _db.Agent.Add(_currentAgent);

            try
            {
                _db.SaveChanges();
                MessageBox.Show("Информация сохранена!");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnChangeLogo_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg";

            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    // 1. Получаем путь к папке проекта (где лежит .exe файл)
                    // Обычно это bin\Debug, поэтому выходим на пару уровней выше, чтобы попасть в корень проекта
                    string projectDirectory = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
                    string folderPath = System.IO.Path.Combine(projectDirectory, "agents");

                    // 2. Если папки agents нет — создаем её
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    // 3. Формируем новое имя файла и путь копирования
                    string fileName = System.IO.Path.GetFileName(fileDialog.FileName);
                    string destPath = System.IO.Path.Combine(folderPath, fileName);

                    // 4. Копируем файл (true — перезаписать, если такой уже есть)
                    File.Copy(fileDialog.FileName, destPath, true);

                    // 5. Записываем в БД ОТНОСИТЕЛЬНЫЙ путь
                    // Именно так картинки обычно хранятся в твоей базе (судя по SQL)
                    _currentAgent.Logo = "\\agents\\" + fileName;

                    // 6. Обновляем картинку на форме
                    AgentLogo.Source = new BitmapImage(new Uri(destPath));

                    MessageBox.Show("Логотип успешно загружен и сохранен в проект!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при копировании файла: " + ex.Message);
                }
            }
        }

        private void BtnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            if (_currentAgent.ProductSale.Count > 0)
            {
                MessageBox.Show("Удаление запрещено, так как у агента есть информация о реализации продукции");
                return;
            }

            if (MessageBox.Show("Вы точно хотите удалить этого агента?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем связанные истории, если они есть
                    _db.AgentPriorityHistory.RemoveRange(_currentAgent.AgentPriorityHistory);
                    _db.Shop.RemoveRange(_currentAgent.Shop);

                    _db.Agent.Remove(_currentAgent);
                    _db.SaveChanges();
                    MessageBox.Show("Агент удален");
                    Manager.MainFrame.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
        }
    }
}
