namespace Client.Services;

public class SpinnerService : ISpinnerService
{
    public event Action OnShow;
    public event Action OnHide;

    private bool _holdSpinner = false;

    public bool HoldSpinner
    {
        get { return _holdSpinner; }
        set
        {
            _holdSpinner = value;
            Hide();
        }
    }

    public void Show()
    {
        OnShow?.Invoke();
    }

    public void Hide()
    {
        if (HoldSpinner == false)
        {
            OnHide?.Invoke();
        }
    }
}
