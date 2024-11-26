using MudBlazor;

namespace ConsolaBlazor.CustomStyle
{
    public class AtowerTheme
    {
        public static MudTheme Default { get; private set; }

        public AtowerTheme()
        {
            Default = new MudTheme()
            {
                PaletteLight = new PaletteLight()
                {
                    Primary = "#06a1b9",
                    Secondary = Colors.Blue.Accent2,
                    AppbarBackground = Colors.Blue.Darken2,
                    Background = "#f8f8f8"
                },
                PaletteDark = new PaletteDark()
                {
                    Primary = Colors.Blue.Lighten1
                },
                LayoutProperties = new LayoutProperties()
                {
                    DrawerWidthLeft = "260px",
                    DrawerWidthRight = "300px"
                }
            };
        }

    }
}
