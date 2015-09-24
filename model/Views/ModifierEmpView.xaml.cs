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
using model.Models;
using model.Service;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutEmploye.xaml
    /// </summary>
    public partial class ModifierEmpView : UserControl
    {
        public ModifierEmpView()
        {
            InitializeComponent();
        }

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void EnregistrerEmp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Le nouveau employé est ajouté dans la liste");
            retourMenu(this, null);
        }

    }
}
