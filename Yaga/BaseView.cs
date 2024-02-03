using Optional;
using UnityEngine;

namespace Yaga
{
    public abstract class BaseView<TModel> : MonoBehaviour, IView<TModel>
    {
        public bool IsPrefab => gameObject.scene.name is null;

        private Option<RectTransform> _rootParent = Option.None<RectTransform>();
        private Option<(TModel Model, Subscriptions Subs)> _model;

        Option<(TModel Model, Subscriptions Subs)> IView<TModel>.Model
        {
            get => _model;
            set => _model = value;
        }
        
        Option<Subscriptions> IView.Model => _model.Map(m => m.Subs);


        IView IView.Create(RectTransform parent, bool isRoot)
        {
            var result = Instantiate(this, parent);
            if (isRoot)
                result._rootParent = parent.Some();
            return result;
        }

        public virtual void Destroy()
        {
            Destroy(_rootParent.Match(parent => parent.gameObject, () => gameObject));
        }
        
        public void OnDestroy()
        {
            _model.MatchSome(model => model.Subs.Dispose());
        }

        public bool Equals(IView other) => other != null && other.GetInstanceID() == GetInstanceID();
        public override string ToString() => gameObject.name;
    }
}