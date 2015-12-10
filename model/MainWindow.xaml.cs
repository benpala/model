using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using model.Service;
using model.Service.MySql;
using model.Views;
namespace model
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IApplicationService
    {
      
        public MainWindow()
        {
            InitializeComponent();
            Configure();
            contentPresenter.Content = new ListePaieView();
     
        }
        private void Configure()
        {
            
            //Déclaration du ApplicationService
            ServiceFactory.Instance.Register<IApplicationService, MainWindow>(this);
            ServiceFactory.Instance.Register<IPaiesService, MySqlPaieService>(new MySqlPaieService());
            ServiceFactory.Instance.Register<IProjetService, MySqlProjetService>(new MySqlProjetService());
            ServiceFactory.Instance.Register<IEmployeService, MySqlEmployeService>(new MySqlEmployeService());
            ServiceFactory.Instance.Register<IPeriodeService, MySqlPeriodeService>(new MySqlPeriodeService());
        }


        public void ChangeView<T>(T view)
        {
            contentPresenter.Content = view as UserControl;
        }

        private void employeLst(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void projetLst(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
			if(contentPresenter.Content.ToString() == "model.Views.GestionProjetView")
            {
                var result = System.Windows.MessageBox.Show("Voulez-vous sauvegarder avant de quitter?", "Confirmation",MessageBoxButton.YesNoCancel, MessageBoxImage.Warning).ToString();
                if(result == "Yes")
                {
                    GestionProjetView g = (GestionProjetView)contentPresenter.Content;
                    g.EnregistrerProjet(sender,null);
                    if(!g.valide)
                    {
                        return;
                    }
                }
                if(result == "Cancel")
                {
                    return;
                }
            }
            applicationService.ChangeView<ProjetView>(new ProjetView());
        }

        private void closeApp_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void paieLst(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ListePaieView>(new ListePaieView());
        }

        private void EcranRapport(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<RapportView>(new RapportView());
        }

        private void click_config(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ConfigurationView>(new ConfigurationView());
        }

        private void aideApp_Click(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Process.Start(@"image/aide.pdf");
           // Uri chemin = new Uri("pack://application:,,,/image/aide.pdf");
            string path = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "aide.pdf");
            Process P = new Process
            {
                StartInfo = { FileName = "AcroRd32.exe", Arguments = path }
            };
            P.Start();
        }
        
    }
  
}
