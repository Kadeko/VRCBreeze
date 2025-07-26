using nadena.dev.ndmf;

[assembly: ExportsPlugin(typeof(VRCBreeze.VRCBreezePlugin))]

namespace VRCBreeze
{
    public sealed class VRCBreezePlugin : Plugin<VRCBreezePlugin>
    {
        protected override void Configure()
        {
            InPhase(BuildPhase.Transforming)
                .AfterPlugin("nadena.dev.modular-avatar")
                .WithRequiredExtension(typeof(VRCBreezeContext), seq => seq.Run(VRCBreezeInstallerPass.Instance));
        }
    }
}