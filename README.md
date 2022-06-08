Yaga
====
**Yaga** is a simple UI lib for Unity3D.
Paradigm of the lib is *MVPb Via MfD* (MVP but View is also Model for Data).
Just joking, it is MVVM library.

Attention! Achtung! Внимание!
-
Breaking changes alert!
Breaking changes are possible due to early development stage.

Installation guide
-
Click on top panel "Window > PackageManager".
Click "+" on the top left and paste link to repository:
```https://github.com/AngryRectangle/Yaga.git```

![Installation screenshot](https://i.postimg.cc/zfmXHrSk/img.png)

Getting Started
-
Firstly you need to create view class which will be in inspector 
and create presenter class which will help you to change your view depending
on values in model.

In this example SimpleTextButtonView is just a wrapper for button with text.
It will allow you change text on button and do something when button is clicked.

```c#
// Create class for view with string model.
public class SimpleTextButtonView : View<string>
{
    // Create fields for UI components.
    [SerializeField] private Text buttonText;
    [SerializeField] private Button button;
    
    // Create presenter for view, which will handle input from view and apply data from model.
    public class Presenter : Presenter<SimpleTextButtonView, string>
    {
        protected override void OnModelSet(SimpleTextButtonView view, string model)
        {
            // Set string value to text on button.
            view.buttonText.text = model;
            // Subscribe on button onClick and log messages after every click.
            view.Subscribe(view.button, () => Debug.Log("Click"));
        }
    }
}
```

Then you have to initialize library classes. You have to do it only once.

```c#
// Call initialization method to initialize singleton.
UiBootstrap.InitializeSingleton();
// Initialize UiControl with canvas prefab.
UiControl.InitializeSingleton(canvasPrefab);

// Bind presenter to make library call its method when it needed.
UiBootstrap.Bind(new SimpleTextButtonView.Presenter());
```

After initialization you only have write single line to create instance of view
with "Sample text" on button. Also, when you click button, you will see "Click"
in the console.
```c#
// Create instance of sample view with "Sample text" on button.
UiControl.Instance.Create(Locator.simpleTextButtonView, "Sample text");
```

## Initialization

UI initialization consists from two steps:
1. Initialization of UiControl and UiBootstrap singletons
2. Presenters binding

### Singletons
UiControl provides you shortcut methods for UI controls. 
It requires `Canvas` prefab to create views with auto-created parents.
So you have to initialize UiControl with canvas prefab:
```c#
UiControl.InitializeSingleton(Canvas canvasPrefab)
```
Also you have to initialize UiBootstrap singleton. 
You can do it either with shortcut method:
```c#
UiBootstrap.InitializeSingleton();
```
Or if you are using DI containers, 
you can put instance of UiBootstrap DI container
to get benefits of autobinding of presenters.
Example with Zenject:
```c#
// Binding inside Installer class.
Container.Bind<UiBootstrap>();
Container.BindInterfacesTo<Presenter>();

// Singleton initialization inside IInitializable class with injected UiBootstrap.
UiBootstrap.InitializeSingleton(_uiBootstrap);
```
### Presenters binding
You have to put your presenter for every view type you have created.
It is possible either with shortcut method for presenters with
parameterless constructors or just with binding instance of your presenter.
```c#
// For presenter with parameterless constructor.
UiBootstrap.Bind<Presenter>();

// For presenters with parameters in constructor.
UiBootstrap.Bind(new Presenter(param1, param2));
```

## View
View is monobehaviours which is used to show your data to player and handler player input.
Each view can have concrete model type like int, string, our your custom type. 
Or it can have no model. Also each view has one and only one Presenter.

Relation scheme between view, presenter and model:
`View ⇆ Presenter ⇆ Model`

Each view is going through these stages:
1. Create - initializes game object and make it inactive after instantiation
2. Open - shows view after model was set
3. Close - hides view after model unset
4. Destroy - preparing view for destroy and call Destroy method on it.

Each stage should be called in represented order
and order violation is strongly not recommended.
Also Open method should be called after model set and clos after unset
but about it in Presenters section.


Особенности Yaga
-

Важной особенностью Yaga является приверженность подходу *MVPb Via MfD*.

Каждому типу View соответствует одна и более визуальных компонент. Визуальная компонента - это префаб, с повешенным на
него скриптом View.

Вся структура UI заранее жёстко задана, исключением являются ListView, который может содержат неограниченное число
дочерних компонент с одним типом View и могут менять их количество в процессе работы.

У каждой визуальной компоненты может быть неограниченное число дочерних визуальных компонент, UI Flow которых
управляет Presenter родительской визуальной компоненты.

UI Flow
-

UI Flow можно описать такой схемой: Create -> Set -> Open -> Update -> Close -> Unset -> Destroy.

- Create - происходит инициализация View, а Presenter подписывается на получение ввода игрока со View.
- Set - устанавливается Model для View, Presenter подписывается на необходимые поля в Model. На этом этапе Presenter
  также может отобразить Model на View.
- Open - View показывается пользователю.
- Update - Presenter получает обновления для View каждый кадр, вызывается только для открытых View.
- Close - View скрывается от пользователя.
- Unset - Model удаляется со View, а Presenter отписывается от обновлений.
- Destroy - Presenter отписывается от всех событий во View, а сама View безвозвратно уничтожается.

Контролировать UI Flow возможно через класс [UiBootstrap](UiBootstrap.cs), но вне Presenter'а рекомендуется делать это
через класс [UiControl](UiControl.cs), который даёт более высокоуровневые возможности и гарантирует корректность UI
Flow. За корректное выполнение UI Flow для дочерних визуальных компонент отвечает Presenter родительской визуальной
компоненты. Реализация UI Flow зависит от реализации в конкретном Presenter.

UI Flow следует понимать как матрёшку из вложенных этапов вида:

- Create
    - Set
        - Open
            - Update
        - Close
    - Unset
- Destroy

Для перехода на более глубокий уровень нужно обязательно выполнить этап на текущем уровне. Точно также для перехода на
более высокий уровень нужно выполнить завершающий этап на текущем уровне. Например, после вызова этапа Open нельзя
вызывать Unset, т.к. перед этим должен быть вызван Close. Корректость UI Flow может быть нарушена при использовании
[UiBootstrap](UiBootstrap.cs) вместо [UiControl](UiControl.cs), что неизбежно происходит в Presenter, потому как каждый
Presenter сам отвечает за корректность обработки этапов для себя и дочерних визуальных компонент.


Observable
-
В Yaga паттерн Observer реализован с использованием IObservable, IOptionalObservable,
IObservableEnumerable, IObservableArray. Последние два просто имеют ивенты при изменении / добавлении / удалении
элементов. Намного интереснее ситуация с IObservable, IOptionalObservable.

### Подписка и отписка

Например, подписка на IObservable вернёт IDisposable объект с помощью которого можно завершить подписку.
Это позволяет отписывать лямбды от IObservable без boilerplate.

```c#
var someObservable = new Observable(100);
var disposable = someObservable.Subscribe(value => Debug.Log(value))
// Some code
disposable.Dispose();
```

### Цепочки биндингов

Пример простой цепочки Observables,
в которой каждый следующий Observable реагирует на изменения в предыдущем.

```c#
var amount = new Observable(100); // Observale that will trigger itemInfo on change
var itemInfo = new Observable<string>(); // Observable that will be triggered by amount

// Specify binding rule for data from amount to itemInfo
itemInfo.Bind(amount, count => $"Current amount is {count}")

//Specify logic that will be invoked when itemInfo changes
var disposable = itemInfo.Subscribe(info => Debug.Log(info))
amount.Data = 5 // Will trigger **itemInfo** that will trigger message "Current amount is 5"
disposable.Dispose();
```

Несколько биндингов можно совместить в один биндинг создавая "цепочки" биндингов.
Как только изменится хотя бы один дочерний Observable, будет изменён и родительский.

```c#
var amount = new Observable(100);
var itemName = new Observable("cake");
var itemInfo = new Observable<string>().Bind(amount, itemName, (count, name) => $"{name} {count}")
var disposable = itemInfo.Subscribe(info => Debug.Log(info))

itemName.Data = "apple" // Will trigger message to console "apple 100"
amount.Data = 5 // Will trigger message to console "apple 5"

disposable.Dispose();
```

### OptionalObservable
OptionalObservable следует использовать в те моменты когда нужно гарантировать
правильную обработку default значений. Гарантию обеспечивает необходимость задать отдельную логику,
на случай default значения.

```c#
var someObservable = new OptionalObservable(100);
var disposable = someObservable.Subscribe(value => Debug.Log(value), () => Debug.Log("No count"))

someObservable.SetDefault(); // Will trigger "No count" message.
someObservable.Data = 5 // Will trigger "5" message.

disposable.Dispose();
```
С OptionalObservable можно также создавать цепочки.
Если хотя бы один из дочерних узлов получит "дефолтное" значение, то и родительский OptionalObservable его получит.
```c#
var amount = new OptionalObservable(100);
var itemName = new OptionalObservable("cake");
var itemInfo = new OptionalObservable<string>().Bind(amount, itemName, (count, name) => $"{name} {count}")
var disposable = itemInfo.Subscribe(info => Debug.Log(info), () => Debug.Log("No valid info"))

itemName.Data = "apple" // Will trigger message to console "apple 100"
amount.SetDefault() // Will trigger message to console "No valid info"

disposable.Dispose();
```

Использование
----

1. Создание Model.
2. Создание View, реализующую интерфейс [IView\<Model>](IView.cs) для созданной модели.
3. Создание Presenter, реализующий интерфейс [IPresenter<View, Model>](Controller.cs) и перегружающий необходимые
   методы.
4. Биндинг Presenter в [UiBootstrap](UiBootstrap.cs)

### Пример простой связки Model-View-Presenter

Простая модель в виде Enum

```c#
public enum EResourceType
{
    Wood,
    Iron
}
```

View и Presenter для этой модели

```c#
public class ResourceIconView : View<EResourceType>
{
    [SerializeField] private Image _resourceIcon;
        
    // This view has no children, so children Enumerable must be empty.
    public override IEnumerable<IView> Children => Array.Empty<IView>();

    public class Presenter : Presenter<ResourceIconView, EResourceType>
    {
        // Some dependencies of Preseter that are managed from outside.
        private readonly ResourceIconProvider _resourceIconProvider;
        public Presenter(ResourceIconProvider resourceIconProvider)
        {
            _resourceIconProvider = resourceIconProvider;
        }

        // Set icon for view, after model was received.
        protected override void OnModelSet(ResourceIconView view, EResourceType model)
        {
            view._resourceIcon.sprite = _resourceIconProvider.Get(model);
        }
    }
}
```

Биндинг Presenter:

```c#
UiBootstrap.Bind<ResourceIconView.Presenter>();
```

### Пример биндинга данных

```c#
public class BuildingCardView : View<BuildingCardView.Model>
{
    [SerializeField] private Button buildingCardButton;

    // Children views. It is required to call Model sets/usets methods automatically.
    public override IEnumerable<IView> Children => new[] {cardInfo};
    public class Presenter : BindPresenter<BuildingCardView, Model>
    {
        protected override void OnModelSet(BuildingCardView view, Model model)
        {                
            // Call base model sets, for example bindings from Unity and also set models for children.
            base.OnModelSet(view, model);
            // Invoke PlaceAttempt every time buildingCardButton is clicked.
            view.Subscribe(view.buildingCardButton.onClick, () => model.PlaceAttempt.Execute());
            // Subcribe on changes of field IsEnoughResources and change button interactable every change.
            view.SubscribeAndCall(model.IsEnoughResources,
                    isEnough => view.buildingCardButton.interactable = isEnough);
        }
    }

    public class Model
    {
        // This field can be observed by views.
        public readonly Observable<bool> IsEnoughResources = new Observable<bool>();
        // Wrapper around event. Subscription on beacon can be disposed everywhere.
        public readonly Beacon PlaceAttempt = new Beacon();
    }
}
```

### Биндинг из редактора

Биндить данные можно прямо из редактора (круто, правда?), если их не нужно как-то преобразовывать. Можно выбрать нужное
поле модели и прикрепить View, к которому биндится поле или свойство.

![Биндинг в Unity](https://i.imgur.com/KLXRb4S.png)

Best practices
----
____

1. Рекомендуется объявлять Presenter внутри класса View, чтобы ограничить доступ к её полям из других классов.
2. Рекомендуется наследовать Presenter от абстрактного класса Presenter<View, Model>,
   это поможет избавить от лишнего кода и поможет контролировать UI Flow.
3. Рекомендуется наследовать свои View от абстрактного класса View<Model>.
4. Рекомендуется в, случае необходимости запуска анимации на дочерней компоненте,
   запускать её или на родительском GameObject, который является частью родительской компоненты,
   или использовать для этого Model.
5. Для отслеживания изменений внутри Model рекомендуется использовать для полей [Observable](Utils/Observable.cs),
   а также метод Subscribe или SubscribeAndCall внутри [View](View.cs).
   Это позволит избавиться от необходимости отписываться вручную и позволит использовать анонимные функции.
6. Для отслеживания пользовательского ввода внутри View рекомендуется использовать [Beacon](Utils/Beacon.cs) вместо
   event
   а также метод Subscribe или SubscribeAndCall внутри [View](View.cs), причины указаны в предыдущем пункте.
