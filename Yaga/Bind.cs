using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Yaga.Utils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Yaga
{
    [Serializable]
    public class Bind
    {
        private const string WholeModel = "&WholeModel";
        [SerializeField] private string fieldName;
        [SerializeField] private GameObject view;
        [SerializeField] private MonoBehaviour acceptedView;
        [SerializeField] private int bindType;
        private IDisposable _deApply;

        public BindType Type => (BindType) bindType;

        public IView View => acceptedView as IView;

        public static Result TryGetView(
            Type parentModelType,
            GameObject view,
            string fieldName,
            out MonoBehaviour acceptedView,
            out BindType bindType
        )
        {
            acceptedView = default;
            bindType = default;

            var viewsOnTarget = view.GetComponents<IView>();
            if (viewsOnTarget.Length == 0)
                return Result.NoViewOnGameObject;
            if (viewsOnTarget.Length > 1)
                return Result.MoreThenOneView;

            var viewOnTarget = viewsOnTarget[0];
            var viewBaseGenerics = viewOnTarget
                .GetType()
                .GetInterfaces()
                .Where(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IView<>));
            var genericsCount = viewBaseGenerics.Count();
            if (genericsCount == 0)
                return Result.NotImplementedGenericIView;
            if (genericsCount > 1)
                return Result.MoreThenOneImplementationOfIVew;

            var modelType = viewBaseGenerics.Single().GetGenericArguments()[0];
            var field = parentModelType
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SingleOrDefault(e => e.Name == fieldName);

            if (field == default && fieldName != WholeModel)
                return Result.NoFieldWithName;

            var fieldType = fieldName == WholeModel ? parentModelType : field.FieldType;
            var isObservable = TryGet(typeof(Utils.IObservable<>), fieldType, out var inObservableType);
            if (modelType.IsAssignableFrom(fieldType))
            {
                if (isObservable)
                    Debug.LogWarning($"It is useless to have model of observable type in view {viewOnTarget}");

                acceptedView = viewOnTarget as MonoBehaviour;
                bindType = BindType.SimpleField;
                return Result.Success;
            }

            if (isObservable && modelType.IsAssignableFrom(inObservableType))
            {
                acceptedView = viewOnTarget as MonoBehaviour;
                bindType = BindType.ObservableField;
                return Result.Success;
            }

            isObservable = TryGet(typeof(IOptionalObservable<>), fieldType, out inObservableType);
            if (isObservable && modelType.IsAssignableFrom(inObservableType))
            {
                acceptedView = viewOnTarget as MonoBehaviour;
                bindType = BindType.OptionalObservableField;
                return Result.Success;
            }

            if (TryGet(typeof(IEnumerable<>), fieldType, out var inEnumerableType))
            {
                if (!TryGet(typeof(IListView<>), viewOnTarget.GetType(), out var inListType))
                    return Result.TypeMismatch;

                if (inEnumerableType != inListType)
                    return Result.TypeMismatch;

                acceptedView = viewOnTarget as MonoBehaviour;
                bindType = BindType.ListView;
                return Result.Success;
            }

            return Result.TypeMismatch;
        }

        public void Apply<TModel>(TModel model)
        {
            if (acceptedView == null)
                return;

            var field = model.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .SingleOrDefault(e => e.Name == fieldName);

            var dataToFollow = fieldName == WholeModel ? model : field.GetValue(model);
            var followType = dataToFollow.GetType();
            switch ((BindType) bindType)
            {
                case BindType.SimpleField:
                    CallSet(dataToFollow, "SetView");
                    break;
                case BindType.ObservableField:
                    var innerType = followType.GetGenericArguments()[0];
                    var methodInfo = followType.GetMethod("Subscribe");
                    var action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(innerType), this,
                        typeof(Bind).GetMethod(nameof(SetView), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(innerType));
                    _deApply = (IDisposable) methodInfo
                        .Invoke(dataToFollow, new object[]
                        {
                            action
                        });

                    action.DynamicInvoke(followType.GetRuntimeProperty("Data").GetValue(dataToFollow));
                    break;
                case BindType.OptionalObservableField:
                    innerType = followType.GetGenericArguments()[0];
                    methodInfo = followType.GetMethod(nameof(OptionalObservable<object>.Subscribe));
                    action = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(innerType), this,
                        typeof(Bind).GetMethod(nameof(OpenView), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(innerType));

                    _deApply = (IDisposable) methodInfo
                        .Invoke(dataToFollow, new object[]
                        {
                            action, new Action(() =>
                            {
                                UiBootstrap.Instance.Unset(View);
                            })
                        });

                    if (!(bool) followType.GetRuntimeProperty(nameof(OptionalObservable<object>.IsDefault))
                        .GetValue(dataToFollow))
                        action.DynamicInvoke(followType.GetRuntimeProperty("Data").GetValue(dataToFollow));
                    break;
                case BindType.ListView:
                    CallSet(dataToFollow, "SetList");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetView<T>(T value) => CallSet(value, "SetView");

        private void OpenView<T>(T value)
        {
            SetView(value);
        }

        public void DeApply() => _deApply?.Dispose();

        private void CallSet(object dataToFollow, string name)
        {
            var followType = dataToFollow.GetType();
            var viewType = View.GetType();
            if (name.Equals("SetList"))
            {
                TryGet(typeof(IEnumerable<>), followType, out followType);
                viewType = viewType.BaseType.GetGenericArguments()[0];
            }

            var methodInfo = typeof(UiBootstrap).GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.MakeGenericMethod(viewType, followType)
                .Invoke(UiBootstrap.Instance, new[] {View, dataToFollow});
        }

        private static bool TryGet(Type interfaceType, Type type, out Type innerType)
        {
            innerType = default;
            foreach (var @interface in type.GetInterfaces())
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == interfaceType)
                {
                    if (innerType != default)
                    {
                        return false;
                    }

                    innerType = @interface.GetGenericArguments()[0];
                }
            }

            return innerType != default;
        }

        private Type GetGeneric(IView view)
        {
            return view
                .GetType()
                .GetInterfaces()
                .SingleOrDefault(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IView<>));
        }

        public enum BindType
        {
            SimpleField,
            ObservableField,
            OptionalObservableField,
            ListView
        }

        public enum Result
        {
            NoViewOnGameObject,
            MoreThenOneView,
            NotImplementedGenericIView,
            MoreThenOneImplementationOfIVew,
            NoFieldWithName,
            TypeMismatch,
            Success
        }

        #if UNITY_EDITOR
        [CustomPropertyDrawer(typeof(Bind))]
        public class Drawer : PropertyDrawer
        {
            private const int HelpBoxHeight = 40;

            public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
            {
                var view = property.serializedObject.targetObject;
                var modelType = GetModelType(view as IView);
                var fieldNameProperty = property.FindPropertyRelative(nameof(fieldName));
                var currentValue = fieldNameProperty.stringValue;
                var variants = GetFieldNames(modelType).ToList();
                variants.Add(WholeModel);
                var index = 0;
                for (var i = 0; i < variants.Count; i++)
                {
                    if (variants[i] != currentValue)
                        continue;
                    index = i;
                    break;
                }

                var previousHeight = 0f;
                var newRect = new Rect(position.x,
                    position.y + previousHeight, position.width,
                    EditorGUI.GetPropertyHeight(fieldNameProperty, label, true));
                EditorGUI.BeginProperty(newRect, label, fieldNameProperty);
                index = EditorGUI.Popup(newRect, index, variants.ToArray());
                fieldNameProperty.stringValue = variants[index];
                EditorGUI.EndProperty();


                var viewProperty = property.FindPropertyRelative(nameof(view));
                previousHeight += EditorGUIUtility.standardVerticalSpacing + newRect.height;
                newRect = new Rect(position.x,
                    position.y + previousHeight, position.width,
                    EditorGUI.GetPropertyHeight(viewProperty, label, true));
                EditorGUI.BeginProperty(newRect, label, viewProperty);
                EditorGUI.BeginChangeCheck();
                var selected
                    = EditorGUI.ObjectField(newRect, viewProperty.objectReferenceValue, typeof(GameObject), true) as
                        GameObject;
                if (EditorGUI.EndChangeCheck())
                {
                    viewProperty.objectReferenceValue = selected;
                }

                if (selected != null)
                {
                    var result = TryGetView(modelType, selected, fieldNameProperty.stringValue, out var viewResult,
                        out var bindType);
                    if (result == Result.Success)
                    {
                        property.FindPropertyRelative(nameof(acceptedView)).objectReferenceValue = viewResult;
                        property.FindPropertyRelative(nameof(bindType)).intValue = (int) bindType;
                    }
                    else
                    {
                        property.FindPropertyRelative(nameof(acceptedView)).objectReferenceValue = null;
                        previousHeight += EditorGUIUtility.standardVerticalSpacing + newRect.height;
                        newRect = new Rect(position.x,
                            position.y + previousHeight, position.width,
                            HelpBoxHeight);

                        var text = GetErrorText(result);
                        EditorGUI.HelpBox(newRect, string.Format(text, selected.name, modelType), MessageType.Error);
                    }
                }

                EditorGUI.EndProperty();
                property.serializedObject.ApplyModifiedProperties();
            }

            private string GetErrorText(Result result)
            {
                switch (result)
                {
                    case Result.NoViewOnGameObject:
                        return "Selected GameObject \"{0}\" does not contains view with model type {1}";
                    case Result.MoreThenOneView:
                        return "Selected GameObject \"{0}\" has more then one contains view with model type {1}";
                    case Result.NotImplementedGenericIView:
                        return "Selected GameObject \"{0}\" doesn't contains IView<{1}> implementation";
                    case Result.MoreThenOneImplementationOfIVew:
                        return "Selected GameObject \"{0}\" doesn't contains IView<{1}> implementation";
                    case Result.NoFieldWithName:
                        return "Selected GameObject \"{0}\" with model {1} has no field with selected name";
                    case Result.TypeMismatch:
                        return "Selected GameObject \"{0}\" view implement another type of model then {1}";
                    default: throw new ArgumentOutOfRangeException(nameof(result), result, null);
                }
            }

            private Type GetModelType(IView view) => GetGeneric(view).GetGenericArguments()[0];

            private Type GetGeneric(IView view)
            {
                return view
                    .GetType()
                    .GetInterfaces()
                    .Single(e => e.IsGenericType && e.GetGenericTypeDefinition() == typeof(IView<>));
            }

            private string[] GetFieldNames(Type type) => type
                .GetFields(BindingFlags.Instance | BindingFlags.Public).Select(e => e.Name)
                .ToArray();

            public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
            {
                var height = EditorGUIUtility.standardVerticalSpacing;
                if (property.FindPropertyRelative(nameof(acceptedView)).objectReferenceValue == null &&
                    property.FindPropertyRelative(nameof(view)).objectReferenceValue != null)
                    height += HelpBoxHeight + EditorGUIUtility.standardVerticalSpacing;

                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(fieldName)), label);
                height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(nameof(view)), label);
                return height;
            }
        }
        #endif
    }
}