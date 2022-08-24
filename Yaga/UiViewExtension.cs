using UnityEngine.Events;
using UnityEngine.UI;
using Yaga.Utils;

namespace Yaga
{
    public static class UiViewExtension
    {
        /// <summary>
        /// Subscribe on button click and dispose subscription when needed.
        /// </summary>
        public static void Subscribe(this View view, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            view.Disposables.Add(new Reflector(() => button.onClick.RemoveListener(action)));
        }
        
        /// <summary>
        /// Subscribe on button click, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall(this View view, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            view.Disposables.Add(new Reflector(() => button.onClick.RemoveListener(action)));
            action();
        }
    }
}