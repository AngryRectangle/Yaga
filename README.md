[![Coverage Status](./Reports/CodeCoverage/Report/badge_shieldsio_linecoverage_red.svg?dummy=8484744)](./Reports/CodeCoverage/Report//index.html)

# Yaga

**Yaga** is a simple UI lib for Unity3D.
Paradigm of the lib is *MVPb Via MfD* (MVP but View is also Model for Data).
Just joking, it is MVVM library.

### Attention! Achtung! Внимание!

Breaking changes alert!
Breaking changes are possible due to early development stage.

## Table of Contents

* [Installation guide](#installation-guide)
* [Getting Started](#getting-started)
* [Initialization](#initialization)
    * [Singletons](#singletons)
    * [Presenters binding](#presenters-binding)
* [View](#view)
* [Presenter](#presenter)
* [Model](#model)
* [Beacon](#beacon)
* [Observables](#observables)
    * [Observable](#observable)
    * [Binding chains](#binding-chains)
    * [OptionalObservable](#OptionalObservable)
* [Editor Binding](#editor-binding)
* [Best Practices](#best-practices)

## Installation guide

Click on top panel "Window > PackageManager".
Click "+" on the top left and paste link to repository:
```https://github.com/AngryRectangle/Yaga.git```

![Installation screenshot](https://i.postimg.cc/zfmXHrSk/img.png)

## Getting Started

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

1. Create - instantiates game object and make it inactive after instantiation
2. Open - shows view after model was set
3. Close - hides view after model unset
4. Destroy - preparing view for destroy and call Destroy method on it.

Each stage should be called in represented order
and order violation is strongly not recommended.
Also Open method should be called after model set and clos after unset
but about it in Presenters section.

## Presenter

Presenter is a bridge between view and model.
In presenter you can subscribe on changes in model and update view
or subscribe on player input and update model.

Presenter have two main methods: `OnModelSet` and `OnModelUnset`.
In those you can react on model setting or unsetting and, for example,
subscribe on event in `OnModelSet` and unsubscribe in `OnModelUnset`.

## Model

Model is structure where you can store your data, events or properties.
You can use any type as model, like int, string, IEnumerable or your own custom class.

## Beacon

Beacons is an useful replacement for events.
One of the main benefits of using beacons is much simpler
unsubscription logic for lambdas compared to events.
After subscription beacon returns to you IDisposable which you can dispose to unsubscribe.
Just compare event realisation:

```c#
event Action<string> TextEvent;

// Subscription logic.
var action = new Action<string>(e => Debug.Log($"Text {e} with length {e.Length}"));
TextEvent += action;

// Unsubscription logic.
TextEvent -= action;
```

And Beacon realisation:

```c#
Beacon<string> TextBeacon = new Beacon<string>();

// Subscription logic.
var disposable = TextBeacon.Add(e => Debug.Log($"Text {e} with length {e.Length}"));

// Unsubscription logic.
disposable.Dispose();
```

## Observables

Observer pattern in Yaga implemented with using of IObservable, IOptionalObservable,
IObservableEnumerable and IObservableArray.

### Observable

Observable is generic wrapper around your data.
It uses all benefits from Beacons, but also stores variable to hold data inside.
Here is usage example:

```c#
Observable<string> observable = new Observable<string>();
var disposable = observable.Subscribe(e => Debug.Log($"New line {e}!"));

// Will trigger message to console
observable.Data = "second";
disposable.Dispose();
```

### Binding chains

You can chain observables to combine data from several sources and process it.
Example of simple chain where each next observable reacts on changes in previous.

```c#
// Observale that will trigger itemInfo on change
var amount = new Observable<int>(100);
// Specify binding rule for data from amount to itemInfo
var itemInfo = Observable.Bind(amount, count => $"Current amount is {count}");
//Specify logic that will be invoked when itemInfo changes
var disposable = itemInfo.Add(info => Debug.Log(info));
// Will trigger itemInfo that will trigger message "Current amount is 5"
amount.Data = 5;

disposable.Dispose();
```

You can combine several observables to one.
When one of the parent observables changes it will trigger changes in all child observables.

```c#
var amount = new Observable<int>(100);
var itemName = new Observable<string>("cake");
var itemInfo = Observable.Bind(amount, itemName, (count, name) => $"{name} {count}");
    
var disposable = itemInfo.Add(info => Debug.Log(info));
// Will trigger message to console "apple 100"
itemName.Data = "apple";
// Will trigger message to console "apple 5"
amount.Data = 5;

disposable.Dispose();
```

### OptionalObservable

You should use optional observables when your observable data can be empty.
With OptionalObservable you can organise data processing
which will guarantee correct processing of empty values.
In subscription method you should provide methods which will process data in two scenarios:
when data is present and when data is not present.

```c#
var someObservable = new OptionalObservable<int>(100);
var disposable = someObservable
    .Subscribe(value => Debug.Log(value), () => Debug.Log("No count"));

someObservable.SetDefault(); // Will trigger "No count" message.
someObservable.Data = 5; // Will trigger "5" message.

disposable.Dispose();
```

You can create chains with OptionalObervables too.
If one of parent observables gets default value,
all children will also get it.

```c#
var amount = new OptionalObservable<int>(100);
var itemName = new OptionalObservable<string>("cake");
var itemInfo = OptionalObservable.Bind(amount, itemName, (count, name) => $"{name} {count}");
var disposable = itemInfo.Subscribe(info => Debug.Log(info), () => Debug.Log("No valid info"));

itemName.Data = "apple"; // Will trigger message to console "apple 100"
amount.SetDefault(); // Will trigger message to console "No valid info"

disposable.Dispose();
```

### Editor binding

You can bind model fields and properties to views even from editor. You can select needed type and attack view you want
to bind to.
**This functionality is not properly tested yet, don't rely on it**

![Editor binding](https://i.imgur.com/KLXRb4S.png)

## Best practices

1. Declare your presenter class inside View class, to make View class fields private and have access to them only from
   presenter. With that you will ba able to avoid extra boilerplate of properties. But your presenter will be more
   coupled to view.
2. Inherit your Presenter class from abstract class [Presenter<View, Model>](Yaga/Presenter.cs) it will help you to
   avoid boilerplate code.
3. Inherit your View class from abstract class [View<Model>](Yaga/View.cs) it will help you to avoid boilerplate code.
4. Use [Observable](Utils/Observable.cs) and Subscribe or SubscribeAndCall method inside [View](View.cs) to track
5. Use [Beacon](Utils/Beacon.cs) instead of event to track user input inside [View](View.cs) with Subscribe or
   SubscribeAndCall methods.
   changes in your model. It will unsubscribe when needed and you won't have bother about it.
