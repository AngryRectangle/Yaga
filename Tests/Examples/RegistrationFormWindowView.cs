using Optional;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yaga;
using Yaga.Reactive;
using Yaga.Reactive.BeaconExtensions;
using Yaga.Reactive.ObservableExtensions;

namespace Tests.Examples
{
    public class RegistrationFormWindowView : View<RegistrationFormWindowView.Model>
    {
        [SerializeField] public TMP_InputField _usernameInput;
        [SerializeField] public TMP_InputField _passwordInput;
        [SerializeField] public TMP_Text _errorText;
        [SerializeField] public Button _submitButton;
        [SerializeField] public RectTransform _loadingOverlay;

        public class Presenter : Presenter<RegistrationFormWindowView, Model>
        {
            protected override void OnSet(RegistrationFormWindowView view, Model model, ISubscriptions subs)
            {
                var username = new Observable<string>();
                var password = new Observable<string>();
                view._usernameInput.Into(username).AddTo(subs);
                view._passwordInput.Into(password).AddTo(subs);

                var passwordError = password.Select(ValidatePassword);
                var isNicknameValid = username.Select(username => !string.IsNullOrWhiteSpace(username));
                var canSubmitForm = isNicknameValid.CombineLatest(
                    passwordError.Select(error => error.HasValue == false),
                    (isNicknameValidValue, isPasswordValid) => isNicknameValidValue && isPasswordValid);
                canSubmitForm.IntoInteractable(view._submitButton).AddTo(subs);

                passwordError.SelectMap(
                    error =>
                    {
                        return error switch
                        {
                            Model.ErrorStatus.TooShort => "Password is too short.",
                            Model.ErrorStatus.NotAllowedChars => "Password contains not allowed characters.",
                            _ => string.Empty
                        };
                    }
                ).Into(view._errorText).AddTo(subs);


                model.IsSubmitting.IntoActive(view._loadingOverlay).AddTo(subs);
                model.OnSubmit.Is(view._submitButton.onClick,
                    () => new Model.RegistrationData(username.Value, password.Value)).AddTo(subs);
            }

            private Option<Model.ErrorStatus> ValidatePassword(string password)
            {
                if (password.Length < 6)
                    return Model.ErrorStatus.TooShort.Some();

                if (!System.Text.RegularExpressions.Regex.IsMatch(password, @"^[a-zA-Z0-9]+$"))
                    return Model.ErrorStatus.NotAllowedChars.Some();

                return Option.None<Model.ErrorStatus>();
            }
        }

        public class Model
        {
            public Observable<bool> IsSubmitting;
            public Beacon<RegistrationData> OnSubmit;

            public Model()
            {
                IsSubmitting = false;
                OnSubmit = new Beacon<RegistrationData>();
            }

            public enum ErrorStatus
            {
                TooShort,
                NotAllowedChars,
            }

            public class RegistrationData
            {
                public string Username;
                public string Password;

                public RegistrationData(string username, string password)
                {
                    Username = username;
                    Password = password;
                }
            }
        }
    }
}