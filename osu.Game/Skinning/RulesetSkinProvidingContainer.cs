// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Rulesets;

namespace osu.Game.Skinning
{
    /// <summary>
    /// A type of <see cref="SkinProvidingContainer"/> that provides access to the beatmap skin and user skin,
    /// each transformed with the ruleset's own skin transformer individually.
    /// </summary>
    public class RulesetSkinProvidingContainer : SkinProvidingContainer
    {
        protected readonly Ruleset Ruleset;
        protected readonly IBeatmap Beatmap;

        protected override Container<Drawable> Content { get; }

        public RulesetSkinProvidingContainer(Ruleset ruleset, IBeatmap beatmap, [CanBeNull] ISkin beatmapSkin)
        {
            Ruleset = ruleset;
            Beatmap = beatmap;

            InternalChild = new BeatmapSkinProvidingContainer(GetRulesetTransformedSkin(beatmapSkin))
            {
                Child = Content = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                }
            };
        }

        [Resolved]
        private SkinManager skinManager { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            UpdateSkins();
        }

        protected override void OnSourceChanged()
        {
            UpdateSkins();
            base.OnSourceChanged();
        }

        protected virtual void UpdateSkins()
        {
            SkinSources.Clear();

            SkinSources.Add(GetRulesetTransformedSkin(skinManager.CurrentSkin.Value));

            // TODO: we also want to return a DefaultLegacySkin here if the current *beatmap* is providing any skinned elements.
            if (skinManager.CurrentSkin.Value is LegacySkin)
                SkinSources.Add(GetRulesetTransformedSkin(skinManager.DefaultLegacySkin));

            SkinSources.Add(GetRulesetTransformedSkin(skinManager.DefaultSkin));
        }

        protected ISkin GetRulesetTransformedSkin(ISkin skin)
        {
            if (skin == null)
                return null;

            var rulesetTransformed = Ruleset.CreateLegacySkinProvider(skin, Beatmap);
            if (rulesetTransformed != null)
                return rulesetTransformed;

            return skin;
        }
    }
}
