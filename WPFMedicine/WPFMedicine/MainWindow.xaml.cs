using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WPFMedicine.Model;
using WPFMedicine.ViewModel;

namespace WPFMedicine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ItemsViewModel vm = new ItemsViewModel();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
            AppLoad();
        }

        public async void AppLoad ()
        {
            vm.ConnectionData();
            await vm.GetItemAsync(); //поиск первых 50 элементов базы
            ItemsList.ItemsSource = vm.Items;
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            int itemId = 0;
            if (vm.Items.Count>0)
            {
                itemId = vm.Items.LastOrDefault().Id+1;
            }
            else
            {
                itemId = 1;
            }

            try
            {
                Item item = new Item
                {
                    Id = itemId,
                    Name = ItemName.Text,
                    Count = int.Parse(ItemCount.Text)
                };
                try
                {
                    await vm.CreateItemAsync(item);
                    vm.Items.Add(item);
                    ItemsList.Items.Refresh();
                    MessageBox.Show($"Объект успешно добавлен! Его ID: {item.Id}");
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
            catch { MessageBox.Show("Ошибка! Проверьте заполненность всех полей!"); }
        }

        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                Item item = new Item
                {
                    Id = int.Parse(ItemIdText.Text),
                    Name = ItemName.Text,
                    Count = int.Parse(ItemCount.Text)

                };
                Item oldItem = vm.Items[vm.Items.FindIndex(it => it.Id == item.Id)];
                if (oldItem != null)
                {
                    var dr = MessageBox.Show($"Вы действительно хотите обновить данные для ID:{item.Id} ? \n Данные: \n Имя: {oldItem.Name} => {item.Name} \n Количество {oldItem.Count} => {item.Count}", "Обновление данных", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        try
                        {
                            await vm.UpdateItemAsync(item);
                            vm.Items[vm.Items.FindIndex(it => it.Id == item.Id)] = item;
                            ItemsList.Items.Refresh();
                            MessageBox.Show($"Запись под ID: {item.Id} успешно обновлена!");
                        }
                        catch (Exception ex) { MessageBox.Show($"Произошла ошибка! Проверьте заполненность всех полей! \n{ex.Message}"); }
                    }
                }
                else MessageBox.Show($"Элемент с ID: {item.Id} не найден!");
                
            }
            catch { MessageBox.Show("Необходимо заполнить все поля на форме (ID, Название, Количество)"); }

        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var dr = MessageBox.Show($"Вы действительно хотите удалить ID:{ItemIdText.Text}?", "Удаление объекта",MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {
                try
                {
                    await vm.DeleteItemAsync(int.Parse(ItemIdText.Text));
                    vm.Items.Remove(vm.Items.Where(item => item.Id == int.Parse(ItemIdText.Text)).First());
                    ItemsList.Items.Refresh();
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

        private async void GetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = await vm.GetItemAsync(int.Parse(ItemIdText.Text));
                
                if(item!= null)
                {
                    vm.SelectedItem = item;
                }
            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
            
        }

        private void ItemData_TextChanged(object sender, TextChangedEventArgs e)
        {
            GetButton.IsEnabled = DeleteButton.IsEnabled = ItemIdText.Text.Length > 0;

                if (ItemIdText.Text.Length > 0 && ItemName.Text.Length > 0 && ItemCount.Text.Length > 0)
                {
                    int lastID = vm.Items.LastOrDefault().Id;
                    bool Update = int.Parse(ItemIdText.Text) <= lastID;
                    UpdateButton.IsEnabled = Update;
                }
            if (ItemName.Text.Length > 0 && ItemCount.Text.Length > 0) AddButton.IsEnabled = true;
            else AddButton.IsEnabled = false;
        }

    }
}
