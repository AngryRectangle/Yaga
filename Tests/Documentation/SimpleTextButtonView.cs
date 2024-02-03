using UnityEngine;
using UnityEngine.UI;

namespace Yaga.Test.Documentation
{
    public class SimpleTextButtonView : View<string>
    {
        [SerializeField] private Text text;
        [SerializeField] private Button button;

        /*
         * Create presenter for view
         * which will handle input from view and apply data from model to view
         */
        public class Presenter : Presenter<SimpleTextButtonView, string>
        {
            protected override void OnSet(SimpleTextButtonView view, string model, ISubscriptionsOwner subs)
            {
                view.text.text = model;
                subs.Subscribe(view.button, () => Debug.Log("Click"));
            }
        }
        
        // For tests.
        public Text Text => text;

        public Button Button => button;
    }
}