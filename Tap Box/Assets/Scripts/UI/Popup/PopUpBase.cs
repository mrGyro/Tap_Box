using System;
using Unity.VisualScripting;
using UnityEngine;

namespace DefaultNamespace.Managers
{
    public class PopUpBase : MonoBehaviour, IInitializable
    {
        public int Priority;
        public string ID;
        public bool IsShowing;
        public Action<PopUpBase> OnShow;
        public Action<PopUpBase> OnClose;
        public virtual void Show(){}
        public virtual void Close(){}
        public virtual void Initialize() { }
    }
}