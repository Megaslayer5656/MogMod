using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace MogMod.Common.Systems
{
    // copied from calamity, who got it from NIGHTSHADE
    // Tomat: Adapted from NIGHTSHADE (ILoadableTagHandler<TSelf>), but I'm
    // granting us this to use here.
    /// <summary>
    /// Abstracted implementation of <see cref="ITagHandler"/> with autoloading
    /// capabilities for defining chat tags like modded content.
    /// </summary>
    /// <typeparam name="TSelf">The chat tag for loading.</typeparam>
    public abstract class AbstractTagHandler<TSelf> : ITagHandler, ILoadable
        where TSelf : AbstractTagHandler<TSelf>, new()
    {
        /// <summary>
        ///     Aliases for the tag.
        /// </summary>
        protected abstract string[] TagNames { get; }

        /// <inheritdoc cref="ITagHandler.Parse"/>
        public abstract TextSnippet Parse(string text, Color baseColor = new(), string options = null);

        public virtual void Load(Mod mod)
        {
            ChatManager.Register<TSelf>(TagNames);
        }

        public virtual void Unload()
        {
            // No need to un-register the tag.  ChatManager gets reinitialized
            // by tML in response to modders having adding tags previously,
            // anyway.
        }
    }
}