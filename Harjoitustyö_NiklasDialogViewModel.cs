using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMKurssi
{
    public class Harjoitustyö_NiklasDialogViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Konstruktori, aseta oletusarvot muuttujille
        /// </summary>
        public Harjoitustyö_NiklasDialogViewModel()
        {
            Name = "Harjoitustyö_Niklas";
            Thickness = 150;
            Width = 150;
            Vinosauvojen_lukumaara = 1 ;
            Pituus_palkki = 7500;
            Korkeus_palkki = 1500;
            Paarre_leveys = 200;
            Paarre_korkeus = 150;
        }
        #region älä tee muutoksia
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        #endregion
        #region tähän tarvittavat parametrit, ovat samat kuin EsimerkkiRakenteessa
        /// <summary>
        /// Rakenteen nimi
        /// </summary>
        public string Name { get; set; }    
        /// <summary>
        /// Paksuus
        /// </summary>
        public double Thickness { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public int Vinosauvojen_lukumaara { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Pituus_palkki { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Korkeus_palkki { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Paarre_leveys { get; set; }
        /// <summary>
        /// Leveys
        /// </summary>
        public double Paarre_korkeus { get; set; }



        #endregion
    }
}
