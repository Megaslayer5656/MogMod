using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace MogMod.Common.Graphics;

// lifted from terrarias shader drawing stuff
// TODO: fix trail drawing spawn pos being a few steps back
[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct TrailDrawer
{
    private static VertexStrip _vertexStrip = new();
    private float transitToDark;
    private Color _trailColor1;
    private Color _trailColor2;
    private float _stripDivider;
    private float _stripLengthMin;
    private float _stripLengthMax;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="proj">The projectile to draw behind.</param>
    /// <param name="gameShaderName">What shader to use.</param>
    /// <param name="outerColor">The color that is applied near the edge of the trail.</param>
    /// <param name="innerColor">The color that is applied near the start of the trail.</param>
    /// <param name="stripDivider">The Divisor applied to the trails width.</param>
    /// <param name="minLength">The minimum length the trail must be.</param>
    /// <param name="maxLength">The maximum length the trail must be.</param>
    public void Draw(Projectile proj, string gameShaderName, Color outerColor, Color innerColor, float stripDivider = 1f, float minLength = 16f, float maxLength = 24f)
    {
        transitToDark = Utils.GetLerpValue(0f, 6f, proj.localAI[0], clamped: true);
        _trailColor1 = innerColor;
        _trailColor2 = outerColor;
        _stripDivider = stripDivider;
        _stripLengthMin = minLength;
        _stripLengthMax = maxLength;
        MiscShaderData miscShaderData = GameShaders.Misc[gameShaderName];
        miscShaderData.UseSaturation(-2f);
        miscShaderData.UseOpacity(MathHelper.Lerp(4f, 8f, transitToDark));
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }
    private Color StripColors(float progressOnStrip)
    {
        float lerpValue = Utils.GetLerpValue(0f - 0.1f * transitToDark, 0.7f - 0.2f * transitToDark, progressOnStrip, clamped: true);
        Color result = Color.Lerp(_trailColor1, _trailColor2, lerpValue) * (1f - Utils.GetLerpValue(0f, 0.98f, progressOnStrip));
        result.A /= 8;
        return result;
    }
    private float StripWidth(float progressOnStrip)
    {
        float lerpValue = Utils.GetLerpValue(0f, 0.06f + transitToDark * 0.01f, progressOnStrip, clamped: true);
        lerpValue = _stripDivider - (0.5f - lerpValue) * (0.5f - lerpValue);
        return MathHelper.Lerp(_stripLengthMin + transitToDark * 14f, _stripLengthMax, Utils.GetLerpValue(0f, 1f, progressOnStrip, clamped: true)) / lerpValue;
    }
}