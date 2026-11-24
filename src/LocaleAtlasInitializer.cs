using Cthangover.Core.Mods;
using Cthangover.Core.Utils;
using Godot;

namespace Cthangover.LocaleAtlas
{
    using Cthangover.Core.Games;
    using Cthangover.Core.Games.LaunchParameters;
    /// <summary>
    /// Activates the key map only on demand: the <c>--locale-atlas</c> user
    /// arg (after <c>--</c>) queues <see cref="LocaleAtlasDriverNode"/>,
    /// which waits for the boot, writes the report and quits. Optional
    /// <c>--locale-out=&lt;dir&gt;</c> (res://, user:// or absolute) sets the
    /// report directory, <c>--timeout=&lt;s&gt;</c> the watchdog.
    /// </summary>
    public class LocaleAtlasInitializer : IModInitializer
    {
        public const string Flag = "--" + LocaleAtlasParameter.Name;
        private const string DefaultOut = "user://locale_atlas";

        private static bool queued;

        public void OnModLoaded(string modId) { }

        public void OnModResourcesReady()
        {
            if (queued)
                return;

            var args = GameLaunchArgs.Current;
            if (!args.Contains<LocaleAtlasParameter>())
                return;

            var output = args.Get<LocaleOutputParameter>().Value ?? DefaultOut;
            var timeout = args.Get<TimeoutParameter>().Value;

            queued = true;
            var directory = ProjectSettings.GlobalizePath(output);
            GameLogger.Log("LOCALE_ATLAS", $"locale atlas requested, report directory '{directory}'");
            ModRoot.AddDeferred(new LocaleAtlasDriverNode(directory, timeout > 0 && timeout <= float.MaxValue ? (float)timeout : 120f));
        }
    }
}
