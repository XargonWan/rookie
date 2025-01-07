using System;

#if WINDOWS
using System.Drawing;
using System.Windows.Forms;
#elif LINUX
using Gtk;
#endif

namespace AndroidSideloader
{
    public partial class Splash
#if WINDOWS
        : Form
#elif LINUX
        : Window
#endif
    {
        public Splash()
#if WINDOWS
            : base()
#elif LINUX
            : base("Splash")
#endif
        {
#if WINDOWS
            InitializeComponent();
#elif LINUX
            SetDefaultSize(400, 300);
            SetPosition(WindowPosition.Center);
            DeleteEvent += delegate { Application.Quit(); };
            ShowAll();
#endif
        }

        public void UpdateBackgroundImage(
#if WINDOWS
            Image newImage
#elif LINUX
            Gdk.Pixbuf newImage
#endif
        )
        {
#if WINDOWS
            this.BackgroundImage = newImage;
#elif LINUX
            Image image = new Image(newImage);
            Add(image);
            ShowAll();
#endif
        }

#if LINUX
        public static void Main()
        {
            Application.Init();
            new Splash();
            Application.Run();
        }
#endif
    }
}
