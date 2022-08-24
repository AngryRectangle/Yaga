using System;
using System.Collections.Generic;
using UnityEngine;
using Yaga.Binding;
using Yaga.Exceptions;

namespace Yaga
{
    public abstract class View : BaseView
    {
        [SerializeField] private Bind[] _bindings;

        private BindingContext _context = new BindingContext();
        public BindingContext Context => _context;
        public IReadOnlyCollection<Bind> Bindings => _bindings;

        public override IEnumerable<IView> Children => Array.Empty<IView>();

        public override void Close()
        {
            base.Close();
            foreach (var bind in _bindings) bind.View.Close();
        }

        public override void Open()
        {
            base.Open();
            foreach (var bind in _bindings)
            {
                if (bind.Type != Bind.BindType.OptionalObservableField)
                    bind.View.Open();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _context.Dispose();
        }
    }

    public abstract class View<TModel> : View, IView<TModel>
    {
        private TModel _model;
        public bool HasModel { get; set; }

        public TModel Model
        {
            get
            {
                if (!HasModel)
                    throw new ModelIsNotSetException();
                return _model;
            }
        }

        TModel IView<TModel>.Model
        {
            get => _model;
            set => _model = value;
        }

        public void Set(TModel model) => UiBootstrap.Instance.Set(this, model);
        public void Unset(TModel model) => UiBootstrap.Instance.Unset(this);
        public bool Equals(View<TModel> other) => other.GetInstanceID() == GetInstanceID();
    }
}