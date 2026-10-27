using System.Net.Mail;

namespace TravelSurvey;

public partial class MainPage : ContentPage
{
    private bool _isReady;
    private readonly DateTime _initialDate;

    public MainPage()
    {
        InitializeComponent();
        _initialDate = DepartureDate.Date ?? new DateTime(2026, 11, 1);
        _isReady = true;
    }

    private async void OnShowResultClicked(object? sender, EventArgs e)
    {
        string name = NameEntry.Text?.Trim() ?? "";
        string email = EmailEntry.Text?.Trim() ?? "";
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(name))
            errors.Add("Введите имя.");

        if (string.IsNullOrWhiteSpace(email))
            errors.Add("Введите электронную почту.");
        else if (!MailAddress.TryCreate(email, out var address)
                 || address.Address != email || !address.Host.Contains('.'))
            errors.Add("Укажите почту в формате name@example.com.");

        if (DestinationPicker.SelectedIndex < 0)
            errors.Add("Выберите направление путешествия.");

        if (DepartureDate.Date is null)
            errors.Add("Выберите дату начала путешествия.");

        if (errors.Count > 0)
        {
            ResultCard.IsVisible = false;
            ResultLabel.Text = "";
            ValidationLabel.Text = string.Join(Environment.NewLine, errors);
            await RevealAndScrollAsync(ValidationLabel, ScrollToPosition.MakeVisible);
            return;
        }

        ValidationLabel.Text = "";
        ValidationLabel.IsVisible = false;

        var transport = new[] { TrainRadio, PlaneRadio, CarRadio }
            .FirstOrDefault(radio => radio.IsChecked)?.Value?.ToString() ?? "Не выбран";

        string wishes = WishesEditor.Text?.Trim() ?? "";
        ResultLabel.Text = $"Имя: {name}\n"
            + $"Электронная почта: {email}\n"
            + $"Направление: {DestinationPicker.SelectedItem}\n"
            + $"Дата начала: {DepartureDate.Date:dd.MM.yyyy}\n"
            + $"Длительность, дней: {DaysStepper.Value:F0}\n"
            + $"Транспорт: {transport}\n"
            + $"Прогулки на природе: {YesNo(NatureCheck.IsChecked)}\n"
            + $"Музеи и выставки: {YesNo(MuseumsCheck.IsChecked)}\n"
            + $"Местный гид: {YesNo(GuideSwitch.IsToggled)}\n"
            + $"Пожелания: {(wishes.Length == 0 ? "Не указаны" : wishes)}";

        await RevealAndScrollAsync(ResultCard, ScrollToPosition.End);
    }

    private async void OnClearClicked(object? sender, EventArgs e)
    {
        _isReady = false;
        NameEntry.Text = "";
        EmailEntry.Text = "";
        WishesEditor.Text = "";
        DestinationPicker.SelectedIndex = -1;
        PlaneRadio.IsChecked = false;
        CarRadio.IsChecked = false;
        TrainRadio.IsChecked = true;
        NatureCheck.IsChecked = false;
        MuseumsCheck.IsChecked = false;
        GuideSwitch.IsToggled = false;
        DepartureDate.Date = _initialDate;
        DaysStepper.Value = 3;
        HideFeedback();
        _isReady = true;
        await FormScroll.ScrollToAsync(0, 0, true);
    }

    private void OnAnswersChanged(object? sender, EventArgs e)
    {
        if (_isReady)
            HideFeedback();
    }

    private void HideFeedback()
    {
        ValidationLabel.Text = "";
        ValidationLabel.IsVisible = false;
        ResultLabel.Text = "";
        ResultCard.IsVisible = false;
    }

    private async Task RevealAndScrollAsync(VisualElement target, ScrollToPosition position)
    {
        if (!target.IsVisible)
        {
            var layoutReady = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            EventHandler? onSizeChanged = null;
            onSizeChanged = (_, _) =>
            {
                FormLayout.SizeChanged -= onSizeChanged;
                layoutReady.TrySetResult(true);
            };
            FormLayout.SizeChanged += onSizeChanged;
            target.IsVisible = true;
            await layoutReady.Task;
        }

        if (target.IsVisible)
            await FormScroll.ScrollToAsync(target, position, true);
    }

    private static string YesNo(bool value) => value ? "Да" : "Нет";
}
