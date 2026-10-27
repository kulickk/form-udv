using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using TravelSurvey;

DispatcherProvider.SetCurrent(new InlineDispatcherProvider());
var app = new App();
var page = new MainPage();
int checks = 0;
T Find<T>(string name) where T : class => page.FindByName<T>(name) ?? throw new Exception(name);
void Check(bool ok, string message) { if (!ok) throw new Exception(message); checks++; }
void Show() => Find<Button>("ShowResultButton").SendClicked();
void Clear() => Find<Button>("ClearButton").SendClicked();
var name = Find<Entry>("NameEntry");
var email = Find<Entry>("EmailEntry");
var picker = Find<Picker>("DestinationPicker");
var date = Find<DatePicker>("DepartureDate");
var days = Find<Stepper>("DaysStepper");
var train = Find<RadioButton>("TrainRadio");
var plane = Find<RadioButton>("PlaneRadio");
var car = Find<RadioButton>("CarRadio");
var nature = Find<CheckBox>("NatureCheck");
var museums = Find<CheckBox>("MuseumsCheck");
var guide = Find<Switch>("GuideSwitch");
var wishes = Find<Editor>("WishesEditor");
var errors = Find<Label>("ValidationLabel");
var result = Find<Label>("ResultLabel");
var card = Find<Border>("ResultCard");
var hint = Find<Label>("GuideHint");

Check(!card.IsVisible && !errors.IsVisible && !hint.IsVisible, "Initial feedback visibility");
Check(picker.SelectedIndex == -1 && picker.Items.Count == 4, "Initial picker");
Check(train.IsChecked && !plane.IsChecked && !car.IsChecked, "Initial transport");
Check(date.Date == new DateTime(2026, 11, 1) && date.Date >= date.MinimumDate && date.Date <= date.MaximumDate, "Initial date range");
Check(days.Value == 3 && days.Minimum == 1 && days.Maximum == 14, "Initial numeric range");
Check(Find<Label>("DurationValue").Text == "Дней: 3", "Initial x:Reference binding");
Show();
Check(errors.IsVisible && errors.Text.Contains("имя") && errors.Text.Contains("почту") && errors.Text.Contains("направление"), "Required fields validation");
Check(!card.IsVisible, "Invalid form has no result");
name.Text = "   "; email.Text = "test@example.com"; picker.SelectedIndex = 0;
Show(); Check(errors.Text.Contains("имя"), "Whitespace name is rejected");
name.Text = "Анна"; email.Text = "invalid";
Show(); Check(errors.IsVisible && errors.Text.Contains("формате"), "Invalid email");
email.Text = "Анна <test@example.com>";
Show(); Check(errors.IsVisible, "Display-name email is rejected");
email.Text = "anna@example.com";
date.Date = new DateTime(2027, 6, 15);
days.Value = 7;
Check(Find<Label>("DurationValue").Text == "Дней: 7", "Live numeric binding");
plane.IsChecked = true;
Check(plane.IsChecked && !train.IsChecked && !car.IsChecked, "Radio exclusivity");
nature.IsChecked = true; museums.IsChecked = true; guide.IsToggled = true;
Check(hint.IsVisible, "Guide visible binding");
wishes.Text = "Номер с видом на горы";
Show();
Check(card.IsVisible && !errors.IsVisible, "Valid result visibility");
foreach (string expected in new[] { "Анна", "anna@example.com", "Горы", "15.06.2027", "дней: 7", "Самолёт", "Прогулки на природе: Да", "Музеи и выставки: Да", "Местный гид: Да", "Номер с видом на горы" })
    Check(result.Text.Contains(expected), "Summary field: " + expected);
wishes.Text = "Изменено";
Check(!card.IsVisible && result.Text == "", "Editing hides stale summary");
car.IsChecked = true; Show();
Check(result.Text.Contains("Транспорт: Автомобиль"), "Car selection");
Clear();
Check(name.Text == "" && email.Text == "" && wishes.Text == "", "Reset text");
Check(picker.SelectedIndex == -1, "Reset picker");
Check(train.IsChecked && !plane.IsChecked && !car.IsChecked, "Reset radio group");
Check(!nature.IsChecked && !museums.IsChecked && !guide.IsToggled && !hint.IsVisible, "Reset booleans and binding");
Check(date.Date == new DateTime(2026, 11, 1) && days.Value == 3, "Reset date and duration");
Check(!card.IsVisible && !errors.IsVisible && result.Text == "" && errors.Text == "", "Reset feedback");
name.Text = "Иван"; email.Text = "ivan@example.com"; picker.SelectedIndex = 1;
Show();
Check(card.IsVisible && result.Text.Contains("Пожелания: Не указаны") && result.Text.Contains("Местный гид: Нет"), "Optional empty answers");
guide.IsToggled = true;
Check(!card.IsVisible && hint.IsVisible, "Switch invalidates result");
Clear(); Clear();
Check(!card.IsVisible && picker.SelectedIndex == -1 && days.Value == 3, "Repeated reset");
Console.WriteLine($"PASS: {checks} behavior checks against the real MAUI XAML and code-behind.");

sealed class InlineDispatcherProvider : IDispatcherProvider
{
    public IDispatcher GetForCurrentThread() => new InlineDispatcher();
}
sealed class InlineDispatcher : IDispatcher
{
    public bool IsDispatchRequired => false;
    public bool Dispatch(Action action) { action(); return true; }
    public bool DispatchDelayed(TimeSpan delay, Action action) { action(); return true; }
    public IDispatcherTimer CreateTimer() => throw new NotSupportedException();
}
