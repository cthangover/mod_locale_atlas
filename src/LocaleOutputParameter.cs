namespace Cthangover.LocaleAtlas
{
    using Cthangover.Core.Games.LaunchParameters;

    /// <summary>Sets the output directory for the localization atlas.</summary>
    public sealed class LocaleOutputParameter : StringLaunchParameter
    {
        /// <summary>Creates a definition with the default value.</summary>
        public LocaleOutputParameter() { }

        /// <summary>Parses the value supplied for this parameter.</summary>
        public LocaleOutputParameter(string value) : base(value) { }

        /// <summary>Canonical argument key.</summary>
        public const string Name = "locale-out";

        /// <inheritdoc />
        public override string Key => Name;

        /// <inheritdoc />
        public override string Documentation => "Sets the output directory for the localization atlas.";
    }
}
