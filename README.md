[![test status](https://github.com/AngryRectangle/Yaga/actions/workflows/tests.yml/badge.svg?branch=main)](https://codecov.io/gh/AngryRectangle/Yaga)
[![codecov](https://codecov.io/gh/AngryRectangle/Yaga/branch/main/graph/badge.svg)](https://codecov.io/gh/AngryRectangle/Yaga)

# Yaga

Yaga is a sleek and powerful UI library for Unity, designed for simplicity and reusability. 
It eradicates the need for verbose boilerplate, providing a fluent API and ensuring code safety. 
Yaga's architecture promotes effortless code reusability and seamless integration with DI frameworks like Zenject, 
streamlining your development process. 
Embrace a library that not only minimizes coding effort but also enriches your project with maintainable, 
efficient, and reliable MVVM UI architecture.

Yaga stands out with its modular design, offering unparalleled flexibility. 
It allows seamless integration or replacement of its internal reactivity system with UniRx, 
catering to your project's specific needs. Whether you choose to harness the power of UniRx, 
rely on Yaga's built-in reactivity, or combine both, Yaga ensures a tailored, 
efficient approach to managing reactivity in your Unity projects.

## Core concepts
### Lifetime
One of the main problems of using MonoBehaviours for UI logic is that you have to somehow access
runtime data from them. It means that your MonoBehaviour view has runtime dependencies.
There are also "editortime" dependencies like prefabs, scriptable objects, 
basically everything that you can use in the inspector.
But you can't, for example, assign as a editortime dependency your games state.
To do that you would have to have it all in MonoBehaviours, but it would be a huge mess,
because having all your logic in MonoBehaviours put a lot of restrictions on your code.
But you can still pass those runtime dependencies to the view when you create it with a lot of boilerplate code.
And there is a catch. Lifetime of your view is not the same as lifetime of your runtime dependencies.
Your views are also editortime, because they are created as a prefab in editor.
Runtime dependencies are created in runtime, after you press play button.
It means that you have to pass runtime dependencies to the view after you instantiated it from prefab.
It provokes a lot of errors, for example, forgetting to pass dependencies before using the view or passing them twice, etc.
To solve this problems Presenters were introduced. They allow you to encapsulate all runtime dependencies inside runtime presenter.

But there is also another problem. Lifetime of data that affects UI can be even shorter than lifetime of the view.
For example, you have inventory view that shows items from inventory.
You want to reuse inventory child views that represent items in inventory because it is much more performant,
than creating new view every time new item added or removed from inventory.
That means some views can exists without data assigned to them. It can also lead to a lot of errors,
such as memory leaking due to not unsubscribing from data changes or something like that.
To solve this problem Models were introduced. 
They allow you to store data that affects UI and can be specific for each instance of the view.
And you can replace model for the view without creating new view.

![Lifetime](https://i.imgur.com/QAdBtoT.png)

### Reactivity
More about reactive programming can be read in [this github article](https://gist.github.com/staltz/868e7e9bc2a7b8c1f754).

### Declarative UI
More about declarative UI can be read in [this article](https://medium.com/israeli-tech-radar/declarative-ui-what-how-and-why-13e092a7516f).

## Attention! Achtung! Внимание!

Breaking changes alert!
Breaking changes are possible due to early development stage.

## Table of Contents

* [Core concepts](#core-concepts)
    * [Lifetime](#lifetime)
    * [Reactivity](#reactivity)
    * [Declarative UI](#declarative-ui)
* [Installation guide](#installation-guide)
* [Getting Started](#getting-started)
* [Initialization](#initialization)
    * [Singletons](#singletons)
    * [Presenters binding](#presenters-binding)
* [View](#view)
* [Presenter](#presenter)
* [Model](#model)
* [Reactivity](#reactivity)
    * [Beacon](#beacon)
    * [Observable](#observable)
    * [Observable chains](#observable-chains)
    * [OptionalObservable](#optionalobservable)
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

In the presenter you describe how UI should react on changes in model or player input.
To do that you often have to use external dependencies like ScriptableObjects with icons or other data.
Without presenters you would have to pass those dependencies to the view every time you create it.
And it would be a lot of boilerplate code. Considering prefab system in Unity, 
it would mean that there are some views that doesn't have dependencies and need to be initialized with them before usage.
It means that there are a lot of space for errors and bugs.

Presenters allow you to encapsulate all dependencies inside presenter. It allows you to easily use DI containers,
and also ensures that if you have presenter for view, you have all dependencies for it.
Just because you can't create a presenter without them.

Presenter have one most important method: `OnSet`. You should override it to describe how view should react on model setting.
The main idea behind that is to use reactivity to describe what UI need to look like depending on the model.
This approach is very different from the more common imperative approach seen in Unity's UI.

Such declarative approach allows you to describe UI in more abstract way 
and also allows you to reuse your UI in different places without writing a lot of boilerplate code.

## Model

The model is structure where you can store your data, events or properties.
You can use any type as a model, like int, string, IEnumerable or your own custom class.

The model should include all data that affects UI and can be specific for each instance of the view.
For example, if you have a button that should be disabled when some condition is met,
you should put this condition to the model and subscribe on it in the presenter.
But if that condition should be the same for all instances of that view, 
you should put it to the presenter dependencies.
If that condition can change its value during UI lifetime, you should use reactivity to track it.

1. In the Model: Include data that impacts the UI and varies per view instance. 
E.g., a condition that disables a button should be in the model, with the presenter subscribing to it.
2. In Presenter Dependencies: Store data that is consistent across all view instances.
3. Use Reactivity (like Observables): For data that changes during the 
UI's lifetime and requires continuous monitoring to update the UI in real-time.

## Reactivity

Yaga provides minimal set of classes required for reactive programming.

### Beacon

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
