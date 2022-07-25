namespace Client.Interfaces;

public interface ISpinnerService
{
    bool HoldSpinner { get; set; }

    event Action OnHide;
    event Action OnShow;

    void Hide();
    void Show();
}