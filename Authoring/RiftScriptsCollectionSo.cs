using UnityEngine;

namespace Rift.Externals.Unity.Authoring
{
    [CreateAssetMenu(menuName = "Rift/Scripts Collection")]
    public class RiftScriptsCollectionSo : ScriptableObject
    {
        public RiftScriptSo[] scripts;

        private void OnValidate()
        {
            if (scripts == null) return;
            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] == null)
                    Debug.LogWarning($"{name}: scripts[{i}] is null", this);
            }
        }
    }
}
