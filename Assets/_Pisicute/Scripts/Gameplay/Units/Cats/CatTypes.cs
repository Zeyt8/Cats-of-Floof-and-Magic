using System.ComponentModel;

public enum CatTypes
{
    [Description("Pallas Cat")]
    Pallas,
    [Description("Big Pallas Cat")]
    BigPallas,
    [Description("Witch Cat")]
    Witch,
    [Description("Void Cat")]
    Void,
    [Description("Sphinx Cat")]
    Sphinx,
    [Description("Tabby Cat")]
    Tabby,
    [Description("Tuxedo Cat")]
    Tuxedo,
    [Description("Orange Cat")]
    Orange,
    [Description("Box Armour Cat")]
    BoxArmour,
    [Description("Nebelung Cat")]
    Nebelung,
    [Description("Teleporting Box Cat")]
    TeleportingBox,
    [Description("Levitating Blob Cat")]
    LevitatingBlob,
    [Description("Desert Blackfoot Cat")]
    DesertBlackFoot,
    [Description("Calico Cat")]
    Calico,
    [Description("Ragdoll Cat")]
    Ragdoll,
    [Description("Scottish Wildcat")]
    ScottishWild,
    [Description("Oriental Cat")]
    Oriental,
    [Description("Mono Coloured")]
    MonoColoured,
}

public static class CatTypesExtensions
{
    public static string GetPrettyName(this System.Enum e)
    {
        var nm = e.ToString();
        var tp = e.GetType();
        var field = tp.GetField(nm);
        var attrib = System.Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

        if (attrib != null)
            return attrib.Description;
        else
            return nm;
    }
}