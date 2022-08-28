using System.Collections.Generic;
using UnityEngine;
using Yaga.Test.Documentation;

namespace Yaga.Test
{
    public class ViewWithChild : View
    {
        [SerializeField] public ModelessView modelessView;
        [SerializeField] public SimpleTextButtonView viewWithModel;
        public override IEnumerable<IView> Children => new IView[]{ modelessView, viewWithModel };
    }
}