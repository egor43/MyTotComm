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

namespace MyTotalCommander
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public class CollectionItems
    {
        List<Element> listitems = new List<Element>();

        public class Element
        {
            public string Name { get; internal set; }
            public FileAttributes Type { get; internal set; }
            public string Size { get; internal set; }
            public string Way { get; internal set; }
            public object Tag { get; internal set; }
        }

        public CollectionItems()
        {
            Element el = new Element();
            el.Name = "...";
            el.Type = FileAttributes.Normal;
            el.Way = "";
            el.Tag = null;
            listitems.Add(el);
        }

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

        public List<Element> GetCollectionElements()
        {
            return listitems;
        }
    }


    public partial class MainWindow : Window
    {
        //TODO: 
        public Stack<DirectoryInfo> olddirectory = new Stack<DirectoryInfo>();
        public MainWindow()
        {
            InitializeComponent();

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
                }
            }
        }

        private void MenuItemClickHandler(object sender, RoutedEventArgs e)
        {
            MenuItem menuitem = sender as MenuItem;
            try
            {
                CollectionItems collItems = new CollectionItems();
                collItems.AddCollectionItems((menuitem.Tag as DriveInfo).RootDirectory.GetDirectories());
                collItems.AddCollectionItems((menuitem.Tag as DriveInfo).RootDirectory.GetFiles());
                dataGrid.ItemsSource = collItems.GetCollectionElements();
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("Диск недоступен!");
            }
        }

        private void ComboBoxItemSelectedkHandler(object sender, RoutedEventArgs e)
        {
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
                    item.Header = v.Name;
                    if (v.Type == FileAttributes.Directory) item.Items.Add("*");
                    TreeVW.Items.Add(item);
                }
            }
            catch (System.IO.IOException)
            {
                TreeVW.Items.Clear();
                MessageBox.Show("Диск недоступен!");
            }
        }

        private void ExpandedHandler(object sender, RoutedEventArgs e)
        {
            CollectionItems collItems = new CollectionItems();
            TreeViewItem element = sender as TreeViewItem;
            if ((element.Tag as FileInfo)!=null)
            {
                Process.Start((element.Tag as FileInfo).FullName);
            }
            else
            {
                collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetDirectories());
                collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetFiles());
                List<CollectionItems.Element> list = collItems.GetCollectionElements();
                list.RemoveAt(0);
                element.Items.Clear();
                foreach (var v in list)
                {
                    TreeViewItem item = new TreeViewItem();
                    item.Tag = v.Tag;
                    item.Expanded += ExpandedHandler;
                    item.Header = v.Name;
                    if (v.Type == FileAttributes.Directory) item.Items.Add("*");
                    element.Items.Add(item);
                }
            }
            e.Handled = true;
        }

        private void menuitmCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            CollectionItems.Element element = datagrid.SelectedItem as CollectionItems.Element;
            if (element != null) tblkWay.Text = element.Way;
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            DataGrid datagrid = sender as DataGrid;
            CollectionItems.Element element = datagrid.SelectedItem as CollectionItems.Element;
            CollectionItems collItems = new CollectionItems();
            if(element.Tag!=null)
            {
                if(element.Type==FileAttributes.Directory)
                {
                    olddirectory.Push((element.Tag as DirectoryInfo).Parent);
                    collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetDirectories());
                    collItems.AddCollectionItems((element.Tag as DirectoryInfo).GetFiles());
                    dataGrid.ItemsSource = collItems.GetCollectionElements();
                }
                else
                {
                    Process.Start(element.Way);
                }
            }
            else
            {
                if(olddirectory.Count>0)
                {
                    collItems.AddCollectionItems(olddirectory.Peek().GetDirectories());
                    collItems.AddCollectionItems(olddirectory.Pop().GetFiles());
                    dataGrid.ItemsSource = collItems.GetCollectionElements();
                }
            }
            e.Cancel = true;
        }
    }
}
