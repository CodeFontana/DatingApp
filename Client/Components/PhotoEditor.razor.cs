﻿using DataAccessLibrary.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client.Components;

public partial class PhotoEditor
{
    [Parameter] public EventCallback<string> OnImageChanged { get; set; }

    private long _maxFileSize = 1024 * 1024 * 10;
    private bool _uploading = false;
    private bool _showError = false;
    private string _errorText;

    private async Task HandleImageUploadAsync(InputFileChangeEventArgs e)
    {
        try
        {
            _uploading = true;
            IBrowserFile imageFile = e.File;

            if (imageFile == null)
            {
                throw new Exception("Image file not found");
            }
            else if (imageFile.Size > _maxFileSize)
            {
                throw new Exception($"Image file exceeds maximum size [{_maxFileSize} bytes]");
            }

            StreamContent fileContent = new(imageFile.OpenReadStream(_maxFileSize));

            using MultipartFormDataContent content = new();
            content.Add(
                content: fileContent,
                name: "\"files\"",
                fileName: imageFile.Name);

            ServiceResponseModel<PhotoModel> result = await MemberService.AddPhotoAsync(AppUserService.AppUser.Username, content);

            if (result.Success)
            {
                Snackbar.Add("Photo added successfully", Severity.Success);
                _showError = false;
                await OnImageChanged.InvokeAsync();
            }
            else
            {
                _showError = true;
                _errorText = $"{result.Message}";
                Snackbar.Add($"{result.Message}", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            _showError = true;
            _errorText = $"{ex.Message}";
            Snackbar.Add($"{ex.Message}", Severity.Error);
        }
        finally
        {
            _uploading = false;
        }
    }

    private async Task HandleImageChangedAsync()
    {
        await OnImageChanged.InvokeAsync();
    }
}
