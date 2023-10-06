using Enterprixe.ValosUITools.Features;
using Epx.BIM;
using Epx.BIM.Models.Concrete;
using Epx.BIM.Models.Geometry;
using Epx.BIM.Models.Steel;
using Epx.BIM.Models.Timber;
using Epx.BIM.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace BIMKurssi
{
    /// <summary>
    /// Esimerkki "pruju" harjoitustyöhön. Vaihda luokkien nimistä OmaEsimerkki oman harjoitustyösi nimeksi
    /// </summary>
    public class Harjoitustyö_NiklasPlugin : PluginTool, IModelViewFeature
    {
        #region ei muutoksia
        protected System.Windows.Media.Media3D.Point3D _startPoint;
        protected System.Windows.Media.Media3D.Point3D _endPoint;
        private int inputPointIndex;
        #endregion
        // luokan EsimerkkiDialogViewModel nimen voi vaihtaa, jätä loppu DialogViewModel
        internal Harjoitustyö_NiklasDialogViewModel _viewModel;
        public Harjoitustyö_NiklasPlugin()
        {
            // alusat haluamasi alkuarvot
            _startPoint = new Point3D();
            _endPoint = new Point3D(5000, 0, 0);
            #region ei muutoksia
            _modelViewNodes.Clear();
            inputPointIndex = 0;
            #endregion
            // muutitko luokan nimen ?
            _viewModel = new Harjoitustyö_NiklasDialogViewModel();
        }
        #region ei muutoksia
        private List<BaseDataNode> _modelViewNodes = new List<BaseDataNode>();
        public IEnumerable<BaseDataNode> ModelViewNodes => _modelViewNodes;
        public override Type AcceptedMasterNodeType => typeof(Epx.BIM.Models.ModelBaseNode);
        public override bool SupportsEditMode => true;        
        #endregion
        // Menussa näkyvän nimi, on syytä muuttaa
        public override string NameForMenu { get { return "Harjoitustyö_Niklas"; } }
        /// <summary>
        /// Menun tooltip teksti, on syytä muuttaa
        /// </summary>
        public override object MenuToolTip
        {
            get
            {
                return "Luo Harjoitustyö_Niklas";
            }
        }
        #region todennäköisesti ei muutoksia, jos kysytään kaksi pistettä: alku- ja loppupiste
        /// <summary>
        /// Alku- ja loppupisteen kysely, tuskin tarvitsee muuttaa
        /// </summary>
        /// <param name="previousInput"></param>
        /// <returns></returns>
        public override PluginInput GetNextPostDialogInput(PluginInput previousInput)
        {
            if (previousInput != null)
            {
                if (previousInput is PluginKeyInput)
                {
                    if (IsInEditMode && (previousInput as PluginKeyInput).InputKey == System.Windows.Input.Key.Enter)
                    {
                        return null;
                    }
                }
                if (previousInput is PluginPointInput && previousInput.Index == 0)
                {
                    PluginInput pp = new PluginPointInput();
                    pp.Prompt = "Valitse loppupiste";
                    if (IsInEditMode) pp.Prompt += " (enter to skip)";
                    pp.Index = 1;
                    inputPointIndex = 1;
                    return pp;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                PluginInput pp = new PluginPointInput();
                pp.Prompt = "Valitse alkupiste";
                if (IsInEditMode) pp.Prompt += " (enter to skip)";
                inputPointIndex = 0;
                return pp;
            }
        }
        /// <summary>
        /// Pisteiden syötön tarkistus, tuskin tarvitsee muuttaa
        /// </summary>
        /// <param name="pluginInput"></param>
        /// <returns></returns>
        public override bool IsPostDialogInputValid(PluginInput pluginInput)
        {
            if (pluginInput is PluginPointInput pointIn)
            {
                if (pointIn.Index == 0)
                {
                    _startPoint = pointIn.Point;
                }
                else if (pointIn.Index == 1)
                {
                    _endPoint = pointIn.Point;
                }
                return true;
            }
            else if (pluginInput is PluginKeyInput)
            {
                if (IsInEditMode && (pluginInput as PluginKeyInput).InputKey == System.Windows.Input.Key.Enter) return true;
            }
            return false;
        }
        #endregion
        /// <summary>
        /// Pluginin attribuuttien alustus, muokkaus tilassa (edit) otetaan viewmodeliin arvot olemassa olevalta luokan instansilta.
        /// Muokka edit tilaan omat attribuuttisi
        /// </summary>
        /// <param name="initialPlugin"></param>
        public override void InitializePluginParameters(PluginTool initialPlugin)
        {
            Harjoitustyö_NiklasRakenne Harjoitustyö_Niklas = IsInEditMode ? PluginDataNode as Harjoitustyö_NiklasRakenne : null;
            inputPointIndex = 0;
            // luodaan dialogin tarvitsema view model luokka
            _viewModel = new Harjoitustyö_NiklasDialogViewModel();
            // jos edit tila, niin asetetaan arvot tietomalli luokalta
            if (Harjoitustyö_Niklas != null)
            {
                _viewModel.Name = Harjoitustyö_Niklas.Name;
                _viewModel.Width = Harjoitustyö_Niklas.Width;
                _viewModel.Thickness = Harjoitustyö_Niklas.Thickness;
                _startPoint = Harjoitustyö_Niklas.Origin;
                _endPoint = _startPoint + Harjoitustyö_Niklas.XAxis;
                // pidetään pisteet vaakassa, kaltevuus parametrien kautta
                _endPoint.Z = _startPoint.Z;
            }

        }
        #region ei muutoksia
        /// <summary>
        /// Näytetään dialogi
        /// </summary>
        /// <param name="isDialogApplyEnabled"></param>
        /// <returns></returns>
        public override bool ShowDialog(bool isDialogApplyEnabled)
        {
            EsimerkkiPluginDialog dlg = new EsimerkkiPluginDialog();
            dlg.Owner = System.Windows.Application.Current.MainWindow;
            dlg.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            dlg.DataContext = _viewModel;
            _DialogResult = (bool) dlg.ShowDialog();
            if (!_DialogResult && _FeatureEngine != null) _FeatureEngine.PluginDialogClosed(_DialogResult);
            return _DialogResult;
        }
        private bool _DialogResult;
        bool IModelViewFeature.DialogResult { get { return _DialogResult; } set { _DialogResult = value; } }
        IEnumerable<BaseDataNode> IModelViewFeature.HiddenNodes => null;

        IEnumerable<ModelDimensionBase> IModelViewFeature.ModelViewDimensions => null;

        IEnumerable<ModelUIContentElement> IModelViewFeature.ModelViewOverlayContents => null;

        private IFeatureEngine _FeatureEngine;
        IFeatureEngine IModelViewFeature.FeatureEngine
        {
            get
            { return _FeatureEngine; }
            set
            { _FeatureEngine = value; }
        }


        void IModelViewFeature.CancelFeature()
        {
            _modelViewNodes.Clear();
            if (_FeatureEngine != null) _FeatureEngine.PluginUpdate3D(false);
        }

        void IModelViewFeature.MouseMoved(Point3D currentPoint)
        {
            Point3D startP = _startPoint;
            Point3D  endP = _endPoint;
            if (inputPointIndex == 0)
            {
                Vector3D vek = _endPoint - _startPoint;
                if (vek.Length < 1) return;
                _startPoint = currentPoint;
                _endPoint = _startPoint + vek;
            }
            else
            {
                if ((currentPoint - _startPoint).Length < 1) return;
                _endPoint = currentPoint;
            }
            _modelViewNodes.Clear();
            _modelViewNodes.AddRange(Excecute(true, out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> updateNodes));
            if (_FeatureEngine != null) _FeatureEngine.PluginUpdate3D(false);
            _startPoint = startP;
            _endPoint = endP;
        }
        /// <summary>
        /// Kelvollisen isännän tarkistus hierarkiassa
        /// </summary>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public override bool IsValidTarget(BaseDataNode targetNode)
        {
            return targetNode != null;
        }
        /// <summary>
        /// Luodaan tai muokataan rakenne
        /// </summary>
        /// <param name="doDelegate"></param>
        /// <param name="undoDelegate"></param>
        /// <param name="update3DNodes"></param>
        /// <returns></returns>
        public override List<BaseDataNode> Excecute(out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> update3DNodes)
        {
            return Excecute(false, out doDelegate, out undoDelegate, out update3DNodes);
        }
        /// <summary>
        /// Näytetään preview, luodaan tai muokataan rakenne
        /// </summary>
        /// <param name="previewMode"></param>
        /// <param name="doDelegate"></param>
        /// <param name="undoDelegate"></param>
        /// <param name="update3DNodes"></param>
        /// <returns></returns>
        private List<BaseDataNode> Excecute(bool previewMode, out Action doDelegate, out Action undoDelegate, out List<BaseDataNode> update3DNodes)
        {
            doDelegate = null;
            undoDelegate = null;
            update3DNodes = new List<BaseDataNode>();
            // luodaan uutta tai preview piirto
            if (!IsInEditMode || previewMode)
            {
                Harjoitustyö_NiklasRakenne rakenne = CreateOrUpdateModel(_viewModel);
                 if (previewMode) return new List<BaseDataNode>() { rakenne };
                doDelegate += delegate
                {
                    Target.AddChild(rakenne);
                };
                // undo:ta ei tarvitse implementoida
                undoDelegate += delegate
                {
 //                   Target.RemoveChild(rakenne);
                };
                PluginDataNode = rakenne;
            }
            // muokataan olemassa olevaa
            else
            {
                Harjoitustyö_NiklasRakenne rakenne = PluginDataNode as Harjoitustyö_NiklasRakenne;
                //Point3D oldOrigin = rakenne.Origin;
                //Vector3D oldX = rakenne.XAxis;
                //EsimerkkiDialogViewModel oldParameters = new EsimerkkiDialogViewModel()
                //{
                //    Name = _viewModel.Name,
                //    Thickness = _viewModel.Thickness,
                //    Width = _viewModel.Width
                //};
                //double oldWidth = rakenne.Width;
                //double oldThickness = rakenne.Thickness;
                //string oldName = rakenne.Name;
                doDelegate += delegate
                {
                    rakenne = CreateOrUpdateModel(_viewModel, rakenne);
                    Target.AddChild(rakenne);
                };
                // undo:ta ei tarvitse implementoida
                undoDelegate += delegate
                {
                    //_startPoint = oldOrigin;
                    //_endPoint = oldOrigin + oldX;
                    //rakenne = CreateOrUpdateModel(oldParameters, rakenne);
                };
            }
            return new List<BaseDataNode>(0);
        }
        #endregion
        /// <summary>
        /// Luodaan uusi rakenne tai muokataan olemassa olevaa. Tämä funktio on keskeinen kohta harjoitustyössä. Tässä funktiossa
        /// luodaan tai muokataan harjoitustyössä määritelty rakenne attribuuttien mukaan. Tässä on yksi esimerkki, joka
        /// luo yhden vaakaosan ja yhden pystyosan sekä tekee niihin boolean työstöjä (leikkaa osia pois). Kaikissa harjoitustöissä
        /// ei tarvita boolean työstöjä.
        /// </summary>
        /// <param name="parametrit">Dialog view model luokka, jossa käyttäjän dialogissa antamat arvot</param>
        /// <param name="oldEsimerkki">Create moodissa on null, muokkaus tilassa olemassa olevan luokan instanssi</param>
        /// <returns></returns>
        private Harjoitustyö_NiklasRakenne CreateOrUpdateModel(Harjoitustyö_NiklasDialogViewModel parametrit, Harjoitustyö_NiklasRakenne oldHarjoitustyö_Niklas = null)
        {
            Harjoitustyö_NiklasRakenne retVal;
            // onko kysessä uuden luonti vai olemassa olevan muokkaaminen
            if (oldHarjoitustyö_Niklas == null)
            {
                // luodaan uusi
                retVal = new Harjoitustyö_NiklasRakenne();
            } else
            {
                // muokataan vanhaa
                retVal = oldHarjoitustyö_Niklas;
                // poisteaan aliosat, luodaan aina uudet
                retVal.RemoveAllChildren(false);
            }
            // asetetaan pääluokan attribuutit talteen, jotta arvot säilyvät editointiin
            retVal.Name = parametrit.Name;
            retVal.Thickness = parametrit.Thickness;
            retVal.Width = parametrit.Width;
            retVal.Origin = _startPoint;
            Vector3D xVec = _endPoint - _startPoint;
            retVal.XAxis = xVec;
            // y-akseli suoraan ylös
            retVal.YAxis = new Vector3D(0, 0, 1);
            // luodaan aliosa, sijainti on isännän koordinaatistossa

            TimberMember3D mem1 = new Epx.BIM.Models.Timber.TimberMember3D();
            mem1.Name = "Vaakassa_A";
            mem1.Origin = new Point3D(0, 0, 0);
            mem1.XAxis = new Vector3D(parametrit.Pituus_palkki, 0, 0);
            mem1.YAxis = new Vector3D(0, 1, 0);
            mem1.SizeY = parametrit.Paarre_korkeus;
            mem1.SizeZ = parametrit.Paarre_leveys;
             retVal.AddChild(mem1);

            TimberMember3D mem2 = new Epx.BIM.Models.Timber.TimberMember3D();
            mem2.Name = "Vaakassa_Y";
            mem2.Origin = new Point3D(0, parametrit.Korkeus_palkki, 0);
            mem2.XAxis = new Vector3D(parametrit.Pituus_palkki, 1, 0);
            mem2.YAxis = new Vector3D(0, 1, 0);
            mem2.SizeY = parametrit.Paarre_korkeus;
            mem2.SizeZ = parametrit.Paarre_leveys;
            retVal.AddChild(mem2);

            TimberMember3D mem3 = new Epx.BIM.Models.Timber.TimberMember3D();
            mem3.Name = "Pystyssä_A";
            mem3.Origin = new Point3D(-0.5 * parametrit.Thickness, -0.5 * parametrit.Paarre_korkeus, 0);
            mem3.XAxis = new Vector3D(0, parametrit.Korkeus_palkki + parametrit.Paarre_korkeus, 0);
            mem3.YAxis = new Vector3D(1, 0, 0);
            mem3.SizeY = parametrit.Paarre_korkeus;
            mem3.SizeZ = parametrit.Paarre_leveys;
            retVal.AddChild(mem3);

            TimberMember3D mem4 = new Epx.BIM.Models.Timber.TimberMember3D();
            mem4.Name = "Pystyssä_L";
            mem4.Origin = new Point3D(parametrit.Pituus_palkki + 0.5 * parametrit.Width, -0.5 * parametrit.Paarre_korkeus, 0);
            mem4.XAxis = new Vector3D(0, parametrit.Korkeus_palkki + parametrit.Paarre_korkeus, 0);
            mem4.YAxis = new Vector3D(1, 0, 0);
            mem4.SizeY = parametrit.Paarre_korkeus;
            mem4.SizeZ = parametrit.Paarre_leveys;
            retVal.AddChild(mem4);
           

            //// luodaan boolean leikkaus osalle, aliosa memberille, sijaainti member koordinaatistossa
            // // RectangularCuboid on suorakulmainen suuntaissärmiö
            // RectangularCuboid subtract = new RectangularCuboid();
            // subtract.Origin = new Point3D(0.5 * xVec.Length, 0.5 * parametrit.Width, -1000);
            // subtract.XAxis = new Vector3D(1, 0, 0); // x-akselin suunta osan x-akselin suuntaan
            // subtract.YAxis = new Vector3D(0, 1, 0); // y-akseli osan y-akselin suuntaan
            // // z-akseli lasketaan automaattisesti ristitulolla ja on osan z-akselin suuntaan
            // subtract.XSize = 500; // 500 leveä kolo
            // subtract.YSize = 100;  // 100 "paksu" kolo, koska origo (keskipiste) osan yläreunassa, leikkaa vain puolet
            // subtract.ZSize = 2000; // tämä mitta riittävä, jotta "puhkaisee" koko osan
            // subtract.BooleanOperationType = ModelGeometry3D.BooleanOperationTypeEnum.SubtractionDynamic; // dynaaminen leikkaus
            // mem.AddChild(subtract);
            // luodaan toinen kolo vinosti
            //subtract = new RectangularCuboid();
            //subtract.XAxis = new Vector3D(1, 1, 0); // x-akselin vinosti
            //subtract.YAxis = Vector3D.CrossProduct(new Vector3D(0, 1, 1),subtract.XAxis); // y-akseli ristitulolla myös vinoon
            //// z-akseli lasketaan automaattisesti ristitulolla ja on osan z-akselin suuntaan
            //// asetetaan origo niin, että leikkaa osan yläreunaa
            //Vector3D zaxis = subtract.ZAxis;
            //zaxis.Normalize();
            //subtract.Origin = new Point3D(0.2 * xVec.Length, 0.5 * parametrit.Width, 0) - zaxis * 1000;
            //subtract.XSize = 200; // 200 leveä kolo
            //subtract.YSize = 100;  // 100 "paksu" kolo, koska origo (keskipiste) osan yläreunassa, leikkaa vain puolet
            //subtract.ZSize = 2000; // tämä mitta riittävä, jotta "puhkaisee" koko osan
            //subtract.BooleanOperationType = ModelGeometry3D.BooleanOperationTypeEnum.SubtractionDynamic; // dynaaminen leikkaus
            //mem.AddChild(subtract);
            // toinen aliosa


            //mem = new TimberMember3D();
            //mem.Name = "sauva";
            //mem.Origin = new Point3D(0, 0, 0);
            //mem.XAxis = new Vector3D(0, parametrit.Korkeus_palkki, 0);
            //mem.YAxis = new Vector3D(1, 0, 0);
            //mem.SizeY = parametrit.Width;
            //mem.SizeZ = parametrit.Thickness;
            //retVal.AddChild(mem);


            //mem = new TimberMember3D();
            //mem.Name = "sauva_puoliväli";
            //mem.Origin = new Point3D(0.5 * parametrit.Pituus_palkki, 0, 0);
            //mem.XAxis = new Vector3D(0, parametrit.Korkeus_palkki, 0);
            //mem.YAxis = new Vector3D(1, 0, 0);
            //mem.SizeY = parametrit.Width;
            //mem.SizeZ = parametrit.Thickness;
            //retVal.AddChild(mem);
            if (parametrit.Vinosauvojen_lukumaara == 1)
            {
                TimberMember3D mem5 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem5.Name = "sauva1";
                mem5.Origin = new Point3D(0, 0, 0);
                mem5.XAxis = new Vector3D(parametrit.Pituus_palkki-0.5*parametrit.Width, parametrit.Korkeus_palkki, 0);
                mem5.YAxis = new Vector3D(1, 0, 0);
                mem5.SizeY = parametrit.Width;
                mem5.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem5);
                //lisätään leikkaus
                PlaneCut cut2 = new PlaneCut();
                cut2.Position = new Point3D(0, 0.5*parametrit.Korkeus_palkki, 0);
                cut2.PlaneNormal = new Vector3D(0, 1, 0);
                mem5.AddChild(cut2);
     
            }
            else if (parametrit.Vinosauvojen_lukumaara == 2)
            {
                TimberMember3D mem6 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem6.Name = "sauva2";
                mem6.Origin = new Point3D(0, 0, 0);
                mem6.XAxis = new Vector3D(0.5 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem6.YAxis = new Vector3D(1, 0, 0);
                mem6.SizeY = parametrit.Width;
                mem6.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem6);

                TimberMember3D mem7 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem7.Name = "sauva2";
                mem7.Origin = new Point3D(0.5 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem7.XAxis = new Vector3D(0.5 * parametrit.Pituus_palkki, -parametrit.Korkeus_palkki, 0);
                mem7.YAxis = new Vector3D(1, 0, 0);
                mem7.SizeY = parametrit.Width;
                mem7.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem7);
            }
            else if (parametrit.Vinosauvojen_lukumaara == 3)
            {
                TimberMember3D mem8 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem8.Name = "sauva3";
                mem8.Origin = new Point3D(0, 0, 0);
                mem8.XAxis = new Vector3D(0.33 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem8.YAxis = new Vector3D(1, 0, 0);
                mem8.SizeY = parametrit.Width;
                mem8.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem8);

                TimberMember3D mem9 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem9.Name = "sauva3";
                mem9.Origin = new Point3D(0.33 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem9.XAxis = new Vector3D(0.33 * parametrit.Pituus_palkki, -parametrit.Korkeus_palkki, 0);
                mem9.YAxis = new Vector3D(1, 0, 0);
                mem9.SizeY = parametrit.Width;
                mem9.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem9);

                TimberMember3D mem10 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem10.Name = "sauva3";
                mem10.Origin = new Point3D(0.66 * parametrit.Pituus_palkki, 0, 0);
                mem10.XAxis = new Vector3D(0.33 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem10.YAxis = new Vector3D(1, 0, 0);
                mem10.SizeY = parametrit.Width;
                mem10.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem10);
            }
            else if (parametrit.Vinosauvojen_lukumaara == 4)
            {
                TimberMember3D mem11 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem11.Name = "sauva4";
                mem11.Origin = new Point3D(0, 0, 0);
                mem11.XAxis = new Vector3D(0.25 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem11.YAxis = new Vector3D(1, 0, 0);
                mem11.SizeY = parametrit.Width;
                mem11.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem11);

                TimberMember3D mem12 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem12.Name = "sauva4";
                mem12.Origin = new Point3D(0.25 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem12.XAxis = new Vector3D(0.25 * parametrit.Pituus_palkki, -parametrit.Korkeus_palkki, 0);
                mem12.YAxis = new Vector3D(1, 0, 0);
                mem12.SizeY = parametrit.Width;
                mem12.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem12);

                TimberMember3D mem13 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem13.Name = "sauva4";
                mem13.Origin = new Point3D(0.5 * parametrit.Pituus_palkki, 0, 0);
                mem13.XAxis = new Vector3D(0.25 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem13.YAxis = new Vector3D(1, 0, 0);
                mem13.SizeY = parametrit.Width;
                mem13.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem13);

                TimberMember3D mem14 = new Epx.BIM.Models.Timber.TimberMember3D();
                mem14.Name = "sauva4";
                mem14.Origin = new Point3D(0.75 * parametrit.Pituus_palkki, parametrit.Korkeus_palkki, 0);
                mem14.XAxis = new Vector3D(0.25 * parametrit.Pituus_palkki, -parametrit.Korkeus_palkki, 0);
                mem14.YAxis = new Vector3D(1, 0, 0);
                mem14.SizeY = parametrit.Width;
                mem14.SizeZ = parametrit.Thickness;
                retVal.AddChild(mem14);
            }



            //// leikataan osa vinosti poikki
            //PlaneCut cut = new PlaneCut();
            //cut.Position = new Point3D(0, 0, 0);
            //cut.PlaneNormal = new Vector3D(1, -1, 0);
            //mem.AddChild(cut);
            // pyöreä pilari
            //ConcreteColumn col = new ConcreteColumn();
            //col.Round = true;
            //col.Diameter = 400;
            //col.Origin = new Point3D(0, 500, 0);
            //col.XAxis = new Vector3D(0, 1, 0);
            //col.YAxis = new Vector3D(1, 0, 0);
            //col.Length = 3000;
            //col.ElementColor = Colors.Red;
            //retVal.AddChild(col);
            //Vector3D vekX = new Vector3D(100, 100, 100);
            //Vector3D veKY = Vector3D.CrossProduct(new Vector3D(0, 0, 100), vekX);
            //Matrix3D mat =Epx.BIM.GeometryTools.Matrix3DTool.Get3DMatrix(new Epx.BIM.BaseTools.PointOrVector3D(100, 100, 00),
            //    new Epx.BIM.BaseTools.PointOrVector3D(vekX.X, vekX.Y, vekX.Z), 
            //    new Epx.BIM.BaseTools.PointOrVector3D(veKY.X, veKY.Y, veKY.Z));
            //Point3D transformed = mat.Transform(new Point3D(0, 0, 0));
            //Matrix3D toLocal = Epx.BIM.GeometryTools.Matrix3DTool.Get3DMatrix(col,true);
            //Point3D trans = toLocal.Transform(col.Origin);
            return retVal;
        }
    }
}
