[![test status](https://github.com/AngryRectangle/Yaga/actions/workflows/tests.yml/badge.svg?branch=main)](https://codecov.io/gh/AngryRectangle/Yaga)
[![codecov](https://codecov.io/gh/AngryRectangle/Yaga/branch/main/graph/badge.svg)](https://codecov.io/gh/AngryRectangle/Yaga)

# Yaga

**Yaga** is a simple UI lib for Unity.
The main goal of the lib is to minimize amount of code you need to write for proper MVVM UI and keep it simple and
reliable.

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

First, create a view class in the inspector and a presenter class 
to dynamically update the view based on model values.

In this example SimpleTextButtonView is just a wrapper for button with text.
It will allow you to change the text on the button and do something when the button is clicked.

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
        protected override void OnSet(SimpleTextButtonView view, string model, ISubscriptionsOwner subs)
        {
            // Set string value to text on button.
            view.buttonText.text = model;
            // Subscribe on button onClick and log messages after every click.
            subs.Subscribe(view.button, () => Debug.Log("Click"));
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
UiBootstrap.Instance.Bind(new SimpleTextButtonView.Presenter());
```

After initialization, you only need to write a single line
to create an instance of the view with 'Sample text' on the button.

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

You have to put your presenter for every view type you will use.
It is possible either with shortcut method for presenters with
parameterless constructors or just with binding instance of your presenter.

```c#
// For presenter with parameterless constructor.
UiBootstrap.Instance.Bind<Presenter>();

// For presenters with parameters in constructor.
UiBootstrap.Instance.Bind(new Presenter(param1, param2));
```

## View

A View is a MonoBehaviour used to display data to the player and handle player input.
Each view has a concrete model type like int, string, or your custom type.
If you want to have view without model you can, but it will be just a wrapper around Unit as a model.
Also each view has one and only one Presenter.

Relation scheme between view, presenter and model:
`View ⇆ Presenter ⇆ Model`

Each view is going through these stages:

1. Create - instantiates game object
2. Set - sets the model for view, creating subscriptions and view update
3. Unset - unsubscribe from model and end all active subscriptions
4. Destroy - destroying views game object

## Presenter

Presenter is a bridge between view and model.
In the presenter you can subscribe on changes in model and update view
or subscribe on player input and update model.

Presenter have two main methods: `OnSet` and `OnUnset`.
In those you can react on model setting or unsetting and, for example,
subscribe on event in `OnSet` and unsubscribe in `OnUnset`.

## Model

Model is structure where you can store your data, events or properties.
You can use any type as model, like int, string, IEnumerable or your own custom class.

## Beacon

Beacons are a useful replacement for events.
One of the main benefits of using beacons is much simpler
unsubscription logic for lambdas compared to events.
After subscription beacon returns to you IDisposable which you can dispose to unsubscribe.
Just compare code with events:

```c#
event Action<string> TextEvent;

// Subscription logic.
var action = new Action<string>(e => Debug.Log($"Text {e} with length {e.Length}"));
TextEvent += action;

// Unsubscription logic.
TextEvent -= action;
```

And same code but with Beacon

```c#
Beacon<string> TextBeacon = new Beacon<string>();

// Subscription logic.
var disposable = TextBeacon.Add(e => Debug.Log($"Text {e} with length {e.Length}"));

// Unsubscription logic.
disposable.Dispose();
```

## Observables

Observer pattern in Yaga implemented with using of IObservable and IOptionalObservable.

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

### Observable chains

You can chain observables to combine and process data from several sources.
Example of simple chain where each next observable reacts on changes in previous.

```c#
// Observable that will trigger itemInfo on change
var amount = new Observable<int>(100);
// Specify binding rule for data from amount to itemInfo
var itemInfo = amount.Select(count => $"Current amount is {count}");
//Specify logic that will be invoked when itemInfo changes
var disposable = itemInfo.Subscribe(info => Debug.Log(info));
// Will trigger itemInfo that will trigger message "Current amount is 5"
amount.Data = 5;

disposable.Dispose();
```

You can combine several observables to one.
When one of the parent observables changes it will trigger changes in all child observables.

```c#
var amount = new Observable<int>(100);
var itemName = new Observable<string>("cake");
var itemInfo = amount.CombineLatest(itemName, (count, name) => $"{name} {count}");
    
var disposable = itemInfo.Subscribe(info => Debug.Log(info));
// Will trigger message to console "apple 100"
itemName.Data = "apple";
// Will trigger message to console "apple 5"
amount.Data = 5;

disposable.Dispose();
```

### OptionalObservable

Use optional observables when your observable data might be empty.
OptionalObservable&lt;T> is wrapper around Observable&lt;Option&lt;T>>. 
Option works as described in <a href="https://github.com/nlkl/Optional/">GitHub repository</a>.
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

## Best practices

1. Declare your presenter class inside the View class to keep View class fields private and ensure they're only accessed by the presenter. 
   With that you will be able to avoid extra boilerplate of properties. But your presenter will be more coupled to view.
2. Inherit your Presenter class from abstract class [Presenter<View, Model>](Yaga/Presenter.cs) it will help you to
   avoid boilerplate code.
3. Inherit your View class from abstract class [View<Model>](Yaga/View.cs) it will help you to avoid boilerplate code.
4. Use [Observable](Utils/Observable.cs) and Subscribe or SubscribeAndCall method inside [View](View.cs) to track
changes of some value over time.
5. Use [Beacon](Utils/Beacon.cs) instead of events to track user input inside [View](View.cs) with Subscribe or
   SubscribeAndCall methods.
   changes in your model. It will unsubscribe when needed and you won't have bother about it.
