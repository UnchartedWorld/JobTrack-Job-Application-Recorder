using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Job_Application_Recorder.Services
{
	public class DialogService : AvaloniaObject
	{
		private static readonly Dictionary<object, Visual> RegistrationMapper =
			new();

		static DialogService()
		{
			RegisterProperty.Changed.Subscribe(RegisterChanged);
		}

		private static void RegisterChanged(AvaloniaPropertyChangedEventArgs<object?> e)
		{
			if (e.Sender is not Visual sender)
			{
				throw new InvalidOperationException("The DialogService can only be registered on a Visual");
			}

			// Unregister any old registered context
			if (e.OldValue.Value != null)
			{
				RegistrationMapper.Remove(e.OldValue.Value);
			}

			// Register any new context
			if (e.NewValue.Value != null)
			{
				RegistrationMapper.Add(e.NewValue.Value, sender);
			}
		}

		/// <summary>
		/// This property handles the registration of Views and ViewModel
		/// </summary>
		public static readonly AttachedProperty<object?> RegisterProperty = AvaloniaProperty.RegisterAttached<DialogService, Visual, object?>(
			"Register");

		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static void SetRegister(AvaloniaObject element, object value)
		{
			element.SetValue(RegisterProperty, value);
		}

		/// <summary>
		/// Accessor for Attached property <see cref="RegisterProperty"/>.
		/// </summary>
		public static object? GetRegister(AvaloniaObject element)
		{
			return element.GetValue(RegisterProperty);
		}

		/// <summary>
		/// Gets the associated <see cref="Visual"/> for a given context. Returns null, if none was registered
		/// </summary>
		/// <param name="context">The context to lookup</param>
		/// <returns>The registered Visual for the context or null if none was found</returns>
		public static Visual? GetVisualForContext(object context)
		{
			return RegistrationMapper.TryGetValue(context, out var result) ? result : null;
		}

		/// <summary>
		/// Gets the parent <see cref="TopLevel"/> for the given context. Returns null, if no TopLevel was found
		/// </summary>
		/// <param name="context">The context to lookup</param>
		/// <returns>The registered TopLevel for the context or null if none was found</returns>
		public static TopLevel? GetTopLevelForContext(object context)
		{
			return TopLevel.GetTopLevel(GetVisualForContext(context));
		}
	}

	/// <summary>
	/// A helper class to manage dialogs via extension methods. Add more on your own
	/// </summary>
	public static class DialogHelper
	{

		/// <summary>
		/// Shows an open file dialog for a registered context, most likely a ViewModel
		/// </summary>
		/// <param name="context">Context of the given dialog.</param>
		/// <param name="title">The dialog title or a default is null.</param>
		/// <param name="selectMany">If true, multiple files are selectable. 
		/// Otherwise, is false and only one file is selectable.</param>
		/// <returns>An array of file names.</returns>
		/// <exception cref="ArgumentNullException">if context was null</exception>
		public static async Task<IEnumerable<string>?> OpenFileDialogAsync
		(this object? context, string? title = null, bool selectMany = false)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			// lookup the TopLevel for the context
			var topLevel = DialogService.GetTopLevelForContext(context);

			if (topLevel != null)
			{
				// Open file dialog
				IReadOnlyList<IStorageFile>? storageFile =
				await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()

				{
					Title = title ?? "Select JSON file",
					AllowMultiple = selectMany,
					FileTypeFilter = new FilePickerFileType[]
				{
				new("JSON File")
				{
					Patterns = new[] {"*.json"},
					AppleUniformTypeIdentifiers = new[] {"public.json"},
					MimeTypes = new[] { "application/json" }
				}
				}
				});

				// return the result
				return storageFile.Select(s => s.Name);
			}
			return null;
		}

		/// <summary>
		/// Opens the Save FileDialog, passes options to the save function and
		/// updates the last used file path.
		/// </summary>
		/// <param name="context">Context of the given dialog.</param>
		/// <param name="title">The dialog title or a default is null.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<IEnumerable<string>?> CreateFileDialogAsync
		(this object? context, string? title = null)
		{
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			// lookup the TopLevel for the context
			var topLevel = DialogService.GetTopLevelForContext(context);

			if (topLevel != null)
			{
				IStorageFile? createFile =
				await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
				{
					Title = title ?? "Create File as JSON",
					SuggestedFileName = "JobAppData",
					ShowOverwritePrompt = true,
					DefaultExtension = ".json",
					FileTypeChoices = new FilePickerFileType[]
					{
						new("JSON File")
						{
							Patterns = new[] {"*.json"},
							AppleUniformTypeIdentifiers = new[] {"public.json"},
							MimeTypes = new[] { "application/json" }
						}
					}
				});

				if (createFile != null)
				{
					var emptyJobAppModel = new{ };

					// Serialises the model to JSON, then saves it.
					await FileService.CreateToJSONFileAsync(createFile, emptyJobAppModel);
                    AppSettings appSettings = new()
                    {
                        LastFilePathUsed = createFile.Path.LocalPath
                    };
                    // Saves file path to appSettings.json.
                    AppSettingsService.SaveAppSettings(appSettings);					
				}

			}
			return null;
		}

		/// <summary>
		/// Opens the Save FileDialog, passes options to the save function and
		/// updates the last used file path.
		/// </summary>
		/// <param name="context"></param>
		/// <param name="jobApplicationContents"></param>
		/// <param name="filePath"></param>
		/// <param name="selectedID"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static async Task<IEnumerable<string>?> SaveOrEditFileDialogAsync
		(this object? context,
		 JobApplication? jobApplicationContents = null,
		 string? filePath = null,
		 Guid? selectedID = null)
		{
			AppSettings appSettings = new();
			string desiredFilePath = filePath ?? appSettings.LastFilePathUsed;

			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (string.IsNullOrEmpty(desiredFilePath))
			{
				throw new ArgumentNullException(nameof(filePath), "File path must have a value");
			}

			// lookup the TopLevel for the context
			var topLevel = DialogService.GetTopLevelForContext(context);

			if (topLevel != null)
			{

				// This will handle normal saving scenarios.

				if (selectedID == null && jobApplicationContents != null)
				{
					IStorageFile? saveFile = await topLevel.StorageProvider.TryGetFileFromPathAsync(desiredFilePath);

					if (saveFile != null)
					{
						await FileService.SaveToJSONFileAsync(saveFile, jobApplicationContents);
					}
				}
				// Else it will assume editing scenarios.
				else if (selectedID != null && jobApplicationContents != null)
				{

				}
				 

			}

			return null;
		}

	}
}
