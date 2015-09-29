﻿using System;
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
using model.Service;
using model.Service.Simple;
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
            ServiceFactory.Instance.Register<IPaiesService, PaieService>(new PaieService());
            ServiceFactory.Instance.Register<IProjetService, ProjetService>(new ProjetService());
            ServiceFactory.Instance.Register<IEmployeService, EmployeService>(new EmployeService());
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

        private void click_config(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<ConfigurationView>(new ConfigurationView());
        }

        private void rapportView(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<RapportView>(new RapportView());
        }

        private void simulationView(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<SimulationView>(new SimulationView());
        }
        
    }
  
}
