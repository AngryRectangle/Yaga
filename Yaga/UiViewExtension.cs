using UnityEngine.Events;
using UnityEngine.UI;
using Yaga.Utils;

namespace Yaga
{
    public static class UiViewExtension
    {
        public static void Subscribe(this View view, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            view._disposables.Add(new Reflector(() => button.onClick.RemoveListener(action)));
        }
        
        public static void SubscribeAndCall(this View view, Button button, UnityAction action)
        {
            button.onClick.AddListener(action);
            view._disposables.Add(new Reflector(() => button.onClick.RemoveListener(action)));
            action();
        }
    }
}