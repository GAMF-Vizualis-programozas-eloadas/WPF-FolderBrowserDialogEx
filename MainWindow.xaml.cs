using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace PictureBrowser
{
	public partial class MainWindow : Window
	{
    /// <summary>
    /// A bélyegkép vezérlő alap szélessége.
    /// </summary>
    private double kepSzelesseg;
    /// <summary>
    /// A bélyegkép vezérlő alap magassága.
    /// </summary>
    private double kepMagassag;
    /// <summary>
    /// Ennyi ideig tart az animáció.
    /// </summary>
    private TimeSpan tsAnimacioIdo;
    /// <summary>
    /// Az animáció során ennyivel nő a kép szélessége.
    /// </summary>
    private double dSz;
    /// <summary>
    /// Az animáció során ennyivel nő a kép magassága.
    /// </summary>
    private double dM;
    /// <summary>
    /// A főablak konstruktora. Adattagok inicializálása és kezdőképek 
    /// betöltése.
    public MainWindow()
		{
			InitializeComponent();
      //A kép vezérlő eredeti szélessége.
      kepSzelesseg = 70;
      //A kép vezélő eredeti magassága. A felhasznált mintaképek mind //1024x766-os méretűek.
      kepMagassag = kepSzelesseg * 766 / 1024;
      //Az animáció során ennyivel nő a kép vezérlő szélessége.
      dSz = 30;
      //Az animáció során ennyivel nő a kép vezérlő magassága.
      dM = dSz * 766 / 1024;
      //Az animáció időigényének megadása.
      tsAnimacioIdo = TimeSpan.FromMilliseconds(500);
      //A kezdőkönyvtár.
      Title = Directory.GetCurrentDirectory(); ;
      //Beolvassuk a képeket a könyvtárból.
      KepeketBetolt();
    }
    /// <summary>
    /// Betölti a képeket a kiválasztott mappából, és kép vezérlők 
    /// formájában elhelyezi őket a WrapPanel-en.
    /// </summary>
    private void KepeketBetolt()
    {
      //A képeket tartalmazó könyvtár objektum létrehozása.
      DirectoryInfo dI = new DirectoryInfo(Title);
      //Töröljük a WrapPanel-en lévő vezérlők listáját.
      wpPictures.Children.Clear();
      try
      {
        //Lekérdezzük a .jpg kiterjesztésű állományokat a 
        //könyvtárból.
        FileInfo[] fI = dI.GetFiles("*.jpg");
        //Minden képet beolvasunk.
        foreach (FileInfo fajl in fI)
        {
          //A helyőrző létrehozása. Ez nagyobb kell legyen, //mint a kép vezérlő. Amikor növeljük a kép vezérlő //méretét, a helyőrzőt fogja kitölteni.
          Border bdHelyorzo = new Border();
          bdHelyorzo.Width = kepSzelesseg + dSz;
          bdHelyorzo.Height = kepMagassag + dM;
          //Felvesszük a helyőrző a panelre.
          wpPictures.Children.Add(bdHelyorzo);
          //Létrehozunk egy kép objektumot, és betöltjük a //fájlból a képet.
          var imKep = new Image
          { //Kép forrás megadása.
            Source = new BitmapImage(new Uri(fajl.FullName, UriKind.Absolute)),
            Width = kepSzelesseg,
            Height = kepMagassag
          };
          //A kép a vezérlőn töltse ki a rendelkezésre álló //helyet az eredeti képarány megtartásával.
          imKep.Stretch = Stretch.Uniform;
          //A kép vezérlő a helyőrző közepére kerüljön.
          imKep.VerticalAlignment = VerticalAlignment.Center;
          imKep.HorizontalAlignment = HorizontalAlignment.Center;
          //Eseménykezelő rendelése az egérgomb lenyomásához.
          imKep.MouseDown += imKep_MouseDown;
          //Eseménykezelő rendelése az egér vezérlő fölé //érkezéséhez.
          imKep.MouseEnter += imKep_MouseEnter;
          //Eseménykezelő rendelése az egér vezérlő fölüli távozásához.
          imKep.MouseLeave += imKep_MouseLeave;
          //Kép elhelyezése a helyőrzőben.
          bdHelyorzo.Child = imKep;
        }
      }
      catch (Exception e)
      {
        //Hibaüzenet, ha nem sikerült valamelyik művelet.
        MessageBox.Show(e.Message);
      }
      if (wpPictures.Children.Count > 0)
      {
        //Beállítjuk a legelső képet nagynak.
        KepBeallit((Image)((Border)wpPictures.Children[0]).Child);
      }
    }
    /// <summary>
    /// A felhasználó az egeret elmozgatta a képről.
    /// </summary>
    /// <param name="sender">A kép vezérlő objektum.</param>
    /// <param name="e"></param>
    private void imKep_MouseLeave(object sender, MouseEventArgs e)
    {
      var imKep = (Image)sender;
      //A vízszintes méretváltoztatást leíró animáció objektum.
      DoubleAnimation dA = new DoubleAnimation();
      //Kezdőméret.
      dA.From = kepSzelesseg + dSz;
      //Végső méret.
      dA.To = kepSzelesseg;
      //Az animáció időtartama.
      dA.Duration = new Duration(tsAnimacioIdo);
      //A függőleges méretváltoztatást leíró animáció objektum.
      DoubleAnimation dB = new DoubleAnimation();
      //Kezdőméret.
      dA.From = kepMagassag + dM;
      //Végső méret.
      dA.To = kepMagassag;
      //Az animáció időtartama.
      dB.Duration = new Duration(tsAnimacioIdo);
      //A két animáció elindítása.
      imKep.BeginAnimation(WidthProperty, dA);
      imKep.BeginAnimation(HeightProperty, dB);
    }
    /// <summary>
    /// A felhasználó az egeret a kép vezérlő fele mozgatja.
    /// </summary>
    /// <param name="sender">A kép vezérlő objektum.</param>
    /// <param name="e"></param>
    private void imKep_MouseEnter(object sender, MouseEventArgs e)
    {
      //Az aktuális kép objektum.
      var imKep = (Image)sender;
      //A vízszintes méretváltoztatást leíró objektum.
      DoubleAnimation dA = new DoubleAnimation();
      //Kezdőméret.
      dA.From = kepSzelesseg;
      //Végső méret.
      dA.To = kepSzelesseg + dSz;
      //Az animáció időtartama.
      dA.Duration = new Duration(tsAnimacioIdo);
      //A függőleges méretváltoztatást leíró objektum.
      DoubleAnimation dB = new DoubleAnimation();
      //Kezdőméret.
      dB.From = kepMagassag;
      //Végső méret.
      dB.To = kepMagassag + dM;
      //Az animáció időtartama.
      dB.Duration = new Duration(tsAnimacioIdo);
      //A két animáció elindítása.
      imKep.BeginAnimation(WidthProperty, dA);
      imKep.BeginAnimation(HeightProperty, dB);
    }
    /// <summary>
    /// Beállítjuk a képet a nagy vezérlőben láthatónak.
    /// </summary>
    /// <param name="imKep"></param>
    private void KepBeallit(Image imKep)
    {
      //A kép forrása.
      imNagyKep.Source = imKep.Source;
      //A kép alatt megjeleneítjük az állomány nevét.
      tbKepNev.Text = imNagyKep.Source.ToString();
    }
    /// <summary>
    /// Egérgomb lenyomása egy kép vezérlőn.
    /// </summary>
    /// <param name="sender">A kép vezérlő.</param>
    /// <param name="e"></param>
    private void imKep_MouseDown(object sender, MouseButtonEventArgs e)
    {
      //Beállítjuk a képet a nagy vezérlőn láthatónak.
      KepBeallit((Image)sender);
    }
    /// <summary>
    /// A képeket tartalmazó könyvtár kiválasztása.
    /// </summary>
    /// <param name="sender">Az eseményt előidéző nyomógomb.</param>
    /// <param name="e">Kiegészítő paraméterek.</param>
    private void btFolder_Click(object sender, RoutedEventArgs e)
    {
      //Mappaválasztó párbeszédablak objektum létrehozása.
      var dlg = new System.Windows.Forms.FolderBrowserDialog();
      //Kezdőkönyvtár beállítása az exe könyvtárára.
      dlg.RootFolder = Environment.SpecialFolder.Desktop;
      
      dlg.SelectedPath = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory() ).FullName).FullName;
      //Párbeszédablak megjeleneítése.
      if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
      {
          //A mappa elérési útvonalának átmásolása az ablak fejlécbe.
          Title = dlg.SelectedPath;
          //A képek betöltése a kiválasztott mappából, és elhelyezésük kép vezérlők formájában a WrapPanel-en.
          KepeketBetolt();
      }
    }
  }
}
