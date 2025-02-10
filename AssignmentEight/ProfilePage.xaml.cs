using Microsoft.Maui.Storage;
using System.Text.Json;
using System.IO;

namespace AssignmentEight;

public partial class ProfilePage : ContentPage
{
    private readonly string _filePath;

    public ProfilePage()
    {
        InitializeComponent();
        _filePath = Path.Combine(FileSystem.AppDataDirectory, "profile.json");
        CreateJsonFileIfNotExists();
        LoadProfile();
    }

    // Create the JSON file if it doesn't exist
    private void CreateJsonFileIfNotExists()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                // Create a new Profile object if the file doesn't exist
                var defaultProfile = new Profile
                {
                    Name = string.Empty,
                    Surname = string.Empty,
                    Email = string.Empty,
                    Bio = string.Empty
                };

                // Serialize the default profile and write it to the file
                var json = JsonSerializer.Serialize(defaultProfile, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_filePath, json);
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to create JSON file: {ex.Message}", "OK");
        }
    }

    // Load profile data from Preferences and JSON file
    private void LoadProfile()
    {
        try
        {
            // Load from Preferences first
            var name = Preferences.Get("Name", string.Empty);
            var surname = Preferences.Get("Surname", string.Empty);
            var email = Preferences.Get("Email", string.Empty);
            var bio = Preferences.Get("Bio", string.Empty);

            // If data exists in Preferences, load it into the UI
            NameEntry.Text = name;
            SurnameEntry.Text = surname;
            EmailEntry.Text = email;
            BioEditor.Text = bio;

            // Check if the profile exists in the JSON file
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var profile = JsonSerializer.Deserialize<Profile>(json);
                if (profile != null)
                {
                    // Populate UI with profile data from JSON file
                    NameEntry.Text = profile.Name;
                    SurnameEntry.Text = profile.Surname;
                    EmailEntry.Text = profile.Email;
                    BioEditor.Text = profile.Bio;
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load profile: {ex.Message}", "OK");
        }
    }

    // Save profile data to Preferences and JSON file
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        var profile = new Profile
        {
            Name = NameEntry.Text,
            Surname = SurnameEntry.Text,
            Email = EmailEntry.Text,
            Bio = BioEditor.Text
        };

        try
        {
            // Save the profile to Preferences
            Preferences.Set("Name", profile.Name);
            Preferences.Set("Surname", profile.Surname);
            Preferences.Set("Email", profile.Email);
            Preferences.Set("Bio", profile.Bio);

            // Serialize the profile object to JSON and save to file
            var json = JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });

            // Write to file
            await File.WriteAllTextAsync(_filePath, json);

            // Show a success message
            await DisplayAlert("Success", "Profile saved successfully!", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to save profile: {ex.Message}", "OK");
        }
    }

    // Handle the picture change
    private async void OnAddPictureClicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Pick a profile picture"
            });

            if (photo != null)
            {
                // Update the profile image UI with the selected picture
                ProfileImage.Source = ImageSource.FromFile(photo.FullPath);

                //  store the image file path for future reference
                Preferences.Set("ProfileImagePath", photo.FullPath);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to pick photo: {ex.Message}", "OK");
        }
    }
}

// Profile class to hold the profile data
public class Profile
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Email { get; set; }
    public string Bio { get; set; }
}
