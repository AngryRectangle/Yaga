Yaga
====
**Yaga** - это простой UI либа-обёртка в парадигме *MVPb Via MfD*
(MVP but View is also Model for Data).
(На самом деле просто MVVM)


Особенности Yaga
-
____

Важной особенностью Yaga является приверженность подходу *MVPb Via MfD*.

Каждому типу View соответствует одна и более визульных компонент. Визуальная компонента - это префаб, с повешенным на
него скриптом View.

Вся структура UI заранее жестко задана, исключением являются ListView, который может содержат неограниченное число
дочерних компонент с одним типом View и могут менять их количество в процессе работы.

У каждой визуальной компоненты может быть неограниченное число дочерних визуальных компонент, UI Flow которых
контролирует контроллер родительской визуальной компоненты.

UI Flow
-
______________

UI Flow можно описать такой схемой: Create -> Set -> Open -> Update -> Close -> Unset -> Destroy.

- Create - происходит инициализация View, а Presenter подписывается на получение ввода игрока со View.
- Set - устанавливается Model для View, Presenter подписывается на необходимые поля в Model. На этом этапе Controller
  также может отобразить Model на View.
- Open - View Presenter видна пользователю.
- Update - Presenter получает обновления для View каждый кадр, вызывается только для открытых View.
- Close - View скрывается от пользователя.
- Unset - Model удаляется со View, а Presenter отписывается от обновлений.
- Destroy - Presenter отписывается от всех событий во View, а сама View безвозвратно уничтожается.

Контролировать UI Flow возможно через класс [UiBootstrap](UiBootstrap.cs), но вне Presenter рекомендуется делать это
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
более высокий уровень нужно выполнить завершающий этап на текущем уровне. Например нельзя после вызова этапа Open
вызывать Unset, т.к. перед этим должен быть вызван Close. Корректость UI Flow может быть нарушена при использовании
[UiBootstrap](UiBootstrap.cs) вместо [UiControl](UiControl.cs), что неизбежно происходит в Presenter, потому как каждый
Presenter сам отвечает за корректность обработки этапов для себя и дочерних визуальных компонент.

Использование
----
_____

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

Биндить данные можно из редактора в случае если их не нужно как-то преобразовывать. Можно выбрать нужное поле и прикрепить View для которого нужно забиндить поле или свойство.

![Биндинг в Unity](https://i.imgur.com/KLXRb4S.png)

Best practices
----
____
1. Рекомендуется объявлять Presenter внутри класса View для того чтобы ограничить доступ к её поля из других классов.
2. Рекомендуется наследовать свои Presenter от абстрактного класса IPresenter<View, Model>,
это поможет избавить от лишнего кода и поможет контролировать UI Flow.
3. Рекомендуется наследовать свои View от абстрактного класса View<Model>.
4. Рекомендуется в случае необходимости запуска анимации на дочерней компоненте
запусить её или на родительском GameObject, который является частью родительской компоненты,
или использовать для этого Model.
5. Для отслеживания изменений внутри Model рекомендуется использовать для полей [Observable](Utils/Observable.cs),
а также метод Subscribe или SubscribeAndCall внутри [View](View.cs).
Это позволит избавиться от необходимости отписываться вручную и позволит использовать анонимные функции.
6. Для отслеживания пользовательского ввода внутри View рекомендуется использовать [Beacon](Utils/Beacon.cs) вместо event
а также метод Subscribe или SubscribeAndCall внутри [View](View.cs).
Это позволит избавиться от необходимости отписываться вручную и позволит использовать анонимные функции.
