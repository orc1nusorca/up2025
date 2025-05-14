using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using PM_04_ISP32_SSE.db_files;

namespace PM_04_ISP32_SSE.pages
{
    public partial class MaterialList : Page
    {
        private const int ItemsPerPage = 15;
        private int _currentPage = 1;
        private int _totalPages = 1;
        private List<db_files.Material> _allMaterials = new List<db_files.Material>();
        private int _totalRecordsInDatabase;
        private db_files.Material _selectedMaterial;
        private bool _isEditWindowOpen = false;

        public List<int> PageNumbers { get; set; } = new List<int>();

        public MaterialList()
        {
            InitializeComponent();
            LViewServices.SelectionChanged += (s, e) =>
            {
                if (LViewServices.SelectedItems.Count > 1)
                {
                    BtnChangeMinQuantity.Visibility = Visibility.Visible;
                    BtnEditMaterial.Visibility = Visibility.Collapsed;
                    BtnDeleteMaterial.Visibility = Visibility.Collapsed;
                }
                else if (LViewServices.SelectedItems.Count > 0)
                {
                    BtnChangeMinQuantity.Visibility = Visibility.Visible;
                    BtnEditMaterial.Visibility = Visibility.Visible;
                    BtnDeleteMaterial.Visibility = Visibility.Visible;
                }
                else
                {
                    BtnChangeMinQuantity.Visibility = Visibility.Collapsed;
                    BtnEditMaterial.Visibility = Visibility.Collapsed;
                    BtnDeleteMaterial.Visibility = Visibility.Collapsed;
                }
            };
            ComboSortBy.SelectedIndex = 0;
            ComboFilterBy.SelectedIndex = 0;
            LoadMaterials();
            UpdateMaterial();
        }

        private void LoadMaterials()
        {
            _allMaterials = App.Context.Materials.ToList();
            UpdateFilterItems();
            UpdateRecordCountText();
            UpdatePagination();
        }

        private void UpdateRecordCountText()
        {

            _totalRecordsInDatabase = App.Context.Materials.Count();
            int displayedCount = _allMaterials.Count;
            TxtRecordCount.Text = $"Записей: {displayedCount} из {_totalRecordsInDatabase}";
        }


        private void UpdateMaterial()
        {
            var material = App.Context.Materials.ToList();

            // поиск
            material = material.Where(p => p.name_material.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();

            // Сортировка
            if (ComboSortBy.SelectedIndex == 1)
            {
                material = material.OrderBy(p => p.name_material).ToList();
            }
            else if (ComboSortBy.SelectedIndex == 2)
            {
                material = material.OrderByDescending(p => p.name_material).ToList();
            }
            else if (ComboSortBy.SelectedIndex == 3)
            {
                material = material.OrderBy(p => p.kol_na_sklade).ToList();
            }
            else if (ComboSortBy.SelectedIndex == 4)
            {
                material = material.OrderByDescending(p => p.kol_na_sklade).ToList();
            }
            else if (ComboSortBy.SelectedIndex == 5)
            {
                material = material.OrderBy(p => p.cena).ToList();
            }
            else if (ComboSortBy.SelectedIndex == 6)
            {
                material = material.OrderByDescending(p => p.cena).ToList();
            }
            

            // Фильтрация
            if (ComboFilterBy.SelectedIndex > 0)
            {
                string selectedType = ComboFilterBy.SelectedItem.ToString();
                material = material.Where(p => p.Type.name_type == selectedType).ToList();
            }

            _allMaterials = material;
            UpdatePagination();
            DisplayCurrentPage();
            UpdateRecordCountText();
        }

        private void UpdateFilterItems()
        {
            var allPossibleTypes = App.Context.Types.Select(t => t.name_type).OrderBy(t => t).ToList();

            var filterItems = new List<string> { "Все типы" };
            filterItems.AddRange(allPossibleTypes);

            var selectedItem = ComboFilterBy.SelectedItem?.ToString();
            ComboFilterBy.ItemsSource = filterItems;

            if (selectedItem != null && filterItems.Contains(selectedItem))
            {
                ComboFilterBy.SelectedItem = selectedItem;
            }
            else
            {
                ComboFilterBy.SelectedIndex = 0;
            }
        }

        private void UpdatePagination()
        {
            _totalPages = Math.Max(1, (int)Math.Ceiling((double)_allMaterials.Count / ItemsPerPage));

            _currentPage = Math.Min(_currentPage, _totalPages);
            if (_totalPages == 0) _currentPage = 1;

            PageNumbers = Enumerable.Range(1, _totalPages).ToList();

            PageNumbersControl.ItemsSource = PageNumbers;
        }

        private void DisplayCurrentPage()
        {
            var paginatedMaterials = _allMaterials.Skip((_currentPage - 1) * ItemsPerPage).Take(ItemsPerPage).ToList();

            LViewServices.ItemsSource = paginatedMaterials;
        }

        private void ComboSortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            UpdateMaterial();
        }

        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            _currentPage = 1;
            UpdateMaterial();
        }

        private void ComboFilterBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentPage = 1;
            UpdateMaterial();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMaterial();
        }

        private void BtnPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                DisplayCurrentPage();
            }
        }

        private void BtnNextPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                DisplayCurrentPage();
            }
        }

        private void PageNumberButton_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            _currentPage = (int)button.Content;
            DisplayCurrentPage();
        }

        private void BtnChangeMinQuantity_Click(object sender, RoutedEventArgs e)
        {
            var selected = LViewServices.SelectedItems.Cast<db_files.Material>().ToList();
            decimal maxValue = selected.Max(m => m.ostatok ?? 0);

            var dialog = new ModalQuantity(maxValue)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() == true)
            {
                foreach (var material in selected)
                {
                    material.ostatok = dialog.NewQuantity;
                }

                App.Context.SaveChanges();
                LViewServices.Items.Refresh();
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (LViewServices.SelectedItem == null)
            {
                MessageBox.Show("Выберите материал для удаления.");
                return;
            }

            _selectedMaterial = (db_files.Material)LViewServices.SelectedItem;

            // Проверка на использование материала в продукции
            if (_selectedMaterial.Material_Produkcia.Any())
            {
                MessageBox.Show("Невозможно удалить материал, так как он используется в производстве продукции.");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить материал '{_selectedMaterial.name_material}'?",
                                       "Подтверждение удаления",
                                       MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Удаляем связанные записи (поставщиков и историю)
                    var relatedSklads = App.Context.Sklads.Where(s => s.id_material == _selectedMaterial.id_material).ToList();
                    var relatedHistory = App.Context.History_UpdateCount.Where(h => h.name_material == _selectedMaterial.name_material).ToList();

                    App.Context.Sklads.RemoveRange(relatedSklads);
                    App.Context.History_UpdateCount.RemoveRange(relatedHistory);

                    // Удаляем сам материал
                    App.Context.Materials.Remove(_selectedMaterial);
                    App.Context.SaveChanges();

                    MessageBox.Show("Материал успешно удален.");
                    _allMaterials.Remove(_selectedMaterial);
                    UpdatePagination();
                    DisplayCurrentPage();
                    UpdateRecordCountText();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении материала: {ex.Message}");
                }
            }
        }
        private void EditMaterial_Click(object sender, RoutedEventArgs e)
        {
            if (LViewServices.SelectedItem == null) return;

            if (_isEditWindowOpen)
            {
                MessageBox.Show("Окно редактирования уже открыто. Закройте его перед редактированием другого материала.");
                return;
            }

            BtnDeleteMaterial.IsEnabled = false;
            BtnChangeMinQuantity.IsEnabled = false;

            _selectedMaterial = (db_files.Material)LViewServices.SelectedItem;
            var editWindow = new MaterialEdit(_selectedMaterial);
            editWindow.Closed += (s, args) =>
            {
                _isEditWindowOpen = false;
                LoadMaterials();
                BtnDeleteMaterial.IsEnabled = true;
                BtnChangeMinQuantity.IsEnabled = true;
            };
            _isEditWindowOpen = true;
            editWindow.Show();
        }

        private void AddMaterialButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isEditWindowOpen)
            {
                MessageBox.Show("Окно редактирования уже открыто. Закройте его перед созданием нового материала.");
                return;
            }

            BtnDeleteMaterial.IsEnabled = false;
            BtnChangeMinQuantity.IsEnabled = false;

            var editWindow = new MaterialEdit(null);
            editWindow.Closed += (s, args) =>
            {
                _isEditWindowOpen = false;
                LoadMaterials();
                BtnDeleteMaterial.IsEnabled = true;
                BtnChangeMinQuantity.IsEnabled = true;
            };
            _isEditWindowOpen = true;
            editWindow.Show();
        }
    }
}