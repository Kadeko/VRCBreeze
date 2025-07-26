using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;
using nadena.dev.ndmf;


namespace VRCBreeze
{
    public sealed class VRCBreezeInstallerPass : Pass<VRCBreezeInstallerPass>
    {
        protected override void Execute(BuildContext context)
        {
            var extContext = context.Extension<VRCBreezeContext>();
            foreach (var creator in context.AvatarRootObject.GetComponentsInChildren<VRCBreezeCreator>(true))
                extContext.Install(creator);
        }
    }
}