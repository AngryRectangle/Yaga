using UnityEngine;
using Yaga.Test.Documentation;

namespace Yaga.Test
{
    public class ViewWithChild : View
    {
        [SerializeField] public ModelessView modelessView;
        [SerializeField] public SimpleTextButtonView viewWithModel;
    }
}