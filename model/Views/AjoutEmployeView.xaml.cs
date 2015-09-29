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
using model.Service.Simple;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutEmploye.xaml
    /// </summary>
    public partial class AjoutEmployeView : UserControl
    {
        public AjoutEmployeView()
        {
            InitializeComponent();
           
        }
       
        private void retourMenu(object sender, RoutedEventArgs e)
        {
            IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
            applicationService.ChangeView<EmployeView>(new EmployeView());
        }

        private void AjouterEmploye(object sender, RoutedEventArgs e)
        {
            //{ID = "00",Prenom = "Pei", Nom = "Li", Photo = "01", Poste = "Chef", Salaire = "40$"}
            Employe employe = new Employe("5002", txtAjoutNom.Text, txtAjoutPrenom.Text, txtAjoutPoste.Text, txtAjoutSalaire.Text); 
            //Enregistre les données et retourne à l'écran d'avant
            EmployeService _serviceEmploye = new EmployeService();
            _serviceEmploye.addOneEmploye(employe);
            retourMenu(this,null);
        }
    }
}
