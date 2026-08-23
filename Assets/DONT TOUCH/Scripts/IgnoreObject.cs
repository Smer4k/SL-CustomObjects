using UnityEngine;

namespace DONT_TOUCH.Scripts
{
    [ExecuteInEditMode]
    public sealed class IgnoreObject : MonoBehaviour
    {
        public void Start()
        {
            gameObject.hideFlags |= HideFlags.NotEditable;
        }
    }
}