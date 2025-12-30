using Collections;

public static class StainExtensions
{
    public static string HEXcolor(this Stain stain)=> StainColorConverter.DecimalToHex((int)stain.Color);
    public static RGBColor RGBcolor(this Stain stain) => StainColorConverter.HexToRGB(stain.HEXcolor());
    public static Vector4 VecColor(this Stain stain) {
        var rgb = stain.RGBcolor();
        return new Vector4(rgb.R / 255f, rgb.G / 255f, rgb.B / 255f, 1);
    }
}