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

namespace MyTotalCommander
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: Добавить между menuDiscBtn и DataGrid нечто, куда будет выводиться путь.
        public MainWindow()
        {
            InitializeComponent();

            foreach (DriveInfo drv in DriveInfo.GetDrives())
            {
                MenuItem mnuitem = new MenuItem();
                ComboBoxItem cbxitem = new ComboBoxItem();
                mnuitem.Tag=cbxitem.Tag = drv;
                mnuitem.Header=cbxitem.Content = drv.ToString();
                if (ComboBox.Items.Count == 0) cbxitem.IsSelected = true;
                menuDiscBtn.Items.Add(mnuitem);
                ComboBox.Items.Add(cbxitem);
                if(drv.IsReady==true)dataGrid.ItemsSource = drv.RootDirectory.GetDirectories();
            }
        }
    }
}
