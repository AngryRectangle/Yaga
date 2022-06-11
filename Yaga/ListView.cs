using System.Collections.Generic;
using UnityEngine;
using Yaga.Exceptions;
using Yaga.Utils;

namespace Yaga
{
    public interface IListView<TModel> : IView<IObservableEnumerable<TModel>>
    {
        
    }
    public class ListView<TChild, TModel> : BaseView, IListView<TModel> 
        where TChild : View<TModel>
    {
        [SerializeField] private TChild _prefab;
        [SerializeField] private RectTransform _childHolder;

        private bool _modelWasSet;
        private List<TChild> _children = new List<TChild>();
        private IObservableEnumerable<TModel> _model;
        public override IEnumerable<IView> Children => _children;

        private void OnItemAdd(int index, TModel added) => AddChild(added);
        private void OnItemRemove(int index, TModel removed) => UiControl.Instance.Destroy(_children[index]);

        private void AddChild(TModel model)
        {
            var view = UiControl.Instance.Create(_prefab, model, _childHolder);
            _children.Add(view);
        }

        public class Presenter : Presenter<ListView<TChild, TModel>, IObservableEnumerable<TModel>>
        {
            protected override void OnModelSet(ListView<TChild, TModel> view, IObservableEnumerable<TModel> model)
            {
                model.ItemAdded += view.OnItemAdd;
                model.ItemRemoved += view.OnItemRemove;
                foreach (var current in model) view.AddChild(current);
            }

            protected override void OnModelUnset(ListView<TChild, TModel> view)
            {
                view._model.ItemAdded -= view.OnItemAdd;
                view._model.ItemRemoved -= view.OnItemRemove;
                foreach (var child in view.Children) child.Close();
                view._children.Clear();
            }
        }

        public bool HasModel { get; set; }
        public IObservableEnumerable<TModel> Model
        {
            get
            {
                if (!HasModel)
                    throw new ModelIsNotSetException();
                return _model;
            }
        }

        IObservableEnumerable<TModel> IView<IObservableEnumerable<TModel>>.Model
        {
            get => _model;
            set => _model = value;
        }
        
        public void Set(IObservableEnumerable<TModel> model) => UiBootstrap.Instance.Set(this, model);
        public void Unset(IObservableEnumerable<TModel> model) => UiBootstrap.Instance.Unset(this);
    }
}