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
using System.Diagnostics;

namespace Total
{
    // Класс описывающий коллекцию
    public class CollectionItems
    {
        List<Element> listitems = new List<Element>();

        //Класс описывающий элемент коллекции
        public class Element
        {
            public string Name { get; internal set; }
            public FileAttributes Type { get; internal set; }
            public string Size { get; internal set; }
            public string Way { get; internal set; }
            public object Tag { get; internal set; }
        }

        //Конструктор класса коллекции
        public CollectionItems()
        {
            Element el = new Element();
            el.Name = "...";
            el.Type = FileAttributes.Normal;
            el.Way = "";
            el.Tag = null;
            listitems.Add(el);
        }

        //Метод добавления элемента в коллекцию
        public void AddCollectionItems(DirectoryInfo[] driveinfo)
        {
            foreach (var v in driveinfo)
            {
                if (v.Attributes == FileAttributes.Directory)
                {
                    Element el = new Element();
                    el.Name = v.Name;
                    el.Type = v.Attributes;
                    el.Way = v.FullName.ToString();
                    el.Tag = v;
                    listitems.Add(el);
                }
            }
        }

        //Метод добавления элемента в коллекцию
        public void AddCollectionItems(FileInfo[] driveinfo)
        {
            foreach (var v in driveinfo)
            {
                if (v.Attributes != FileAttributes.Hidden)
                {
                    Element el = new Element();
                    el.Name = v.Name;
                    el.Type = v.Attributes;
                    el.Size = v.Length.ToString();
                    el.Way = v.FullName.ToString();
                    el.Tag = v;
                    listitems.Add(el);
                }
            }
        }

        //Метод возращающий колекцию
        public List<Element> GetCollectionElements()
        {
            return listitems;
        }
    }

    //Класс описывающй взадействие с MainWindow.xaml
    public partial class MainWindow : Window
    {
        //описание полей класса 
        public Stack<DirectoryInfo> olddirectory = new Stack<DirectoryInfo>();
        public string WayTree;
        public string WayGridNameElement;
        public string WayTreeNameElement;
        public string WayGrid;
        public bool focus;

        //Начальная инициализация пользовательского интерфейса
        public MainWindow()
        {
            InitializeComponent();

            //Динамическое загрузка элементов(дисков, директорий, файлов)
            foreach (DriveInfo drv in DriveInfo.GetDrives())
            {
                MenuItem mnuitem = new MenuItem();
                ComboBoxItem cbxitem = new ComboBoxItem();
                cbxitem.Selected += ComboBoxItemSelectedkHandler;
                mnuitem.Tag = cbxitem.Tag = drv;
                mnuitem.Click += MenuItemClickHandler;
                mnuitem.Header = cbxitem.Content = drv.ToString();
                if (ComboBox.Items.Count == 0) cbxitem.IsSelected = true;
                menuDiscBtn.Items.Add(mnuitem);
                ComboBox.Items.Add(cbxitem);
                if (drv.IsReady == true)
                {
                    CollectionItems collItems = new CollectionItems();
                    collItems.AddCollectionItems(drv.RootDirectory.GetDirectories());
                    collItems.AddCollectionItems(drv.RootDirectory.GetFiles());
                    dataGrid.ItemsSource = collItems.GetCollectionElements();
                    WayGrid = (mnuitem.Tag as DriveInfo).RootDirectory.FullName;
                    WayGridNameElement = "";
                    WayTree = (cbxitem.Tag as DriveInfo).RootDirectory.FullName;
                    WayTreeNameElement = "";
                }
            }
        }

        //Обработчик события выбора диска относящихся к DataGrid
        private void MenuItemClickHandler(object sender, RoutedEventArgs e)
        {
            focus = true;
            MenuItem menuitem = sender as MenuItem;
            try
            {
                CollectionItems collItems = new CollectionItems();
                collItems.AddCollectionItems((menuitem.Tag as DriveInfo).RootDirectory.GetDirectories());
                collItems.AddCollectionItems((menuitem.Tag as DriveInfo).RootDirectory.GetFiles());
                dataGrid.ItemsSource = collItems.GetCollectionElements();
                WayGrid = (menuitem.Tag as DriveInfo).RootDirectory.FullName;
                WayGridNameElement = "";
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Диск недоступен!");
            }
        }

        //Обработчик события выбора диска относящихся к TreeView
        private void ComboBoxItemSelectedkHandler(object sender, RoutedEventArgs e)
        {
            focus = false;
            ComboBoxItem cbxitem = sender as ComboBoxItem;
            TreeVW.Items.Clear();
            try
            {
                CollectionItems collItems = new CollectionItems();
                collItems.AddCollectionItems((cbxitem.Tag as DriveInfo).RootDirectory.GetDirectories());
                collItems.AddCollectionItems((cbxitem.Tag as DriveInfo).RootDirectory.GetFiles());
                List<CollectionItems.Element> list = collItems.GetCollectionElements();
                list.RemoveAt(0);
                foreach (var v in list)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = v.Tag;
                    item.Expanded += ExpandedHandler;
                    item.GotFocus += ElementGotFocusHandler;
                    item.Header = v.Name;
                    if (v.Type == FileAttributes.Directory) item.Items.Add("*");
                    TreeVW.Items.Add(item);
                }
                WayTree = (cbxitem.Tag as DriveInfo).RootDirectory.FullName;
                WayTreeNameElement = "";
            }
            catch (System.IO.IOException)
            {
                TreeVW.Items.Clear();
                MessageBox.Show("Диск недоступен!");
            }
        }

        //Обработка события раскрытия ветки TreeView
        private void ExpandedHandler(object sender, RoutedEventArgs e)
        {
            focus = false;
            CollectionItems collItems = new CollectionItems();
            TreeViewItem element = sender as TreeViewItem;
            if ((element.Tag as FileInfo) != null)
            {
                WayTree = (element.Tag as FileInfo).DirectoryName;
                WayTreeNameElement = (element.Tag as FileInfo).Name;
                Process.Start((element.Tag as FileInfo).FullName);
            }
            else
            {
                WayTree = (element.Tag as DirectoryInfo).Parent.FullName;
                WayTreeNameElement = (element.Tag as DirectoryInfo).Name;
                try
                {
                    collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetDirectories());
                    collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetFiles());
                }
                catch
                {
                    MessageBox.Show("Не хватает прав доступа!");
                }

                List<CollectionItems.Element> list = collItems.GetCollectionElements();
                list.RemoveAt(0);
                element.Items.Clear();
                foreach (var v in list)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = v.Tag;
                    item.Selected += SelectedItemHandler;
                    item.Expanded += ExpandedHandler;
                    item.Header = v.Name;
                    if (v.Type == FileAttributes.Directory) item.Items.Add("*");
                    element.Items.Add(item);
                }
            }
            e.Handled = true;
        }

        private void ElementGotFocusHandler(object sender, RoutedEventArgs e)
        {
            focus = false;
            TreeVW.Opacity = 1;
            dataGrid.Opacity = 0.3;
            TreeViewItem element = sender as TreeViewItem;
            if (element.Tag is FileInfo)
            {
                WayTreeNameElement = (element.Tag as FileInfo).Name;
                WayTree = (element.Tag as FileInfo).DirectoryName;
            }
            e.Handled = true;
        }

        private void SelectedItemHandler(object sender, RoutedEventArgs e)
        {
            focus = false;
            TreeVW.Opacity = 1;
            dataGrid.Opacity = 0.3;
            TreeViewItem send = sender as TreeViewItem;
            if (send.Tag is FileInfo)
            {
                WayTreeNameElement = (send.Tag as FileInfo).Name;
                WayTree = (send.Tag as FileInfo).DirectoryName;
                e.Handled = true;
            }
            else
            {
                WayTreeNameElement = (send.Tag as DirectoryInfo).Name;
                WayTree = (send.Tag as DirectoryInfo).Parent.FullName;
                e.Handled = true;
            }
        }

        private void menuitmCopy_Click(object sender, RoutedEventArgs e)
        {
            if (focus)
            {
                if (WayGridNameElement != "")
                {
                    //string inPath = System.IO.Path.Combine(WayTree, WayGridNameElement);
                    string outPath = System.IO.Path.Combine(WayGrid, WayGridNameElement);
                    string inPath;
                    if (File.GetAttributes(System.IO.Path.Combine(WayTree, WayTreeNameElement)) == FileAttributes.Directory) inPath = System.IO.Path.Combine(WayTree, WayTreeNameElement, WayGridNameElement);
                    else inPath = System.IO.Path.Combine(WayTree, WayGridNameElement);
                    if (File.GetAttributes(outPath) == FileAttributes.Directory)
                    {
                        try
                        {
                            perebor(outPath, inPath);
                        }
                        catch
                        {
                            MessageBox.Show("Не хватает прав доступа!");
                        }

                    }
                    else
                    {
                        System.IO.File.Copy(outPath, inPath, true);

                    }
                }
                else
                {
                    MessageBox.Show("Не выбран файл для копирования");
                }

            }
            else
            {
                if (WayTreeNameElement != null)
                {
                    string outPath = System.IO.Path.Combine(WayTree, WayTreeNameElement);
                    string inPath;
                    if (File.GetAttributes(System.IO.Path.Combine(WayGrid, WayGridNameElement)) == FileAttributes.Directory) inPath = System.IO.Path.Combine(WayGrid, WayGridNameElement, WayTreeNameElement);
                    else inPath = System.IO.Path.Combine(WayGrid, WayTreeNameElement);
                    if (File.GetAttributes(outPath) == FileAttributes.Directory)
                    {
                        try
                        {
                            perebor(outPath, inPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }

                    }
                    else
                    {
                        try
                        {
                            System.IO.File.Copy(outPath, inPath, true);
                        }
                        catch
                        {
                            MessageBox.Show("Не хватает прав доступа!");
                        }

                    }
                }
                else
                {
                    MessageBox.Show("Не выбран файл для копирования");
                }
            }

        }

        //begin_dir - директория источник.
        //end_dir - директория приёмник.
        private void perebor(string begin_dir, string end_dir)
        {
            //Берём нашу исходную папку
            DirectoryInfo dir_inf = new DirectoryInfo(begin_dir);
            //Перебираем все внутренние папки
            if (Directory.Exists(end_dir) != true)
            {
                Directory.CreateDirectory(end_dir);
            }
            foreach (DirectoryInfo dir in dir_inf.GetDirectories())
            {
                //Проверяем - если директории не существует, то создаём;
                if (Directory.Exists(end_dir + "\\" + dir.Name) != true)
                {
                    Directory.CreateDirectory(end_dir + "\\" + dir.Name);
                }

                //Рекурсия (перебираем вложенные папки и делаем для них то-же самое).
                perebor(dir.FullName, end_dir + "\\" + dir.Name);
            }

            //Перебираем файлики в папке источнике.
            DirectoryInfo directory = new DirectoryInfo(begin_dir);
            foreach (var file in directory.GetFiles())
            {
                //Определяем  имя файла с расширением - без пути (но с слешем "\").
                string filik = file.Name;
                //Копируем файл с перезаписью из источника в приёмник.
                //System.IO.File.Copy(outPath, inPath, true);
                File.Copy(System.IO.Path.Combine(begin_dir, filik), System.IO.Path.Combine(end_dir, filik), true);
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            focus = true;
            DataGrid datagrid = sender as DataGrid;
            CollectionItems.Element element = datagrid.SelectedItem as CollectionItems.Element;
            if (element != null)
            {
                if (element.Tag is FileInfo)
                {
                    WayGrid = (element.Tag as FileInfo).DirectoryName;
                    WayGridNameElement = (element.Tag as FileInfo).Name;
                }
                else if (element.Tag != null)
                {
                    WayGrid = (element.Tag as DirectoryInfo).Parent.FullName;
                    WayGridNameElement = (element.Tag as DirectoryInfo).Name;
                }
                tblkWay.Text = element.Way;
            }

        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            focus = true;
            DataGrid datagrid = sender as DataGrid;
            CollectionItems.Element element = datagrid.SelectedItem as CollectionItems.Element;
            CollectionItems collItems = new CollectionItems();
            if (element.Tag != null)
            {
                if (element.Type == FileAttributes.Directory)
                {
                    WayGrid = (element.Tag as DirectoryInfo).Parent.FullName;
                    WayGridNameElement = (element.Tag as DirectoryInfo).Name;
                    olddirectory.Push((element.Tag as DirectoryInfo).Parent);
                    try
                    {
                        collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetDirectories());
                        collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetFiles());
                    }
                    catch
                    {
                        MessageBox.Show("Не хватает прав доступа!");
                    }

                    dataGrid.ItemsSource = collItems.GetCollectionElements();
                }
                else
                {
                    WayGrid = (element.Tag as FileInfo).DirectoryName;
                    WayGridNameElement = (element.Tag as FileInfo).Name;
                    Process.Start(element.Way);
                }
            }
            else
            {
                if (olddirectory.Count > 0)
                {
                    WayGrid = olddirectory.Peek().FullName;
                    WayGridNameElement = "";
                    collItems.AddCollectionItems(olddirectory.Peek().GetDirectories());
                    collItems.AddCollectionItems(olddirectory.Pop().GetFiles());
                    dataGrid.ItemsSource = collItems.GetCollectionElements();
                }
            }
            e.Cancel = true;
        }

        private void dataGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            dataGrid.Opacity = 1;
            TreeVW.Opacity = 0.3;
            focus = true;
        }

        private void TreeVW_GotFocus(object sender, RoutedEventArgs e)
        {
            dataGrid.Opacity = 0.3;
            TreeVW.Opacity = 1;
            focus = false;
        }
    }
}
