using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private Employe _Employe;
        public ModifierEmpView()
        {
            InitializeComponent();
            DataContext = this;
        }
        public ModifierEmpView(IDictionary<string, object> parametre): this()
        {
            _Employe = parametre["Employe"] as Employe;
        }

        public Employe Employe
        {
            get
            {
                return _Employe;
            }

            set
            {
                if (_Employe == value)
                {
                    return;
                }
                _Employe = value;
            }
        }

        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void EnregistrerEmp(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Les informations sont modifiées");
            retourMenu(this, null);
        }



    }
}
