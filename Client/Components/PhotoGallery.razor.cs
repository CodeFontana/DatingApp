namespace Client.Components;

public partial class PhotoGallery
{
    [Inject] ISpinnerService SpinnerService { get; set; }
    [Inject] IPhotoService PhotoService { get; set; }
    [Parameter] public MemberModel Member { get; set; }

    private MudCarousel<PhotoModel> _carousel;
    private int _position = 0;

    protected override async Task OnParametersSetAsync()
    {
        SpinnerService.HoldSpinner = true;

        foreach (PhotoModel p in Member.Photos)
        {
            p.Filename = await PhotoService.GetPhotoAsync(Member.Username, p.Filename);
        }

        SpinnerService.HoldSpinner = false;
    }

    private void HandleThumbnailClick(int index)
    {
        _position = index;
    }
}
