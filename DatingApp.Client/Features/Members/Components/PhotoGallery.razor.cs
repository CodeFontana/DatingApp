namespace DatingApp.Client.Features.Members.Components;

public partial class PhotoGallery
{
    [Inject] public required ISpinnerService SpinnerService { get; set; }
    [Inject] public required IPhotoService PhotoService { get; set; }
    [Parameter] public required MemberResponse Member { get; set; }

    private MudCarousel<PhotoResponse> _carousel = null!;
    private int _position = 0;

    protected override async Task OnParametersSetAsync()
    {
        SpinnerService.HoldSpinner = true;

        foreach (PhotoResponse p in Member.Photos)
        {
            p.Filename = await PhotoService.GetPhotoAsync(Member.Username, p.Filename);
        }

        SpinnerService.HoldSpinner = false;
        await base.OnParametersSetAsync();
    }

    private void HandleThumbnailClick(int index)
    {
        _position = index;
    }
}
