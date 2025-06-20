using Microsoft.Win32;
using PM_04_ISP32_SSE.db_files;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
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
using System.Runtime.Remoting.Contexts;

namespace PM_04_ISP32_SSE.pages
{
    /// <summary>
    /// Логика взаимодействия для MaterialEdit.xaml
    /// </summary>

    public partial class MaterialEdit : Window
    {
        private Material _material;
        private bool _isNewMaterial;

        public MaterialEdit(Material material)
        {
            InitializeComponent();

            if (material == null)
            {
                _material = new Material();
                _isNewMaterial = true;
                Title = "Добавление нового материала";
            }
            else
            {
                _material = App.Context.Materials.Find(material.id_material);
                _isNewMaterial = false;
                Title = $"Редактирование материала: {_material.name_material}";
            }

            LoadData();
        }

        private void LoadData()
        {
            TypeComboBox.ItemsSource = App.Context.Types.ToList();
            TypeComboBox.DisplayMemberPath = "name_type";
            TypeComboBox.SelectedValuePath = "id_type";
            TypeComboBox.SelectedValue = _material.id_type;

            SuppliersComboBox.ItemsSource = App.Context.Postavshiks.ToList();
            SuppliersComboBox.DisplayMemberPath = "name_postavshik";
            SuppliersComboBox.SelectedValuePath = "id_postavshik";

            NameTextBox.Text = _material.name_material;
            CountTextBox.Text = _material.kol_na_sklade?.ToString();
            UnitTextBox.Text = _material.ed_ismer;
            PackageTextBox.Text = _material.kol_v_upakovke?.ToString();
            MinCountTextBox.Text = _material.ostatok?.ToString();
            PriceTextBox.Text = _material.cena?.ToString();
            DescriptionTextBox.Text = _material.opisanie;

            LoadImage();

            LoadMaterialSuppliers();
        }
        private void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            string imageFolder = @"..\..\assets\image\";

            var openFileDialog = new OpenFileDialog
            {
                Filter = "Images|*.jpg;*.jpeg;*.png",
                InitialDirectory = Path.GetFullPath(imageFolder)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileName(openFileDialog.FileName);

                _material.image = $"image\\{fileName}";

                MaterialImage.Source = new BitmapImage(new Uri(openFileDialog.FileName));
            }
        }

        private void LoadImage()
        {
            if (!string.IsNullOrEmpty(_material.image))
            {
                string imagePath = Path.GetFullPath($@"..\..\assets\{_material.image}");
                if (File.Exists(imagePath))
                {
                    MaterialImage.Source = new BitmapImage(new Uri(imagePath));
                    return;
                }
            }
            MaterialImage.Source = new BitmapImage( new Uri(Path.GetFullPath(@"..\..\assets\noimg.jpg")));
        }

        private void LoadMaterialSuppliers()
        {
            try
            {
                if (_material.id_material > 0)
                {
                    SuppliersListView.ItemsSource = App.Context.Sklads.Where(s => s.id_material == _material.id_material).Include(s => s.Postavshik).ToList();
                }
                else
                {
                    SuppliersListView.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке поставщиков: {ex.Message}");
            }
        }

        private void AddSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика из списка");
                return;
            }

            // Проверяем, что цена введена корректно
            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену (положительное число)");
                return;
            }

            try
            {
                var selectedSupplier = (db_files.Postavshik)SuppliersComboBox.SelectedItem;

                if (_isNewMaterial)
                {
                    // Сохраняем материал с указанной ценой
                    _material.cena = price;
                    App.Context.Materials.Add(_material);
                    App.Context.SaveChanges();
                    _isNewMaterial = false;
                }

                // Проверяем, не добавлен ли уже этот поставщик
                if (!App.Context.Sklads.Any(s => s.id_material == _material.id_material &&
                                            s.id_postavshik == selectedSupplier.id_postavshik))
                {
                    var newSklad = new db_files.Sklad
                    {
                        id_material = _material.id_material,
                        id_postavshik = selectedSupplier.id_postavshik,
                        stoimost_ed = price // Используем цену из поля ввода
                    };

                    App.Context.Sklads.Add(newSklad);
                    App.Context.SaveChanges();

                    // Обновляем список поставщиков
                    LoadMaterialSuppliers();

                    // Обновляем цену материала
                    _material.cena = price;
                    App.Context.Entry(_material).State = EntityState.Modified;
                    App.Context.SaveChanges();
                }
                else
                {
                    MessageBox.Show("Этот поставщик уже добавлен");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка добавления поставщика: {ex.Message}");
            }
        }

        private void RemoveSupplierButton_Click(object sender, RoutedEventArgs e)
        {
            if (SuppliersListView.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика для удаления.");
                return;
            }

            var selectedSklad = (db_files.Sklad)SuppliersListView.SelectedItem;
            App.Context.Sklads.Remove(selectedSklad);
            App.Context.SaveChanges();

            LoadMaterialSuppliers();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput()) return;

            try
            {
                // Для существующего материала сохраняем старые значения перед изменением
                decimal? oldCount = null;
                decimal? newCount = null;

                if (!_isNewMaterial)
                {
                    // Получаем текущие значения из БД перед изменением
                    var materialInDb = App.Context.Materials.AsNoTracking()
                        .FirstOrDefault(m => m.id_material == _material.id_material);

                    if (materialInDb != null)
                    {
                        oldCount = materialInDb.kol_na_sklade;
                        newCount = decimal.Parse(CountTextBox.Text);
                    }
                }

                // Обновляем данные материала
                _material.name_material = NameTextBox.Text;
                _material.id_type = (int)TypeComboBox.SelectedValue;
                _material.kol_na_sklade = decimal.Parse(CountTextBox.Text);
                _material.ed_ismer = UnitTextBox.Text;
                _material.kol_v_upakovke = decimal.Parse(PackageTextBox.Text);
                _material.ostatok = decimal.Parse(MinCountTextBox.Text);
                _material.cena = decimal.Parse(PriceTextBox.Text);
                _material.opisanie = DescriptionTextBox.Text;

                if (_isNewMaterial)
                {
                    App.Context.Materials.Add(_material);
                }
                else
                {
                    App.Context.Entry(_material).State = EntityState.Modified;

                    // Добавляем запись в историю, если количество изменилось
                    if (oldCount.HasValue && newCount.HasValue && oldCount != newCount)
                    {
                        var historyRecord = new History_UpdateCount
                        {
                            name_material = _material.name_material,
                            count_old = oldCount,
                            count_new = newCount
                        };

                        App.Context.History_UpdateCount.Add(historyRecord);
                    }
                }

                App.Context.SaveChanges();

                MessageBox.Show("Материал успешно сохранен.");
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении материала: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("Введите наименование материала.");
                return false;
            }

            if (TypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Выберите тип материала.");
                return false;
            }

            if (!decimal.TryParse(CountTextBox.Text, out decimal count) || count < 0)
            {
                MessageBox.Show("Введите корректное количество на складе (неотрицательное число).");
                return false;
            }

            if (string.IsNullOrWhiteSpace(UnitTextBox.Text))
            {
                MessageBox.Show("Введите единицу измерения.");
                return false;
            }

            if (!decimal.TryParse(PackageTextBox.Text, out decimal package) || package <= 0)
            {
                MessageBox.Show("Введите корректное количество в упаковке (положительное число).");
                return false;
            }

            if (!decimal.TryParse(MinCountTextBox.Text, out decimal minCount) || minCount < 0)
            {
                MessageBox.Show("Введите корректное минимальное количество (неотрицательное число).");
                return false;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Введите корректную стоимость за единицу (неотрицательное число).");
                return false;
            }

            return true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isNewMaterial)
            {
                Close();
                return;
            }

            if (_material.Material_Produkcia.Any())
            {
                MessageBox.Show("Невозможно удалить материал, так как он используется в производстве продукции.");
                return;
            }

            var result = MessageBox.Show($"Вы уверены, что хотите удалить материал '{_material.name_material}'?","Подтверждение удаления", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    if (!string.IsNullOrEmpty(_material.image))
                    {
                        string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", _material.image);
                        if (File.Exists(imagePath))
                        {
                            try
                            {
                                File.Delete(imagePath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Ошибка удаления изображения: {ex.Message}");
                            }
                        }
                    }
                    var relatedSklads = App.Context.Sklads.Where(s => s.id_material == _material.id_material).ToList();
                    var relatedHistory = App.Context.History_UpdateCount.Where(h => h.name_material == _material.name_material).ToList();

                    App.Context.Sklads.RemoveRange(relatedSklads);
                    App.Context.History_UpdateCount.RemoveRange(relatedHistory);

                    App.Context.Materials.Remove(_material);
                    App.Context.SaveChanges();

                    MessageBox.Show("Материал успешно удален.");
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении материала: {ex.Message}");
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!decimal.TryParse(MinCountTextBox.Text, out decimal minCount) || !decimal.TryParse(CountTextBox.Text, out decimal currentCount) || !decimal.TryParse(PackageTextBox.Text, out decimal packageCount) || !decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                PurchaseCostTextBlock.Text = "Заполните все необходимые поля";
                return;
            }

            if (currentCount >= minCount)
            {
                PurchaseCostTextBlock.Text = "Дополнительная закупка не требуется";
                return;
            }

            decimal needed = minCount - currentCount;
            decimal packages = Math.Ceiling(needed / packageCount);
            decimal totalCost = packages * packageCount * price;

            PurchaseCostTextBlock.Text = $"Требуется закупить: {packages} упаковок\n" + $"Общая стоимость: {totalCost:C}";
        }
    }
}
