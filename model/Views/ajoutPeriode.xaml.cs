using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using model.Service.MySql;

namespace model.Views
{
    /// <summary>
    /// Logique d'interaction pour ajoutPeriode.xaml
    /// </summary>
    public partial class ajoutPeriode : UserControl, INotifyPropertyChanged, INotifyPropertyChanging
    {
        private IPeriodeService _periodeService;
        private IApplicationService _applicationService;
        ObservableCollection<PeriodePaie> _nouvPeriodes = new ObservableCollection<PeriodePaie>();
        public RetrievePaieArgs RetrieveArgs { get; set; }
        private ObservableCollection<PeriodePaie> _periode = new ObservableCollection<PeriodePaie>();
        public ajoutPeriode()
        {
            InitializeComponent();

            DataContext = this;
            RetrieveArgs = new RetrievePaieArgs();
            _periodeService = ServiceFactory.Instance.GetService<IPeriodeService>();
            _applicationService = ServiceFactory.Instance.GetService<IApplicationService>();

            try  { Periodes = new ObservableCollection<PeriodePaie>(_periodeService.RetrieveAll()); }
            catch (Exception) {  MessageBox.Show("Votre base de données n'est pas accessible. Veuillez vous référer au document de configuration"); }
        }
        #region INotifyPropertyChanged INotifyPropertyChanging
        public event PropertyChangedEventHandler PropertyChanged;

        protected PropertyChangedEventHandler PropertyChangedHandler
        {
            get { return PropertyChanged; }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        protected PropertyChangingEventHandler PropertyChangingHandler
        {
            get { return PropertyChanging; }
        }


        protected virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanging;
            if (handler != null)
            {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        public ObservableCollection<PeriodePaie> Periodes
        {
            get{ return _periode; }
            set
            { 
                if (_periode == value) {  return; }
                RaisePropertyChanging();
                _periode = value;
                RaisePropertyChanged();
            }
        }
        public ObservableCollection<PeriodePaie> nouvPeriodes
        {
            get{  return _nouvPeriodes; }
            set
            {
                if (_nouvPeriodes == value){ return; }
                RaisePropertyChanging();
                _nouvPeriodes = value;
                RaisePropertyChanged();
            }

        }
        private void click_periodeList(object sender, RoutedEventArgs e)
        {
            try
            {
                PeriodePaie Periode = new PeriodePaie(Convert.ToDateTime(datedebut.Text), Convert.ToDateTime(datefin.Text));
                string message ="";
                foreach (PeriodePaie p in Periodes)
                {
                    if (p.Debut == Periode.Debut)
                    {
                        message = "Cette période à déjà été enregistré dans le passé!";
                    }   
                }
                if(message == "")
                {
                    if (nouvPeriodes.Count() == 0)
                    {
                        nouvPeriodes.Add(Periode);
                    }
                    else
                    {
                        foreach (PeriodePaie p2 in nouvPeriodes)
                        {
                            if (p2.Debut == Periode.Debut)
                            {
                               message = "Vous avez déjà choisi cette période!";
                            }
                        }
                        if(message == "")
                        {
                            nouvPeriodes.Add(Periode);
                        }
                        else
                        {
                            MessageBox.Show(message);
                        }
                    }
                }
                else
                {
                    MessageBox.Show(message);
                }
               
            }catch(Exception){
                MessageBox.Show("Les champs que vous avez entrez ne correspondes pas à des dates.");
            }
        }

        private void deletelast_click_list(object sender, RoutedEventArgs e)
        {
            try{  nouvPeriodes.Remove(nouvPeriodes.Last()); } catch (Exception ){ MessageBox.Show("Action non permise."); }
        }

        private void click_enregistrer(object sender, RoutedEventArgs e)
        { 
            try
            {
              
                foreach (PeriodePaie p in nouvPeriodes)
                {
                    if(p.register())
                    {
                        nouvPeriodes.Remove(p);
                        if (nouvPeriodes.Count() == 0)
                        {
                            break;
                        }
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                MessageBox.Show("Tous les dates on bien étés enregistrés.");
                IApplicationService applicationService = ServiceFactory.Instance.GetService<IApplicationService>();
                applicationService.ChangeView<ajoutPeriode>(new ajoutPeriode());
            }
            catch (Exception)
            {
                MessageBox.Show("Les dates n'ont pas étés correctement ajoutés.");
            }
        }
    }
}
