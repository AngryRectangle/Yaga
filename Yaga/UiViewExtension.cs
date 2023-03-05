using UnityEngine.Events;
using UnityEngine.UI;

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
            view.AddUnsubscription(() => button.onClick.RemoveListener(action));
        }
        
        /// <summary>
        /// Subscribe on button click, execute action and dispose subscription when needed.
        /// </summary>
        public static void SubscribeAndCall(this View view, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            view.AddUnsubscription(() => button.onClick.RemoveListener(action));
            action();
        }
    }
}