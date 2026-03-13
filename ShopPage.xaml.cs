using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data.Entity; // Для работы .Include()
using System.Windows.Media;

namespace Юмагулов_Глазки_save
{

    public partial class ShopPage : Page
    {
        private int _currentPage = 1;
        private int _maxPage = 1;
        private int page_max = 10;
        private List<Agent> _allAgentsList = new List<Agent>();
        private Юмагуловглазки2Entities _db = new Юмагуловглазки2Entities();

        public ShopPage()
        {
            InitializeComponent();
            var alltypes = _db.AgentType.ToList();
            alltypes.Insert(0, new AgentType { Title = "Все типы" });
            ComboFilter.ItemsSource = alltypes;
            ComboFilter.SelectedIndex = 0;
            ComboSort.SelectedIndex = 0;
            UpdateData();
        }

        private void LoadData()
        {
            try
            {
                var list = _db.Agent.Include(a => a.AgentType).ToList();

                AgentListView.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UpdateData()
        {
            var currentAgents = _db.Agent
                .Include(a => a.AgentType)
                .Include("ProductSale.Product")
                .ToList();

            string searchText = TBoxSearch.Text.ToLower().Replace(" ", "");
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                currentAgents = currentAgents.Where(p =>
                    p.Title.ToLower().Contains(searchText) ||
                    p.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Contains(searchText)
                ).ToList();
            }

            if (ComboFilter.SelectedIndex > 0)
            {
                var selectedType = ComboFilter.SelectedItem as AgentType;
                currentAgents = currentAgents.Where(p => p.AgentTypeID == selectedType.ID).ToList();
            }

            switch (ComboSort.SelectedIndex)
            {
                case 1: currentAgents = currentAgents.OrderBy(p => p.Title).ToList(); break;
                case 2: currentAgents = currentAgents.OrderByDescending(p => p.Title).ToList(); break;
            }

            _allAgentsList = currentAgents;

            _maxPage = (int)Math.Ceiling(_allAgentsList.Count / (double)page_max);
            if (_maxPage == 0) _maxPage = 1;
            if (_currentPage > _maxPage) _currentPage = _maxPage;

            AgentListView.ItemsSource = _allAgentsList.Skip((_currentPage - 1) * page_max).Take(page_max).ToList();

            UpdatePagingControls();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Manager.MainFrame != null)
                Manager.MainFrame.Navigate(new AddEditPage());
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateData();
        }

        private void ComboFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }

        private void ComboSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateData();
        }
        private void UpdatePagingControls()
        {
            TextBlockPageInfo.Text = $"{_currentPage} из {_maxPage}";
            StackPanelPages.Children.Clear();

            for (int i = 1; i <= _maxPage; i++)
            {
                Button btn = new Button
                {
                    Content = i.ToString(),
                    Width = 30,
                    Height = 30,
                    Margin = new Thickness(2),
                    Padding = new Thickness(5, 0, 5, 0),
                    Background = i == _currentPage ? Brushes.LightGray : Brushes.White
                };
                int pageNum = i;
                btn.Click += (s, e) => { _currentPage = pageNum; UpdateData(); };
                StackPanelPages.Children.Add(btn);
            }
        }
        private void BtnPrev_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdateData();
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _maxPage)
            {
                _currentPage++;
                UpdateData();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var selectedAgent = (sender as Button).DataContext as Agent;
            Manager.MainFrame.Navigate(new AddEditPage(selectedAgent));
        }
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                _db.ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                UpdateData();
            }
        }
    }
}